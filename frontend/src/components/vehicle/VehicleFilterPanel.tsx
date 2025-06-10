import {
    Box,
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    Slider,
    TextField,
    Typography,
} from "@mui/material";
import makeStyles from "../../util/makeStyles";
import React, { useMemo, useState } from "react";
import DefaultButton from "../button/DefaultButton.tsx";
import useApiStates from "../../util/useApiStates.ts";

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

export interface FilterValues {
    minSeats?: number;
    maxSeats?: number;
    minPrice?: number;
    maxPrice?: number;
    distance?: number;
    drivetrain?: string;
}

interface Props {
    onApply: (filters: FilterValues) => void;
}

const VehicleFilterPanel: React.FC<Props> = ({ onApply }) => {
    const { classes } = useStyles();
    const [filters, setFilters] = useState<FilterValues>({});

    const { cars } = useApiStates("cars");

    const maxPossibleSeats = useMemo(() => {
        if (!cars.value || cars.value.length === 0) return 10;
        return Math.max(...cars.value.map((car) => car.seats || 0));
    }, [cars.value]);

    const maxPricePerMinutes = useMemo(() => {
        if (!cars.value || cars.value.length === 0) return 5;
        return Math.max(...cars.value.map((car) => car.pricePerMinute || 0));
    }, [cars.value]);

    return (
        <Box className={classes.root}>
            <Typography variant="h6">Vehicle Filters</Typography>

            <Box>
                <Typography gutterBottom>Seats</Typography>
                <Slider
                    className={classes.slider}
                    value={[
                        filters.minSeats ?? 1,
                        filters.maxSeats ?? maxPossibleSeats,
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
                    max={maxPossibleSeats}
                />
            </Box>
            <TextField
                label="Max Distance (km)"
                type="number"
                inputProps={{ min: 0 }}
                value={filters.distance ?? ""}
                onChange={(e) => {
                    const value = Number(e.target.value);
                    if (value >= 0 || e.target.value === "") {
                        setFilters({ ...filters, distance: value });
                    }
                }}
            />

            <Box className={classes.sliderContainer}>
                <Typography gutterBottom>Price per Minute (€)</Typography>
                <Slider
                    className={classes.slider}
                    value={[filters.minPrice ?? 0, filters.maxPrice ?? 1]}
                    onChange={(_, newVal) => {
                        const [min, max] = newVal as number[];
                        setFilters({
                            ...filters,
                            minPrice: min,
                            maxPrice: max,
                        });
                    }}
                    valueLabelDisplay="auto"
                    min={0}
                    max={maxPricePerMinutes}
                    step={0.01}
                />
            </Box>

            <FormControl fullWidth>
                <InputLabel>Drivetrain</InputLabel>
                <Select
                    value={filters.drivetrain ?? ""}
                    onChange={(e) =>
                        setFilters({ ...filters, drivetrain: e.target.value })
                    }
                >
                    <MenuItem value="">All</MenuItem>
                    <MenuItem value="Front-Wheel Drive (FWD)">
                        Front-Wheel Drive (FWD)
                    </MenuItem>
                    <MenuItem value="Rear-Wheel Drive (RWD)">
                        Rear-Wheel Drive (RWD)
                    </MenuItem>
                    <MenuItem value="All-Wheel Drive (AWD)">
                        All-Wheel Drive (AWD)
                    </MenuItem>
                    <MenuItem value="Four-Wheel Drive (4WD)">
                        Four-Wheel Drive (4WD)
                    </MenuItem>
                </Select>
            </FormControl>

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
