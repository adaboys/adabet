import { useState, useRef } from 'react';
import { useField } from 'formik';
import classNames from 'classnames';
import PasswordIcon from '@assets/icons/key.svg';
import EyeSlashIcon from '@assets/icons/eye-slash.svg';
import EyeIcon from '@assets/icons/eye.svg';

import styles from './PasswordField.module.scss';

const INPUT_TYPE_TEXT = 'text';
const INPUT_TYPE_PASSWORD = 'password';

const PasswordField = ({
    label,
    placeholder,
    disabled,
    className,
    onChange,
    hideErrorMessage,
    hideErrorAnimation,
    sizeLg,
    fullBorder,
    autoComplete,
    required,
    ...props
}) => {
    const [field, meta, helpers] = useField(props);
    const isError = meta.touched && meta.error;
    const [type, setType] = useState(INPUT_TYPE_PASSWORD);

    const onChangeValue = (evt) => {
        const value = evt.target.value;
        helpers?.setValue(value || '');
        onChange && onChange(value);
    }

    const onShowPassword = () => {
        if (field.value) {
            const newType = type === INPUT_TYPE_PASSWORD ? INPUT_TYPE_TEXT : INPUT_TYPE_PASSWORD;
            setType(newType);
        }
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
                    {required && <span>*</span>}
                </label>
            )}
            <div className={styles.inputGroup}>
                <span className={styles.iconLeft}><PasswordIcon /></span>
                <input
                    {...field}
                    type={type}
                    autoComplete="off"
                    placeholder={placeholder}
                    disabled={disabled}
                    className={classNames(styles.hasIconLeft, styles.hasIconRight, {
                        [styles.noAnimation]: !!hideErrorAnimation
                    })}
                    onChange={onChangeValue}
                />
                <span className={styles.iconRight} onClick={onShowPassword}>
                    {
                        type === INPUT_TYPE_PASSWORD
                            ?
                            <EyeSlashIcon />
                            :
                            <EyeIcon />
                    }
                </span>
            </div>
            {isError && !hideErrorMessage && (
                <div className={styles.feedback}>{meta.error}</div>
            )}
        </div>
    );
};

export default PasswordField;
