
import { FormattedMessage } from "react-intl";
import { useState, useContext } from "react";

import { BasicModal } from "../../Common";
import RequestResetPasswordForm from "./RequestResetPasswordForm";
import ConfirmResetPasswordForm from "./ConfirmResetPasswordForm";
import LoginWithSocial from '../LoginWithSocial';
import { OverlayContext } from '@hocs';
import { overlayTypes } from '@constants';

import CloseIcon from '@assets/icons/close.svg';

import styles from "../AuthModal.module.scss";

const RESET_STEP_REQUEST = 'request';
const RESET_STEP_CONFIRM = 'confirm';

const ResetPasswordModal = ({ overlay: { hide } }) => {
    const overlay = useContext(OverlayContext);
    const [step, setStep] = useState(RESET_STEP_REQUEST);
    const [email, setEmail] = useState();

    const onRequestResetSuccess = (email) => {
        setEmail(email);
        setStep(RESET_STEP_CONFIRM);
    }

    const getResetPasswordStepForm = () => {
        if (step === RESET_STEP_CONFIRM) {
            return (
                <ConfirmResetPasswordForm email={email} hide={hide} />
            )
        }
        return (
            <RequestResetPasswordForm onRequestResetSuccess={onRequestResetSuccess} />
        )
    }

    const onShowLogin = () => {
        overlay.show(overlayTypes.LOGIN);
    }

    return (
        <BasicModal
            isOpen={true}
            overlayClassName={styles.authModal}
            contentClassName={styles.wrapper}
        >
            <div className={styles.banner}>
                <img src="/images/forgot-password-banner.png" alt="Forgot password" />
            </div>
            <div className={styles.form}>
                <div className={styles.header}>
                    <h3 className={styles.title}>
                        Forgot Password
                    </h3>
                    <span onClick={hide} className={styles.btnClose}><CloseIcon /></span>
                </div>
                <div className={styles.content}>
                    {getResetPasswordStepForm()}
                </div>
                <div className={styles.actions}>
                    <FormattedMessage
                        key="login"
                        defaultMessage="Already registered?<a>Log In</a>"
                        values={{ a: content => <a onClick={onShowLogin}>{content}</a> }}
                    />
                </div>
                <LoginWithSocial/>
            </div>
        </BasicModal>
    )
};

export default ResetPasswordModal;

