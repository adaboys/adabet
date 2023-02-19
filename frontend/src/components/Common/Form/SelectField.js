import classNames from "classnames";
import { useField } from "formik";
import React from "react";
import Select, { components } from "react-select";
// import { Select } from "..";

import styles from './SelectField.module.scss';

import IconClose from '../../../assets/icons/close.svg';
import ChevronDownIcon from '@assets/icons/chevron-down.svg';

const ClearIndicator = props => {
    const {
        getStyles,
        innerProps: { ref, ...restInnerProps },
    } = props;

    return (
        <div
            {...restInnerProps}
            ref={ref}
            style={getStyles('clearIndicator', props)}
            className={styles.clearIndicatorIcon}
        >
            <IconClose stroke="inherit" />
        </div>
    );
};

const DropdownIndicator = props => {
    return (
      <components.DropdownIndicator {...props}>
        <ChevronDownIcon style={{marginRight: '14px'}} />
      </components.DropdownIndicator>
    );
};

const customStyles = (isError, outsideStyles = {}) => {
    const {
        placeholder,
        control,
        valueContainer,
        dropdownIndicator,
        loadingIndicator,
        indicatorsContainer,
        clearIndicator,
        ...rest
    } = outsideStyles

    const mergeStyles = (outsideStyle, insideStyle) => (provided, state) => ({
        ...provided,
        ...insideStyle(state),
        ...(outsideStyle && outsideStyle({ ...state, isError }))
    })

    return {
        placeholder: mergeStyles(placeholder, _ => ({
            fontSize: '0.938rem',
            lineHeight: '22px',
            color: '#B2BAC6'
        })),
        control: mergeStyles(control, ({ isFocused }) => ({
            border: '1px solid rgb(242, 242, 242)',
            borderRadius: 0,
            borderBottomColor: isError ? '#FD5A14' : 'gb(242, 242, 242)',
            boxShadow: 'none',
            '&:hover': {
                borderBottomColor: isError ? '#FD5A14' : 'gb(242, 242, 242)',
            }
        })),
        valueContainer: mergeStyles(valueContainer, _ => ({
            padding: '12px',
        })),
        // dropdownIndicator: mergeStyles(dropdownIndicator, ({ isFocused }) => ({
        //     color: isError ? '#f33060' : isFocused ? '#0F62F9' : '#E5E5E5'
        // })),
        loadingIndicator: mergeStyles(loadingIndicator, _ => ({
            color: '#0F62F9'
        })),
        indicatorsContainer: mergeStyles(indicatorsContainer, _ => ({
            '> *': {
                padding: '8px 5px !important'
            }
        })),
        clearIndicator: mergeStyles(clearIndicator, ({ isFocused }) => ({
            stroke: isFocused ? '#adadad': '#E5E5E5',
            "&:hover": {
                stroke: 'red'
            }
        })),
        ...(Object.keys(rest).reduce((rs, cur) => ({
            ...rs,
            [cur]: mergeStyles(rest[cur], _ => ({}))
        }), {}))
    }
}

const SelectField = ({
    label,
    required,
    className,
    name,
    options,
    optionLabelKey = 'label',
    optionValueKey = 'value',
    onChange,
    hideErrorMessage,
    selectStyles,
    ...rest
}) => {
    const [field, meta, helpers] = useField({ name });
    const preparedOptions = (options || []).map(option => ({ value: option[optionValueKey], label: option[optionLabelKey], option}));
    const isError = meta.touched && meta.error

    const onChangeValue = (item) => {
        helpers.setValue(item?.value || '');

        onChange && onChange(item);
    }

    return (
        <div
            className={classNames(styles.selectField, {
                [className]: !!className,
                [styles.error]: isError
            })}
        >
            {label && (
                <label>
                    {label}
                    {required && <span>*</span>}
                </label>
            )}
            <Select
                // classNamePrefix="select"
                components={{
                    ...components,
                    IndicatorSeparator: () => null,
                    ClearIndicator,
                    DropdownIndicator
                }}
                {...field}
                options={preparedOptions}
                value={preparedOptions.find(option => option.value === field.value) || ''}
                onChange={onChangeValue}
                styles={customStyles(isError, selectStyles)}
                {...rest}
            />
            {isError && !hideErrorMessage && (
                 <div className={styles.feedback}>{meta.error}</div>
            )}
        </div>
    )

}

export default SelectField;
