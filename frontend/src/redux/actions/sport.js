import { createAction } from 'redux-actions';

export const actionTypes = {
    GET_SPORT_LIST: 'sport/GET_SPORT_LIST',
    GET_MATCHES_HIGHLIGHT: 'sport/GET_MATCHES_HIGHLIGHT',
    GET_MATCHES_TOP: 'sport/GET_MATCHES_TOP',
    GET_MATCHES_LIVE: 'sport/GET_MATCHES_LIVE',
    GET_MATCHES_UPCOMING: 'sport/GET_MATCHES_UPCOMING',
    GET_MATCHES_FAVORITE: 'sport/GET_MATCHES_FAVORITE',
    GET_MATCHES_HISTORY: 'sport/GET_MATCHES_HISTORY',
    TOGGLE_FAVORITE_MATCH: 'sport/TOGGLE_FAVORITE_MATCH',
    UPDATE_FAVORITE_LOCAL: 'sport/UPDATE_FAVORITE_LOCAL',
    GET_TOTAL_BADGES: 'sport/GET_TOTAL_BADGES',
}

export const actions = {
    getSportList: createAction(actionTypes.GET_SPORT_LIST),
    getMatchesHighlight: createAction(actionTypes.GET_MATCHES_HIGHLIGHT),
    getMatchesTop: createAction(actionTypes.GET_MATCHES_TOP),
    getMatchesLive: createAction(actionTypes.GET_MATCHES_LIVE),
    getMatchesUpcoming: createAction(actionTypes.GET_MATCHES_UPCOMING),
    getMatchesFavorite: createAction(actionTypes.GET_MATCHES_FAVORITE),
    getMatchesHistory: createAction(actionTypes.GET_MATCHES_HISTORY),
    toggleFavoriteMatch: createAction(actionTypes.TOGGLE_FAVORITE_MATCH),
    updateFavoriteLocal: createAction(actionTypes.UPDATE_FAVORITE_LOCAL),
    getTotalBadges: createAction(actionTypes.GET_TOTAL_BADGES),
}

