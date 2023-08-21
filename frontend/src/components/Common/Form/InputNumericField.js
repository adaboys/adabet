import React from 'react';
import { useField } from 'formik';
import classNames from 'classnames';

import styles from './InputTextField.module.scss';

const InputNumericField = ({
    iconLeft,
    iconRight,
    label,
    placeholder,
    disabled,
    className,
    type = 'text',
    onChange,
    onBlur,
    hideErrorMessage,
    sizeLg,
    fullBorder,
    autocomplete,
    pattern,
    ...props
}) => {
    const [field, meta, helpers] = useField(props);
    const isError = meta.touched && meta.error;

    const onChangeValue = (evt) => {
        let validValue = '';
        const regex = new RegExp(pattern, 'g');

        if (pattern) {
            const value = (evt.target.value || '').replaceAll(',', '')
            if (regex.test(value)) {
                validValue = value;
            }
            else {
                validValue = field.value;
            }
        }
        else {
            const value = (evt.target.value || '').replace(/\D/g, '');
            validValue = value === '' ? value : +value;
        }

        helpers?.setValue(validValue);
        onChange && onChange(validValue);
    }

    const formatDisplayValue = value => {
        const parts = value.toString().split('.');
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        return parts.join('.');
    }

    return (
        <div
            className={classNames(
                styles.inputTextField,
                isError && styles.error,
                className,
                fullBorder && styles.fullBorder,
                sizeLg && styles.sizeLg
            )}
        >
            {label && (
                <label>
                    {label}
                    {props.required && <span>*</span>}
                </label>
            )}
            <div className={styles.inputGroup}>
                {iconLeft && <span className={styles.iconLeft}>{iconLeft}</span>}
                <input
                    {...field}
                    value={formatDisplayValue(field.value)}
                    type={type}
                    placeholder={placeholder}
                    disabled={disabled}
                    className={classNames({
                        [styles.hasIconLeft]: !!iconLeft,
                        [styles.hasIconRight]: !!iconRight
                    })}
                    autocomplete={autocomplete}
                    onChange={onChangeValue}
                    autoComplete="off"
                    onBlur={onBlur}
                />
                {iconRight && <span className={styles.iconRight}>{iconRight}</span>}
            </div>
            {isError && !hideErrorMessage && (
                <div className={styles.feedback}>{meta.error}</div>
            )}
        </div>
    );
};

export default InputNumericField;
