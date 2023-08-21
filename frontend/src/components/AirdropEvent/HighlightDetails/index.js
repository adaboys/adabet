import moment from 'moment';
import { useRouter } from 'next/router';
import { useContext, useEffect, useState } from 'react';
import { useDispatch } from 'react-redux';

import { BasicModal, Button, EmptyResult, Loading } from '@components/Common';
import PaginationNextPrev from '@components/Common/PaginationNextPrev';
import MatchShortInfo from '../MathShortInfo';
import Table from '../Table';

import ArrowTailRight from '@assets/icons/arrow-tail-right.svg';
import FireworksIcon from '@assets/icons/fireworks.svg';

import { overlayTypes } from '@constants';
import { OverlayContext } from '@hocs';
import { useAuth, useNotification } from '@hooks';
import { airdropEventActions, loadingActions } from '@redux/actions';
import { checkMatchEnded } from '@utils';

import styles from './index.module.scss';

const HighlightDetails = () => {
    const { user, isAuthenticated } = useAuth();
    const { showError } = useNotification();
    const { back, query } = useRouter();
    const [showConfirm, setShowConfirm] = useState(false);
    const [showSuccess, setShowSuccess] = useState(false);
    const [loading, setLoading] = useState(false);
    const [filter, setFilter] = useState({ page: 1 });
    const [predictedUserData, setPredictedUserData] = useState({});
    const [predictDetails, setPredictDetails] = useState({});
    const [loadingPredictDetails, setLoadingPredictDetails] = useState(false);
    const [score, setScore] = useState({ score1: 0, score2: 0 });
    const dispatch = useDispatch();
    const { page_count = 0, predictions = [] } = predictedUserData;
    const isMatchEnded = checkMatchEnded(predictDetails);
    const overlay = useContext(OverlayContext);

    const getPredictedUsers = () => {
        setLoading(true);
        dispatch(airdropEventActions.getPredictedUsers({
            matchId: query.id,
            params: {
                page: filter?.page,
                item: 10,
            },
            onCompleted: ({ data }) => {
                setPredictedUserData(data || {});
                setLoading(false);
            },
            onError: error => {
                console.log({ error });
                setLoading(false);
            }
        }));
    }

    const getPredictMatchDetails = () => {
        setLoadingPredictDetails(true);
        dispatch(airdropEventActions.getPredictMatchDetails({
            matchId: query.id,
            onCompleted: ({ data }) => {
                setPredictDetails(data?.match || {});
                setLoadingPredictDetails(false);
            },
            onError: error => {
                console.log({ error });
                setLoadingPredictDetails(false);
            }
        }));
    }

    const handleSubmit = () => {
        dispatch(loadingActions.showLoadingFullScreen());
        dispatch(airdropEventActions.predict({
            matchId: query.id,
            params: {
                score1: score.score1,
                score2: score.score2,
                reward_address: user?.wallet_address,
            },
            onCompleted: ({ data, status, message }) => {
                if (status === 200) {
                    setShowSuccess(true);
                } else {
                    showError(message ?? 'Error undefined!');
                }
                setShowConfirm(false);
                dispatch(loadingActions.hideLoadingFullScreen());
               console.log({ data });
            },
            onError: error => {
                console.log({ error });
                setShowConfirm(false);
                dispatch(loadingActions.hideLoadingFullScreen());
                showError(error?.message ?? 'Error undefined!');
            }
        }));
    }

    const handleBetClick = () => {
        if (isAuthenticated) {
            setShowConfirm(true)
        } else {
            overlay.show(overlayTypes.LOGIN, { callback: () => setShowConfirm(true) });
        }
    }

    useEffect(() => {
        getPredictMatchDetails();
    }, []);

    useEffect(() => {
        getPredictedUsers();
    }, [ filter ]);

    return (
        <div className={styles.highlightDetails}>
            <BasicModal
                isOpen={showConfirm}
                contentClassName={styles.confirmBet}
                onRequestClose={() => setShowConfirm(false)}
            >
                <h3 className={styles.title}>Submit</h3>
                <div className={styles.desc}>
                    Your order bet {predictDetails?.t1} - {predictDetails?.t2} ({score.score1} - {score.score2})
                </div>
                <div className={styles.actions}>
                    <Button className={styles.btnCancel} onClick={() => setShowConfirm(false)}>Cancel</Button>
                    <Button onClick={handleSubmit}>Confirm</Button>
                </div>
                <div className={styles.fee}>Fee 0.5 ADA</div>
            </BasicModal>
            <BasicModal
                isOpen={showSuccess}
                contentClassName={styles.successBet}
                onRequestClose={() => setShowSuccess(false)}
            >
                <FireworksIcon />
                <div className={styles.mes}>Congratulate ! Your bets have been successfully placed !</div>
                <Button onClick={() => setShowSuccess(false)}>View all bets</Button>
            </BasicModal>
            <div className="container">
                <button className={styles.btnBack} onClick={back}>
                    <ArrowTailRight /> Back
                </button>

                <main>
                    <div className={styles.match}>
                        <MatchShortInfo
                            match={predictDetails || {}}
                            className={styles.infoBox}
                            betable
                            score={score}
                            setScore={setScore}
                        />
                        <div className={styles.actions}>
                            <div className={styles.label}>Bet to get 50 ADA & 5% bonus</div>
                            <Button disabled={isMatchEnded} onClick={handleBetClick}>Bet Now</Button>
                            <div className={styles.fee}>
                                Fee 0.5 ADA
                            </div>
                        </div>
                    </div>
                    <div className={styles.separate} />
                    <div className={styles.betHistory}>
                        {!!page_count && (
                            <div className={styles.filter}>
                                {/* <BasicForm
                                    initialValues={{ orderBy: '' }}
                                >
                                    <SelectField
                                        options={[{ label: 'Most recent', value: 'recent' }]}
                                        name="orderBy"
                                        placeholder="Most recent"
                                        isSearchable={false}
                                    />
                                </BasicForm> */}
                                <PaginationNextPrev
                                    currentPage={filter.page}
                                    totalPage={page_count}
                                    className={styles.pagination}
                                    onNext={() => setFilter(prev => ({ ...prev, page: prev.page + 1 }))}
                                    onPrev={() => setFilter(prev => ({ ...prev, page: Math.max(prev.page - 1, 0) }))}
                                />
                            </div>
                        )}
                        <div className={styles.list}>
                            {loading ? (
                                <Loading />
                            ) : (
                                predictions?.length ? (
                                    <Table
                                        columns={[
                                            { title: 'Username', key: 'player_name' },
                                            { title: 'Bet Date', key: 'predicted_at', render: date => moment(date).format('DD/MM/YYYY HH:mm:ss') },
                                            { title: 'Score', key: 'odds', align: 'center', width: '80px', render: (_, rowData) => `${rowData.predict_score1} - ${rowData.predict_score2}` },
                                            { title: 'ADA', key: 'reward_coin_amount', align: 'center', width: '80px' },
                                        ]}
                                        data={predictions}
                                    />
                                ) : (
                                    <EmptyResult />
                                )
                            )}
                        </div>
                    </div>
                </main>
            </div>
        </div>
    );
};

export default HighlightDetails;