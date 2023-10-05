import Link from 'next/link';
import classNames from 'classnames';
import { casinoURL, overlayTypes, paths } from '@constants';
import { useContext } from 'react';
import { useRouter } from 'next/router';

import { useAuth } from '@hooks';
import { OverlayContext } from '@hocs';

import styles from './MainMenu.module.scss';

const MainMenu = () => {

    const { pathname } = useRouter();
    const { isAuthenticated } = useAuth();
    const overlay = useContext(OverlayContext);

    const onShowStatistics = e => {
        e.preventDefault();
        overlay.show(overlayTypes.STATISTICS);
    }

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
                <Link href={paths.topMatches}>
                    <a>Inplay</a>
                </Link>
            </li>
            {/* <li className={classNames(styles.item, { [styles.active]: false })}>
                <a>Promotion</a>
            </li> */}
            {isAuthenticated && (
                <li className={classNames(styles.item, { [styles.active]: false })}>
                    <a onClick={onShowStatistics}>Statistics</a>
                </li>
            )}
            {/* <li className={classNames(styles.item, { [styles.active]: false })}>
                <a>Result</a>
            </li> */}
            <li className={classNames(styles.item, { [styles.active]: pathname === paths.aboutUs })}>
                <Link href={paths.aboutUs}>
                    <a>About</a>
                </Link>
            </li>
            <li className={classNames(styles.item, { [styles.active]: pathname.includes('airdrop-event') })}>
                <Link href={paths.highlightMatches}>
                    <a>Airdrop Event</a>
                </Link>
            </li>
            <li className={classNames(styles.item, { [styles.active]: false })}>
                <a href={casinoURL}>Casino</a>
            </li>

        </ul>
    )
}

export default MainMenu;