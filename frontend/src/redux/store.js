
import { applyMiddleware, createStore, compose } from 'redux';
import {createWrapper } from 'next-redux-wrapper';
import createSagaMiddleware from 'redux-saga'

import rootSaga from '../redux/sagas';
import rootReducer from '../redux/reducers';
import { ssrMode } from '@constants';

const getMiddleWare = (sagaMiddleware) => {
   if (process.env.NODE_ENV === 'development') {
      const composeEnhancers = ssrMode ? compose : window?.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;

      return composeEnhancers(applyMiddleware(sagaMiddleware));
   }

   return applyMiddleware(sagaMiddleware);
}

export const makeStore = (initialState) => {
   const sagaMiddleware = createSagaMiddleware();
   const store = createStore(
      rootReducer,
      initialState,
      getMiddleWare(sagaMiddleware)
   );
   store.sagaTask = sagaMiddleware.run(rootSaga);
   return store;
};

export const wrapper = createWrapper(makeStore, { debug: false });