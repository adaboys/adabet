import { useEffect, useContext } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useRouter } from 'next/router';
import classNames from 'classnames';

import Header from './Header';
import Footer from './Footer';
import { FullScreenLoading } from '..';

import { accountActions, loadingActions } from '@redux/actions';
import { useAuth } from '@hooks';
import { OverlayContext } from '@hocs';
import { overlayTypes, paths, SOURCE_WEB_GAME } from '@constants';

import styles from './index.module.scss';

const MainLayout = ({ children }) => {
    const dispatch = useDispatch();
    const { query, replace, pathname } = useRouter();
    const overlay = useContext(OverlayContext);

    const { isAuthenticated } = useAuth();
    const isGetTokenForGamePlaySuccess = useSelector(state => state.account.isGetTokenForGamePlaySuccess);
    const needGetTokenForGamePlay = query.source === SOURCE_WEB_GAME && query.redirectUrl;

    useEffect(() => {
        if (isAuthenticated) {
            dispatch(accountActions.getProfile());
            if(needGetTokenForGamePlay) {
                dispatch(loadingActions.showLoadingFullScreen());
            }
        }
        else if (needGetTokenForGamePlay) {
            overlay.show(overlayTypes.LOGIN);
        }
    }, [isAuthenticated])

    useEffect(() => {
        if(needGetTokenForGamePlay) {
            if(isGetTokenForGamePlaySuccess === false) {
                dispatch(loadingActions.hideLoadingFullScreen());
                overlay.show(overlayTypes.LOGIN);
            }
            else if(isGetTokenForGamePlaySuccess) {
                replace(query.redirectUrl);
            }
        }
    }, [isGetTokenForGamePlaySuccess])

    return (
        <main className={classNames(styles.mainLayout,{
                [styles.home]: pathname === paths.home,
                [styles.account]: pathname.includes(paths.account)
            })}
        >
            <Header />
            <div className={styles.content}>
                {children}
            </div>
            <Footer />
            <FullScreenLoading />
        </main>
    )
}

export default MainLayout;
