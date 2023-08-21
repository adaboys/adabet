import { createAction } from 'redux-actions';

export const actionTypes = {
    GET_CONNECTED_WALLET: 'wallet/GET_CONNECTED_WALLET',
    UPDATE_CONNECTED_WALLET_LOCAL: 'wallet/UPDATE_CONNECTED_WALLET_LOCAL',
    REQUEST_LINK_WALLET: 'wallet/REQUEST_LINK_WALLET',
    VERIFY_LINK_WALLET: 'wallet/VERIFY_LINK_WALLET',
    UNLINK_WALLET: 'wallet/UNLINK_WALLET'
}

export const actions = {
    getConnectedWallet: createAction(actionTypes.GET_CONNECTED_WALLET),
    updateConnectWalletLocal: createAction(actionTypes.UPDATE_CONNECTED_WALLET_LOCAL),
    requestLinkWallet: createAction(actionTypes.REQUEST_LINK_WALLET),
    verifyLinkWallet: createAction(actionTypes.VERIFY_LINK_WALLET),
    unlinkWallet: createAction(actionTypes.UNLINK_WALLET),
}

