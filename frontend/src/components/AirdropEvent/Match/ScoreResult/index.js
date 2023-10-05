import React from 'react';
import Link from 'next/link';

import { paths } from '@constants';

import PaginationNextPrev from '@components/Common/PaginationNextPrev';
import { BasicForm, InputTextField, SelectField } from '@components/Common';
import Table from '@components/AirdropEvent/Table';

import ArrowDownIcon from '@assets/icons/arrow-down.svg';
import SearchIcon from '@assets/icons/search-normal.svg';

import styles from './index.module.scss';

const ScoreResult = () => {
    return (
        <div className={styles.scoreResult}>
            <div className={styles.filter}>
                <BasicForm
                    initialValues={{ orderBy: '' }}
                >
                    <InputTextField
                        iconLeft={<SearchIcon />}
                        name="search"
                        className={styles.inputSearch}
                        placeholder="Find match by enter name team..."
                    />
                    <SelectField
                        options={[{ label: 'Most recent', value: 'recent' }]}
                        name="orderBy"
                        placeholder="Most recent"
                        isSearchable={false}
                        className={styles.select}
                    />
                </BasicForm>
            </div>
            <div className={styles.list}>
                <Table
                    columns={[
                        { title: 'Date', key: 'date' },
                        { title: 'Match', key: 'match', render: match => <b>{match}</b> },
                        { title: 'Score', key: 'score', align: 'center', width: '80px' },
                        { title: 'Player', key: 'player', align: 'center', width: '80px' },
                        {
                            title: 'View',
                            align: 'center',
                            width: '60px',
                            render: () => (
                                <Link href={{ pathname: paths.scoreResultDetails, query: { id: 1 } }}>
                                    <ArrowDownIcon className={styles.viewDetails} />
                                </Link>
                            )
                        },
                    ]}
                    data={[
                        {
                            date: '02/06/2023',
                            match: 'Barcelona - Real Madrid',
                            score: '1 : 1',
                            player: '2653',
                        },
                        {
                            date: '02/06/2023',
                            match: 'Barcelona - Real Madrid',
                            score: '1 : 1',
                            player: '2653',
                        },
                        {
                            date: '02/06/2023',
                            match: 'Barcelona - Real Madrid',
                            score: '1 : 1',
                            player: '2653',
                        },
                        {
                            date: '02/06/2023',
                            match: 'Barcelona - Real Madrid',
                            score: '1 : 1',
                            player: '2653',
                        },
                        {
                            date: '02/06/2023',
                            match: 'Barcelona - Real Madrid',
                            score: '1 : 1',
                            player: '2653',
                        },
                        {
                            date: '02/06/2023',
                            match: 'Barcelona - Real Madrid',
                            score: '1 : 1',
                            player: '2653',
                        },
                    ]}
                />
            </div>
            <div className={styles.paginationContainer}>
                <div className={styles.total}>Total 12 page</div>
                <div className={styles.searchResult}>8 results for the keyword "Barcelona" were found</div>
                <div className={styles.pagination}>
                    <PaginationNextPrev currentPage={2} />
                </div>
            </div>
        </div>
    );
};

export default ScoreResult;