import { useState, useContext } from 'react';
import Link from 'next/link';
import dynamic from 'next/dynamic';
import classNames from 'classnames';
import { useRouter } from 'next/router';

import MobileSideBarMenu from './MobileSideBarMenu';
import MainMenu from './MainMenu';
import { Desktop, Mobile, Button } from '@components/Common';

import { useAuth } from '@hooks';
import { paths, overlayTypes } from '@constants';
import { OverlayContext } from '@hocs';

import HamburgerIcon from '@assets/icons/hamburger.svg';
import AvatarIcon from '@assets/icons/avatar.svg';

import styles from './Header.module.scss';

const AccountInfo = dynamic(() => import('./AccountInfo'), { ssr: false });

const Header = () => {
    const [isShowSidebar, setIsShowSidebar] = useState(false);
    const overlay = useContext(OverlayContext);
    const { pathname } = useRouter();

    const { isAuthenticated, user, logout } = useAuth();

    const onShowLogin = () => {
        overlay.show(overlayTypes.LOGIN);
    }

    const onShowRegister = () => {
        overlay.show(overlayTypes.REGISTER);
    }

    return (
        <header className={classNames(styles.header, {
            [styles.home]: pathname === paths.home,
        })}>
            <div className="container">
                <div className={styles.logo}>
                    <Link href={paths.home}>
                        <img src="/images/logo.svg" alt="AdaBet" />
                    </Link>
                </div>
                <Desktop>
                    <MainMenu />
                    <div className={styles.account}>
                        {
                            isAuthenticated
                                ?
                                <AccountInfo user={user} logout={logout}/>
                                :
                                <>
                                    <a onClick={onShowLogin} className={styles.btnLogin}>Login</a>
                                    <Button className={styles.btnSignup} onClick={onShowRegister}>Sign up</Button>
                                </>
                        }

                    </div>
                    {/* <GroupLanguage /> */}
                </Desktop>
                <div className={styles.sidebarMenu}>
                    {
                        isAuthenticated
                            ?
                            <Mobile>
                                <Link href={paths.account}><AvatarIcon className="cursor-pointer" /></Link>
                            </Mobile>
                            :
                            null
                    }

                    <a className={styles.hamburger} onClick={() => setIsShowSidebar(true)}><HamburgerIcon /></a>
                    <MobileSideBarMenu
                        onShowRegister={onShowRegister}
                        onShowLogin={onShowLogin}
                        isShow={isShowSidebar}
                        onClose={() => setIsShowSidebar(false)}
                    />
                </div>
            </div>
        </header>
    )
}

export default Header;