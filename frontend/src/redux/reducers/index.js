import { combineReducers } from 'redux';
import loading from './loading';
import account from './account';
import sport from './sport';

const rootReducer = combineReducers({
    loading,
    sport,
    account,
});

export default rootReducer;