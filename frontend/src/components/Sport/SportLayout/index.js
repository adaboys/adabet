import Link from 'next/link';
import { useRouter } from 'next/router';

import { paths } from '@constants';

import HomeIcon from '@assets/icons/home.svg';
import VideoIcon from '@assets/icons/video.svg';
import StarIcon from '@assets/icons/star.svg';
import NoteIcon from '@assets/icons/note.svg';
import SearchIcon from '@assets/icons/search.svg';

import styles from './index.module.scss';
import classNames from 'classnames';

const SportLayout = ({ children, sideLeftContent: SideLeftContent }) => {
    const { pathname } = useRouter();
    const menus = [
        { url: paths.sport, icon: HomeIcon },
        { icon: VideoIcon },
        { icon: StarIcon },
        { icon: NoteIcon },
        { icon: SearchIcon }
    ]
    return (
        <div className={styles.sportLayout}>
            <div className={styles.sideLeftMenu}>
                <ul className={styles.headerMenu}>
                    {
                        menus.map((menu, index) => (
                            <li key={index} className={classNames(styles.menuItem, { [styles.active]: pathname === menu.url })}>
                                {
                                    menu.url
                                        ?
                                        <Link href={menu.url}>
                                            <a>{<menu.icon />}</a>
                                        </Link>
                                        :
                                        <a>{<menu.icon />}</a>
                                }

                            </li>
                        ))
                    }
                </ul>
                <div className={styles.sideLeftContent}>
                    <SideLeftContent/>
                </div>
            </div>
            <div className={styles.mainContent}>
                {children}
            </div>
        </div>
    )
}

export default SportLayout;
