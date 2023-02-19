import { FormattedMessage, defineMessages, useIntl } from 'react-intl';
import { useDispatch } from 'react-redux';
import { useRouter } from 'next/router';

import { Button } from '@components/Common';
import GoogleLogin from './GoogleLogin';
import FacebookLogin from './FacebookLogin';

import { useNotification, useCardano } from '@hooks';
import { loadingActions, accountActions } from '@redux/actions';
import { CLIENT_TYPE } from '@constants';
import { externalWallets } from '@constants/wallet';

import styles from './index.module.scss';

const messages = defineMessages({
    loginSuccess: 'Login successful!',
    loginFailed: 'Login failed!',
    wasNotLinkWallet: 'You must connect wallet to login!'
})

const LoginWithSocial = ({

}) => {
    const dispatch = useDispatch();
    const intl = useIntl();
    const { push } = useRouter();

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
                {/* {
                    externalWallets.map(externalWallet => (
                        <Button
                            className={styles.btnLoginSocial}
                            key={externalWallet.key}
                            onClick={() => onLoginWallet(externalWallet.key)}
                        >
                            <img src={externalWallet.imgUrl} alt={externalWallet.name} />
                            <FormattedMessage key="continueWithWallet" defaultMessage="Continue with {walletName}" values={{ walletName: externalWallet.name }} />
                        </Button>
                    ))
                } */}
            </div>
        </div>
    )
}

export default LoginWithSocial;