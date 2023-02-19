import { all } from 'redux-saga/effects';

import account from './account';
import sport from './sport';

const sagas = [
    ...account,
    ...sport,
];

function* rootSaga() {
    yield all(sagas);
}

export default rootSaga;