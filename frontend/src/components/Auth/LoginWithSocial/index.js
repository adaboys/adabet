import { FormattedMessage, defineMessages, useIntl } from 'react-intl';
import { useDispatch } from 'react-redux';
import { useRouter } from 'next/router';

import { Button } from '@components/Common';
import GoogleLogin from './GoogleLogin';
import FacebookLogin from './FacebookLogin';

import YoroiIcon from '@assets/icons/yoroi.svg';
import NamiIcon from '@assets/icons/nami.svg';
import FlintIcon from '@assets/icons/flint.svg';

import { useNotification, useCardano } from '@hooks';
import { loadingActions, accountActions } from '@redux/actions';
import { CLIENT_TYPE, overlayTypes } from '@constants';
import { externalWallets } from '@constants/wallet';

import styles from './index.module.scss';
import { useContext } from 'react';
import { OverlayContext } from '@hocs';

const messages = defineMessages({
    loginSuccess: 'Login successful!',
    loginFailed: 'Login failed!',
    wasNotLinkWallet: 'You must connect wallet to login!'
})

const iconsMap = {
    yoroi: YoroiIcon,
    nami: NamiIcon,
    flint: FlintIcon,
}

const LoginWithSocial = ({
    hide,
}) => {
    const dispatch = useDispatch();
    const intl = useIntl();
    const { push } = useRouter();
    const overlay = useContext(OverlayContext);

    const { showSuccess, showError } = useNotification();
    const { enableWallet, getSignData, getChangeAddress, checkNetwordId } = useCardano();

    const onLoginSocial = (provider, access_token) => {
        dispatch(loadingActions.showLoadingFullScreen());
        dispatch(accountActions.loginSocial({
            params: {
                provider,
                access_token: access_token,
                client_type: CLIENT_TYPE
            },
            onCompleted: (response) => {
                onLoginCompleted(response);
                dispatch(loadingActions.hideLoadingFullScreen());
            },
            onError: () => {
                onLoginFail();
            }
        }));
    }

    const onVerifyLogintWallet = async (wallet, changeAddress, signature) => {
        const signed = await getSignData(wallet.key === 'nami' ? changeAddress.raw : changeAddress.walletAddress, signature);
        console.log({ signed });
        if (signed?.signature && signed?.key) {
            const params = {
                wallet_address: changeAddress.walletAddress,
                wallet_address_in_hex: changeAddress.raw,
                wallet_name: wallet.name,
                signed_key: signed.key,
                signed_signature: signed.signature,
                client_type: CLIENT_TYPE
            };
            dispatch(accountActions.verifyLoginWallet({
                params,
                onCompleted: response => {
                    if(response?.code === 'email_required') {
                        onShowConfirmEmail(params);
                    }
                    else {
                        onLoginCompleted(response);
                    }
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

    const onShowConfirmEmail = (externalWallet) => {
        overlay.show(overlayTypes.REGISTER, { externalWallet });
    }

    return (
        <div className={styles.loginWithSocial}>
            <div className={styles.divider}>
                <div className={styles.item}></div>
                <span>or continue with</span>
                <div className={styles.item}></div>
            </div>
            <div className={styles.socials}>
                <FacebookLogin onLogin={onLoginSocial} className={styles.btnLogin} />
                <GoogleLogin className={styles.btnLogin} onLogin={onLoginSocial} />
                {
                    externalWallets.map(externalWallet => {
                        const Icon = iconsMap[externalWallet.key];
                        return (
                            <Button
                                key={externalWallet.key}
                                onClick={() => onLoginWallet(externalWallet.key)}
                                secondary
                                className={styles.btnLogin}
                            >
                                <Icon />
                            </Button>
                        );
                    })
                }
            </div>
        </div>
    )
}

export default LoginWithSocial;