import { useRef } from 'react';
import { Address } from '@emurgo/cardano-serialization-lib-asmjs';
import { useIntl, defineMessages } from 'react-intl';

import { appMode } from '@constants';
import useNotification from './useNotification';

const TESTNET_NETWORK = 0;
const MAINNET_NETWORK = 1;

const messages = defineMessages({
    mainnetNetwordFail: 'Wallet enablement failed! Please check your settings once again and make sure you are using Mainnet Network',
    testNetwordFail: 'Wallet enablement failed! Please check your settings once again and make sure you are using Testnet Network'
})

const useCardano = () => {
    const cardanoAPI = useRef(null);
    const intl = useIntl();
    const { showPopupWarning} = useNotification();

    const checkIfWalletEnabled = async (walletName) => {
        let walletIsEnabled = false;

        try {
            walletIsEnabled = await window.cardano[walletName].isEnabled();
        } catch (err) {
            console.log(err)
        }

        return walletIsEnabled;
    }

    const enableWallet = async (walletKey) => {
        try {
            cardanoAPI.current = await window.cardano[walletKey].enable();
        } catch (err) {
            console.log(err);
        }
        return await checkIfWalletEnabled(walletKey);
    }

    const getNetworkId = async () => {
        try {
            const networkId = await cardanoAPI.current.getNetworkId();
            return networkId;
        } catch (err) {
            console.log(err)
        }
        return 0;
    }

    const checkNetwordId = async () => {
        const networkId = await getNetworkId();
        if(appMode === 'development') {
            if(networkId === TESTNET_NETWORK) {
                return true;
            }
            else {
                showPopupWarning(intl.formatMessage(messages.testNetwordFail));
                return false;
            }
        }

        return true;
    }

    const getChangeAddress = async () => {
        try {
            const raw = await cardanoAPI.current.getChangeAddress();
            const walletAddress = Address.from_bytes(Buffer.from(raw, "hex")).to_bech32()
            return {
                raw,
                walletAddress
            }
        } catch (err) {
            console.log(err);
        }
        return null;
    }

    const getSignData = async (walletAddress, signature) => {
        try {
            const signed = await cardanoAPI.current.signData(walletAddress, signature);           
            return signed;
        } catch (err) {
            console.log(err);
        }
        return null;
    }

    return {
        enableWallet,
        getChangeAddress,
        getSignData,
        getNetworkId,
        checkNetwordId
    }
}

export default useCardano;