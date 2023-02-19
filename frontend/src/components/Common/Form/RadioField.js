import classNames from 'classnames';
import { useField } from 'formik';
import React from 'react';

import Radio from '../Radio';
import styles from './RadioField.module.scss';

const RadioField = ({
    children,
    className,
    onChange,
    ...props
}) => {
    const [ field, meta ] = useField(props);

    const isChecked = meta.value === props.value

    const handleChange = e => {
        field.onChange(e)
        onChange && onChange(e)
    }

    return (
        <div className={classNames(
            styles.radioField,
            className
        )}>
            <Radio
                checked={isChecked}
                {...field}
                {...props}
                onChange={handleChange}
            >
                {children}
            </Radio>
        </div>
    );
};

export default RadioField;