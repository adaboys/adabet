import { HYDRATE } from 'next-redux-wrapper';
import { handleActions } from 'redux-actions';
import { sportActionTypes } from '../actions';
import { createSuccessActionType } from '../helper';

const {
    GET_SPORT_LIST,
    GET_MATCHES_HIGHLIGHT,
    GET_MATCHES_TOP,
    GET_MATCHES_LIVE,
    GET_MATCHES_UPCOMING
} = sportActionTypes;

const initialState = {
    sports: [],
    matchesHighlight: [],
    matchesTop: [],
    matchesLive: [],
    matchesUpcoming: []
};

const sport = handleActions(
    {
        [HYDRATE]: (state, action) => {
            return { ...state, ...action.payload.sport }
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
    },
    initialState
);

export default sport;