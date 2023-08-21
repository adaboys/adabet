import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import Link from 'next/link';

import { sportActions } from '@redux/actions';
import { paths } from '@constants';

import MatchHighlightItem from '@components/Common/MatchHighlightItem';

import ArrowTailRight from '@assets/icons/arrow-tail-right.svg';

import styles from './StartPrediction.module.scss';

const StartPrediction = () => {
    const matches = useSelector(state => state.sport?.matchesHighlight || []);
    const loading = useSelector(state => state.loading.matchesHighlight);
    const dispatch = useDispatch();

    useEffect(() => {
        dispatch(sportActions.getMatchesHighlight({ id: 1 }));
    }, []);

    return (
        <div className={styles.startPrediction}>
            <h3 className={styles.title}>Let's Start Prediction</h3>
            <div className={styles.prediction}>
                {loading ? (
                    <Loading style={{ margin: '40px 0' }} />
                ) : (
                    matches.slice(0, 3).map((item, index) => <MatchHighlightItem item={item} key={index} />)
                )}
            </div>
            <div className={styles.actions}>
                <Link href={paths.topMatches}>
                    <button>
                        Prediction more
                        <ArrowTailRight />
                    </button>
                </Link>
            </div>
        </div>
    );
};

export default StartPrediction;