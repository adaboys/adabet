import Doughnut from '@components/Common/Chart/Doughnut';
import React from 'react';

import styles from './Tokenomics.module.scss';

const data = [
    { name: 'Foundation', percent: 5, color: '#F4A6BA' },
    { name: 'Seed', percent: 4, color: '#F4A659' },
    { name: 'Private', percent: 7, color: '#A2B9FA' },
    { name: 'Public', percent: 11, color: '#04D1DB' },
    { name: 'Team', percent: 20, color: '#9FE498' },
    { name: 'Advisor', percent: 2, color: '#E5E5B1' },
    { name: 'Airdrop', percent: 1, color: '#017EC5' },
    { name: 'ISPO', percent: 1, color: '#E55A24' },
    { name: 'Marketing', percent: 5, color: '#815EEA' },
    { name: 'Ecosystem', percent: 44, color: '#3FD317' },
]

const Tokenomics = () => {
    const sliceArray = (arr, column) => {
        const itemEachCol = Math.ceil(arr.length / column);
        const result = [];

        for (let i = 0; i < column; i++) {
            const sliceFrom = i * itemEachCol;
            result.push(arr.slice(sliceFrom, sliceFrom + itemEachCol));
        }

        return result;
    }

    const dataChart = {
        labels: data.map(el => el.name),
        datasets: [
            {
                data: data.map(el => el.percent),
                backgroundColor: data.map(el => el.color),
            }
        ],
    }

    return (
        <div className={styles.tokenomics}>
            <h3 className={styles.title}>Tokenomics</h3>
            <div className={styles.content}>
                <div className={styles.pie}>
                    <Doughnut
                        data={dataChart}
                    />
                </div>
                <div className={styles.details}>
                    {sliceArray(data, 2).map((col, colIndex) => (
                        <div key={colIndex} className={styles.col}>
                            {col.map((item, index) => (
                                <div className={styles.item} key={index}>
                                    <div className={styles.color} style={{ backgroundColor: item.color }} />
                                    <div className={styles.percent}>{`${item.percent} %`}</div>
                                    <div className={styles.name}>{item.name}</div>
                                </div>
                            ))}
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default Tokenomics;