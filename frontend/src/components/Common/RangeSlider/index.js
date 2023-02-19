import { useCallback, useRef } from "react";
import { useIntl } from "react-intl";

import styles from "./index.module.scss";

const RangeSlider = ({
    minValue,
    maxValue,
    minLabel,
    maxLabel,
    step = 1,
    onChange
}) => {

    const intl = useIntl();
    const rangeValueRef = useRef(null);

    const setValue = (evt) => {
        const range = evt.target;
        const rangeValue = rangeValueRef.current;
        const newValue = Number((range.value - range.min) * 100 / (range.max - range.min));
        const newPosition = 10 - (newValue * 0.2);
        rangeValue.innerHTML = `<span>${intl.formatNumber(range.value)}</span>`;
        rangeValue.style.left = `calc(${newValue}% + (${newPosition}px))`;
        onChange && onChange(range.value);
    };

    return (
        <div className={styles.rangeSlider}>
            {
                minLabel && maxLabel
                    ?
                    <div className={styles.boxMinmax}>
                        <span>{minLabel}</span><span>{maxLabel}</span>
                    </div>
                    :
                    null
            }

            <div className={styles.sliderGroup}>
                <input
                    onInput={setValue}
                    className={styles.rsRange}
                    type="range"
                    min={minValue}
                    max={maxValue}
                    defaultValue={minValue}
                    step={step}
                />
                <div ref={rangeValueRef} className={styles.rsValue}>
                    <span>{intl.formatNumber(minValue)}</span>
                </div>
            </div>
        </div>
    )
};

export default RangeSlider;
