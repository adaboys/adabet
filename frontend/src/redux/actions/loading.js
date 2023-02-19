import { createAction } from 'redux-actions';

export const actionTypes = {
    START_LOADING: 'loading/START_LOADING',
    FINISH_LOADING: 'loading/FINISH_LOADING',
    SHOW_LOADING_FULLSCREEN: 'loading/SHOW_LOADING_FULLSCREEN',
    HIDE_LOADING_FULLSCREEN: 'loading/HIDE_LOADING_FULLSCREEN',
}

export const actions = {
    startLoading: createAction(actionTypes.START_LOADING, requestType => requestType),
    finishLoading: createAction(actionTypes.FINISH_LOADING, requestType => requestType),
    showLoadingFullScreen: createAction(actionTypes.SHOW_LOADING_FULLSCREEN),
    hideLoadingFullScreen: createAction(actionTypes.HIDE_LOADING_FULLSCREEN)
}

