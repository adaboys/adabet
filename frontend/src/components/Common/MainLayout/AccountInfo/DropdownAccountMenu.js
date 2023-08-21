import { useContext } from 'react';
import Link from 'next/link';

import UserAvatar from './UserAvatar';
import { DropdownPanel } from '@components/Common';

import { overlayTypes, paths } from '@constants';
import { OverlayContext } from '@hocs';

import UserIcon from '@assets/icons/user.svg';
import WalletIcon from '@assets/icons/wallet-menu.svg';
import StatisticsIcon from '@assets/icons/statistics.svg';
import LanguageIcon from '@assets/icons/language.svg';
import TransactionIcon from '@assets/icons/transactions.svg';
import SupportIcon from '@assets/icons/support.svg';
import LogoutIcon from '@assets/icons/logout.svg';
import MoneyIcon from '@assets/icons/money-change.svg';
import ConnectIcon from '@assets/icons/connect.svg';

import styles from './DropdownAccountMenu.module.scss'

const DropdownAccountMenu = ({ activeRef, user, onClose, onLogout }) => {
    const overlay = useContext(OverlayContext);

    const onShowStatistics = e => {
        e.preventDefault();
        overlay.show(overlayTypes.STATISTICS);
        onClose()
    }

    return (
        <DropdownPanel activeRef={activeRef} onClickOutside={onClose} className={styles.dropdownAccountMenu}>
            <UserAvatar user={user}/>
            <div className={styles.menuList}>
                <Link href={paths.account}>
                    <a className={styles.menuItem}><UserIcon /> User Information</a>
                </Link>
                <Link href={paths.accountDeposit}>
                    <a className={styles.menuItem}><WalletIcon /> Deposit</a>
                </Link>
                <Link href={paths.accountWithdraw}>
                    <a className={styles.menuItem}><MoneyIcon /> Withdraw</a>
                </Link>
                <Link href={paths.accountConnectWallet}>
                    <a className={styles.menuItem}><ConnectIcon /> Connect Wallet</a>
                </Link>
                <Link href={paths.transaction}>
                    <a className={styles.menuItem}><TransactionIcon /> Transactions</a>
                </Link>
                {/* <a className={styles.menuItem} onClick={onShowStatistics}><StatisticsIcon />Statistics</a>
                <a className={styles.menuItem}><LanguageIcon />Language</a>
                <a className={styles.menuItem}><SupportIcon />Support</a> */}
            </div>
            <div className={styles.actions}>
                <div className={styles.logout} onClick={() => onLogout()}>
                    <LogoutIcon /> Logout
                </div>
            </div>
        </DropdownPanel>
    )
}

export default DropdownAccountMenu;