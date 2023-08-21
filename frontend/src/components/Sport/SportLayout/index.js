import SideLeftContent from './SideLeftContent';
import { SportHeaderMenu } from '@components/Common';

import styles from './index.module.scss';

const SportLayout = ({ children }) => {
    
    return (
        <div className={styles.sportLayout}>
            <div className={styles.sideLeftMenu}>
                <SportHeaderMenu/>
                <SideLeftContent/>
            </div>
            <div className={styles.mainContent}>
                {children}
            </div>
        </div>
    )
}

export default SportLayout;
