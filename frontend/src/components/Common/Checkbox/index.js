import classNames from "classnames";
import React from "react";

import styles from './index.module.scss';

const ENTER_KEY = 13;
const SPACE_KEY = 32;

const Checkbox = ({
    name,
    checked,
    onChange = () => null,
    children,
    className,
    ...props
}) => {
    const ref = React.useRef(null);

    return (
        <div className={classNames(styles.checkbox, { [className]: !!className })}
            ref={ref}
            onClick={evt => {
                evt.preventDefault();
                onChange(evt);
                if (ref.current) {
                    ref.current.blur();
                }
            }}
        >
            <label className={styles.label}>
                <input
                    {...props}
                    tabIndex={-1}
                    type="checkbox"
                    name={name}
                    checked={checked}
                    readOnly
                />
                <div
                    ref={ref}
                    tabIndex={0}
                    onKeyDown={evt => {
                        if (evt.which === SPACE_KEY || evt.which === ENTER_KEY) {
                            evt.preventDefault();
                            onChange(evt);
                        }
                    }}
                >
                    <span />
                </div>
            </label>
            {children}
        </div>
    );
};

export default Checkbox;
