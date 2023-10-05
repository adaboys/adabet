import { useState } from 'react';

import SendCoinForm from './SendCoinForm';
import ConfirmSendCoin from './ConfirmSendCoin';
import TabLayout from '../TabLayout';

import WalletIcon from '@assets/icons/wallet-menu.svg';

import { walletTabs } from '@constants/masterData';

import styles from './index.module.scss';

const Withdraw = () => {
    const [coinData, setCoinData] = useState(null);

    const onSendCoinSuccess = (values) => {
        setCoinData(values)
    }

    return (
        <TabLayout
            tabs={walletTabs}
            title="Wallet"
            icon={<WalletIcon />}
        >
            <div className={styles.withdraw}>
                {/* <p className={styles.title}>Withdraw</p> */}
                {
                    coinData
                    ?
                    <ConfirmSendCoin coinData={coinData} onSendCoinSuccess={onSendCoinSuccess}/>
                    :
                    <SendCoinForm onSendCoinSuccess={onSendCoinSuccess}/>
                }
            </div>
        </TabLayout>
    )
}

export default Withdraw;