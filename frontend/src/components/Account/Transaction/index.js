import classNames from 'classnames';
import moment from 'moment';
import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { EmptyResult, Loading } from '@components/Common';
import PaginationNextPrev from '@components/Common/PaginationNextPrev';
import { CURRENCY_ADA, DATE_FORMAT_DISPLAY_US } from '@constants';
import { useAuth } from "@hooks";
import useDidMountEffect from '@hooks/useDidMountEffect';
import { accountActions } from '@redux/actions';
import { formatNumber } from '@utils';

import AccountLayout from '../AccountLayout';

import ArrowDownIcon from '@assets/icons/arrow-down.svg';
import TransactionIcon from '@assets/icons/transactions.svg';

import styles from './index.module.scss';

const TRANSACTION_TYPE = {
    WITHDRAW: 1,
    SWAP: 2,
    HISTORY: 3,
}

const actions = [
    { label: 'Withdraw', value: TRANSACTION_TYPE.WITHDRAW },
    { label: 'Buy or swap', value: TRANSACTION_TYPE.SWAP },
    { label: 'History', value: TRANSACTION_TYPE.HISTORY },
]

const statusMap = {
    2: 'Succeed',
    3: 'Failed'
}

const statusClassMap = {
    2: styles.succeed,
    3: styles.failed,
}

const index = () => {
    const { currency } = useAuth();
    const { balances } = useSelector(state => state.account);
    const [filter, setFilter] = useState({
        page: 1,
        action: 1,
        currency,
    });
    const [loading, setLoading] = useState();
    const [transactionLogData, setTransactionLogData] = useState({});
    const { page_count = 0, histories = [], has_next_page } = transactionLogData;
    const dispatch = useDispatch();

    const extraPaginationProp = filter.action === TRANSACTION_TYPE.HISTORY
        ? { disableNext: !has_next_page }
        : {}

    const getCurrencyInfo = id => balances?.find(cur => cur.id === id)

    const getTransactionLog = () => {
        setLoading(true);
        const params = {
            page: filter.page,
            item: 5,
            coin_id: filter.currency || CURRENCY_ADA,
        }
        if (filter.action && filter.action !== TRANSACTION_TYPE.HISTORY) {
            params.action = filter.action;
        }

        const apiAction = filter.action === TRANSACTION_TYPE.HISTORY
                ? accountActions.getCoinTransactionAll
                : accountActions.getCoinTransaction;

        dispatch(apiAction({
            params,
            onCompleted: ({ data }) => {
                setTransactionLogData(data || {});
                setLoading(false);
            },
            onError: error => {
                console.log({ error });
                setLoading(false);
            }
        }));
    }

    const onViewOnChain = (hash) => {
        window.open(`${process.env.NEXT_PUBLIC_CADARNOSCAN_URL}transaction/${hash}`, '_blank');
    }

    const renderHeaderByType = () => {
        if (filter.action === TRANSACTION_TYPE.HISTORY) {
            return (
                <tr>
                    <th>Time</th>
                    <th>Amount</th>
                    <th>Transaction</th>
                </tr>
            );
        }

        return (
            <tr>
                <th>Time</th>
                <th>Amount</th>
                <th>Fee</th>
                <th>State</th>
                <th>Transaction</th>
            </tr>
        );
    }

    const renderBodyByType = (his, index) => {
        let currencyInfo;
        if (filter.action === TRANSACTION_TYPE.HISTORY) {
            currencyInfo = getCurrencyInfo(filter.currency);
            return (
                <tr key={index}>
                    <td>{moment(his.created_at).format(DATE_FORMAT_DISPLAY_US)}</td>
                    <td>
                        <div className={styles.amount}>
                            {formatNumber(his.receive_amount) || '0'}
                            {currencyInfo && <img src={`/images/tokens/${currencyInfo.name}.png`} />}
                        </div>
                    </td>
                    <td>
                        <button onClick={() => onViewOnChain(his.tx_hash)} className={styles.btnViewDetail}>
                            View detail
                            <ArrowDownIcon />
                        </button>
                    </td>
                </tr>
            );
        }

        currencyInfo = getCurrencyInfo(his.send_coin);
        return (
            <tr key={index}>
                <td>{moment(his.created_at).format(DATE_FORMAT_DISPLAY_US)}</td>
                <td>
                    <div className={styles.amount}>
                        {formatNumber(his.send_amount) || '0'}
                        {currencyInfo && <img src={`/images/tokens/${currencyInfo.name}.png`} />}
                    </div>
                </td>
                <td>
                    <div className={styles.amount}>
                        {formatNumber(his.fee_in_ada) || '0'}
                        {currencyInfo && <img src={`/images/tokens/ADA.png`} />}
                    </div>
                </td>
                <td className={statusClassMap[his.status]}>{statusMap[his.status] || 'Processing'}</td>
                <td>
                    <button onClick={() => onViewOnChain(his.tx_hash)} className={styles.btnViewDetail}>
                        View detail
                        <ArrowDownIcon />
                    </button>
                </td>
            </tr>
        );
    }

    useDidMountEffect(() => {
        setFilter(prev => ({ ...prev, page: 1, currency }));
    }, [currency]);

    useEffect(() => {
        getTransactionLog();
    }, [filter]);

    return (
        <AccountLayout>
            <div className={styles.transactionLog}>
                <div className={styles.title}>
                    <TransactionIcon />
                    Transactions
                </div>
                <div className={styles.filterAction}>
                    {actions.map(el => (
                        <div
                            className={classNames(styles.actionItem, filter?.action === el.value && styles.active)}
                            onClick={() => setFilter(prev => ({ ...prev, action: el.value, page: 1 }))}
                        >
                            {el.label}
                        </div>
                    ))}
                </div>
                <div className={styles.list}>
                    {loading ? (
                        <div className={styles.loadingContainer}>
                            <Loading />
                        </div>
                    ) : (
                        <table>
                            <thead>{renderHeaderByType()}</thead>
                            <tbody>
                                {histories?.length ? (
                                    histories.map(renderBodyByType)
                                ) : (
                                    <tr>
                                        <td colSpan={5}><EmptyResult /></td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    )}
                </div>
                {!!page_count || (filter.action === TRANSACTION_TYPE.HISTORY && !!histories?.length && !loading) && (
                    <div className={styles.pagination}>
                        <PaginationNextPrev
                            currentPage={filter.page}
                            totalPage={page_count}
                            onNext={() => setFilter(prev => ({ ...prev, page: prev.page + 1 }))}
                            onPrev={() => setFilter(prev => ({ ...prev, page: Math.max(prev.page - 1, 0) }))}
                            {...extraPaginationProp}
                        />
                    </div>
                )}
            </div>
        </AccountLayout>
    );
};

export default index;