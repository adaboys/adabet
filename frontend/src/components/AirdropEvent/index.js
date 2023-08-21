import React from 'react';

import styles from './index.module.scss';
import LeaderBoard from './LeaderBoard';
import Match from './Match';

const InPlay = () => {
    return (
        <div className={styles.airdropEvent}>
            <div className="container">
                <h3 className={styles.title}>Airdrop Event</h3>
                <div className={styles.content}>
                    <div className={styles.left}>
                        <Match />
                    </div>
                    <div className={styles.right}>
                        <LeaderBoard />
                    </div>
                </div>
            </div>
        </div>
    );
};

export default InPlay;