import { useAuth } from '@hooks';
import { paths } from '@constants';
import NavMenu from './NavMenu';
import { Redirect, Loading, SportHeaderMenu } from '@components/Common';

import styles from './index.module.scss';

const AccountLayout = ({ children, loading }) => {
    const { isAuthenticated, user, loaded } = useAuth();

    if (!isAuthenticated)
        return <Redirect url={paths.home} />

    return (
        <div className={ styles.accountLayout}>
            <div className={styles.leftSide}>
                <SportHeaderMenu className={styles.sportMenu}/>
                <NavMenu />
            </div>
            <div className={styles.rightContent}>
                {
                    !loaded || !user || loading
                        ?
                        <Loading style={{ margin: '50px auto' }} />
                        :
                        children
                }
            </div>
        </div>
    )
}

export default AccountLayout;