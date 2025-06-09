import React, { useState } from "react";
import {
    Box,
    Button,
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    Slider,
    TextField,
    Typography,
} from "@mui/material";

export interface FilterValues {
    minSeats?: number;
    maxSeats?: number;
    minPrice?: number;
    maxPrice?: number;
    drivetrain?: string;
}

interface Props {
    onApply: (filters: FilterValues) => void;
}

const VehicleFilterPanel: React.FC<Props> = (props) => {
    const { onApply } = props;

    const [filters, setFilters] = useState<FilterValues>({});

    return (
        <Box display="flex" flexDirection="column" gap={2} p={2}>
            <Typography variant="h6">Vehicle Filter</Typography>

            <TextField
                label="Min. Sitze"
                type="number"
                value={filters.minSeats ?? ""}
                onChange={(e) =>
                    setFilters({ ...filters, minSeats: Number(e.target.value) })
                }
            />
            <TextField
                label="Max. Sitze"
                type="number"
                value={filters.maxSeats ?? ""}
                onChange={(e) =>
                    setFilters({ ...filters, maxSeats: Number(e.target.value) })
                }
            />

            <Slider
                value={[filters.minPrice ?? 0, filters.maxPrice ?? 1]}
                onChange={(_, newVal) => {
                    const [min, max] = newVal as number[];
                    setFilters({ ...filters, minPrice: min, maxPrice: max });
                }}
                valueLabelDisplay="auto"
                min={0}
                max={1}
                step={0.01}
            />
            <Typography variant="body2">Price Per Minute</Typography>

            <FormControl fullWidth>
                <InputLabel>Drive</InputLabel>
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

            <Button variant="contained" onClick={() => onApply(filters)}>
                Apply
            </Button>
        </Box>
    );
};

export default VehicleFilterPanel;
