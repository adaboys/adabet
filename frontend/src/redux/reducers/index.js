import { combineReducers } from 'redux';
import loading from './loading';
import account from './account';
import sport from './sport';
import bet from './bet';
import wallet from './wallet';

const rootReducer = combineReducers({
    loading,
    sport,
    account,
    bet,
    wallet
});

export default rootReducer;