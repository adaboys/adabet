import { call, put, takeLatest, select } from 'redux-saga/effects';

import { accountActions, accountActionTypes } from '../actions';
import apiConfig from '@constants/apiConfig';
import { eraseCookie, setCookie } from '@utils/localStorage';
import { storageKeys } from '@constants';
import { sendRequest } from '@utils/api';
import { processCallbackAction, processLoadingAction } from '../helper';

const {
    LOGIN,
    LOGIN_SOCIAL,
    LOGOUT,
    GET_PROFILE,
    REGISTER,
    CONFIRM_REGISTER,
    GET_BALANCE,
    SEND_CONFIRM_COIN,
    SEND_ACTUAL_COIN,
    REQUEST_LOGIN_WALLET,
    VERIFY_LOGIN_WALLET,
    UPDATE_TOKEN,
    CHANGE_PASSWORD,
    REQUEST_RESET_PASSWORD,
    CONFIRM_RESET_PASSWORD,
    UPDATE_PROFILE,
    GET_STATISTICS,
    GET_COIN_TRANSACTION,
    GET_COIN_TRANSACTION_ALL,
} = accountActionTypes;

const login = (action) => {
    return processCallbackAction(apiConfig.account.login, action);
}

const loginSocial = (action) => {
    return processCallbackAction(apiConfig.account.loginSocial, action);
}

function* logout() {
    try {
        const { accessToken, refreshToken } =  yield select(state => state.account);
        yield call(sendRequest, {...apiConfig.account.logout, accessToken }, { refresh_token: refreshToken });
        eraseCookie(storageKeys.ACCESS_TOKEN);
        eraseCookie(storageKeys.REFRESH_TOKEN);
        yield put(accountActions.updateCommonInfo({ accessToken: null, refreshToken: null, profileData: null }));
    } catch (error) {
        console.log(error);
    }
}

function* updateToken(action) {
    const { accessToken, refreshToken } = action.payload || {};
    if (accessToken && refreshToken) {
        const tokenExpiredTimsMs = 1000 * 60 * 60 * 24 * 365; // 1 year
        setCookie(storageKeys.ACCESS_TOKEN, accessToken, tokenExpiredTimsMs);
        setCookie(storageKeys.REFRESH_TOKEN, refreshToken, tokenExpiredTimsMs);
        yield put(accountActions.updateCommonInfo({ accessToken, refreshToken }));
    }
}

const getProfile = (action) => {
    return processLoadingAction(apiConfig.account.getProfile, action);
}

const register = (action) => {
    return processCallbackAction(apiConfig.account.register, action);
}

const confirmRegister = (action) => {
    return processCallbackAction(apiConfig.account.confirmRegister, action);
}

const getBalance = (action) => {
    return processLoadingAction(apiConfig.account.getBalance, action);
}

const sendConfirmCoin = (action) => {
    return processCallbackAction(apiConfig.account.sendConfirmCoin, action);
}

const sendActualCoin = (action) => {
    return processCallbackAction(apiConfig.account.sendActualCoin, action);
}

const requestLoginWallet = (action) => {
    return processCallbackAction(apiConfig.account.requestLoginWallet, action);
}

const verifyLoginWallet = (action) => {
    return processCallbackAction(apiConfig.account.verifyLoginWallet, action);
}

const changePassword = (action) => {
    return processCallbackAction(apiConfig.account.changePassword, action);
}

const requestResetPassword = (action) => {
    return processCallbackAction(apiConfig.account.requestResetPassword, action);
}

const confirmResetPassword = (action) => {
    return processCallbackAction(apiConfig.account.confirmResetPassword, action);
}

const updateProfile = (action) => {
    return processCallbackAction(apiConfig.account.updateProfile, action);
}

const getStatistics = (action) => {
    return processCallbackAction(apiConfig.account.statistics, action);
}

const getCoinTransaction = (action) => {
    return processCallbackAction(apiConfig.account.coinTransaction, action);
}

const getCoinTransactionAll = (action) => {
    return processCallbackAction(apiConfig.account.coinTransactionAll, action);
}

export default [
    takeLatest(LOGIN, login),
    takeLatest(LOGIN_SOCIAL, loginSocial),
    takeLatest(LOGOUT, logout),
    takeLatest(REGISTER, register),
    takeLatest(CONFIRM_REGISTER, confirmRegister),
    takeLatest(GET_PROFILE, getProfile),
    takeLatest(GET_BALANCE, getBalance),
    takeLatest(SEND_CONFIRM_COIN, sendConfirmCoin),
    takeLatest(SEND_ACTUAL_COIN, sendActualCoin),
    takeLatest(REQUEST_LOGIN_WALLET, requestLoginWallet),
    takeLatest(VERIFY_LOGIN_WALLET, verifyLoginWallet),
    takeLatest(UPDATE_TOKEN, updateToken),
    takeLatest(CHANGE_PASSWORD, changePassword),
    takeLatest(REQUEST_RESET_PASSWORD, requestResetPassword),
    takeLatest(CONFIRM_RESET_PASSWORD, confirmResetPassword),
    takeLatest(UPDATE_PROFILE, updateProfile),
    takeLatest(GET_STATISTICS, getStatistics),
    takeLatest(GET_COIN_TRANSACTION, getCoinTransaction),
    takeLatest(GET_COIN_TRANSACTION_ALL, getCoinTransactionAll),
];