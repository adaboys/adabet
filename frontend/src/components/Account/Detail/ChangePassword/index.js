import { defineMessages, useIntl } from 'react-intl';
import * as Yup from 'yup';
import { useRef, useState } from 'react';
import { useDispatch } from 'react-redux';

import { BasicForm, PasswordField, ValidateSubmitButton } from '@components/Common';
import DetailLayout from '../DetailLayout';

import { commonMessages, validationMessages } from '@constants/intl';
import { useNotification } from '@hooks';
import { accountActions } from '@redux/actions';

const changePasswordMessages = defineMessages({
    currentPassword: 'Current password',
    newPassword: 'New password',
    newPasswordConfirm: 'Retype new password',
    changePasswordSuccess: 'Change password successful!',
    changePasswordFailed: 'Change password failed!',
    passwordNotMatch: 'The password and confirmation password do not match',
});

import styles from './index.module.scss';

const ChangePassword = () => {
    const intl = useIntl();
    const formikRef = useRef();

    const dispatch = useDispatch();
    const { showSuccess, showError } = useNotification();

    const [loading, setLoading] = useState(false);

    const onSubmit = values => {
        setLoading(true);
        dispatch(accountActions.changePassword({
            params: {
                current_pass: values.currentPassword,
                new_pass: values.newPassword,
                logout_everywhere: true,
            },
            onCompleted: response => {
                if(response?.status === 200) {
                    showSuccess(intl.formatMessage(changePasswordMessages.changePasswordSuccess));
                    formikRef.current.resetForm();
                } else {
                    const errMsg = response?.message || intl.formatMessage(changePasswordMessages.changePasswordFailed);
                    showError(errMsg);
                }
                setLoading(false);
            },
            onError: err => {
                showError(intl.formatMessage(changePasswordMessages.changePasswordFailed));
                setLoading(false);
            }
        }));
    }

    return (
        <DetailLayout>
            <div className={styles.changePassword}>
                <BasicForm
                    formikRef={formikRef}
                    initialValues={{
                        currentPassword: '',
                        newPassword: '',
                        newPasswordConfirm: '',
                    }}
                    validationSchema={Yup.object().shape({
                        currentPassword: Yup.string()
                            .required(intl.formatMessage(validationMessages.required)),
                        newPassword: Yup.string()
                            .required(intl.formatMessage(validationMessages.required)),
                        newPasswordConfirm: Yup.string().oneOf([Yup.ref('newPassword'), null], intl.formatMessage(changePasswordMessages.passwordNotMatch))
                            .required(intl.formatMessage(validationMessages.required))
                    })}
                    onSubmit={onSubmit}
                    className={styles.form}
                >
                    <PasswordField placeholder={intl.formatMessage(changePasswordMessages.currentPassword)} name="currentPassword" />
                    <PasswordField placeholder={intl.formatMessage(changePasswordMessages.newPassword)} name="newPassword" />
                    <PasswordField placeholder={intl.formatMessage(changePasswordMessages.newPasswordConfirm)} name="newPasswordConfirm" />

                    <ValidateSubmitButton
                        className={styles.btnUpdate}
                        text={intl.formatMessage(commonMessages.confirm)}
                        primary
                        type="submit"
                        loading={loading}
                    />
                </BasicForm>
            </div>
        </DetailLayout>
    );
};

export default ChangePassword;