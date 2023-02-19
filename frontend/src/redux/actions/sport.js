import { createAction } from 'redux-actions';

export const actionTypes = {
    GET_SPORT_LIST: 'sport/GET_SPORT_LIST',
    GET_MATCHES_HIGHLIGHT: 'sport/GET_MATCHES_HIGHLIGHT',
    GET_MATCHES_TOP: 'sport/GET_MATCHES_TOP',
    GET_MATCHES_LIVE: 'sport/GET_MATCHES_LIVE',
    GET_MATCHES_UPCOMING: 'sport/GET_MATCHES_UPCOMING',
}

export const actions = {
    getSportList: createAction(actionTypes.GET_SPORT_LIST),
    getMatchesHighlight: createAction(actionTypes.GET_MATCHES_HIGHLIGHT),
    getMatchesTop: createAction(actionTypes.GET_MATCHES_TOP),
    getMatchesLive: createAction(actionTypes.GET_MATCHES_LIVE),
    getMatchesUpcoming: createAction(actionTypes.GET_MATCHES_UPCOMING),
}

