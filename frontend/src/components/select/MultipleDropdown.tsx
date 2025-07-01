import React, { useCallback } from "react";
import {
    Box,
    Chip,
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    type SelectChangeEvent,
} from "@mui/material";
import { makeStyles } from "tss-react/mui";

const useStyles = makeStyles()(() => ({
    box: {
        display: "flex",
        flexWrap: "wrap",
        gap: 0.5,
    },
}));

interface Props {
    label: string;
    options: string[];
    value: string[];
    onChange: (value: string[]) => void;
}

const MultipleDropdown: React.FC<Props> = (props) => {
    const { label, options, value, onChange } = props;

    const { classes } = useStyles();

    const handleChange = useCallback(
        (event: SelectChangeEvent<string[]>) => {
            onChange(event.target.value as string[]);
        },
        [onChange]
    );

    return (
        <FormControl fullWidth>
            <InputLabel>{label}</InputLabel>
            <Select
                multiple
                value={value}
                onChange={handleChange}
                renderValue={(selected) => (
                    <Box className={classes.box}>
                        {(selected as string[]).map((val) => (
                            <Chip key={val} label={val} />
                        ))}
                    </Box>
                )}
            >
                {options.map((option, index) => (
                    <MenuItem key={`${option}-${index}`} value={option}>
                        {option}
                    </MenuItem>
                ))}
            </Select>
        </FormControl>
    );
};

export default MultipleDropdown;
