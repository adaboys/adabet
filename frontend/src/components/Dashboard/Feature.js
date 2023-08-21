import React from 'react';

import styles from './Feature.module.scss';

const Feature = () => {
    return (
        <div className={styles.feature}>
            <div className={styles.item}>
                <h3 className={styles.title}>Safe & Secure</h3>
                <div className={styles.desc}>Secure betting transactions based on the Cardano blockchain blockchain technology.</div>
            </div>
            <div className={styles.item}>
                <h3 className={styles.title}>Training Mode</h3>
                <div className={styles.desc}>Practice playing experience on the brand new betting platform of ADABET.iO</div>
            </div>
            <div className={styles.item}>
                <h3 className={styles.title}>Just A Click</h3>
                <div className={styles.desc}>Convenient deposit and withdrawal with real time 24/7. Secure identity and secure transactions on blockchain technology.</div>
            </div>
        </div>
    );
};

export default Feature;