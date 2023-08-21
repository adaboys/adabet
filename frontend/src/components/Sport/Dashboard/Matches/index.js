import { useEffect, useContext, useMemo, useState, useRef } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { useRouter } from 'next/router';


import { Loading, EmptyResult, Redirect } from '@components/Common';
import { sportActions, sportActionTypes } from '@redux/actions';
import { useAuth, useNotification } from '@hooks';
import { OverlayContext } from '@hocs';
import { generateSportUrl } from '@utils'
import { matcheTypes, overlayTypes, paths } from '@constants';

import Tabs from '../../SportLayout/Tabs';
import FilterTags from './FilterTags';
import MatchesList from './MatchesList';
import MatchesHistoryModal from './MatchesHistoryModal';

import CupIcon from '@assets/icons/cup.svg';
import VideoIcon from '@assets/icons/video.svg';
import ClockIcon from '@assets/icons/clock.svg';
import StarIcon from '@assets/icons/star.svg';

import styles from './index.module.scss';

const metadates = {
    [matcheTypes.TOP]: {
        stateDataListKey: 'matchesTop',
        dataLoading: sportActionTypes.GET_MATCHES_TOP,
        getMatchesAction: sportActions.getMatchesTop
    },
    [matcheTypes.LIVE]: {
        stateDataListKey: 'matchesLive',
        dataLoading: sportActionTypes.GET_MATCHES_LIVE,
        getMatchesAction: sportActions.getMatchesLive
    },
    [matcheTypes.UPCOMING]: {
        stateDataListKey: 'matchesUpcoming',
        dataLoading: sportActionTypes.GET_MATCHES_UPCOMING,
        getMatchesAction: sportActions.getMatchesUpcoming
    },
    [matcheTypes.FAVORITE]: {
        stateDataListKey: 'matchesFavorite',
        dataLoading: sportActionTypes.GET_MATCHES_FAVORITE,
        getMatchesAction: sportActions.getMatchesFavorite
    }
}

const Matches = ({ queries, addBet, isOddsSelected }) => {
    const dispatch = useDispatch();
    const { query, push } = useRouter();
    const [isOpenHistoryModal, setIsOpenHistoryModal] = useState(false);
    const currentMatch = useRef(null);
    
    const metadata = metadates[query?.matchType];
    const matches = useSelector(state => state.sport[metadata.stateDataListKey] || []);
    const loading = useSelector(state => state.loading[metadata.dataLoading]);
    const { isAuthenticated } = useAuth();
    const overlay = useContext(OverlayContext);
    const { showError, showSuccess } = useNotification();

    const onToggleMatch = (matchId, toggleOn) => {
        const action = toggleOn ? 'Add' : 'Remove'
        if(isAuthenticated) {
            dispatch(sportActions.toggleFavoriteMatch({
                matchId,
                toggleOn,
                onCompleted: response => {
                    if(response?.status === 200) {
                        showSuccess(`${action} favorite successful!`);
                        if(query.matchType === matcheTypes.FAVORITE) {
                            getMatches();
                        }
                        else {
                            dispatch(sportActions.updateFavoriteLocal({ dataKey: metadata.stateDataListKey, toggleOn, matchId }))
                        }
                    }
                    else {
                        showError(`${action} favorite failed!`);
                    }
                },
                onError: err => {
                    console.log(err);
                    showError(`${action} favorite failed!`);
                }
            }));
        }
        else {
            overlay.show(overlayTypes.LOGIN);
        }
    }

    const getMatches = (loading) => {
        dispatch(metadata.getMatchesAction({ id: queries.id, page: 1, item: 20, loading }));
    }

    const onTabClick = (matchType) => {
        const url = generateSportUrl({ ...query, matchType });
        if(matchType === matcheTypes.FAVORITE && !isAuthenticated) {
            overlay.show(overlayTypes.LOGIN, { callback: () => push(url) })
        }
        else {
            push(url);
        }
    }

    const onShowhistory = (matchesItem) => {
        dispatch(sportActions.getMatchesHistory({
            matchId: matchesItem.id,
            onCompleted: response => {
                if(response?.data) {
                    currentMatch.current = {
                        ...matchesItem,
                        history: response.data
                    }
                    setIsOpenHistoryModal(true);
                }
                else {
                    throw new Error('error');
                }
            },
            onError: err => {
                console.log(err);
                showError('Get match history failed!');
            }
        }));
    }

    useEffect(() => {
        if (queries.id) {
            getMatches(true);
        }
    }, [query])

    const tabs = useMemo(() => {
        return [
            { name: 'Top Matches', icon: CupIcon, key: matcheTypes.TOP },
            { name: 'Live Matches', icon: VideoIcon, key: matcheTypes.LIVE },
            { name: 'Upcoming Events', icon: ClockIcon, key: matcheTypes.UPCOMING },
            { name: 'Favorite', icon: StarIcon, key: matcheTypes.FAVORITE } //activeIcon: () => <StarActiveIcon style={{ width: '20px', height: '20px'}}/>
        ]
    }, [query])

    if (query.matchType === matcheTypes.FAVORITE && !isAuthenticated)
        return <Redirect url={paths.topMatches} />

    return (
        <div className={styles.matches}>
            <Tabs
                tabs={tabs}
                // tabActiveClassName={styles.tabActive}
                onTabClick={onTabClick}
                activeTab={query.matchType}
            />
            <FilterTags/>
            {
                loading !== false
                ?
                <Loading style={{margin: '40px 0'}}/>
                :
                matches?.length
                ?
                <MatchesList
                    matches={matches}
                    addBet={addBet}
                    isOddsSelected={isOddsSelected}
                    onToggleMatch={onToggleMatch}
                    onShowhistory={onShowhistory}
                />
                :
                <EmptyResult/>
            }

            {
                isOpenHistoryModal
                ?
                <MatchesHistoryModal
                    matchInfo={currentMatch.current}
                    onClose={() => setIsOpenHistoryModal(false)}
                />
                :
                null
            }
            
        </div>
    )
}

export default Matches;
