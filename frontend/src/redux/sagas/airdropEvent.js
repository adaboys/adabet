import { takeLatest } from 'redux-saga/effects';

import { airdropEventActionTypes } from '../actions';
import apiConfig from '@constants/apiConfig';

import { processCallbackAction } from '../helper';

const {
    GET_MATCH_LIST,
    GET_PREDICTED_USERS,
    LEADERBOARD,
    PREDICT,
    GET_PREDICT_MATCH_DETAILS,
} = airdropEventActionTypes;

const getMatchList = (action) => {
    const { payload } = action;

    return processCallbackAction({
        ...apiConfig.airdropEvent.getMatchList,
        path: apiConfig.airdropEvent.getMatchList.path.replace(':sportId', payload.sportId)
    }, action);
}
const getPredictedUsers = (action) => {
    const { payload } = action;

    return processCallbackAction({
        ...apiConfig.airdropEvent.getPredictedUsers,
        path: apiConfig.airdropEvent.getPredictedUsers.path.replace(':matchId', payload.matchId)
    }, action);
}
const getLeaderboard = (action) => {
    return processCallbackAction(apiConfig.airdropEvent.getLeaderboard, action);
}

const predict = (action) => {
    const { payload } = action;

    return processCallbackAction({
        ...apiConfig.airdropEvent.predict,
        path: apiConfig.airdropEvent.predict.path.replace(':matchId', payload.matchId)
    }, action);
}

const getPredictMatchDetails = (action) => {
    const { payload } = action;

    return processCallbackAction({
        ...apiConfig.airdropEvent.getPredictMatchDetails,
        path: apiConfig.airdropEvent.getPredictMatchDetails.path.replace(':matchId', payload.matchId),
    }, action);
}

export default [
    takeLatest(GET_MATCH_LIST, getMatchList),
    takeLatest(GET_PREDICTED_USERS, getPredictedUsers),
    takeLatest(LEADERBOARD, getLeaderboard),
    takeLatest(PREDICT, predict),
    takeLatest(GET_PREDICT_MATCH_DETAILS, getPredictMatchDetails),
];