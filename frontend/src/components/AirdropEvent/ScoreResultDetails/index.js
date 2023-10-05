import React from 'react';
import { useRouter } from 'next/router';
import classNames from 'classnames';

import ArrowTailRight from '@assets/icons/arrow-tail-right.svg';

import MatchBet from './MatchBet';
import Table from '../Table';
import { SelectField, BasicForm } from '@components/Common';
import PaginationNextPrev from '@components/Common/PaginationNextPrev';

import styles from './index.module.scss';

const statusMap = {
    1: 'WON',
    2: 'LOST'
}

const statusClassMap = {
    1: styles.win,
    2: styles.lost,
}

const ScoreResultDetails = () => {
    const { back } = useRouter();
    return (
        <div className={styles.scoreResultDetails}>
            <div className="container">
                <button className={styles.btnBack} onClick={back}>
                    <ArrowTailRight /> Back
                </button>

                <main>
                    <MatchBet className={styles.matchBet} />
                    <div className={styles.separate} />
                    <div className={styles.betHistory}>
                        <div className={styles.filter}>
                            <BasicForm
                                initialValues={{ orderBy: '' }}
                            >
                                <SelectField
                                    options={[ { label: 'Most recent', value: 'recent' } ]}
                                    name="orderBy"
                                    placeholder="Most recent"
                                    isSearchable={false}
                                />
                            </BasicForm>
                            <PaginationNextPrev
                                currentPage={1}
                                totalPage={2}
                                className={styles.pagination}
                            />
                        </div>
                        <div className={styles.list}>
                            <Table
                                columns={[
                                    { title: 'Username', key: 'username' },
                                    { title: 'Bet Date', key: 'date' },
                                    { title: 'Odds', key: 'odds', align: 'center', width: '80px' },
                                    { title: 'ADA', key: 'amount', align: 'center', width: '80px' },
                                    {
                                        title: 'Status',
                                        key: 'status',
                                        align: 'center',
                                        width: '80px',
                                        render: status => <span className={classNames(styles.status, statusClassMap[status])}>{statusMap[status]}</span>
                                    },
                                    { title: 'Reward', key: 'rewardStatus', align: 'center', width: '120px' },
                                ]}
                                data={[
                                    {
                                        username: 'andiez8596',
                                        date: '02/06/2023 - 03:22:47',
                                        odds: '1 : 1',
                                        amount: '20',
                                        status: 1,
                                        rewardStatus: 'Received',
                                    },
                                    {
                                        username: 'andiez8596',
                                        date: '02/06/2023 - 03:22:47',
                                        odds: '1 : 1',
                                        amount: '20',
                                        status: 2,
                                        rewardStatus: '-',
                                    },
                                    {
                                        username: 'andiez8596',
                                        date: '02/06/2023 - 03:22:47',
                                        odds: '1 : 1',
                                        amount: '20',
                                        status: 2,
                                        rewardStatus: 'Not received',
                                    },
                                    {
                                        username: 'andiez8596',
                                        date: '02/06/2023 - 03:22:47',
                                        odds: '1 : 1',
                                        amount: '20',
                                        status: 1,
                                        rewardStatus: 'Not received',
                                    },
                                ]}
                            />
                        </div>
                    </div>
                </main>
            </div>
        </div>
    );
};

export default ScoreResultDetails;