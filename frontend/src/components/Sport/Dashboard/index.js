import classNames from 'classnames';

import SportLayout from '../SportLayout';
import SideLeftContent from './SideLeftContent';
import Highlight from './Highlight';
import Matches from './Matches';

import FlashIcon from '@assets/icons/flash.svg';
import CalendarIcon from '@assets/icons/calendar.svg';

import styles from './index.module.scss';

const Dashboard = () => {
    return (
        <div className={styles.dashboard}>
            <SportLayout sideLeftContent={SideLeftContent}>
                <div className={styles.tabHeader}>
                    <div className={classNames(styles.tabItem, { [styles.active]: true })}>
                        <FlashIcon/>
                        Highlight
                    </div>
                    <div className={styles.tabItem}>
                        <CalendarIcon />
                        Schedule
                    </div>
                </div>
                <div className={styles.tabContent}>
                    <Highlight/>
                    <Matches/>
                </div>
            </SportLayout>
        </div>
    )
}

export default Dashboard;