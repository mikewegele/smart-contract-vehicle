import React from "react";
import { TextField } from "@mui/material";
import type { TextFieldProps } from "@mui/material";
import makeStyles from "../../util/makeStyles.ts";

const useStyles = makeStyles(() => ({
    input: {
        borderRadius: "8px",
        transition: "border-color 0.2s",
        "& .MuiOutlinedInput-root": {
            borderRadius: "8px",
        },
        margin: "20px",
    },
}));

type Props = TextFieldProps;

const DefaultTextField: React.FC<Props> = (props) => {
    const { classes } = useStyles();
    return <TextField className={classes.input} {...props} />;
};

export default DefaultTextField;
