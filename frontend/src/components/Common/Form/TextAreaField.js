import React from 'react';
import { useField } from 'formik';
import classNames from 'classnames';

import styles from './InputTextField.module.scss';

const TextAreaField = ({
    iconLeft,
    iconRight,
    label,
    placeholder,
    disabled,
    className,
    type = 'text',
    // onChange,
    hideErrorMessage,
    sizeLg,
    fullBorder,
    ...props
}) => {
    const [ field, meta, helpers ] = useField(props);
    const isError = meta.touched && meta.error

    // const onChangeValue = (evt) => {
    //     const value = evt.target.value;
    //     helpers.setValue(value || '');
    //     onChange && onChange(value);
    // }

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
                {
                    iconLeft
                    ?
                    <span className={styles.iconLeft}>{iconLeft}</span>
                    :
                    null
                }
                <textarea
                    {...field}
                    type={type}
                    placeholder={placeholder}
                    disabled={disabled}
                    className={classNames({
                        [className]: !!className,
                        [styles.hasIconLeft]: !!iconLeft,
                        [styles.hasIconRight]: !!iconRight
                    })}
                    // onChange={onChangeValue}
                >
                    
                </textarea> 
                {
                    iconRight
                    ?
                    <span className={styles.iconRight}>{iconRight}</span>
                    :
                    null
                }
            </div>
            {isError && !hideErrorMessage && (
                 <div className={styles.feedback}>{meta.error}</div>
            )}
        </div>
    );
};

export default TextAreaField;
