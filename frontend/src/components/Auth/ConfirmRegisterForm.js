import { useState } from 'react';
import * as Yup from 'yup';
import { useDispatch } from 'react-redux';
import { useIntl, defineMessages, FormattedMessage } from "react-intl";

import { BasicForm, InputTextField, ValidateSubmitButton } from "@components/Common";
import { commonMessages, validationMessages } from "@constants/intl";
import { accountActions } from '@redux/actions';
import { useNotification } from '@hooks';

import PasswordIcon from "@assets/icons/key.svg";
import EyeIcon from "@assets/icons/eye.svg";
import EyeSlashIcon from "@assets/icons/eye-slash.svg";
import UserIcon from "@assets/icons/user.svg";

import styles from './ConfirmRegisterForm.module.scss';

const messages = defineMessages({
    yourPlayerName: 'Your player name',
    otp: 'OTP',
    register: 'Register',
    retypePassword: 'Retype Password',
    passwordNotMatch: 'The password and confirmation password do not match',
    registerSuccess: 'Register successful!',
    registerFailed: 'Register failed!',
    errorInvalidOtp: 'Otp is invalid',
    playerNameSameValueEmail: 'The email and player name must be not same value'
});

const ConfirmRegisterForm = ({ email, hide, externalWallet }) => {
    const intl = useIntl();
    const dispatch = useDispatch();
    const [isSubmitting, setIsSubmitting] = useState();
    const { showError, showSuccess } = useNotification();

    const onSubmitConfirmRegister = (values) => {
        setIsSubmitting(true);
        const confirmRegisterAction = externalWallet ? accountActions.verifyLoginWallet : accountActions.confirmRegister;
        dispatch(confirmRegisterAction({
            params: {
                email,
                ...(externalWallet ? { ...externalWallet, request_otp: false, otp: values.otpCode, } : { otp_code: values.otpCode })
            },
            onCompleted: (response) => {
                if (response?.status === 200) {
                    showSuccess(intl.formatMessage(messages.registerSuccess));
                    hide();
                }
                else if (response?.code === 'invalid_register') {
                    showError(intl.formatMessage(messages.errorInvalidOtp));
                }
                else {
                    showError(intl.formatMessage(messages.registerFailed));
                }
                setIsSubmitting(false);
            },
            onError: (err) => {
                console.log(err);
                showError(intl.formatMessage(messages.registerFailed));
                setIsSubmitting(false);
            }
        }))
    }

    return (
        <BasicForm
            initialValues={{
                otpCode: ''
            }}
            validationSchema={Yup.object().shape({
                otpCode: Yup.string()
                    .required(intl.formatMessage(validationMessages.required))
            })}
            onSubmit={onSubmitConfirmRegister}
            className={styles.confirmRegisterForm}
        >
            <p className={styles.description}>
                <FormattedMessage
                    key="registerDescription"
                    defaultMessage="We have sent a confirmation code to {email}"
                    values={{ email }}
                />
            </p>

            <InputTextField
                placeholder={intl.formatMessage(messages.otp)}
                name="otpCode"
            />

            <ValidateSubmitButton
                className={styles.btnLogin}
                text={intl.formatMessage(messages.register)}
                primary
                type="submit"
                loading={isSubmitting}
            />
        </BasicForm>
    );
};

export default ConfirmRegisterForm;
