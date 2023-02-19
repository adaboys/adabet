import React from 'react';
import { useField } from 'formik';
import classNames from 'classnames';

import styles from './CheckBoxField.module.scss';

const CheckboxField = ({
    children,
    name,
    disabled,
    ...rest
}) => {
    const [ field, meta ] = useField({ name });

    return (
        <label
            className={classNames({
                [styles.pcCheckboxField]: true,
                [styles.error]: meta.touched && meta.error,
            })}
        >
            <input
                type="checkbox"
                name={name}
                {...field}
                {...rest}
                disabled={disabled}
            />
            <span className={styles.checkmark} />
            {children}
            {/* {meta.touched && meta.error && (
                 <div className="input-feedback">{meta.error}</div>
            )} */}
        </label>
    );
};

export default CheckboxField;
