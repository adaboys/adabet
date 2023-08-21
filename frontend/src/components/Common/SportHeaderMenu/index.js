import { useContext, useEffect } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/router';
import classNames from 'classnames';
import { useDispatch, useSelector } from 'react-redux';

import { OverlayContext } from '@hocs';
import { paths, overlayTypes, SPORT_DEFAULT_ID, matcheTypes } from '@constants';
import { useAuth } from '@hooks';
import { sportActions } from '@redux/actions';

import HomeIcon from '@assets/icons/home.svg';
import VideoIcon from '@assets/icons/video.svg';
import StarIcon from '@assets/icons/star.svg';
import NoteIcon from '@assets/icons/note.svg';
import SearchIcon from '@assets/icons/search.svg';

import styles from './index.module.scss';
import { generateSportUrl } from '@utils';


const SportHeaderMenu = ({ className }) => {
    const { pathname, query, asPath, push } = useRouter();
    const overlay = useContext(OverlayContext);
    const { isAuthenticated } = useAuth();
    const dispatch = useDispatch();
    const { totalBadges } = useSelector(state => state.sport);
    const { fav_cnt: favoriteCount, bet_cnt: openBetCount } = totalBadges || {};

    const onShowLogin = (returnUrl) => {
        overlay.show(overlayTypes.LOGIN, { callback: () => push(returnUrl) });
    }

    const favoriteUrl = generateSportUrl({...query, matchType: matcheTypes.FAVORITE});
    const betUrl = `${paths.myBet}?id=${query.id || SPORT_DEFAULT_ID}`;
    
    const menus = [
        { url: generateSportUrl({...query, matchType: matcheTypes.TOP}) , icon: HomeIcon },
        { icon: VideoIcon, url: generateSportUrl({...query, matchType: matcheTypes.LIVE}) },
        {
            icon: StarIcon,
            count: favoriteCount,
            ...(isAuthenticated ? { url: favoriteUrl} : { click: () => onShowLogin(favoriteUrl) })
        },
        {
            icon: NoteIcon,
            count: openBetCount,
            ...(isAuthenticated ? { url: betUrl } : { click: () => onShowLogin(betUrl) })
        },
        { icon: SearchIcon }
    ];

    useEffect(() => {
        if(isAuthenticated) {
            dispatch(sportActions.getTotalBadges({ id: query?.id || SPORT_DEFAULT_ID }));
        }
    }, [isAuthenticated])

    return (
        <ul className={classNames(styles.sportHeaderMenu, {[className]: !!className})}>
            {
                menus.map((menu, index) => (
                    <li key={index} className={classNames(styles.menuItem, { [styles.active]: asPath?.split('?')?.[0] === menu.url?.split('?')?.[0] })}>
                        {
                            menu.url && menu.url !== pathname
                                ?
                                <Link href={menu.url}>
                                    <a>{<menu.icon />}
                                    {
                                        menu.count
                                        ?
                                        <span>{menu.count}</span>
                                        :
                                        null
                                    }
                                    </a>
                                </Link>
                                :
                                <a onClick={menu.click}>{<menu.icon />}
                                {
                                        menu.count
                                        ?
                                        <span>{menu.count}</span>
                                        :
                                        null
                                    }
                                </a>
                        }

                    </li>
                ))
            }
        </ul>
    )
}

export default SportHeaderMenu;