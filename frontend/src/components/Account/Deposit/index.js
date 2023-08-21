import { defineMessages, useIntl } from 'react-intl';
import QRCode from 'react-qr-code';

import WalletIcon from '@assets/icons/wallet-menu.svg';

import { Button } from '@components/Common';

import { useAuth, useDevices, useNotification } from '@hooks';

import styles from './index.module.scss';
import { walletTabs } from '@constants/masterData';
import TabLayout from '../TabLayout';

const messages = defineMessages({
    yourWallet: 'Your deposit address',
    copyClipboard: 'Copy to clipboard',
    copySuccess: 'Copy successful!'
});

const Deposit = () => {
    const intl = useIntl();
    const { user } = useAuth();

    const { showSuccess } = useNotification();
    const { isDesktop } = useDevices();

    const copyWalletAddress = () => {
        navigator.clipboard.writeText(user.wallet_address || '');
        showSuccess(intl.formatMessage(messages.copySuccess));
    }

    const onViewOnChain = () => {
        window.open(`${process.env.NEXT_PUBLIC_CADARNOSCAN_URL}address/${user?.wallet_address}`, '_blank');
    }

    return (
        <TabLayout
            tabs={walletTabs}
            title="Wallet"
            icon={<WalletIcon />}
        >
            <div className={styles.deposit}>
                <div className={styles.info}>
                    <p className={styles.title}>
                        {intl.formatMessage(messages.yourWallet)}
                    </p>
                    <input className={styles.walletAddress} defaultValue={user?.wallet_address || ''} />
                    <div className={styles.actions}>
                        <Button
                            className={styles.btnViewOnChain}
                            onClick={onViewOnChain}
                            disabled={!user?.wallet_address}
                        >
                            View on chain
                            <img src="/images/cadarnoscan.png" alt="Cadarnoscan" />
                        </Button>
                        <Button
                            className={styles.btnCopy}
                            onClick={copyWalletAddress}
                            disabled={!user?.wallet_address}
                        >
                            {intl.formatMessage(messages.copyClipboard)}
                        </Button>
                    </div>
                </div>
                <div className={styles.wallet}>
                    <div className={styles.qrCodeWrapper}>
                        <QRCode value={user?.wallet_address} size={240}/>
                    </div>
                </div>
            </div>
        </TabLayout>
    )
}

export default Deposit;