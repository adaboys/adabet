import classNames from 'classnames';
import Modal from 'react-modal';
import styles from './BasicModal.module.scss';

const BasicModal = ({ isOpen, children, contentClassName, overlayClassName, ...props }) => {

    return (
        <Modal
            isOpen={isOpen}
            contentLabel="Basic Modal"
            ariaHideApp={false}
            // htmlOpenClassName="htmlOpenClassName"
            bodyOpenClassName={classNames(styles.basicModal, isOpen && styles.open)}
            // portalClassName="portalClassName"
            overlayClassName={classNames(styles.overlay, overlayClassName)}
            className={classNames(styles.content, contentClassName)}
            {...props}
        >
            {children}
        </Modal>
    )
}

export default BasicModal;
