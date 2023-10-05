import { useEffect, useState, useMemo } from 'react';
import { defineMessages, FormattedMessage, FormattedNumber, useIntl } from 'react-intl';
import { useDispatch, useSelector } from 'react-redux';
import { useRouter } from 'next/router';

import AccountLayout from '../AccountLayout';
import { Button, ConfirmModal, CopyWrapper, Desktop, EllipsisText } from '@components/Common';
import ConnectWalletModal from './ConnectWalletModal';

import { walletActions, loadingActions, walletActionTypes } from '@redux/actions';
import { externalWallets } from '@constants/wallet';

import ConnectWalletIcon from '@assets/icons/connect-wallet.svg';
import RemoveIcon from '@assets/icons/close-modal.svg';
import CopyIcon from '@assets/icons/copy-wallet.svg';

import styles from './index.module.scss';
import { useCardano, useNotification } from '@hooks';

const messages = defineMessages({
    connectWalletFail: 'Connect wallet failed!',
    connectWalletSuccess: 'Connect wallet successful!',
    unlinkWalletFail: 'Disconnect wallet failed!',
    unlinkWalletSuccess: 'Disconnect wallet successful! '
})

const ConnectWallet = () => {
    const [isShowConnectWalletModal, setIsShowConnectWalletModal] = useState(false);
    const [walletSelected, setWalletSelected] = useState(null);
    const [isDisconnecting, setIsDisconnecting] = useState(false);
    const dispatch = useDispatch();
    const { push } = useRouter();
    const intl = useIntl();
    const { showError, showSuccess } = useNotification();
    const { enableWallet, getChangeAddress, getSignData, checkNetwordId } = useCardano();

    const connectedWallets = useSelector(state => state.wallet.connectedWallets || []);
    const loading = useSelector(state => state.loading[walletActionTypes.GET_CONNECTED_WALLET]);

    const onVerifyConnectWallet = async (wallet, changeAddress, signature) => {
        const signed = await getSignData(wallet.key === 'nami' ? changeAddress.raw : changeAddress.walletAddress, signature);
        if (signed?.signature && signed?.key) {
            dispatch(walletActions.verifyLinkWallet({
                params: {
                    changeAddress,
                    walletName: wallet.name,
                    signedKey: signed.key,
                    signedSignature: signed.signature
                },
                onCompleted: response => {
                    dispatch(loadingActions.hideLoadingFullScreen());
                    if(response?.status === 200) {
                        getConnectedWallet();
                        setIsShowConnectWalletModal(false);
                        showSuccess(intl.formatMessage(messages.connectWalletSuccess));
                    }
                    else {
                        showError(intl.formatMessage(messages.connectWalletFail));
                    }
                },
                onError: err => {
                    console.log(err);
                    onConnectWalletFail();
                }
            }))
        }
        else {
            onConnectWalletFail();
        }
    }

    const onConnectWallet = async (walletKey) => {
        dispatch(loadingActions.showLoadingFullScreen());
        try {
            const walletEnabled = await enableWallet(walletKey);
            const wallet = externalWallets.find(wallet => wallet.key === walletKey);
            if (walletEnabled) {
                const isNetwordValid = await checkNetwordId();
                if(!isNetwordValid) {
                    dispatch(loadingActions.hideLoadingFullScreen());
                    return;
                }
                const changeAddress = await getChangeAddress(walletKey);
                if(changeAddress) {
                    dispatch(walletActions.requestLinkWallet({
                        params: {
                            walletAddress: changeAddress.walletAddress
                        },
                        onCompleted: response => {
                            if (response?.data?.signature) {
                                onVerifyConnectWallet(wallet, changeAddress, response.data.signature);
                            }
                            else {
                                onConnectWalletFail();
                            }
                        },
                        onError: err => {
                            console.log(err);
                            onConnectWalletFail();
                        }
                    }))
                }
                else {
                    onConnectWalletFail();
                }
            }
            else if (wallet?.downloadUrl) {
                push(wallet.downloadUrl);
            }
        }
        catch(err) {
            console.log(err)
            onConnectWalletFail();
        }
    }

    const onConnectWalletFail = () => {
        dispatch(loadingActions.hideLoadingFullScreen());
        showError(intl.formatMessage(messages.connectWalletFail));
    }

    const onDisconnectWallet = () => {
        if(walletSelected) {
            setIsDisconnecting(true);
            dispatch(walletActions.unlinkWallet({
                params: {
                    walletAddress: walletSelected.address
                },
                onCompleted: response => {
                    if(response?.status === 200) {
                        const currentConnectedWallets = connectedWallets.filter(connectedWallet => connectedWallet.address !== walletSelected.address);
                        dispatch(walletActions.updateConnectWalletLocal({
                            connectedWallets: currentConnectedWallets
                        }));
                        showSuccess(intl.formatMessage(messages.unlinkWalletSuccess));
                        setWalletSelected(null);
                    }
                    else {
                        showError(intl.formatMessage(messages.unlinkWalletFail));
                    }
                    setIsDisconnecting(false);
                },
                onError: err => {
                    console.log(err);
                    setIsDisconnecting(false);
                    showError(intl.formatMessage(messages.unlinkWalletFail));
                }
            }))
        }
    }

    const onShowConnectWalletModal = () => {
        setIsShowConnectWalletModal(true);
    }

    const onShowDisconnectModal = (wallet) => {
        setWalletSelected(wallet);
    }

    const getConnectedWallet = () => {
        dispatch(walletActions.getConnectedWallet());
    }

    const myWallets = useMemo(() => {
        return externalWallets.map(externalWallet => {
            const connectedWallet = connectedWallets.find(connectedWallet => connectedWallet.name === externalWallet.name) || null;
            return {
                ...externalWallet,
                isConnected: !!connectedWallet,
                address: connectedWallet?.address,
                ada: connectedWallet?.ada || 0,
                abe: connectedWallet?.abe || 0,
                gem: connectedWallet?.gem || 0
            }
        });
    }, [connectedWallets])

    useEffect(() => {
        getConnectedWallet();
    }, [])

    const myConnectedWallets = myWallets.filter(myWallet => myWallet.isConnected);

    return (
        <AccountLayout loading={loading}>
            <div className={styles.connectWallet}>
                <p className={styles.title}>
                    <FormattedMessage key="title" defaultMessage="Connected wallets" />
                </p>
                <div className={styles.myWallets}>
                    {
                        myConnectedWallets?.length
                        ?
                        myConnectedWallets.map(myWallet => (
                            <div className={styles.walletItem} key={myWallet.key}>
                                <div className={styles.info}>
                                    <img src={myWallet.imgUrl} alt={myWallet.name}/>
                                    <div className={styles.contentBox}>
                                        <span>{myWallet.name}</span>
                                        <div className={styles.walletAddress}>
                                            <EllipsisText className={styles.content} text={myWallet.address}/>
                                            <CopyWrapper text={myWallet.address}>
                                                <CopyIcon/>
                                            </CopyWrapper>
                                        </div>
                                        <div className={styles.balance}>
                                            <span className={styles.coin}>
                                                {/* <AdaIcon/> */}
                                                <FormattedNumber value={myWallet.ada}/> ADA
                                            </span>
                                            {','}
                                            <span className={styles.coin}>
                                                {/* <img src="/images/isky-coin.png" alt="ISKY" /> */}
                                                <FormattedNumber value={myWallet.abe}/> ABE
                                            </span>
                                            <span className={styles.coin}>
                                                {/* <img src="/images/isky-coin.png" alt="ISKY" /> */}
                                                <FormattedNumber value={myWallet.gem}/> GEM
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <a className={styles.unlink} onClick={() => onShowDisconnectModal(myWallet)}>
                                    <RemoveIcon/>
                                    <FormattedMessage key="disconnect" defaultMessage="Disconnect"/>
                                </a>
                            </div>
                        ))
                        :
                        <div className={styles.divider}></div>
                    }
                </div>
                <Desktop>
                    <center>
                        <Button className={styles.btnConnectWallet} icon={<ConnectWalletIcon />} onClick={onShowConnectWalletModal}>
                            <FormattedMessage key="btnConnectWallet" defaultMessage="Connect a wallet" />
                        </Button>
                    </center>
                </Desktop>
                <ConnectWalletModal
                    isOpen={isShowConnectWalletModal}
                    onClose={() => setIsShowConnectWalletModal(false)}
                    onConnectWallet={onConnectWallet}
                    wallets={myWallets}
                />
                <ConfirmModal
                    isOpen={!!walletSelected}
                    onClose={() => setWalletSelected(null)}
                    onOk={onDisconnectWallet}
                    title="Are you sure to disconnect this wallet?"
                    loading={isDisconnecting}
                />
            </div>
        </AccountLayout>
    )
}

export default ConnectWallet;