import React from "react";
import { MapContainer, TileLayer } from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import makeStyles from "../../util/makeStyles.ts";

import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png';
import markerIcon from 'leaflet/dist/images/marker-icon.png';
import markerShadow from 'leaflet/dist/images/marker-shadow.png';

L.Icon.Default.mergeOptions({
    iconUrl: markerIcon,
    iconRetinaUrl: markerIcon2x,
    shadowUrl: markerShadow,
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

const VehicleMap: React.FC = () => {
    const { classes } = useStyles();

    return (
        <div className={classes.outerWrapper}>
            <div className={classes.mapWrapper}>
                <MapContainer center={center} zoom={13} style={{ height: "100%", width: "100%" }}>
                    <TileLayer
                        url="https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png"
                        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/">CARTO</a>'
                    />
                </MapContainer>
            </div>
        </div>
    );
};

export default VehicleMap;
