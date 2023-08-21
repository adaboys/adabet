import { defineMessages, useIntl } from 'react-intl';
import { useState } from 'react';
import * as Yup from 'yup';
import { useDispatch } from 'react-redux';

import { InputTextField, ValidateSubmitButton, BasicForm } from '@components/Common';

import { commonMessages, validationMessages } from '@constants/intl';
import { useNotification } from '@hooks';
import { accountActions } from '@redux/actions';

import styles from './index.module.scss';

const messages = defineMessages({
    otpCode: 'OTP (check in your email and input here please!)',
    transferCoinFailed: 'Transfer coin failed!',
    transferCoinSuccess: 'Transfer coin successful!'
});

const ConfirmSendCoin = ({ onSendCoinSuccess }) => {
    const intl = useIntl();
    const dispath = useDispatch();
    const [isSubmitting, setIsSubmitting] = useState(false);

    const { showPopupError, showPopupSuccess } = useNotification();

    const onSubmit = (values) => {
        setIsSubmitting(true);
        dispath(accountActions.sendActualCoin({
            params: values,
            onCompleted: (result) => {
                if (result?.status === 200) {
                    showPopupSuccess(intl.formatMessage(messages.transferCoinSuccess));
                    onSendCoinSuccess(null);
                }
                else {
                    showPopupError(intl.formatMessage(messages.transferCoinFailed));
                }
                setIsSubmitting(false);
            },
            onError: (result) => {
                showPopupError(intl.formatMessage(messages.transferCoinFailed));
                setIsSubmitting(false);
            }
        }))
    }

    return (
        <BasicForm
            initialValues={{
                otp_code: ''
            }}
            validationSchema={Yup.object().shape({
                otp_code: Yup.string()
                    .required(intl.formatMessage(validationMessages.required))
            })}
            onSubmit={onSubmit}
        >

            <InputTextField
                label={intl.formatMessage(messages.otpCode)}
                name="otp_code"
            />

            <ValidateSubmitButton
                className={styles.btnUpdate}
                text={intl.formatMessage(commonMessages.confirm)}
                primary
                type="submit"
                loading={isSubmitting}
            />
        </BasicForm>
    )
}

export default ConfirmSendCoin;