import { takeLatest } from 'redux-saga/effects';

import { sportActionTypes } from '../actions';
import apiConfig from '@constants/apiConfig';

import { processLoadingAction, processCallbackAction, processAction } from '../helper';

const {
   GET_SPORT_LIST,
   GET_MATCHES_HIGHLIGHT,
   GET_MATCHES_TOP,
   GET_MATCHES_LIVE,
   GET_MATCHES_UPCOMING,
   GET_MATCHES_FAVORITE,
   TOGGLE_FAVORITE_MATCH,
   GET_TOTAL_BADGES,
   GET_MATCHES_HISTORY
} = sportActionTypes;


const getSportList = (action) => {
    return processLoadingAction(apiConfig.sport.getSportList, action);
}

const getMatchesHighlight = ({ payload, type }) => {
    return processLoadingAction({
        ...apiConfig.sport.getMatchesHighlight,
        path: apiConfig.sport.getMatchesHighlight.path.replace(':sportId', payload?.id)
    }, { type });
}

const getMatchesTop = ({ payload, type }) => {
    const sendRequestAction = payload?.loading ? processLoadingAction : processAction;
    return sendRequestAction({
        ...apiConfig.sport.getMatchesTop,
        path: apiConfig.sport.getMatchesTop.path.replace(':sportId', payload?.id)
    }, { type });
}

const getMatchesLive = ({ payload, type }) => {
    const sendRequestAction = payload?.loading ? processLoadingAction : processAction;
    return sendRequestAction({
        ...apiConfig.sport.getMatchesLive,
        path: apiConfig.sport.getMatchesLive.path.replace(':sportId', payload?.id)
    }, { type });
}

const getMatchesUpcoming = ({ payload, type }) => {
    const sendRequestAction = payload?.loading ? processLoadingAction : processAction;
    return sendRequestAction({
        ...apiConfig.sport.getMatchesUpcoming,
        path: apiConfig.sport.getMatchesUpcoming.path.replace(':sportId', payload?.id)
    }, { type, payload: { page: payload.page, item: payload.item } });
}

const getMatchesFavorite = ({ payload, type }) => {
    const sendRequestAction = payload?.loading ? processLoadingAction : processAction;
    return sendRequestAction({
        ...apiConfig.sport.getMatchesFavorite,
        path: apiConfig.sport.getMatchesFavorite.path.replace(':sportId', payload?.id)
    }, { type, payload: { page: payload.page, item: payload.item } });
}

const toggleFavoriteMatch = (action) => {
    const { payload } = action;

    return processCallbackAction({
        ...apiConfig.sport.toogleFavoriteMatch,
        path: `${apiConfig.sport.toogleFavoriteMatch.path.replace(':matchId', payload.matchId)}?toggle_on=${payload.toggleOn}`
    }, action);
}

const getTotalBadges = ({ payload, type }) => {
    return processLoadingAction({
        ...apiConfig.sport.totalBages,
        path: apiConfig.sport.totalBages.path.replace(':sportId', payload?.id)
    }, { type });
}

const getMatchesHistory = (action) => {
    const { payload } = action;

    return processCallbackAction({
        ...apiConfig.sport.getMatchesHistory,
        path: `${apiConfig.sport.getMatchesHistory.path.replace(':matchId', payload.matchId)}`
    }, action);
}


export default [
    takeLatest(GET_SPORT_LIST, getSportList),
    takeLatest(GET_MATCHES_HIGHLIGHT, getMatchesHighlight),
    takeLatest(GET_MATCHES_TOP, getMatchesTop),
    takeLatest(GET_MATCHES_LIVE, getMatchesLive),
    takeLatest(GET_MATCHES_UPCOMING, getMatchesUpcoming),
    takeLatest(GET_MATCHES_FAVORITE, getMatchesFavorite),
    takeLatest(TOGGLE_FAVORITE_MATCH, toggleFavoriteMatch),
    takeLatest(GET_TOTAL_BADGES, getTotalBadges),
    takeLatest(GET_MATCHES_HISTORY, getMatchesHistory)
];