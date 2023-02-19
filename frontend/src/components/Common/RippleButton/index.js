import classNames from "classnames";
import useRipple from "useripple"

import styles from './index.module.scss';

const RippleButton = ({ children, className }) => {
    const [addRipple, ripples] = useRipple({ background: "rgba(255, 255, 255, 0.7)" })
    return (
        <div className={classNames(styles.rippleButton, { [className]: !!className })} onClick={addRipple}>
            {children}
            {ripples}
        </div>

    )
};

export default RippleButton;
