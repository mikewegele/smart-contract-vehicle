import React, { useCallback } from "react";
import { MapContainer, Marker, Popup, TileLayer } from "react-leaflet";
import L, { type LatLngTuple } from "leaflet";
import "leaflet/dist/leaflet.css";
import makeStyles from "../../util/makeStyles.ts";
import markerIcon2x from "leaflet/dist/images/marker-icon-2x.png";
import markerIcon from "leaflet/dist/images/marker-icon.png";
import markerShadow from "leaflet/dist/images/marker-shadow.png";
import type { CarTO, Point } from "../../api";

L.Icon.Default.mergeOptions({
    iconUrl: markerIcon,
    iconRetinaUrl: markerIcon2x,
    shadowUrl: markerShadow,
});

const createVehicleIcon = (imageUrl: string) =>
    L.icon({
        iconUrl: imageUrl,
        iconSize: [50, "auto"],
        iconAnchor: [20, 40],
        popupAnchor: [0, -40],
        className: "vehicle-marker-icon",
    });

const useStyles = makeStyles(() => ({
    outerWrapper: {
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        padding: "2rem",
        width: "100%",
    },
    mapWrapper: {
        width: "80%",
        maxWidth: "1000px",
        height: "500px",
        borderRadius: "12px",
        overflow: "hidden",
        boxShadow: "0 4px 12px rgba(0, 0, 0, 0.1)",
        border: "1px solid #e0e0e0",
    },
}));

const center: [number, number] = [52.52, 13.405];

const DEFAULT_IMAGE =
    "https://www.fett-wirtz.de//assets/components/phpthumbof/cache/i3-rendering.a175f4b33a701463542158cc33d89ecf.webp";

interface Props {
    vehicles: CarTO[];
}

const VehicleMap: React.FC<Props> = (props) => {
    const { vehicles } = props;

    const { classes } = useStyles();

    const mapPointToLocation = useCallback((point: Point): LatLngTuple => {
        const [lng, lat] = point.coordinates ?? [];

        if (typeof lng !== "number" || typeof lat !== "number") {
            throw new Error("Invalid coordinates");
        }

        return [lat, lng];
    }, []);

    console.log(vehicles);

    return (
        <div className={classes.outerWrapper}>
            <div className={classes.mapWrapper}>
                <MapContainer
                    center={center}
                    zoom={13}
                    style={{ height: "100%", width: "100%" }}
                >
                    <TileLayer
                        url="https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png"
                        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/">CARTO</a>'
                    />
                    {vehicles.map((vehicle, index) => (
                        <Marker
                            key={index}
                            position={mapPointToLocation(
                                vehicle.currentPosition
                            )}
                            icon={createVehicleIcon(
                                vehicle.trimImagePath || DEFAULT_IMAGE
                            )}
                        >
                            <Popup maxWidth={200}>
                                <div style={{ textAlign: "center" }}>
                                    <strong>{vehicle.modelName}</strong>
                                    <br />
                                    <img
                                        src={
                                            vehicle.trimImagePath ||
                                            DEFAULT_IMAGE
                                        }
                                        alt={vehicle.trimImagePath || undefined}
                                        style={{
                                            width: "100%",
                                            maxWidth: "150px",
                                            height: "auto",
                                            marginTop: "0.5rem",
                                        }}
                                    />
                                </div>
                            </Popup>
                        </Marker>
                    ))}
                </MapContainer>
            </div>
        </div>
    );
};

export default VehicleMap;
