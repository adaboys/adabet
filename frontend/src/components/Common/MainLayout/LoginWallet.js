import classNames from 'classnames';
import { useDispatch } from "react-redux";
import { defineMessages, useIntl } from "react-intl";
import { useRouter } from 'next/router';

import { externalWallets } from '@constants/wallet';
import { CLIENT_TYPE } from '@constants';
import { useNotification, useCardano } from "@hooks";
import { loadingActions, accountActions } from '@redux/actions';

import styles from './LoginWallet.module.scss';

const messages = defineMessages({
    loginSuccess: 'Login successful!',
    loginFailed: 'Login failed!',
    wasNotLinkWallet: 'You must connect wallet to login!'
})

const LoginWallet = ({ className }) => {
    const { push } = useRouter();
    const dispatch = useDispatch();
    const intl = useIntl();
    const { enableWallet, getSignData, getChangeAddress, checkNetwordId } = useCardano();
    const { showSuccess, showError } = useNotification();

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
                    onLoginCompeted(response);
                    dispatch(loadingActions.hideLoadingFullScreen());
                },
                onError: err => {
                    console.log(err);
                    onLoginWalletFail();
                }
            }))
        }
        else {
            onLoginWalletFail();
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
                            onLoginWalletFail();
                        }
                    },
                    onError: err => {
                        console.log(err);
                        onLoginWalletFail();
                    }
                }))
            }
            else {
                onLoginWalletFail();
            }
        }
        else if (wallet?.downloadUrl) {
            push(wallet.downloadUrl);
        }
    }

    const onLoginWalletFail = () => {
        dispatch(loadingActions.hideLoadingFullScreen());
        showError(intl.formatMessage(messages.loginFailed));
    }

    const onLoginCompeted = (response) => {
        const accessToken = response?.data?.access_token;
        const refreshToken = response?.data?.refresh_token;

        if (accessToken && refreshToken) {
            dispatch(accountActions.updateToken({
                accessToken,
                refreshToken
            }));
            showSuccess(intl.formatMessage(messages.loginSuccess));
        }
        else if (response?.code === 'must_link_wallet') {
            showError(intl.formatMessage(messages.wasNotLinkWallet))
        }
        else {
            showError(intl.formatMessage(messages.loginFailed));
        }

    }


    return (
        <div className={classNames(className, styles.loginWallet)}>
            {
                externalWallets.map(externalWallet => (
                    <div className={styles.walletItem} key={externalWallet.key} onClick={() => onLoginWallet(externalWallet.key)}>
                        <img src={externalWallet.imgUrl} alt={externalWallet.name}/>
                        {externalWallet.name}
                    </div>
                ))
            }
        </div>
    )
}

export default LoginWallet;