using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.IO.CoordinateSystems;
namespace SmartContractVehicle.Service
{
    public static class GeometryService
    {

        /// <summary>
        /// Creates a circle (as a polygon) around a given WGS84 point using the specified radius in meters.
        /// </summary>
        /// <param name="center">The center point (SRID:4326).</param>
        /// <param name="radiusMeters">Radius in meters.</param>
        /// <param name="numPoints">Number of points to use for the circle approximation.</param>
        /// <returns>A Polygon representing the circle.</returns>
        public static Polygon CreateCircleFromPoint(Point center, double radiusMeters, int numPoints = 32)
        {
            if (center.SRID != 4326)
                throw new ArgumentException("Center point must be in SRID 4326 (WGS84)");

            // Define WGS84 and projected UTM zone
            var wgs84 = GeographicCoordinateSystem.WGS84;

            // Use UTM zone based on center point
            int utmZone = (int)Math.Floor((center.X + 180) / 6) + 1;
            bool isNorthernHemisphere = center.Y >= 0;
            string utmWkt = $"PROJCS[\"WGS 84 / UTM zone {utmZone}{(isNorthernHemisphere ? "N" : "S")}\", " +
                            "GEOGCS[\"WGS 84\", " +
                            "DATUM[\"WGS_1984\", SPHEROID[\"WGS 84\",6378137,298.257223563]], " +
                            "PRIMEM[\"Greenwich\",0], UNIT[\"degree\",0.0174532925199433]], " +
                            $"PROJECTION[\"Transverse_Mercator\"], PARAMETER[\"latitude_of_origin\",0], " +
                            $"PARAMETER[\"central_meridian\",{((utmZone - 1) * 6 - 180 + 3)}], " +
                            "PARAMETER[\"scale_factor\",0.9996], PARAMETER[\"false_easting\",500000], " +
                            $"PARAMETER[\"false_northing\",{(isNorthernHemisphere ? 0 : 10000000)}], " +
                            "UNIT[\"metre\",1]]";

            var utm = CoordinateSystemWktReader.Parse(utmWkt) as ProjectedCoordinateSystem;

            // Create transformation
            var transformFactory = new CoordinateTransformationFactory();
            var toUtm = transformFactory.CreateFromCoordinateSystems(wgs84, utm);
            var toWgs84 = transformFactory.CreateFromCoordinateSystems(utm, wgs84);

            // Transform center to UTM
            double[] utmCenter = toUtm.MathTransform.Transform(new[] { center.X, center.Y });

            // Create UTM geometry factory
            var geometryFactoryUtm = new GeometryFactory(new PrecisionModel(), 32600 + utmZone); // EPSG:326XX

            // Create point in UTM
            var utmPoint = geometryFactoryUtm.CreatePoint(new Coordinate(utmCenter[0], utmCenter[1]));

            // Buffer in meters (in projected UTM space)
            Geometry circleUtm = utmPoint.Buffer(radiusMeters, numPoints);

            // Transform circle back to WGS84
            Coordinate[] coords = circleUtm.Coordinates.Select(c =>
            {
                var wgs = toWgs84.MathTransform.Transform([c.X, c.Y]);
                return new Coordinate(wgs[0], wgs[1]);
            }).ToArray();

            var geometryFactoryWgs84 = new GeometryFactory(new PrecisionModel(), 4326);
            return geometryFactoryWgs84.CreatePolygon(coords);
        }

    }
}
