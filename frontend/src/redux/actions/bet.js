import { createAction } from 'redux-actions';

export const actionTypes = {
    UPDATE_BET: 'bet/UPDATE_BET',
    REMOVE_BET: 'bet/REMOVE_BET',
    PLACE_BET: 'bet/PLACE_BET',
    MY_BET: 'bet/MY_BET',
}

export const actions = {
    updateBet: createAction(actionTypes.UPDATE_BET),
    removeBet: createAction(actionTypes.REMOVE_BET),
    placeBet: createAction(actionTypes.PLACE_BET),
    myBet: createAction(actionTypes.MY_BET),
}
