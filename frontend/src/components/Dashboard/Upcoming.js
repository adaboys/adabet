import React, { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';

import { sportActions } from '@redux/actions';

import MatchesList from '@components/Sport/Dashboard/Matches/MatchesList';

import styles from './Upcoming.module.scss';
import { Loading } from '@components/Common';

const Upcoming = () => {
    const matches = useSelector(state => state.sport?.matchesUpcoming || []);
    const loading = useSelector(state => state.loading.matchesUpcoming);
    const dispatch = useDispatch();

    useEffect(() => {
        dispatch(sportActions.getMatchesUpcoming({ id: 1, page: 1, item: 20 }));
    }, [])

    return (
        <div className={styles.upcoming}>
            <h3 className={styles.title}>Upcoming Events</h3>
            {loading ? (
                <Loading style={{margin: '40px 0'}}/>
            ) : (
                <div className={styles.list}>
                    <MatchesList matches={matches.slice(0, 3)} />
                </div>
            )}
        </div>
    );
};

export default Upcoming;