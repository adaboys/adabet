import { HYDRATE } from 'next-redux-wrapper';
import { handleActions } from 'redux-actions';
import { walletActionTypes } from '../actions';
import { createSuccessActionType } from '../helper';

const { GET_CONNECTED_WALLET, UPDATE_CONNECTED_WALLET_LOCAL } = walletActionTypes;

const initialState = {
    connectedWallets: []
};

const wallet = handleActions(
    {
        [HYDRATE]: (state, action) => {
            return { ...state, ...action.payload.dashboard }
        },
        [createSuccessActionType(GET_CONNECTED_WALLET)]: (state, { payload }) => {
            return {
                ...state,
                connectedWallets: payload.data?.wallets || []
            };
        },
        [UPDATE_CONNECTED_WALLET_LOCAL]: (state, { payload }) => {
            return {
                ...state,
                connectedWallets: payload.connectedWallets || []
            };
        },
    },
    initialState
);

export default wallet;