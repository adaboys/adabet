import React from 'react';

import styles from './index.module.scss';

import AddIcon from '@assets/icons/add.svg';
import MinusIcon from '@assets/icons/minus.svg';
import classNames from 'classnames';

const NumericStepper = ({
    value,
    onIncrease,
    onDecrease,
    minValue,
    maxValue,
    className,
    disabled,
}) => {
    const disableDecrease = disabled || minValue !== undefined ? value <= minValue : false;
    const disableIncrease = disabled || maxValue !== undefined ? value >= maxValue : false;

    return (
        <div className={classNames(styles.numericStepper, className)}>
            <button
                disabled={disableDecrease}
                onClick={() => !disableDecrease && onDecrease?.()}
            >
                <MinusIcon />
            </button>
            <span>{value}</span>
            <button
                disabled={disableIncrease}
                onClick={() => !disableIncrease && onIncrease?.()}
            >
                <AddIcon />
            </button>
        </div>
    );
};

export default NumericStepper;