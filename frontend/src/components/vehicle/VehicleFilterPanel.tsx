import { Box, Collapse, Slider, TextField, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import DefaultButton from "../button/DefaultButton.tsx";
import useApiStates from "../../util/useApiStates.ts";
import type { GeoSpatialQueryTO } from "../../api";
import type { Position } from "../../util/location/useGeolocation.ts";
import MultipleDropdown from "../select/MultipleDropdown.tsx";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import { makeStyles } from "tss-react/mui";

const useStyles = makeStyles()(() => ({
    root: {
        background: "rgba(245, 245, 245, 0.7)",
        borderRadius: "12px",
        padding: "16px",
        boxShadow: "0 8px 24px rgba(0,0,0,0.12)",
        backdropFilter: "blur(12px)",
        border: "1px solid rgba(255, 255, 255, 0.6)",
        display: "flex",
        flexDirection: "column",
        gap: "16px",
    },
    sliderContainer: {
        padding: "8px",
    },
    advanced: {
        color: "#34495e",
        background: "rgba(255, 255, 255, 0.25)",
        backdropFilter: "blur(10px)",
        WebkitBackdropFilter: "blur(10px)",
        borderRadius: "16px",
        boxShadow: "0 4px 20px rgba(0, 0, 0, 0.1)",
        marginTop: "12px",
        textAlign: "center",
        alignItems: "center",
        border: "none",
        "&:hover": {
            background: "rgba(255, 255, 255, 0.25)",
            boxShadow:
                "inset 0 1px 0 rgba(255,255,255,0.9), 0 10px 14px rgba(0,0,0,0.2)",
            transform: "translateY(-2px)",
        },

        "&:active": {
            background: "rgba(255, 255, 255, 0.25)",
            boxShadow: "inset 0 2px 6px rgba(0,0,0,0.25)",
            transform: "translateY(1px)",
        },
    },
    slider: {
        color: "#00796b",
        "& .MuiSlider-rail": {
            opacity: 0.3,
            backgroundColor: "rgba(0, 121, 107, 0.3)",
            backdropFilter: "blur(8px)",
            borderRadius: 4,
        },
        "& .MuiSlider-track": {
            backgroundColor: "rgba(0, 121, 107, 0.7)",
            borderRadius: 4,
            backdropFilter: "blur(8px)",
        },
        "& .MuiSlider-thumb": {
            width: "28px",
            height: "20px",
            borderRadius: "6px",
            "&:hover, &.Mui-focusVisible": {
                boxShadow:
                    "inset 0 0 3px rgba(255, 255, 255, 0.4), 0 0 10px rgba(0, 121, 107, 0.6)",
            },
            "&.Mui-active": {
                backgroundColor: "rgba(255, 255, 255, 0.02)",
                backdropFilter: "blur(1px)",
                boxShadow: `
            inset 0 0 4px rgba(255, 255, 255, 0.3),
            0 0 12px rgba(0, 121, 107, 0.6),
            0 0 2px rgba(255, 255, 255, 0.2)
        `,
                border: "1px solid rgba(255, 255, 255, 0.3)",
            },
        },
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

    const [showAdvanced, setShowAdvanced] = useState(false);

    return (
        <Box className={classes.root}>
            <Typography variant="h6">Vehicle Filters</Typography>

            <Box>
                <Typography gutterBottom>Seats</Typography>
                <Slider
                    className={classes.slider}
                    value={[filters.minSeats ?? 1, filters.maxSeats ?? 6]}
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
                    max={6}
                />
            </Box>
            <TextField
                label="Max Distance (m)"
                type="number"
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
                        filters.maxPricePerMinute ?? 1,
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
                    max={1}
                    step={0.01}
                />
            </Box>

            <MultipleDropdown
                label="Drivetrain"
                options={cars.driveTrains.map(
                    (driveTrain) => driveTrain.name ?? ""
                )}
                value={filters.allowedDrivetrains ?? []}
                onChange={(newValue) =>
                    setFilters({ ...filters, allowedDrivetrains: newValue })
                }
            />
            <DefaultButton
                buttonclassname={classes.advanced}
                onClick={() => setShowAdvanced(!showAdvanced)}
                endIcon={
                    <ExpandMoreIcon
                        style={{
                            transform: showAdvanced
                                ? "rotate(180deg)"
                                : "rotate(0deg)",
                            transition: "transform 0.3s",
                        }}
                    />
                }
            >
                Advanced Options
            </DefaultButton>

            <Collapse in={showAdvanced} timeout="auto" unmountOnExit>
                <MultipleDropdown
                    label="Fuel Type"
                    options={cars.fuelTypes.map((f) => f.name ?? "")}
                    value={filters.allowedFueltypes ?? []}
                    onChange={(newVal) =>
                        setFilters({ ...filters, allowedFueltypes: newVal })
                    }
                />

                <Box className={classes.sliderContainer}>
                    <Typography gutterBottom>
                        Min Remaining Reach (km)
                    </Typography>
                    <Slider
                        className={classes.slider}
                        value={filters.minRemainingReach ?? 0}
                        onChange={(_, newVal) =>
                            setFilters({
                                ...filters,
                                minRemainingReach: newVal as number,
                            })
                        }
                        valueLabelDisplay="auto"
                        min={0}
                        max={600}
                        step={10}
                    />
                </Box>
            </Collapse>

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
