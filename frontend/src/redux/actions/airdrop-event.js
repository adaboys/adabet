import { createAction } from 'redux-actions';

export const actionTypes = {
    GET_MATCH_LIST: 'airdrop-event/GET_MATCH_LIST',
    GET_PREDICTED_USERS: 'airdrop-event/GET_PREDICTED_USERS',
    LEADERBOARD: 'airdrop-event/LEADERBOARD',
    PREDICT: 'airdrop-event/PREDICT',
    GET_PREDICT_MATCH_DETAILS: 'airdrop-event/GET_PREDICT_MATCH_DETAILS',
}

export const actions = {
    getMatchList: createAction(actionTypes.GET_MATCH_LIST),
    getPredictedUsers: createAction(actionTypes.GET_PREDICTED_USERS),
    getLeaderboard: createAction(actionTypes.LEADERBOARD),
    predict: createAction(actionTypes.PREDICT),
    getPredictMatchDetails: createAction(actionTypes.GET_PREDICT_MATCH_DETAILS),
}
