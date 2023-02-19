import classNames from "classnames";
import React from "react";

import styles from './index.module.scss';

const Radio = ({
  checked,
  children,
  className,
  ...props
}) => {

  return (
    <label
      className={classNames(
        styles.radio,
        !!checked && styles.checked,
        className
      )}
    >
      <input type="radio" checked={checked} {...props} />{" "}
      <div className={styles.radioButton}>
        <span />
      </div>
      {children}
    </label>
  );
};

export default Radio;

