import React, { Fragment } from 'react';
import moment from 'moment/moment';

import FootballIcon from '@assets/icons/football.svg';
import DividerIcon from '@assets/icons/breadcrumb-divider.svg';
import TwoUserIcon from '@assets/icons/two-user.svg';
import PeopleIcon from '@assets/icons/people.svg';

import styles from './index.module.scss';
import classNames from 'classnames';
import NumericStepper from '@components/Common/NumericStepper';
import { getStartTime, resolveTeamImgPathById } from '@utils';

const MatchShortInfo = ({
    className,
    betable,
    small,
    match = {},
    score,
    setScore,
}) => {
    // const cats = small ? ['Soccer', 'International'] : ['Soccer', 'International', 'Worldcup']
    const time = match.start_at ? moment.utc(match.start_at).local() : null;
    const formatTime = match.start_at ? getStartTime(match.start_at) : null;
    const isEnded = !time || time <= moment();

    return (
        <div className={classNames(styles.matchShortInfo, small && styles.small, className)}>
            <div className={styles.head}>
                <div className={styles.matchTypes}>
                    <FootballIcon />
                    {/* {cats.map((el, index) => (
                        <Fragment key={index}>
                            <span>{el}</span>
                            {index !== cats.length - 1 && <DividerIcon />}
                        </Fragment>
                    ))} */}
                    <span>{match.league}</span>
                </div>
                <div className={styles.user}>
                    <TwoUserIcon />
                    {match?.participant || (match.winner || 0) + (match.loser || 0)}
                </div>
            </div>
            <div className={styles.content}>
                <div className={styles.team}>
                    <div className={styles.flag}>
                        {match.img1 ? (
                            <img src={resolveTeamImgPathById(match.img1)} alt="" />
                        ) : (
                            <FootballIcon width={32} height="auto" />
                        )}
                    </div>
                    <h3 className={styles.name}>{match.t1}</h3>
                    {betable && !isEnded && (
                        <NumericStepper
                            value={score?.score1}
                            className={styles.numericStepper}
                            maxValue={10}
                            minValue={0}
                            disabled={isEnded || !betable}
                            onIncrease={() => setScore?.(({ score1, score2 }) => ({ score1: score1 + 1, score2 }))}
                            onDecrease={() => setScore?.(({ score1, score2 }) => ({ score1: Math.max(score1 - 1, 0), score2 }))}
                        />
                    )}
                </div>
                <div className={styles.time}>
                    <div className={styles.day}>{isEnded ? 'Ended' : formatTime && formatTime.date}</div>
                    <div className={styles.hour}>{!isEnded && formatTime && formatTime.time}</div>
                    <div className={styles.score}>
                        {isEnded && !!match.s1 && !!match.s2 && `${match.s1} : ${match.s2}`}
                        <PeopleIcon />
                    </div>
                </div>
                <div className={styles.team}>
                    <div className={styles.flag}>
                        {match.img2 ? (
                            <img src={resolveTeamImgPathById(match.img2)} alt="" />
                        ) : (
                            <FootballIcon width={32} height="auto" />
                        )}
                    </div>
                    <h3 className={styles.name}>{match.t2}</h3>
                    {betable && !isEnded && (
                        <NumericStepper
                            value={score?.score2}
                            className={styles.numericStepper}
                            maxValue={10}
                            minValue={0}
                            disabled={isEnded || !betable}
                            onIncrease={() => setScore?.(({ score1, score2 }) => ({ score1, score2:  score2 + 1 }))}
                            onDecrease={() => setScore?.(({ score1, score2 }) => ({ score1, score2: Math.max(score2 - 1, 0) }))}
                        />
                    )}
                </div>
            </div>
        </div>
    );
};

export default MatchShortInfo;