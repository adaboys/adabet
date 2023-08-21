import React, { useEffect, useState } from 'react';

import CrownIcon from '@assets/icons/crown.svg';
// import CaretIcon from '@assets/icons/caret-up.svg';
import StarIcon from '@assets/icons/round-star.svg';

import styles from './LeaderBoard.module.scss';
import classNames from 'classnames';
import { Loading, EmptyResult } from '@components/Common';
import { useDispatch } from 'react-redux';
import { airdropEventActions } from '@redux/actions';

// const GrowthState = ({ down, amount, className }) => (
//     <div className={classNames(styles.growthItem, className, down && styles.down)}>
//         {amount}
//         <CaretIcon />
//     </div>
// );

const filterElapsedTimeOptions = [
    { label: 'All time', value: 'total' },
    { label: 'This Week', value: 'week' },
    { label: 'This Month', value: 'month' },
]

const LeaderBoard = () => {
    const [loading, setLoading] = useState(false);
    const [filterElapsedTime, setFilterElapsedTime] = useState(filterElapsedTimeOptions[0].value);
    const [leaderboard, setLeaderboard] = useState([]);
    const dispatch = useDispatch();

    const getLeaderboard = () => {
        setLoading(true);
        dispatch(airdropEventActions.getLeaderboard({
            params: {
                coin_id: 1, // ada hard
                up_to_now: filterElapsedTime,
                page: 0,
                item: 10,
            },
            onCompleted: ({ data }) => {
                setLeaderboard(data?.leaderboard || []);
                setLoading(false);
            },
            onError: error => {
                console.log({ error });
                setLoading(false);
            }
        }));
    }

    useEffect(() => {
        getLeaderboard();
    }, [filterElapsedTime]);

    const top3 = [...leaderboard, false, false, false].slice(0, 3);

    return (
        <div className={styles.leaderboard}>
            <h3 className={styles.title}>
                <CrownIcon />Leaderboard
            </h3>
            <div className={styles.filter}>
                {filterElapsedTimeOptions.map((el) => (
                    <button
                        className={filterElapsedTime === el.value ? styles.active : ''}
                        onClick={() => setFilterElapsedTime(el.value)}
                        disabled={loading}
                        key={el.value}
                    >
                        {el.label}
                    </button>
                ))}
            </div>
            {loading ? (
                <Loading style={{ margin: '40px 0' }} />
            ) : (
                leaderboard.length ? (
                    <>
                        <div className={styles.top}>
                            {top3.map((player, index) => (
                                <div
                                    className={classNames(
                                        styles.item,
                                        styles[`top${index + 1}`],
                                        index && player && styles.hasData,
                                    )}
                                    key={index}
                                >
                                    {player && (
                                        <>
                                            <div className={styles.name}>{player.player}</div>
                                            <div className={styles.info}>
                                                <div className={styles.increase}>+ {player.reward_sum} ADA</div>
                                                {/* <GrowthState amount={4} down className={styles.growth} /> */}
                                            </div>
                                            <div className={styles.avatar}>
                                                {player.ava && <img src={player.ava} alt="" />}
                                                <div className={styles.rank}>
                                                    <StarIcon />
                                                    <span>{index + 1}</span>
                                                </div>
                                            </div>
                                        </>
                                    )}
                                </div>
                            ))}
                        </div>
                        <div className={styles.topRest}>
                            {leaderboard.slice(3).map((el, index) => (
                                <div className={styles.mem} key={index}>
                                    <div className={styles.no}>{index + 4}.</div>
                                    <div className={styles.name}>{el.player}</div>
                                    <div className={styles.increase}>+ {el.reward_sum} ADA</div>
                                    {/* <GrowthState down={el.down} amount={el.growth} className={styles.growth} /> */}
                                </div>
                            ))}
                        </div>
                    </>
                ) : (
                    <EmptyResult />
                )
            )}
        </div>
    );
};

export default LeaderBoard;