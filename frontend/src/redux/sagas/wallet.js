import { takeLatest } from 'redux-saga/effects';
import apiConfig from '../../constants/apiConfig';

import { walletActionTypes } from '../actions';
import { CLIENT_TYPE } from '@constants';
import { processLoadingAction, processCallbackAction } from '@redux/helper';


const {
    GET_CONNECTED_WALLET,
    REQUEST_LINK_WALLET,
    VERIFY_LINK_WALLET,
    UNLINK_WALLET
} = walletActionTypes;

const getConnectedWallet = (action) => {
    return processLoadingAction(apiConfig.wallet.getConnectedWallet, action);
}

const requestLinkWallet = ({ payload }) => {
    const { params, onCompleted, onError } = payload;
    return processCallbackAction({
        ...apiConfig.wallet.requestLinkWallet,
        path: apiConfig.wallet.requestLinkWallet.path.replace(':walletAddress', params.walletAddress)
    }, {
       payload: {
            params: {
            },
            onCompleted,
            onError
       } 
    });
}

const verifyLinkWallet = ({ payload }) => {
    const { params, onCompleted, onError } = payload;
    return processCallbackAction({
        ...apiConfig.wallet.verifyLinkWallet,
        path: apiConfig.wallet.verifyLinkWallet.path.replace(':walletAddress', params.changeAddress.walletAddress)
    }, {
       payload: {
            params: {
                wallet_name: params.walletName,
                wallet_address_in_hex: params.changeAddress.raw,
                signed_signature: params.signedSignature,
                signed_key: params.signedKey,
                client_type: CLIENT_TYPE
            },
            onCompleted,
            onError
       } 
    });
}

const unlinkWallet = ({ payload }) => {
    const { params, onCompleted, onError } = payload;
    return processCallbackAction({
        ...apiConfig.wallet.unlinkWallet,
        path: apiConfig.wallet.unlinkWallet.path.replace(':walletAddress', params.walletAddress)
    }, {
       payload: {
            params: {},
            onCompleted,
            onError
       } 
    });
}

export default [
    takeLatest(GET_CONNECTED_WALLET, getConnectedWallet),
    takeLatest(REQUEST_LINK_WALLET, requestLinkWallet),
    takeLatest(VERIFY_LINK_WALLET, verifyLinkWallet),
    takeLatest(UNLINK_WALLET, unlinkWallet)
];