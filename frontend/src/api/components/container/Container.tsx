import React, {type PropsWithChildren} from "react";
import makeStyles from "../../../util/makeStyles.ts";

const useStyles = makeStyles(() => ({
    container: {
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "100vh",
        background: "linear-gradient(135deg, #e0f7fa, #4c838b)",
        padding: "2rem",
    },
}));

const Container: React.FC<PropsWithChildren> = (props) => {

    const { classes } = useStyles();

    return (
        <div className={classes.container}>
            {props.children}
        </div>
    );
};

export default Container;