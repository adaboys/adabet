import { all } from 'redux-saga/effects';

import account from './account';
import sport from './sport';
import bet from './bet';
import airdropEvent from './airdropEvent';
import wallet from './wallet';
import coin from './coin';

const sagas = [
    ...account,
    ...sport,
    ...bet,
    ...airdropEvent,
    ...wallet,
    ...coin,
];

function* rootSaga() {
    yield all(sagas);
}

export default rootSaga;