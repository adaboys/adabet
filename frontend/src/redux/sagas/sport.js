import { takeLatest } from 'redux-saga/effects';

import { sportActionTypes } from '../actions';
import apiConfig from '@constants/apiConfig';

import { processLoadingAction } from '../helper';

const {
   GET_SPORT_LIST,
   GET_MATCHES_HIGHLIGHT,
   GET_MATCHES_TOP,
   GET_MATCHES_LIVE,
   GET_MATCHES_UPCOMING
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
    return processLoadingAction({
        ...apiConfig.sport.getMatchesTop,
        path: apiConfig.sport.getMatchesTop.path.replace(':sportId', payload?.id)
    }, { type });
}

const getMatchesLive = ({ payload, type }) => {
    return processLoadingAction({
        ...apiConfig.sport.getMatchesLive,
        path: apiConfig.sport.getMatchesLive.path.replace(':sportId', payload?.id)
    }, { type });
}

const getMatchesUpcoming = ({ payload, type }) => {
    return processLoadingAction({
        ...apiConfig.sport.getMatchesUpcoming,
        path: apiConfig.sport.getMatchesUpcoming.path.replace(':sportId', payload?.id)
    }, { type });
}

export default [
    takeLatest(GET_SPORT_LIST, getSportList),
    takeLatest(GET_MATCHES_HIGHLIGHT, getMatchesHighlight),
    takeLatest(GET_MATCHES_TOP, getMatchesTop),
    takeLatest(GET_MATCHES_LIVE, getMatchesLive),
    takeLatest(GET_MATCHES_UPCOMING, getMatchesUpcoming)
];