import { defineMessages, FormattedMessage, useIntl } from "react-intl";

import { BasicModal, Button } from "@components/Common";
import { commonMessages } from "@constants/intl";
import WarningIcon from '@assets/icons/warning.svg';

import styles from './MessageModal.module.scss';
import classNames from "classnames";

const messages = defineMessages({
    error: 'Error',
    warning: 'Warning',
    success: 'Success'
})

const MessageModal = ({ overlay: { hide, context } }) => {
    const intl = useIntl();

    const type = context?.type || 'success';
    const message = context?.message;

    const getTitle = () => {
        if(context.title)
            return context.title;
        else if(type === 'error')
            return intl.formatMessage(messages.error);
        else if(type === 'warning')
            return intl.formatMessage(messages.warning);
        else
            return intl.formatMessage(messages.success);
    }
    
    return (
        <BasicModal
            isOpen={true}
            contentClassName={
                classNames(styles.messageModal, {
                    [styles.error]: type === 'error',
                    [styles.success]: type === 'success'
                })
            }
        >
            <div className={styles.icon}>
                <WarningIcon/>
            </div>
            <h1 className={styles.title}>
                {getTitle()}
                
            </h1>
            <p className={styles.content}>
                { message }
            </p>
            <div className={styles.actions}>
                <Button className={styles.btnOk} onClick={hide}>
                    <FormattedMessage {...commonMessages.ok}/>
                </Button>
            </div>
        </BasicModal>
    )
}

export default MessageModal;