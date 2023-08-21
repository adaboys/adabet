import React, { useContext, useEffect, useState } from 'react';
import { useDispatch } from 'react-redux';
import Link from 'next/link';
import { useRouter } from 'next/router';

import { BasicModal, BasicSlider, Button, Loading, EmptyResult } from '@components/Common';
import MatchItem from './MatchItem';
import MatchShortInfo from '@components/AirdropEvent/MathShortInfo';

import ArrowDownIcon from '@assets/icons/arrow-down.svg';

import { overlayTypes, paths } from '@constants';
import { airdropEventActions, loadingActions } from '@redux/actions';
import { useAuth, useNotification } from '@hooks';
import { OverlayContext } from '@hocs';

import styles from './index.module.scss';

const Highlight = () => {
    const { user, isAuthenticated } = useAuth();
    const { query } = useRouter();
    const [showQuickBet, setShowQuickBet] = useState(false);
    const [loading, setLoading] = useState(false);
    const [matchList, setMatchList] = useState([]);
    const [matchSelected, setMatchSelected] = useState(null);
    const [score, setScore] = useState({ score1: 0, score2: 0 });
    const { showError, showSuccess } = useNotification();
    const dispatch = useDispatch();
    const overlay = useContext(OverlayContext);

    const getMatchList = () => {
        setLoading(true);
        dispatch(airdropEventActions.getMatchList({
            sportId: query.id ?? 1,
            params: {
                page: 1,
                item: 18,
            },
            onCompleted: ({ data }) => {
                setMatchList(data?.matches || []);
                setLoading(false);
            },
            onError: error => {
                console.log({ error });
                setLoading(false);
            }
        }));
    }

    const processBet = (match) => {
        setMatchSelected(match);
        setScore({ score1: 0, score2: 0 });
        setShowQuickBet(true);
    }

    const quickBet = match => {
        if (!isAuthenticated) {
            return overlay.show(overlayTypes.LOGIN, { callback: () => processBet(match) });
        }

        processBet(match);
    }

    const handleSubmit = () => {
        dispatch(loadingActions.showLoadingFullScreen());
        dispatch(airdropEventActions.predict({
            matchId: matchSelected.id,
            params: {
                score1: score.score1,
                score2: score.score2,
                reward_address: user?.wallet_address,
            },
            onCompleted: ({ data, status, message }) => {
                if (status === 200) {
                    showSuccess('Bet success');
                } else {
                    showError(message ?? 'Error undefined!');
                }
                setShowQuickBet(false);
                dispatch(loadingActions.hideLoadingFullScreen());
            },
            onError: error => {
                console.log({ error });
                setShowQuickBet(false);
                showError(error?.message ?? 'Error undefined!');
                dispatch(loadingActions.hideLoadingFullScreen());
            }
        }));
    }

    useEffect(() => {
        getMatchList();
    }, []);

    const renderMatch = () => {
        const itemPerSlice = 6;
        const sliceCount = matchList.length / itemPerSlice;

        const result = [];
        for (let slice = 0; slice < sliceCount; slice++) {
            const currentStartIndex = slice * itemPerSlice;
            result.push(
                <div className={styles.matchContainer} key={slice}>
                    {matchList.slice(currentStartIndex, currentStartIndex + itemPerSlice).map(match => (
                        <MatchItem
                            setShowQuickBet={quickBet}
                            match={match}
                            key={match.id}
                        />
                    ))}
                </div>
            );
        }

        return result;
    }

    return (
        <div className={styles.highlight}>
            <BasicModal
                isOpen={showQuickBet}
                contentClassName={styles.quickBet}
                onRequestClose={() => setShowQuickBet(false)}
            >
                <div className={styles.head}>
                    <h3>Quick Bet</h3>
                    <Link href={{ pathname: paths.highlightDetails, query: { id: matchSelected?.id } }}>
                        <Button>
                            Show more
                            <ArrowDownIcon />
                        </Button>
                    </Link>
                </div>
                <MatchShortInfo
                    className={styles.matchInfo}
                    betable
                    small
                    match={matchSelected}
                    score={score}
                    setScore={setScore}
                />
                <div className={styles.actions}>
                    <Button className={styles.btnCancel} onClick={() => setShowQuickBet(false)}>Cancel</Button>
                    <Button onClick={handleSubmit}>Confirm</Button>
                </div>
                <div className={styles.fee}>Fee 0.5 ADA</div>
            </BasicModal>
            <div className={styles.sliderBox}>
                {loading ? (
                    <Loading style={{ margin: '40px 0' }} />
                ) : (
                    matchList.length ? (
                        <BasicSlider
                            draggable
                            desktopItems={1}
                            tabletItems={1}
                            arrows={false}
                            showDots
                            renderDotsOutside
                            dotListClass={styles.dots}
                        >
                            {renderMatch()}
                        </BasicSlider>
                    ) : (
                        <EmptyResult />
                    )
                )}
            </div>
        </div>
    );
};

export default Highlight;