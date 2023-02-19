import classNames from 'classnames';
import Button from '../Button';
import styles from './index.module.scss';

const ChipButton = ({ children, onClick, isActive, className }) => {
    return (
        <Button
            className={classNames(styles.chipButton,{ [styles.active]: isActive, [className]: !!className })}
            onClick={onClick || null}
        >
            {children}
        </Button>
    )
}

export default ChipButton;
