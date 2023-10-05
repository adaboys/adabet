import classNames from "classnames";
import { useField } from "formik";
import React from "react";
import Select, { components } from "react-select";
// import { Select } from "..";

import styles from './SelectField.module.scss';

import IconClose from '../../../assets/icons/close.svg';
import DividerIcon from '@assets/icons/breadcrumb-divider.svg';

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
        <DividerIcon className={styles.indicator} />
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
        menu,
        option,
        singleValue,
        ...rest
    } = outsideStyles

    const mergeStyles = (outsideStyle, insideStyle) => (provided, state) => ({
        ...provided,
        ...insideStyle(state),
        ...(outsideStyle && outsideStyle({ ...state, isError }))
    })

    return {
        placeholder: mergeStyles(placeholder, _ => ({
            fontSize: '14px',
            lineHeight: '21px',
            color: 'white'
        })),
        control: mergeStyles(control, ({ isFocused }) => ({
            backgroundColor: 'rgba(16, 20, 43)',
            border: 'none',
            borderRadius: 32,
            boxShadow: 'none',
        })),
        valueContainer: mergeStyles(valueContainer, _ => ({
            padding: '7px 24px',
        })),
        menu: mergeStyles(menu, _ => ({
            backgroundColor: 'rgba(16, 20, 43)',
        })),
        option: mergeStyles(option, _ => ({
            backgroundColor: 'transparent',
            '&:hover': {
                backgroundColor: 'rgba(0, 0, 0, 0.6)',
            },
            padding: '7px 24px',
        })),
        singleValue: mergeStyles(singleValue, _ => ({
            fontSize: '14px',
            lineHeight: '21px',
            color: 'white'
        })),
        loadingIndicator: mergeStyles(loadingIndicator, _ => ({
            color: '#0F62F9'
        })),
        indicatorsContainer: mergeStyles(indicatorsContainer, _ => ({
            '> *': {
                paddingRight: '24px'
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
    customComponents,
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
                    DropdownIndicator,
                    ...customComponents,
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

SelectField.components = components;

export default SelectField;
