import { useState } from 'react';
import * as Yup from 'yup';
import { useDispatch } from 'react-redux';
import { useIntl, defineMessages, FormattedMessage } from "react-intl";

import { BasicForm, InputTextField, PasswordField, Checkbox, ValidateSubmitButton } from "@components/Common";

import { validationMessages, commonMessages } from "@constants/intl";
import { accountActions } from '@redux/actions';
import { useNotification } from '@hooks';

import EmailIcon from "@assets/icons/sms.svg";
import UserIcon from "@assets/icons/user.svg";

import styles from './RegisterForm.module.scss';

const messages = defineMessages({
    yourName: 'Your name',
    emailAddress: 'Email address',
    btnContinue: 'Continute',
    retypePassword: 'Retype Password',
    passwordNotMatch: 'The password and confirmation password do not match',
    registerSuccess: 'Register successful!',
    registerFailed: 'Register failed!',
    emailValidationFailed: 'Your email must be a valid email',
    errorUserExisted: 'User existed'
});

const RegisterForm = ({ onRegisterSuccess, externalWallet }) => {
    const intl = useIntl();
    const dispatch = useDispatch();
    const [isSubmitting, setIsSubmitting] = useState();
    const [acceptTerm, setIsAcceptTerm] = useState();
    const { showError } = useNotification();

    const onSubmitRegister = (values) => {
        setIsSubmitting(true);
        const registerAction = externalWallet ? accountActions.verifyLoginWallet : accountActions.register;
        dispatch(registerAction({
            params: {
                ...values,
                ...(externalWallet && { ...externalWallet, request_otp: true })
            },
            onCompleted: (response) => {
                if (response?.status === 200 || response?.code === 'duplicated_register') {
                    onRegisterSuccess(values.email);
                }
                else if (response?.code === 'user_existed') {
                    showError(intl.formatMessage(messages.errorUserExisted));
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
                name: '',
                email: '',
                password: '',
                rePassword: ''
            }}
            validationSchema={Yup.object().shape(externalWallet ? {
                email: Yup.string()
                    .required(intl.formatMessage(validationMessages.required))
                    .email(intl.formatMessage(messages.emailValidationFailed)),
            } : {
                name: Yup.string()
                .required(intl.formatMessage(validationMessages.required)),
                email: Yup.string()
                    .required(intl.formatMessage(validationMessages.required))
                    .email(intl.formatMessage(messages.emailValidationFailed)),
                password: Yup.string()
                    .required(intl.formatMessage(validationMessages.required)),
                rePassword: Yup.string().oneOf([Yup.ref('password'), null], intl.formatMessage(messages.passwordNotMatch))
                    .required(intl.formatMessage(validationMessages.required))
            })}
            onSubmit={onSubmitRegister}
            className={styles.registerForm}
        >
            <div className={styles.content}>
                {externalWallet ? (
                    <InputTextField
                        placeholder={intl.formatMessage(messages.emailAddress)}
                        name="email"
                        iconLeft={<EmailIcon />}
                    />
                ) : (
                    <>
                        <InputTextField
                            placeholder={intl.formatMessage(messages.yourName)}
                            name="name"
                            iconLeft={<UserIcon />}
                        />
                        <InputTextField
                            placeholder={intl.formatMessage(messages.emailAddress)}
                            name="email"
                            iconLeft={<EmailIcon />}
                        />
                        <PasswordField placeholder={intl.formatMessage(commonMessages.password)} name="password" />
                        <PasswordField placeholder={intl.formatMessage(messages.retypePassword)} name="rePassword" />
                    </>
                )}
                <p className={styles.terms}>
                    <Checkbox checked={acceptTerm} onChange={() => setIsAcceptTerm((pre) => !pre)}>
                        <span className={styles.termsContent}>
                            <FormattedMessage
                                key="terms"
                                defaultMessage="I agree to the <a>Terms and Conditions & Data Protection Guidelinest</a> & confirm I am at least 18 year olds"
                                values={{ a: content => <a className="underline">{content}</a> }}
                            />
                        </span>
                    </Checkbox>
                </p>
                <ValidateSubmitButton
                    className={styles.btnRegister}
                    text={intl.formatMessage(messages.btnContinue)}
                    primary
                    type="submit"
                    loading={isSubmitting}
                    disabled={!acceptTerm}
                />
            </div>
        </BasicForm>
    );
};

export default RegisterForm;
