import Tabs from '../../SportLayout/Tabs';
import FilterTags from './FilterTags';

import CupIcon from '@assets/icons/cup.svg';
import VideoIcon from '@assets/icons/video.svg';
import ClockIcon from '@assets/icons/clock.svg';

import { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { NumberParam, useQueryParams } from 'use-query-params';

import { Loading, EmptyResult } from '@components/Common';
import { sportActions, sportActionTypes } from '@redux/actions';
import styles from './index.module.scss';
import { useState } from 'react';
import { useMemo } from 'react';
import MatchesList from './MatchesList';

const MATCHES_TOP_TAB = 'matches-top-tab';
const MATCHES_LIVE_TAB = 'matches-live-tab';
const MATCHES_UPCOMING_TAB = 'matches-upcoming-tab';

const metadates = {
    [MATCHES_TOP_TAB]: {
        stateDataListKey: 'matchesTop',
        dataLoading: sportActionTypes.GET_MATCHES_TOP,
        getMatchesAction: sportActions.getMatchesTop
    },
    [MATCHES_LIVE_TAB]: {
        stateDataListKey: 'matchesLive',
        dataLoading: sportActionTypes.GET_MATCHES_LIVE,
        getMatchesAction: sportActions.getMatchesLive
    },
    [MATCHES_UPCOMING_TAB]: {
        stateDataListKey: 'matchesUpcoming',
        dataLoading: sportActionTypes.GET_MATCHES_UPCOMING,
        getMatchesAction: sportActions.getMatchesUpcoming
    }
}

const Matches = () => {
    const dispatch = useDispatch();
    const [queries] = useQueryParams({ id: NumberParam });
    const [activeTab, setActiveTab] = useState(MATCHES_TOP_TAB);

    const metadata = metadates[activeTab];
    const matches = useSelector(state => state.sport[metadata.stateDataListKey] || []);
    const loading = useSelector(state => state.loading[metadata.dataLoading]);

    useEffect(() => {
        if (queries.id) {
            dispatch(metadata.getMatchesAction({ id: queries.id, activeTab }));
        }
    }, [queries, activeTab])

    const tabs = useMemo(() => {
        return [
            { name: 'Top Matches', icon: CupIcon, key: MATCHES_TOP_TAB },
            { name: 'Live Matches', icon: VideoIcon, key: MATCHES_LIVE_TAB },
            { name: 'Upcoming Events', icon: ClockIcon, key: MATCHES_UPCOMING_TAB }
        ]
    }, [activeTab])

    return (
        <div className={styles.matches}>
            <Tabs tabs={tabs} tabActiveClassName={styles.tabActive} onTabClick={setActiveTab} activeTab={activeTab}/>
            <FilterTags/>
            {
                loading !== false
                ?
                <Loading style={{margin: '40px 0'}}/>
                :
                matches?.length
                ?
                <MatchesList matches={matches}/>
                :
                <EmptyResult/>
            }
            
        </div>
    )
}

export default Matches;
