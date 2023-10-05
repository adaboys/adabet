import { HYDRATE } from 'next-redux-wrapper';
import { handleActions } from 'redux-actions';
import { sportActionTypes } from '../actions';
import { createSuccessActionType } from '../helper';

const {
    GET_SPORT_LIST,
    GET_MATCHES_HIGHLIGHT,
    GET_MATCHES_TOP,
    GET_MATCHES_LIVE,
    GET_MATCHES_UPCOMING,
    GET_MATCHES_FAVORITE,
    UPDATE_FAVORITE_LOCAL,
    GET_TOTAL_BADGES
} = sportActionTypes;

const initialState = {
    sports: [],
    matchesHighlight: [],
    matchesTop: [],
    matchesLive: [],
    matchesUpcoming: [],
    matchesFavorite: [],
    totalBadges: {}
};

const sport = handleActions(
    {
        [HYDRATE]: (state, action) => {
            return { ...state, ...action.payload?.sport }
        },
        [createSuccessActionType(GET_SPORT_LIST)]: (state, { payload }) => {
            return {
                ...state,
                sports: payload.data?.sports || []
            };
        },
        [createSuccessActionType(GET_MATCHES_HIGHLIGHT)]: (state, { payload }) => {
            return {
                ...state,
                matchesHighlight: payload.data?.matches || []
            };
        },
        [createSuccessActionType(GET_MATCHES_TOP)]: (state, { payload }) => {
            return {
                ...state,
                matchesTop: payload.data?.matches || []
            };
        },
        [createSuccessActionType(GET_MATCHES_LIVE)]: (state, { payload }) => {
            return {
                ...state,
                matchesLive: payload.data?.matches || []
            };
        },
        [createSuccessActionType(GET_MATCHES_UPCOMING)]: (state, { payload }) => {
            return {
                ...state,
                matchesUpcoming: payload.data?.matches || []
            };
        },
        [createSuccessActionType(GET_MATCHES_FAVORITE)]: (state, { payload }) => {
            return {
                ...state,
                matchesFavorite: payload.data?.matches || []
            };
        },
        [UPDATE_FAVORITE_LOCAL]: (state, { payload }) => {
            const { dataKey, toggleOn, matchId } = payload;
            const matchList = state[dataKey] || [];
            matchList.forEach(match => {
                if(match.id === matchId) {
                    console.log(toggleOn)
                    console.log(match);
                    match.favorited = toggleOn;
                }
            })
            console.log(dataKey);
            return {
                ...state,
                [dataKey]: [...matchList]
            };
        },
        [createSuccessActionType(GET_TOTAL_BADGES)]: (state, { payload }) => {
            return {
                ...state,
                totalBadges: payload.data || {}
            };
        },
    },
    initialState
);

export default sport;