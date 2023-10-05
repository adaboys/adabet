import { call, put, takeLatest, select } from 'redux-saga/effects';

import { accountActions, coinActionTypes } from '../actions';
import apiConfig from '@constants/apiConfig';
import { eraseCookie, setCookie } from '@utils/localStorage';
import { storageKeys } from '@constants';
import { sendRequest } from '@utils/api';
import { processCallbackAction, processLoadingAction } from '../helper';

const {
    CALC_SWAP_AMOUNT,
    SWAP_COINT,
} = coinActionTypes;

const calcSwapAmount = (action) => {
    return processCallbackAction(apiConfig.coin.calcAmount, action);
}

const swapCoin = (action) => {
    return processCallbackAction(apiConfig.coin.swap, action);
}


export default [
    takeLatest(CALC_SWAP_AMOUNT, calcSwapAmount),
    takeLatest(SWAP_COINT, swapCoin),
];