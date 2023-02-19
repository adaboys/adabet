import { useState } from 'react';
import * as Yup from 'yup';
import { useDispatch } from 'react-redux';
import { useIntl, defineMessages, FormattedMessage } from "react-intl";

import { BasicForm, InputTextField, ValidateSubmitButton } from "@components/Common";

import { validationMessages } from "@constants/intl";
import { accountActions } from '@redux/actions';
import { useNotification } from '@hooks';

import EmailIcon from "@assets/icons/sms.svg";

import styles from './RequestResetPasswordForm.module.scss';

const messages = defineMessages({
    yourPlayerName: 'Your player name',
    yourEmail: 'Your email',
    btnContinue: 'Continute',
    retypePassword: 'Retype Password',
    passwordNotMatch: 'The password and confirmation password do not match',
    registerSuccess: 'Register successful!',
    registerFailed: 'Register failed!',
    emailValidationFailed: 'Your email must be a valid email',
    errorUserExisted: 'User existed'
});

const RequestResetPasswordForm = ({ onRequestResetSuccess }) => {
    const intl = useIntl();
    const dispatch = useDispatch();
    const [isSubmitting, setIsSubmitting] = useState();
    const { showError } = useNotification();

    const onSubmitRegister = (values) => {
        setIsSubmitting(true);
        dispatch(accountActions.requestResetPassword({
            params: values,
            onCompleted: (response) => {
                if(response?.status === 200) {
                    onRequestResetSuccess(values.email);
                } else {
                    showError(response?.message || intl.formatMessage(messages.registerFailed));
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
                email: ''
            }}
            validationSchema={Yup.object().shape({
                email: Yup.string()
                    .required(intl.formatMessage(validationMessages.required))
                    .email(intl.formatMessage(messages.emailValidationFailed))
            })}
            onSubmit={onSubmitRegister}
            className={styles.requestResetPasswordForm}
        >
            <p className={styles.terms}>
                <FormattedMessage
                    key="note"
                    defaultMessage="Please enter your registered email address"
                />
            </p>

            <InputTextField
                placeholder={intl.formatMessage(messages.yourEmail)}
                name="email"
                iconLeft={<EmailIcon />}
            />

            <ValidateSubmitButton
                className={styles.btnRegister}
                text={intl.formatMessage(messages.btnContinue)}
                primary
                type="submit"
                loading={isSubmitting}
            />
        </BasicForm>
    );
};

export default RequestResetPasswordForm;
