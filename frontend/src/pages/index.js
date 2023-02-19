import React from 'react';
import { wrapper } from '../redux/store';
import Dashboard from '@components/Dashboard';
// import { homeActions } from '@redux/actions';


const DashboardPage = () => {
    return (
        <Dashboard/>
    )
}

DashboardPage.getInitialProps = wrapper.getInitialPageProps(store => () => {
    // store.dispatch(homeActions.getData());
});

export default DashboardPage;