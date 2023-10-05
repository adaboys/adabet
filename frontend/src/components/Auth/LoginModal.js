import { useState } from 'react';
import { defineMessages, useIntl } from 'react-intl';
import { useDispatch } from 'react-redux';
import classNames from 'classnames';
import { useRouter } from 'next/router';

import { BasicModal } from '@components/Common';
import LoginWithSocial from './LoginWithSocial';
import LoginForm from './LoginForm';

import { useNotification, useCardano } from '@hooks';
import { loadingActions, accountActions } from '@redux/actions';
import { CLIENT_TYPE, overlayTypes } from '@constants';
import { externalWallets } from '@constants/wallet';

import CloseIcon from '@assets/icons/close.svg';
// import AppleIcon from '@assets/icons/apple.svg';

import styles from "./AuthModal.module.scss";

const messages = defineMessages({
    loginSuccess: 'Login successful!',
    loginFailed: 'Login failed!',
    wasNotLinkWallet: 'You must connect wallet to login!'
})

const LoginModal = ({ overlay: { hide, show, context } }) => {
    const [isSubmitting, setIsSubmitting] = useState();
    const dispatch = useDispatch();
    const intl = useIntl();
    const { push } = useRouter();

    const { showSuccess, showError } = useNotification();
    const { enableWallet, getSignData, getChangeAddress, checkNetwordId } = useCardano();

    const onLogin = (values) => {
        setIsSubmitting(true);
        dispatch(accountActions.login({
            params: { ...values, client_type: CLIENT_TYPE },
            onCompleted: (response) => {
                onLoginCompleted(response);
                setIsSubmitting(false);
                context?.callback && context.callback();
            },
            onError: () => {
                setIsSubmitting(false);
                showError(intl.formatMessage(messages.loginFailed));
            }
        }))
    }

    const onVerifyLogintWallet = async (wallet, changeAddress, signature) => {
        const signed = await getSignData(wallet.key === 'nami' ? changeAddress.raw : changeAddress.walletAddress, signature);
        if (signed?.signature && signed?.key) {
            dispatch(accountActions.verifyLoginWallet({
                params: {
                    wallet_address: changeAddress.walletAddress,
                    wallet_address_in_hex: changeAddress.raw,
                    wallet_name: wallet.name,
                    signed_key: signed.key,
                    signed_signature: signed.signature,
                    client_type: CLIENT_TYPE
                },
                onCompleted: response => {
                    onLoginCompleted(response);
                    dispatch(loadingActions.hideLoadingFullScreen());
                },
                onError: err => {
                    console.log(err);
                    onLoginFail();
                }
            }))
        }
        else {
            onLoginFail();
        }
    }

    const onLoginWallet = async (walletKey) => {
        dispatch(loadingActions.showLoadingFullScreen());
        const walletEnabled = await enableWallet(walletKey);
        const wallet = externalWallets.find(wallet => wallet.key === walletKey);
        if (walletEnabled) {
            const isNetwordValid = await checkNetwordId();
            if (!isNetwordValid) {
                dispatch(loadingActions.hideLoadingFullScreen());
                return;
            }
            const changeAddress = await getChangeAddress(walletKey);
            if (changeAddress) {
                dispatch(accountActions.requestLoginWallet({
                    params: {
                        wallet_address: changeAddress.walletAddress
                    },
                    onCompleted: response => {
                        if (response?.data?.signature) {
                            onVerifyLogintWallet(wallet, changeAddress, response.data.signature);
                        }
                        else {
                            onLoginFail();
                        }
                    },
                    onError: err => {
                        console.log(err);
                        onLoginFail();
                    }
                }))
            }
            else {
                onLoginFail();
            }
        }
        else if (wallet?.downloadUrl) {
            push(wallet.downloadUrl);
        }
    }

    const onLoginFail = () => {
        dispatch(loadingActions.hideLoadingFullScreen());
        showError(intl.formatMessage(messages.loginFailed));
    }

    const onLoginCompleted = (response) => {
        const accessToken = response?.data?.access_token;
        const refreshToken = response?.data?.refresh_token;
        if (accessToken && refreshToken) {
            dispatch(accountActions.updateToken({
                accessToken,
                refreshToken
            }));
            hide();
            showSuccess(intl.formatMessage(messages.loginSuccess));
        }
        else {
            showError(intl.formatMessage(messages.loginFailed));
        }

    }

    const onShowRegister = () => {
        show(overlayTypes.REGISTER);
    }

    return (
        <BasicModal
            isOpen={true}
            overlayClassName={classNames(styles.authModal, { [styles.disabled]: isSubmitting })}
            contentClassName={styles.wrapper}
        >
            <div className={styles.banner}>
                <img src="/images/login-banner.png" alt="Login" />
            </div>
            <div className={styles.form}>
                <div className={styles.header}>
                    <h3 className={styles.title}>
                        Login
                    </h3>
                    <span onClick={hide} className={styles.btnClose}><CloseIcon /></span>
                </div>
                <div className={styles.content}>
                    <LoginForm onLogin={onLogin} isSubmitting={isSubmitting} />
                </div>
                <div className={styles.actions}>
                    Don’t have an account?
                    <a onClick={onShowRegister}>Sign up</a>
                </div>
                <LoginWithSocial hide={hide}/>
            </div>

        </BasicModal>
    )
};

export default LoginModal;

