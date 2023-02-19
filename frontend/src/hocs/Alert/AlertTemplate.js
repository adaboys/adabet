import classNames from "classnames";
import CloseIcon from "@assets/icons/close.svg";
import styles from './AlertTemplate.module.scss';

const AlertTemplate = ({ message, options, close, style }) => {
    return (
        <div
            style={style}
            className={classNames(
                styles.alert, {
                [styles.alertSuccess]: options.type === 'success',
                [styles.alertError]: options.type === 'error'
            })}>
            {message ? <div className={styles.content}>{message}</div> : null}
            <CloseIcon onClick={close} className={styles.closeIcon} />
        </div>
    )
}

export default AlertTemplate;