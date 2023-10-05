import React, { useState } from 'react';
import { useRouter } from 'next/router';

import { paths } from '@constants';

import Tabs from '@components/Sport/SportLayout/Tabs';
import ScoreResult from './ScoreResult';

import FlashIcon from '@assets/icons/flash.svg';
import ChartIcon from '@assets/icons/chart.svg';

import styles from './index.module.scss';
import Highlight from './Highlight';

const MATCHES_HIGHLIGHT = 'highlight';
const SCORE_RESULT = 'score-result';

const tabs = [
    { name: 'Highligh Match', icon: FlashIcon, key: MATCHES_HIGHLIGHT },
    { name: 'Score Result', icon: ChartIcon, key: SCORE_RESULT },
]

const Match = () => {
    const { query, push } = useRouter();
    const activeTab = query.tab;

    const handleChangeTab = tab => {
        push({ pathname: paths.airdropEvent, query: { tab } });
    }

    return (
        <div className={styles.match}>
            <div className={styles.tabContainer}>
                <Tabs
                    tabs={tabs}
                    tabClassName={styles.tabItem}
                    tabActiveClassName={styles.tabActive}
                    onTabClick={handleChangeTab}
                    activeTab={activeTab}
                />
            </div>
            <div className={styles.content}>
                {activeTab === SCORE_RESULT && <ScoreResult />}
                {activeTab === MATCHES_HIGHLIGHT && <Highlight />}
            </div>
        </div>
    );
};

export default Match;