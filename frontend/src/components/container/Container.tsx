import React, { type PropsWithChildren } from "react";
import makeStyles from "../../util/makeStyles.ts";

const useStyles = makeStyles(() => ({
    container: {
        display: "flex",
        flexDirection: "column",
        height: "100%",
        background: "linear-gradient(135deg, #e0f7fa, #4c838b)",
        paddingTop: "4rem",
    },
    center: {
        alignItems: "center",
        justifyContent: "center",
    },
    box: {
        display: "flex",
        flexDirection: "column",
        padding: "1rem",
        flexGrow: 1,
    },
}));

interface Props {
    center?: boolean;
    className?: string;
}

const Container: React.FC<PropsWithChildren<Props>> = (props) => {
    const { classes, cx } = useStyles();

    return (
        <div
            className={cx(
                classes.container,
                props.center ? classes.center : ""
            )}
        >
            <div className={classes.box}>{props.children}</div>
        </div>
    );
};

export default Container;
