import { useEffect, useState } from 'react';
import classNames from 'classnames';
import { useDispatch, useSelector } from 'react-redux';
import Link from 'next/link';

import SportLayout from '../SportLayout';
import { ChipButton, EmptyResult, Redirect, Loading } from '@components/Common';
import { paths, SPORT_DEFAULT_ID } from '@constants';
import { betActions, betActionTypes } from '@redux/actions';

import NoteIcon from '@assets/icons/note.svg';

import { useAuth } from '@hooks';

import styles from './index.module.scss';
import { useRouter } from 'next/router';
import BetItem from './BetItem';

const ALL_TAB = 'all';
const OPEN_TAB = 'open';
const WON_TAB = 'won';
const LOST_TAB = 'lost';

const tabs = [
    { key: ALL_TAB, text: 'All' },
    { key: OPEN_TAB, text: 'Open bets' },
    { key: WON_TAB, text: 'Won' },
    { key: LOST_TAB, text: 'Lost' }
]

const MyBet = () => {
    const [activeTab, setActiveTab] = useState(ALL_TAB);
    const { isAuthenticated } = useAuth();
    const dispatch = useDispatch();
    const { query } = useRouter();
    const bets = useSelector(state => state.bet.myBets);
    const loading = useSelector(state => state.loading[betActionTypes.MY_BET]);
    useEffect(() => {
        // if(query.id) {
            dispatch(betActions.myBet(
                {
                    ...(activeTab !== ALL_TAB && { tab: activeTab }),
                    page: 1,
                    item: 100,
                    id: query.id || SPORT_DEFAULT_ID
                }
            ))
        // }
    }, [activeTab])

    if (!isAuthenticated)
        return <Redirect url={paths.topMatches} />
    return (
        <div className={styles.myBet}>
            <SportLayout>
                <div className={styles.header}>
                    <NoteIcon />
                    My bets
                </div>
                <div className={styles.filter}>
                    <div className={styles.tags}>
                        {
                            tabs.map(tab => (
                                <ChipButton
                                    className={classNames(styles.item, {[styles.active]: tab.key === activeTab})}
                                    onClick={() => setActiveTab(tab.key)}
                                >
                                    {tab.text}
                                </ChipButton>
                            ))
                        }
                        {/* <Link href="/airdrop-event/highlight"><ChipButton className={styles.item}>Airdrop Event</ChipButton></Link> */}
                    </div>
                </div>
                {
                    loading !== false
                    ?
                    <Loading style={{margin: '80px 0'}}/>
                    :
                    bets?.length
                    ?
                    <div className={styles.betList}>
                        {
                            bets.map((bet, index) => (
                                <BetItem key={index} bet={bet}/>
                            ))
                        }
                    </div>
                    :
                    <EmptyResult/>
                }
            </SportLayout>
        </div>
    )
}

export default MyBet;