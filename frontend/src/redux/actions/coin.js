import { createAction } from 'redux-actions';

export const actionTypes = {
    CALC_SWAP_AMOUNT: 'coin/CALC_SWAP_AMOUNT',
    SWAP_COINT: 'coin/SWAP_COINT',
}

export const actions = {
    calcSwapAmount: createAction(actionTypes.CALC_SWAP_AMOUNT),
    swapCoin: createAction(actionTypes.SWAP_COINT),
}

