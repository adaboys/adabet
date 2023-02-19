import Link from 'next/link';
import classNames from 'classnames';
import { paths } from '@constants';

import styles from './MainMenu.module.scss';
import { useRouter } from 'next/router';
import { useAuth } from '@hooks';

const MainMenu = () => {

    const { pathname } = useRouter();
    const { isAuthenticated } = useAuth();
    const isMarketplacePath = [
        paths.marketplace,
        paths.nftDetail
    ].includes(pathname);

    const isInventoryPath = [
        paths.inventory,
        paths.inventorySubPath
    ].includes(pathname);

    const isToEarnPath = pathname === paths.earn;

    return (
        <ul className={styles.mainMenu}>
            <li className={classNames(styles.item, { [styles.active]: pathname === paths.home })}>
                <Link href={paths.home}>
                    <a>Home</a>
                </Link>
            </li>
            <li className={classNames(styles.item, { [styles.active]: pathname === paths.sport })}>
                <Link href={paths.sport}>
                    <a>Inplay</a>
                </Link>
            </li>
            <li className={classNames(styles.item, { [styles.active]: false })}>
                <a>Promotion</a>
            </li>
            <li className={classNames(styles.item, { [styles.active]: false })}>
                <a>Statistics</a>
            </li>
            <li className={classNames(styles.item, { [styles.active]: false })}>
                <a>Result</a>
            </li>
            <li className={classNames(styles.item, { [styles.active]: false })}>
                <a>Blog</a>
            </li>

        </ul>
    )
}

export default MainMenu;