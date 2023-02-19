import { createAction } from 'redux-actions';

export const actionTypes = {
    GET_PROFILE: 'account/GET_PROFILE',
    UPDATE_PROFILE: 'account/UPDATE_PROFILE',
    REGISTER: 'account/REGISTER',
    CONFIRM_REGISTER: 'account/CONFIRM_REGISTER',
    LOGIN: 'account/LOGIN',
    LOGIN_SOCIAL: 'account/LOGIN_SOCIAL',
    LOGOUT: 'account/LOGOUT',
    GET_BALANCE: 'account/GET_BALANCE',
    SEND_CONFIRM_COIN: 'account/SEND_CONFIRM_COIN',
    SEND_ACTUAL_COIN: 'account/SEND_ACTUAL_COIN',
    REQUEST_LOGIN_WALLET: 'account/REQUEST_LOGIN_WALLET',
    VERIFY_LOGIN_WALLET: 'account/VERIFY_LOGIN_WALLET',
    UPDATE_COMMON_INFO: 'account/UPDATE_COMMON_INFO',
    UPDATE_TOKEN: 'account/UPDATE_TOKEN',
    REFRESHING_TOKEN: 'account/REFRESHING_TOKEN',
    CHANGE_PASSWORD: 'account/CHANGE_PASSWORD',
    REQUEST_RESET_PASSWORD: 'account/REQUEST_RESET_PASSWORD',
    CONFIRM_RESET_PASSWORD: 'account/CONFIRM_RESET_PASSWORD',
}

export const actions = {
    login: createAction(actionTypes.LOGIN),
    loginSocial: createAction(actionTypes.LOGIN_SOCIAL),
    logout: createAction(actionTypes.LOGOUT),
    register: createAction(actionTypes.REGISTER),
    confirmRegister: createAction(actionTypes.CONFIRM_REGISTER),
    getProfile: createAction(actionTypes.GET_PROFILE),
    updateProfile: createAction(actionTypes.UPDATE_PROFILE),
    getBalance: createAction(actionTypes.GET_BALANCE),
    sendConfirmCoin: createAction(actionTypes.SEND_CONFIRM_COIN),
    sendActualCoin: createAction(actionTypes.SEND_ACTUAL_COIN),
    requestLoginWallet: createAction(actionTypes.REQUEST_LOGIN_WALLET),
    verifyLoginWallet: createAction(actionTypes.VERIFY_LOGIN_WALLET),
    updateCommonInfo: createAction(actionTypes.UPDATE_COMMON_INFO),
    updateToken: createAction(actionTypes.UPDATE_TOKEN),
    refreshingToken: createAction(actionTypes.REFRESHING_TOKEN),
    changePassword: createAction(actionTypes.CHANGE_PASSWORD),
    requestResetPassword: createAction(actionTypes.REQUEST_RESET_PASSWORD),
    confirmResetPassword: createAction(actionTypes.CONFIRM_RESET_PASSWORD),
}

