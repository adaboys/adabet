import React, { useState } from 'react';
import * as Yup from 'yup';
import { useIntl, defineMessages, FormattedMessage } from "react-intl";

import { BasicForm, InputTextField, ValidateSubmitButton } from "@components/Common";

import styles from './SubscribeForm.module.scss';
import { useDispatch } from 'react-redux';
import { dashboardActions } from '@redux/actions';
import { useNotification } from '@hooks';

const messages = defineMessages({
    yourEmail: 'Your email sddress',
    btnSubscrible: 'Subscrible',
    subscribleSuccess: 'Subscrible successful!',
    subscribleFail: 'Subscrible failed!'
});

const SubscribeForm = () => {
    const intl = useIntl();
    const [isSubmitting, setIsSubmitting] = useState();
    const dispatch = useDispatch();
    const { showError, showSuccess } = useNotification();

    const onSubmitScrible = (values) => {
        setIsSubmitting(true);
        dispatch(dashboardActions.subscribleReceiveMail({
            params: values,
            onCompleted: ressponse => {
                if(ressponse?.status === 200) showSuccess(intl.formatMessage(messages.subscribleSuccess));
                else showError(intl.formatMessage(messages.subscribleFail));
                setIsSubmitting(false);
            },
            onError: err => {
                console.log(err);
                setIsSubmitting(false);
                showError(intl.formatMessage(messages.subscribleFail));
            }
        }))
    }


    return (
        <BasicForm
            initialValues={{
                email: ''
            }}
            onSubmit={onSubmitScrible}
            validationSchema={Yup.object().shape({
                email: Yup.string()
                    .required()
                    .email()
            })}
            className={styles.subscribleForm}>
            <p>
                <FormattedMessage key="title" defaultMessage="Subscribe to our newsletter"/>
            </p>
            <div className={styles.groupInput}>
                <InputTextField
                    className={styles.emailInput}
                    placeholder={intl.formatMessage(messages.yourEmail)}
                    name="email"
                    hideErrorMessage
                    hideErrorAnimation
                />

                <ValidateSubmitButton
                    className={styles.btnSubscrible}
                    text={intl.formatMessage(messages.btnSubscrible)}
                    primary
                    type="submit"
                    loading={isSubmitting}
                />
            </div>
        </BasicForm>
    );
};

export default SubscribeForm;
