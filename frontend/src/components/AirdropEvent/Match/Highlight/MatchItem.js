import Link from 'next/link';
import React from 'react';
import moment from 'moment';

import { paths } from '@constants';

import MatchShortInfo from '@components/AirdropEvent/MathShortInfo';
import { Button } from '@components/Common';

import ArrowDownIcon from '@assets/icons/arrow-down.svg';

import styles from './MatchItem.module.scss';

const MatchItem = ({ setShowQuickBet, match }) => {
    const time = match.start_at ? moment.utc(match.start_at).local() : null;
    const isEnded = !time || time <= moment();

    return (
        <div className={styles.matchItem} style={{ ['--background']: 'url(/images/match-example.png)' }}>
            <div className={styles.content}>
                <MatchShortInfo className={styles.custom} small match={match} />
                <div className={styles.actions}>
                    <Button disabled={isEnded} onClick={() => setShowQuickBet(match)} className={styles.quickBet}>Quick Bet</Button>
                    <Link href={{ pathname: paths.highlightDetails, query: { id: match.id } }}>
                        <Button className={styles.btnShowMore}>Show more <ArrowDownIcon /></Button>
                    </Link>
                </div>
            </div>
        </div>
    );
};

export default MatchItem;