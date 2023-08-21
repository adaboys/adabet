import { takeLatest } from 'redux-saga/effects';

import { betActionTypes } from '../actions';
import apiConfig from '@constants/apiConfig';

import { processCallbackAction, processLoadingAction } from '../helper';

const {
    PLACE_BET,
    MY_BET
} = betActionTypes;


const placeBet = (action) => {
    const { payload } = action;

    return processCallbackAction({
        ...apiConfig.sport.placeBet,
        path: apiConfig.sport.placeBet.path.replace(':sportId', payload.sportId)
    }, action);
}

const getMyBet = ({ payload, type }) => {
    return processLoadingAction({
        ...apiConfig.sport.myBet,
        path: apiConfig.sport.myBet.path.replace(':sportId', payload?.id)
    }, { type, payload: { page: payload.page, item: payload.item, tab: payload.tab } });
}

export default [
    takeLatest(PLACE_BET, placeBet),
    takeLatest(MY_BET, getMyBet),
];