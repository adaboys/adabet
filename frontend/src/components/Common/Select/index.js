import React from "react";
import ReactSelect, { components } from "react-select";

import ChevronDownIcon from '@assets/icons/chevron-down.svg';

import styles from './index.module.scss';

const selectStyles = (isError, customStyles = {}) => {
    const {
        placeholder,
        control,
        valueContainer,
        dropdownIndicator,
        loadingIndicator,
        indicatorsContainer,
        clearIndicator,
        ...rest
    } = customStyles

    const mergeStyles = (outsideStyle, insideStyle) => (provided, state) => ({
        ...provided,
        ...insideStyle(state),
        ...(outsideStyle && outsideStyle({ ...state, isError }))
    })

    return {
        placeholder: mergeStyles(placeholder, _ => ({
            color: '#BDBDBD',
        })),
        control: mergeStyles(control, ({ isFocused }) => ({
            lineHeight: '18px',
            padding: '0 16px',
            border: '1px solid #BDBDBD;',
            borderRadius: 0,
            // borderBottomColor: isError ? '#f33060' : isFocused ? '#0F62F9' : '#E5E5E5',
            boxShadow: 'none',
            background: 'transparent',
            color: 'red',
            '&:hover': {
                // borderBottomColor: isError ? '#f33060' : isFocused ? '#0F62F9' : '#E5E5E5',
            }
        })),
        valueContainer: mergeStyles(valueContainer, _ => ({
            paddingLeft: 0
        })),
        menu: (provider, state) => ({
            ...provider,
            // marginTop: '2px',
            background: '#000',
            '&:hover': {
                
            }
        }),
        option: (provided, state) => ({
            ...provided,
            // color: state.isSelected ? 'red' : 'blue',
            background: state.isSelected ? '#36373D' : '#000',
            '&:hover': {
                background: '#36373D'
            }
          }),
        singleValue: (provider, state) => ({
            color: '#fff'
        }),
        dropdownIndicator: mergeStyles(dropdownIndicator, ({ isFocused }) => ({
            // color: isError ? '#f33060' : isFocused ? '#0F62F9' : '#E5E5E5'
        })),
        loadingIndicator: mergeStyles(loadingIndicator, _ => ({
            // color: '#0F62F9'
        })),
        indicatorsContainer: mergeStyles(indicatorsContainer, _ => ({
            '> *': {
                padding: '8px 5px !important'
            }
        })),
        clearIndicator: mergeStyles(clearIndicator, ({ isFocused }) => ({
            stroke: isFocused ? '#adadad' : '#E5E5E5',
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

const DropdownIndicator = props => {
    return (
        <components.DropdownIndicator {...props}>
            <ChevronDownIcon style={{ stroke: '#4F4F4F', width: '18px' }} />
        </components.DropdownIndicator>
    );
};

const Select = ({
    value,
    onChange,
    clearable,
    clearValue,
    name,
    options,
    isOptionDisabled,
    customComponents = {},
    defaultValue,
    menuIsOpen,
    customStyles,
    optionLabelKey = "label",
    optionValueKey = "value",
    errors,
    ...props
}) => {
    const handleChange = (newValue) => {
        if (onChange) {
            if (name) onChange(newValue, name);
            else onChange(newValue);
        }
    };

    return (
        <div className={styles.select}>
            <ReactSelect
                defaultValue={defaultValue}
                onChange={handleChange}
                value={value}
                clearValue={clearValue}
                menuIsOpen={menuIsOpen}
                menuShouldScrollIntoView
                tabSelectsValue={false}
                getOptionLabel={option => option[optionLabelKey]}
                getOptionValue={option => option[optionValueKey]}
                openMenuOnFocus
                styles={selectStyles(false, customStyles)}
                options={options}
                isOptionDisabled={isOptionDisabled}
                placeholder=""
                isClearable={clearable}
                classNamePrefix={`select-${name}`}
                components={{
                    IndicatorSeparator: () => null,
                    DropdownIndicator,
                    ...customComponents
                }}
                {...props}
            />
        </div>
    );
};

export default Select;
