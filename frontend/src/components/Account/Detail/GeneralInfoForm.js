import { useState } from "react";
import { useIntl, defineMessages } from "react-intl";
import { useDispatch } from "react-redux";
import * as Yup from 'yup';

import { BasicForm, InputTextField, ValidateSubmitButton } from "@components/Common";

import { commonMessages, validationMessages } from '@constants/intl';
import { useAuth, useNotification } from "@hooks";
import { accountActions } from "@redux/actions";

import styles from './GeneralInfoForm.module.scss';

const messages = defineMessages({
    fullNameLabel: 'Your Name',
    fullNamePlaceholder: 'Enter your name',
    emailLabel: 'Email Address',
    emailPlaceholder: 'Enter email address',
    updatedSuccess: 'Update successful!',
    updateFail: 'Update failed!'
});

const GeneralInfoForm = () => {
    const intl = useIntl();
    const { user } = useAuth();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const dispatch = useDispatch();
    const { showError, showSuccess } = useNotification(); 

    const onSubmit = (values) => {
        setIsSubmitting(true);
        dispatch(accountActions.updateProfile({
            params: {
                full_name: values.fullName,
                telno: values.telno,
            },
            onCompleted: (response) => {
                if (response?.status === 200) {
                    showSuccess(intl.formatMessage(messages.updatedSuccess));
                } else {
                    showError(response?.message || intl.formatMessage(messages.updateFail));
                }
                setIsSubmitting(false);
            },
            onError: (err) => {
                showError(intl.formatMessage(messages.updateFail));
                setIsSubmitting(false);
            }
        }))
    }

    return (
        <BasicForm
            initialValues={{
                fullName: user.name,
                email: user.email,
                telno: user.telno
            }}
            validationSchema={Yup.object().shape({
                fullName: Yup.string()
                    .required(intl.formatMessage(validationMessages.required)),
            })}
            onSubmit={onSubmit}
            className={styles.generalInfoForm}
        >
            <div className="row">
                <div className="col-sm-6">
                    <InputTextField
                        label={intl.formatMessage(messages.fullNameLabel)}
                        placeholder={intl.formatMessage(messages.fullNamePlaceholder)}
                        name="fullName"
                    />

                </div>
                <div className="col-sm-6">
                    <InputTextField
                        label={intl.formatMessage(messages.emailLabel)}
                        placeholder={intl.formatMessage(messages.emailPlaceholder)}
                        name="email"
                        disabled={true}
                    />

                </div>
            </div>
            <div className="row">
                <div className="col-sm-6">
                    <InputTextField
                        label="Phone Number"
                        placeholder="Enter Your Phone"
                        name="telno"
                    />
                </div>
            </div>
            <ValidateSubmitButton
                className={styles.btnUpdate}
                text={intl.formatMessage(commonMessages.update)}
                primary
                type="submit"
                loading={isSubmitting}
            />
        </BasicForm>
    )
}

export default GeneralInfoForm;
