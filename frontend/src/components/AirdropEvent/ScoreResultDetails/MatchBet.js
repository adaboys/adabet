import React from 'react';

import MatchShortInfo from '../MathShortInfo';

import styles from './MatchBet.module.scss';
import classNames from 'classnames';

const MatchBet = () => {
    return (
        <div className={styles.matchBet}>
            <MatchShortInfo className={styles.infoBox} />
            <div className={styles.statistics}>
                <h4>Statistical:</h4>
                <div className={styles.status}>
                    <div className={styles.label}>WON</div> 125 players
                </div>
                <div className={styles.status}>
                    <div className={classNames(styles.label, styles.lost)}>LOST</div> 982 players
                </div>
            </div>
        </div>
    );
};

export default MatchBet;