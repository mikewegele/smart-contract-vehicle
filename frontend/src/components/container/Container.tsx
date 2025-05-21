import React, {type PropsWithChildren} from "react";
import makeStyles from "../../util/makeStyles.ts";

const useStyles = makeStyles(() => ({
    container: {
        display: "flex",
        flexDirection: "column",
        minHeight: "100vh",
        background: "linear-gradient(135deg, #e0f7fa, #4c838b)",
        padding: "2rem",
    },
    center: {
        alignItems: "center",
        justifyContent: "center",
    }
}));

interface Props {
    center?: boolean;
}

const Container: React.FC<PropsWithChildren<Props>> = (props) => {

    const { classes, cx } = useStyles();

    return (
        <div className={cx(classes.container, props.center ? classes.center : "")}>
            {props.children}
        </div>
    );
};

export default Container;