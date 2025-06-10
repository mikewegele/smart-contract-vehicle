import { Box, Slider, TextField, Typography } from "@mui/material";
import makeStyles from "../../util/makeStyles";
import React, { useEffect, useState } from "react";
import DefaultButton from "../button/DefaultButton.tsx";
import useApiStates from "../../util/useApiStates.ts";
import type { GeoSpatialQueryTO } from "../../api";
import type { Position } from "../../util/location/useGeolocation.ts";
import MultipleDropdown from "../select/MultipleDropdown.tsx";

const useStyles = makeStyles(() => ({
    root: {
        backgroundColor: "#f5f5f5",
        borderRadius: "12px",
        padding: "16px",
        boxShadow: "0 2px 8px rgba(0, 0, 0, 0.1)",
        display: "flex",
        flexDirection: "column",
        gap: "16px",
    },
    sliderContainer: {
        paddingLeft: "8px",
        paddingRight: "8px",
    },
    slider: {
        color: "#00796b",
    },
}));

interface Props {
    onApply: (filters: GeoSpatialQueryTO) => void;
    position: Position | null;
}

const VehicleFilterPanel: React.FC<Props> = (props) => {
    const { onApply, position } = props;

    const { classes } = useStyles();
    const [filters, setFilters] = useState<GeoSpatialQueryTO>({});

    useEffect(() => {
        setFilters({
            ...filters,
            maxDistance: 3000,
            userLocation: {
                coordinates: [position?.longitude, position?.latitude],
            },
        });
    }, [position]);

    const { cars } = useApiStates("cars");

    return (
        <Box className={classes.root}>
            <Typography variant="h6">Vehicle Filters</Typography>

            <Box>
                <Typography gutterBottom>Seats</Typography>
                <Slider
                    className={classes.slider}
                    value={[
                        filters.minSeats ?? 1,
                        filters.maxSeats ?? cars.maxSeats,
                    ]}
                    onChange={(_, newVal) => {
                        const [min, max] = newVal as number[];
                        setFilters({
                            ...filters,
                            minSeats: min,
                            maxSeats: max,
                        });
                    }}
                    valueLabelDisplay="auto"
                    step={1}
                    marks
                    min={1}
                    max={cars.maxSeats}
                />
            </Box>
            <TextField
                label="Max Distance (m)"
                type="number"
                inputProps={{ min: 0 }}
                value={filters.maxDistance ?? ""}
                onChange={(e) => {
                    const value = Number(e.target.value);
                    if (value >= 0 || e.target.value === "") {
                        setFilters({ ...filters, maxDistance: value });
                    }
                }}
            />

            <Box className={classes.sliderContainer}>
                <Typography gutterBottom>Price per Minute (€)</Typography>
                <Slider
                    className={classes.slider}
                    value={[
                        filters.minPricePerMinute ?? 0,
                        filters.maxPricePerMinute ?? cars.maxPricePerMinute,
                    ]}
                    onChange={(_, newVal) => {
                        const [min, max] = newVal as number[];
                        setFilters({
                            ...filters,
                            minPricePerMinute: min,
                            maxPricePerMinute: max,
                        });
                    }}
                    valueLabelDisplay="auto"
                    min={0}
                    max={cars.maxPricePerMinute}
                    step={0.01}
                />
            </Box>

            <MultipleDropdown
                label="Drivetrain"
                options={cars.driveTrains}
                value={filters.allowedDrivetrains ?? []}
                onChange={(newValue) =>
                    setFilters({ ...filters, allowedDrivetrains: newValue })
                }
            />

            <DefaultButton
                variant="contained"
                color="primary"
                onClick={() => onApply(filters)}
            >
                Apply Filters
            </DefaultButton>
        </Box>
    );
};

export default VehicleFilterPanel;
