import React from "react";

const generateClassName = (prefix: string): string => {
    const array = new Uint8Array(9);
    window.crypto.getRandomValues(array);
    return `${prefix}-${Array.from(array).map(byte => byte.toString(36)).join('')}`;
};

const createCSS = (styles: React.CSSProperties): string =>
    Object.entries(styles)
        .map(([key, value]) => {
            if (
                typeof value === "object" &&
                value !== null &&
                !Array.isArray(value)
            ) {
                return `${key} { ${createCSS(value as React.CSSProperties)} }`;
            }
            return `${key.replace(/([A-Z])/g, "-$1").toLowerCase()}:${value};`;
        })
        .join("");

const makeStyles = <
    T extends Record<
        string,
        React.CSSProperties | ((props?: any) => React.CSSProperties)
    >,
>(
    styles: (props?: any) => T,
) => {
    return (props?: any) => {
        const classes = {} as Record<keyof T, string>;
        const stylesObj = styles(props || {});

        for (const key in stylesObj) {
            const className = generateClassName(key);
            const styleDefinition = stylesObj[key];
            const styleWithDynamicValues =
                typeof styleDefinition === "function"
                    ? styleDefinition(props || {})
                    : styleDefinition;

            const styleString = createCSS(
                styleWithDynamicValues as React.CSSProperties,
            );
            const styleElement = document.createElement("style");
            styleElement.textContent = `.${className} { ${styleString} }`;
            document.head.appendChild(styleElement);
            classes[key] = className;
        }

        const cx = (...classNames: (string | undefined | false)[]) =>
            classNames.filter(Boolean).join(" ");

        return {classes, cx};
    };
};

export default makeStyles;