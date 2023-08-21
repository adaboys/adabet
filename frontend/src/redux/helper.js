import { accountActions, accountActionTypes, loadingActions } from '@redux/actions';
import { call, put, select, take } from 'redux-saga/effects';
import jwtDecode from 'jwt-decode';
import { eraseCookie } from '@utils/localStorage';

import { sendRequest, handleApiResponse } from '@utils/api';
import { storageKeys, CLIENT_TYPE } from '@constants';
import apiConfig from '@constants/apiConfig';

const { startLoading, finishLoading } = loadingActions;

export const createSuccessActionType = (type) => `${type}_SUCCESS`;
export const createFailureActionType = (type) => `${type}_FAILURE`;

function* sendRequestWithRefreshToken(options, params) {
    let canSendRequest = true;
    if (options.isAuth) {
        const { accessToken, refreshToken } =  yield select(state => state.account);
        if (accessToken && refreshToken) {
            let accessTokenExpiredTime = 0;
            options.accessToken = accessToken;
            try {
                const jwtData = jwtDecode(accessToken);
                accessTokenExpiredTime = (jwtData?.exp || 0) * 1000;
            }
            catch(ex) {
                console.log(ex)
            }
            if (accessTokenExpiredTime < new Date().getTime()) {
                const { isRefreshingToken } = yield select(state => state.account);
                if (isRefreshingToken) {
                    yield take(accountActionTypes.UPDATE_TOKEN);
                }
                else {
                    yield put(accountActions.refreshingToken(true));
                    const refreshResponse = yield call(sendRequest, apiConfig.account.refreshToken, {
                        access_token: accessToken,
                        refresh_token: refreshToken,
                        client_type: CLIENT_TYPE
                    });
                    yield put(accountActions.refreshingToken(false));

                    const newAccessToken = refreshResponse?.responseData?.data?.access_token;
                    const newRefreshToken = refreshResponse?.responseData?.data?.refresh_token;

                    if (refreshResponse?.success && newAccessToken && newRefreshToken) {
                        options.accessToken = newAccessToken;
                        yield put(accountActions.updateToken({ accessToken: newAccessToken, refreshToken: newRefreshToken }));
                    }
                    else {
                        canSendRequest = false;
                    }
                }
            }
        }
        else {
            canSendRequest = false;
        }
    }
    else if(options.isMaybeAuth) {
        const { accessToken } =  yield select(state => state.account);
        if(accessToken) {
            let accessTokenExpiredTime = 0;
            try {
                const jwtData = jwtDecode(accessToken);
                accessTokenExpiredTime = (jwtData?.exp || 0) * 1000;
            }
            catch(ex) {
                console.log(ex)
            }
            if (accessTokenExpiredTime >= new Date().getTime()) {
                options.accessToken = accessToken;
                options.isAuth = true;
            }
        }
    }

    if (canSendRequest) {
        const response = yield call(sendRequest, options, params);
        if (response?.isLogout) {
            yield* clearUserData();
            return null;
        }
        return response;
    }
    else {
        yield* clearUserData();
        return null;
    }
}

function* clearUserData() {
    eraseCookie(storageKeys.ACCESS_TOKEN);
    eraseCookie(storageKeys.REFRESH_TOKEN);
    yield put(accountActions.updateCommonInfo({ accessToken: null, refreshToken: null, profileData: null }));
}

export function* processLoadingAction(options, { payload, type }) {
    const SUCCESS = createSuccessActionType(type);
    const FAILURE = createFailureActionType(type);
    yield put(startLoading(type));
    try {
        const response = yield* sendRequestWithRefreshToken(options, payload);
        yield put({
            type: response?.success ? SUCCESS : FAILURE,
            payload: response?.responseData
        });
    } catch (e) {
        console.log(e);
        yield put({
            type: FAILURE,
            payload: e,
            error: true
        });
    }
    yield put(finishLoading(type));
}

export function* processAction(options, { payload, type }) {
    const SUCCESS = createSuccessActionType(type);
    const FAILURE = createFailureActionType(type);
    try {
        const response = yield* sendRequestWithRefreshToken(options, payload);
        yield put({
            type: response?.success ? SUCCESS : FAILURE,
            payload: response?.responseData
        });
    } catch (e) {
        console.log(e);
        yield put({
            type: FAILURE,
            payload: e,
            error: true
        });
    }
}

export function* processCallbackAction(options, { payload }) {
    const { params, onCompleted, onError } = payload;
    try {
        const result = yield* sendRequestWithRefreshToken(options, params);
        handleApiResponse(result, onCompleted, onError);
    } catch (error) {
        console.log(error);
        onError(error);
    }
}

