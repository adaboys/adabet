
import { FormattedMessage } from 'react-intl';
import { useState, useContext } from 'react';

import { BasicModal } from '../Common';
import RegisterForm from './RegisterForm';
import ConfirmRegisterForm from './ConfirmRegisterForm';
import LoginWithSocial from './LoginWithSocial';

import { OverlayContext } from '@hocs';
import { overlayTypes } from '@constants';

import CloseIcon from '@assets/icons/close.svg';


import styles from './AuthModal.module.scss';

const REGISTER_STEP_EMAIL = 'email';
const REGISTER_STEP_OTP = 'otp';

const RegisterModal = ({ overlay: { hide } }) => {
    const overlay = useContext(OverlayContext);
    const [step, setStep] = useState(REGISTER_STEP_EMAIL);
    const [email, setEmail] = useState();

    const onRegisterSuccess = (email) => {
        setEmail(email);
        setStep(REGISTER_STEP_OTP);
    }

    const getRegisterStepForm = () => {
        if (step === REGISTER_STEP_OTP) {
            return (
                <ConfirmRegisterForm email={email} hide={hide} />
            )
        }
        return (
            <RegisterForm onRegisterSuccess={onRegisterSuccess} />
        )
    }

    const onShowLogin = () => {
        overlay.show(overlayTypes.LOGIN);
    }

    return (
        <BasicModal
            isOpen={true}
            // onRequestClose={hide}
            overlayClassName={styles.authModal}
            contentClassName={styles.wrapper}
        >
            <div className={styles.banner}>
                <img src="/images/signup-banner.png" alt="Signup" />
            </div>

            <div className={styles.form}>
                <div className={styles.header}>
                    <h3 className={styles.title}>
                        Sign Up
                    </h3>
                    <span onClick={hide} className={styles.btnClose}><CloseIcon /></span>
                </div>
                <div className={styles.content}>
                    {getRegisterStepForm()}
                </div>
                <div className={styles.actions}>
                    <FormattedMessage
                        key="alreadyRegistered"
                        defaultMessage="Already have an account? <a>Log In</a>"
                        values={{ a: content => <a onClick={onShowLogin}>{content}</a> }}
                    />
                </div>
                <LoginWithSocial/>
            </div>
        </BasicModal>
    )
};

export default RegisterModal;

