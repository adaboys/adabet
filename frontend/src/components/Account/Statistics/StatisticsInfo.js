import { useEffect, useState } from 'react';
import { useDispatch } from 'react-redux';

import { BasicForm, EmptyResult, Loading, SelectField } from '@components/Common';

import { accountActions } from '@redux/actions';
import { formatNumber } from '@utils';

import CoinIcon from '@assets/icons/coin.svg';
import CrownIcon from '@assets/icons/crown.svg';
import StatisticsIcon from '@assets/icons/statistics.svg';
import TicketIcon from '@assets/icons/ticket.svg';

import styles from './StatisticsInfo.module.scss';

const StatisticsInfo = () => {
    const [timeSelected, settimeSelected] = useState('all');
    const [data, setData] = useState({});
    const dispatch = useDispatch();
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        setLoading(true);
        dispatch(accountActions.getStatistics({
            params: { time: timeSelected },
            onCompleted: res => {
                setData(res.data || []);
                setLoading(false);
            },
            onError: err => {
                console.log({ err });
                setLoading(false);
            },
        }));
    }, [timeSelected]);

    return (
        <div className={styles.statisticsInfo}>
            <div className={styles.header}>
                <div className={styles.title}>
                    <StatisticsIcon />
                    Statistics
                </div>
                <div className={styles.filter}>
                    <BasicForm
                        initialValues={{ time: 'all' }}
                    >
                        <SelectField
                            options={[
                                { label: 'All', value: 'all' },
                                { label: 'By today', value: 'today' },
                                { label: 'By week', value: 'week' },
                                { label: 'By month', value: 'month' },
                            ]}
                            name="time"
                            isSearchable={false}
                            onChange={({ value }) => settimeSelected(value)}
                        />
                    </BasicForm>
                </div>
            </div>
            <div className={styles.summary}>
                <div className={styles.item}>
                    <div className={styles.title}><CrownIcon /> Total wins</div>
                    <div className={styles.value}>{formatNumber(data.totalWonCount) || '0'}</div>
                </div>
                <div className={styles.item}>
                    <div className={styles.title}><TicketIcon /> Total bets</div>
                    <div className={styles.value}>{formatNumber(data.totalBetCount) || '0'}</div>
                </div>
                <div className={styles.item}>
                    <div className={styles.title}><CoinIcon /> Total wagered</div>
                    <div className={styles.value} title={formatNumber(data.totalWagered) || '0'}>$ {formatNumber(data.totalWagered) || '0'}</div>
                </div>
            </div>
            <div className={styles.details}>
                <table>
                    <thead>
                        <tr>
                            <th>Currency</th>
                            <th>Win</th>
                            <th>Bet</th>
                            <th>Wagered</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr></tr>
                        {loading ? (
                            <tr style={{ height: '136px' }}>
                                <td colSpan={4}>
                                    <Loading style={{ margin: '42px 0' }} />
                                </td>
                            </tr>
                        ) : (
                            data?.coinStats?.length ? data?.coinStats?.map((stat, index) => (
                                <tr key={index}>
                                    <td>
                                        <div className={styles.currency}>
                                            <img src={`/images/tokens/${stat.currency}.png`} />
                                            {stat.currency}
                                        </div>
                                    </td>
                                    <td>{formatNumber(stat.wonCount) || 0}</td>
                                    <td>{formatNumber(stat.betCount) || 0}</td>
                                    <td>{stat.wager}</td>
                                </tr>
                            )) : (
                                <tr style={{ height: '136px' }}>
                                    <td colSpan={4}>
                                        <EmptyResult boxClassName={styles.empty} />
                                    </td>
                                </tr>
                            )
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default StatisticsInfo;