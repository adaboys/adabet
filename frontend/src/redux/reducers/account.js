import { handleActions } from 'redux-actions';
import { HYDRATE } from 'next-redux-wrapper';

import { accountActionTypes } from '../actions';
import { createSuccessActionType, createFailureActionType } from '../helper';

const {
    GET_PROFILE,
    GET_BALANCE,
    REFRESHING_TOKEN,
    UPDATE_COMMON_INFO

} = accountActionTypes;

const initialState = {
    profileData: null,
    balance: {},
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
                balance: action.payload.data || {}
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
    },
    initialState
);

export default account;