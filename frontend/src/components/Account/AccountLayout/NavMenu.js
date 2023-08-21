import { useIntl, defineMessages } from 'react-intl';
import { useRouter } from 'next/router';
import Link from 'next/link';
import classNames from 'classnames';

import { paths } from '@constants';
// import { useAuth } from '@hooks';

import UserIcon from '@assets/icons/user.svg';
import WalletIcon from '@assets/icons/wallet-menu.svg';
import ConnectIcon from '@assets/icons/connect.svg';
import TransactionIcon from '@assets/icons/transactions.svg';

import styles from './NavMenu.module.scss';
import { walletTabs } from '@constants/masterData';

const messages = defineMessages({
    userDetail: 'Information & Sercure',
    wallet: 'Wallet',
    withdraw: 'Withdraw',
    deposit: 'Deposit',
    transaction: 'Transactions',
    connectWallet: 'Connect wallet',
    changePassword: 'Change password',
    logout: 'Logout',
    transaction: 'Transactions',
    wallet: 'Wallet',
})

const NavMenu = () => {
    const intl = useIntl();
    const { pathname } = useRouter();

    const menuList = [
        {
            name: intl.formatMessage(messages.userDetail),
            url: paths.account,
            icon: UserIcon,
            activePaths: [ paths.changePassword ],
        },
        {
            name: intl.formatMessage(messages.wallet),
            url: paths.accountDeposit,
            icon: WalletIcon,
            activePaths: walletTabs.map(el => el.key),
        },
        // {
        //     name: intl.formatMessage(messages.transaction),
        //     url: paths.accountTransaction,
        //     icon: MoneyIcon
        // },
        {
            name: intl.formatMessage(messages.connectWallet),
            url: paths.accountConnectWallet,
            icon: ConnectIcon
        },
        {
            name: intl.formatMessage(messages.transaction),
            url: paths.transaction,
            icon: TransactionIcon
        },
        // {
        //     name: intl.formatMessage(messages.changePassword),
        //     url: paths.accountChangePassword,
        //     isActive: pathname === paths.accountChangePassword
        // },
        // {
        //     name: intl.formatMessage(messages.logout),
        //     onClick: onLogout
        // },
    ]

    return (
        <>
            <ul className={styles.navMenu}>
                {
                    menuList?.map((menuItem, index) => (
                        <li key={index} className={styles.menuItem}>
                            {
                                menuItem.url
                                    ?
                                    <Link href={menuItem.url}>
                                        <a className={classNames({ [styles.active]: pathname === menuItem.url || menuItem.activePaths?.includes(pathname) })}>
                                            <menuItem.icon />
                                            {menuItem.name}
                                        </a>
                                    </Link>
                                    :
                                    <a onClick={menuItem.onClick} className={classNames({ [styles.active]: pathname === menuItem.url })}>
                                        <menuItem.icon />
                                        {menuItem.name}
                                    </a>
                            }

                        </li>
                    ))
                }
            </ul>
        </>
    )
}

export default NavMenu;