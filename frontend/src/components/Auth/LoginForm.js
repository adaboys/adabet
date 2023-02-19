import React, { useState, useContext } from 'react';
import * as Yup from 'yup';
import { useIntl, defineMessages, FormattedMessage } from "react-intl";

import { BasicForm, InputTextField, PasswordField, ValidateSubmitButton } from "@components/Common";
import { overlayTypes } from '@constants';
import { OverlayContext } from '@hocs';
import { validationMessages } from '@constants/intl';

import EmailIcon from "@assets/icons/sms.svg";

import styles from './LoginForm.module.scss';
import { commonMessages } from "@constants/intl";

const messages = defineMessages({
    yourEmail: 'Your email',
    emailValidationFailed: 'Your email must be a valid email',
});

const LoginForm = ({ onLogin, isSubmitting }) => {
    const intl = useIntl();
    const overlay = useContext(OverlayContext);

    // const loginRedirectUrl = query?.redirectTo;

    const onShowRegister = () => {
        overlay.show(overlayTypes.REGISTER);
    }

    const onShowResetPassword = () => {
        overlay.show(overlayTypes.RESET_PASSWORD);
    }

    return (
        <BasicForm
            initialValues={{
                email: '',
                password: '',
            }}
            onSubmit={onLogin}
            validationSchema={Yup.object().shape({
                email: Yup.string()
                    .required(intl.formatMessage(validationMessages.required))
                    .email(intl.formatMessage(messages.emailValidationFailed)),
                password: Yup.string()
                    .required(intl.formatMessage(validationMessages.required)),
            })}
            className={styles.loginForm}>

            <InputTextField
                placeholder={intl.formatMessage(messages.yourEmail)}
                name="email"
                iconLeft={<EmailIcon />}
            />

            <PasswordField placeholder={intl.formatMessage(commonMessages.password)} name="password"/>
            <div className={styles.actions}>
                <a onClick={onShowResetPassword}>
                    <FormattedMessage key="forgotPassword" defaultMessage="Forgot password?" />
                </a>
            </div>
            <ValidateSubmitButton
                className={styles.btnLogin}
                text={intl.formatMessage(commonMessages.login)}
                primary
                type="submit"
                loading={isSubmitting}
            />
        </BasicForm>
    );
};

export default LoginForm;
