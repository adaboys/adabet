import { useContext, useState } from 'react';
import * as Yup from 'yup';
import { useDispatch } from 'react-redux';
import { useIntl, defineMessages, FormattedMessage } from "react-intl";

import { BasicForm, InputTextField, PasswordField, ValidateSubmitButton } from "@components/Common";
import { commonMessages, validationMessages } from "@constants/intl";
import { accountActions } from '@redux/actions';
import { useNotification } from '@hooks';

import styles from './ConfirmResetPasswordForm.module.scss';
import { OverlayContext } from '@hocs';
import { overlayTypes } from '@constants';

const messages = defineMessages({
    otp: 'OTP',
    resetPassword: 'Reset password',
    retypePassword: 'Retype Password',
    passwordNotMatch: 'The password and confirmation password do not match',
    resetPasswordSuccess: 'Reset password successful!',
    resetPasswordFailed: 'Reset password failed!',
    errorInvalidOtp: 'Otp is invalid',
    playerNameSameValueEmail: 'The email and player name must be not same value'
});

const ConfirmResetPasswordForm = ({ email }) => {
    const overlay = useContext(OverlayContext);
    const intl = useIntl();
    const dispatch = useDispatch();
    const [isSubmitting, setIsSubmitting] = useState();
    const { showError, showSuccess } = useNotification();

    const onSubmit = (values) => {
        setIsSubmitting(true);
        dispatch(accountActions.confirmResetPassword({
            params: {
                email,
                otp_code: values.otpCode,
                new_pass: values.password,
            },
            onCompleted: (response) => {
                if (response?.status === 200) {
                    showSuccess(intl.formatMessage(messages.resetPasswordSuccess));
                    overlay.show(overlayTypes.LOGIN);
                } else if (response?.code === 'invalid_resetPassword') {
                    showError(intl.formatMessage(messages.errorInvalidOtp));
                } else {
                    showError(response?.message || intl.formatMessage(messages.resetPasswordFailed));
                }
                setIsSubmitting(false);
            },
            onError: (err) => {
                console.log(err);
                showError(intl.formatMessage(messages.resetPasswordFailed));
                setIsSubmitting(false);
            }
        }))
    }

    return (
        <BasicForm
            initialValues={{
                otpCode: '',
                password: '',
                rePassword: ''
            }}
            validationSchema={Yup.object().shape({
                otpCode: Yup.string()
                    .required(intl.formatMessage(validationMessages.required)),
                password: Yup.string()
                    .required(intl.formatMessage(validationMessages.required)),
                rePassword: Yup.string().oneOf([Yup.ref('password'), null], intl.formatMessage(messages.passwordNotMatch))
                    .required(intl.formatMessage(validationMessages.required))
            })}
            onSubmit={onSubmit}
            className={styles.confirmResetPasswordForm}
        >
            <p className={styles.terms}>
                <FormattedMessage
                    key="resetPasswordDescription"
                    defaultMessage="We have sent a confirmation code to {email}"
                    values={{ email }}
                />
            </p>

            <InputTextField
                placeholder={intl.formatMessage(messages.otp)}
                name="otpCode"
            />

            <PasswordField
                placeholder={intl.formatMessage(commonMessages.password)}
                type="password"
                autoComplete="off"
                name="password"
            />

            <PasswordField
                placeholder={intl.formatMessage(messages.retypePassword)}
                type="password"
                autoComplete="off"
                name="rePassword"
            />

            <ValidateSubmitButton
                className={styles.btnLogin}
                text={intl.formatMessage(messages.resetPassword)}
                primary
                type="submit"
                loading={isSubmitting}
            />
        </BasicForm>
    );
};

export default ConfirmResetPasswordForm;
