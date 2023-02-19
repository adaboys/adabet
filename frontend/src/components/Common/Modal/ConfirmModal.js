import { FormattedMessage } from "react-intl";

import { BasicModal, Button } from "@components/Common";
import { commonMessages } from "@constants/intl";

import styles from './ConfirmModal.module.scss';

const ConfirmModal = ({
    isOpen,
    onClose,
    onOk,
    title,
    content,
    loading
}) => {
    return (
        <BasicModal
            isOpen={isOpen}
            contentClassName={styles.confirmModal}
        >
            <h1 className={styles.title}>
                {title}
            </h1>
            <p className={styles.content}>
                {content}
            </p>
            <div className={styles.actions}>
                <Button className={styles.btnCancel} onClick={onClose} disabled={loading}>
                    <FormattedMessage {...commonMessages.no} />
                </Button>
                <Button className={styles.btnOk} onClick={onOk} loading={loading}>
                    <FormattedMessage {...commonMessages.yes} />
                </Button>
            </div>
        </BasicModal>
    )
}

export default ConfirmModal;