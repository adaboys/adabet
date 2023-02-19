import React from 'react';
import classNames from 'classnames';
import ClipLoader from "react-spinners/ClipLoader";
import styles from './index.module.scss';

const Button = ({
    primary = true,
    secondary,
    large,
    gray,
    type,
    onClick,
    children,
    className,
    loading,
    disabled,
    icon,
    style
}) => (
    <button
        disabled={disabled || loading}
        className={classNames({
            [styles.button]: true,
            [styles.primary]: primary,
            [styles.secondary]: secondary,
            [styles.large]: large,
            [styles.gray]: gray,
            [styles.disabled]: disabled || loading,
            [className]: !!className
        })}
        style={style}
        type={type}
        onClick={onClick}
    >
        {
            icon
            ?
            <span className={styles.icon}>{icon}</span>
            :
            null
        }
        {loading && <ClipLoader size={20} color="#fff" css={{marginRight: '7px'}}/>}
        {children}

    </button>
)

export default Button;
