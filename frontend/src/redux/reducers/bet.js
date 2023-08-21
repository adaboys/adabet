import { handleActions } from 'redux-actions';

import { betActionTypes } from '../actions';

import { setObjectData, getObjectData, removeItem } from '@utils/localStorage';
import { storageKeys, dataStorageVersion } from '@constants';
import { createSuccessActionType, createFailureActionType } from '../helper';

const { UPDATE_BET, REMOVE_BET, MY_BET } = betActionTypes;

const initialState = {
    data: getObjectData(storageKeys.BET_DATA) || {},
    myBets: []
};

const bet = handleActions(
    {
        [REMOVE_BET]: (state) => {
            removeItem(storageKeys.BET_DATA);
            return {
                ...state,
                data: {}
            }
        },
        [UPDATE_BET]: (state, { payload }) => {
            setObjectData(storageKeys.BET_DATA, {...payload, version: dataStorageVersion});
            return {
                ...state,
                data: payload
            }
        },
        [createSuccessActionType(MY_BET)]: (state, { payload }) => {
            return {
                ...state,
                myBets: payload?.data?.bets || []
            };
        },
    },
    initialState
);

export default bet;
