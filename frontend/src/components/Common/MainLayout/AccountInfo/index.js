import { useEffect, useState, useRef, useMemo } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { FormattedNumber } from 'react-intl';
import { useRouter } from 'next/router';
import classNames from 'classnames';
import Link from 'next/link';

import UserAvatar from './UserAvatar';
import DropdownAccountMenu from './DropdownAccountMenu';
import DropdownBalanceMenu from './DropdownBalanceMenu';

import { accountActions } from '@redux/actions';

import WalletIcon from '@assets/icons/walletmoney.svg';
import AdaIcon from '@assets/icons/ada.svg';
import ArrowDownIcon from '@assets/icons/polygon-11x8.svg';


import styles from './index.module.scss';
import { CURRENCY_ADA, paths } from '@constants';

const AccountInfo = ({ user, logout }) => {
    const [isOpenAccountMenu, setIsOpenAccountMenu] = useState(false);
    const [isOpenBalanceMenu, setIsOpenBalanceMenu] = useState(false);
    const { pathname } = useRouter();
    const dispatch = useDispatch();
    const { balances, currency } = useSelector(state => state.account);

    const accountMenuRef = useRef(null);
    const balanceMenuRef = useRef(null);
    const balanceInfo = useMemo(() => {
        if (balances?.length > 0) {
            return balances.find(item => item.id === currency)
        }
        return null
    }, [balances, currency]);

    const onChangeCurrency = currency => {
        dispatch(accountActions.updateCurrency(currency));
        setIsOpenBalanceMenu(false);
    }

    useEffect(() => {
        dispatch(accountActions.getBalance());
    }, [])

    useEffect(() => {
        setIsOpenAccountMenu(false);
        setIsOpenBalanceMenu(false);
    }, [pathname])

    return (
        <div className={styles.accountInfo}>
            <Link href={paths.accountDeposit}>
                <button className={styles.deposit}>
                    Deposit
                    <WalletIcon />
                </button>
            </Link>
            <div className={styles.userInfo}>
                <span ref={accountMenuRef} onClick={() => setIsOpenAccountMenu(prev => !prev)}><UserAvatar user={user} /></span>
                {
                    isOpenAccountMenu
                        ?
                        <DropdownAccountMenu activeRef={accountMenuRef} user={user} onClose={() => setIsOpenAccountMenu(false)} onLogout={logout} />
                        :
                        null
                }
            </div>
            {
                balanceInfo
                    ?
                    <>
                        <div ref={balanceMenuRef} className={styles.balanceInfo} onClick={() => setIsOpenBalanceMenu(prev => !prev)}>
                            <div className={styles.amount}>
                                <div className={styles.unit}>
                                    {
                                        balanceInfo.id === CURRENCY_ADA
                                        ?
                                        <AdaIcon />
                                        :
                                        <img src={`/images/tokens/${balanceInfo.name}.png`} />
                                    }
                                    {balanceInfo.name}
                                </div>
                                <div className={styles.value}>
                                    <FormattedNumber value={balanceInfo?.amount || 0} maximumFractionDigits="2" />
                                </div>
                                <ArrowDownIcon className={classNames(styles.arrowIcon, {[styles.isOpen]: isOpenBalanceMenu})} />
                            </div>
                        </div>
                        {isOpenBalanceMenu && (
                            <DropdownBalanceMenu
                                activeRef={balanceMenuRef}
                                currency={currency} balances={balances}
                                onClose={() => setIsOpenBalanceMenu(false)}
                                onChangeCurrency={onChangeCurrency}
                            />
                        )}
                    </>
                    :
                    null
            }
        </div>
    )
}

export default AccountInfo;