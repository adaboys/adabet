import { useState } from "react";
import { FormattedMessage } from "react-intl";
import classNames from "classnames";

import { BasicModal, Button, Radio } from "@components/Common";
import { commonMessages } from "@constants/intl";

import styles from './ConnectWalletModal.module.scss';

const ConnectWalletModal = ({
    wallets,
    isOpen,
    onClose,
    onConnectWallet
}) => {
    const availableWallet = wallets?.find(wallet => !wallet.isConnected) || null;
    const [walletSelected, setWalletSelected] = useState(availableWallet);

    return (
        <BasicModal
            isOpen={isOpen}
            contentClassName={styles.connectWalletModal}
        >
            <h1 className={styles.title}>
                <FormattedMessage key="title" defaultMessage="Select wallet to connect"/>
            </h1>
            <div className={styles.wallets}>
                {
                    wallets.map(wallet => (
                        wallet.isConnected
                        ?
                        <div key={wallet.key} className={styles.walletItem}>
                            <img src={wallet.imgUrl} alt={wallet.name}/>
                            {wallet.name}
                            <span className={styles.connected}>
                                <FormattedMessage key="connected" defaultMessage="(Connected)"/>
                            </span>
                        </div>
                        :
                        <Radio
                            className={classNames(styles.walletItem, {[styles.active]: wallet.key === walletSelected?.key})}
                            checked={wallet.key === walletSelected?.key}
                            onChange={() => setWalletSelected(wallet)}
                            key={wallet.key}
                            disabled={wallet.isConnected}
                        >
                            <img src={wallet.imgUrl} alt={wallet.name}/>
                            {wallet.name}
                        </Radio>
                    ))
                }
            </div>
            <div className={styles.actions}>
                <Button className={styles.btnCancel} onClick={onClose}>
                    <FormattedMessage {...commonMessages.cancel} />
                </Button>
                <Button className={styles.btnOk} onClick={() => onConnectWallet(walletSelected?.key)} disabled={!walletSelected?.key}>
                    <FormattedMessage key="connect" defaultMessage="Connect" />
                </Button>
            </div>
        </BasicModal>
    )
}

export default ConnectWalletModal;