import { handleActions } from 'redux-actions';
import { HYDRATE } from 'next-redux-wrapper';

import { accountActionTypes } from '../actions';
import { createSuccessActionType, createFailureActionType } from '../helper';
import { setCookie } from '@utils/localStorage';
import { storageKeys, CURRENCY_ADA } from '@constants';

const {
    GET_PROFILE,
    GET_BALANCE,
    REFRESHING_TOKEN,
    UPDATE_COMMON_INFO,
    UPDATE_CURRENCY
} = accountActionTypes;

const initialState = {
    profileData: null,
    balances: [],
    currency: CURRENCY_ADA,
    isGetTokenForGamePlaySuccess: undefined,
    isRefreshingToken: false,
    isMobile: false,
    accessToken: null,
    refreshToken: null
};

const account = handleActions(
    {
        [HYDRATE]: (state, action) => {
            return {...state, ...action.payload.account }
        },
        [createSuccessActionType(GET_PROFILE)]: (state, action) => {
            return {
                ...state,
                profileData: action.payload.data,
                isGetTokenForGamePlaySuccess: true
            };
        },
        [createFailureActionType(GET_PROFILE)]: (state, action) => {
            return {
                ...state,
                profileData: null,
                isGetTokenForGamePlaySuccess: false
            };
        },
        [createSuccessActionType(GET_BALANCE)]: (state, action) => {
            return {
                ...state,
                balances: action.payload?.data?.currencies || []
            };
        },
        [REFRESHING_TOKEN]: (state, action) => {
            return {
                ...state,
                isRefreshingToken: action.payload
            };
        },
        [UPDATE_COMMON_INFO]: (state, action) => {
            const payload = action.payload || {};
            return {
                ...state,
                ...payload
            };
        },
        [UPDATE_CURRENCY]: (state, { payload }) => {
            const expiredTimsMs = 1000 * 60 * 60 * 24 * 365; // 1 year
            setCookie(storageKeys.BET_CURRENCY, payload, expiredTimsMs);
            return {
                ...state,
                currency: payload
            }
        }
    },
    initialState
);

export default account;