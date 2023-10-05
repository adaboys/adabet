import classNames from 'classnames';
import dynamic from 'next/dynamic';
import { NumberParam, useQueryParams } from 'use-query-params';

import SportLayout from '../SportLayout';
import Highlight from './Highlight';
import Matches from './Matches';
import { useAuth, useBetting } from '@hooks';

import FlashIcon from '@assets/icons/flash.svg';
import CalendarIcon from '@assets/icons/calendar.svg';

import styles from './index.module.scss';

const Betslip = dynamic(() => import('./Betslip'), {
    ssr: false,
})

const Dashboard = () => {
    const [queries] = useQueryParams({ id: NumberParam });
    const { addBet, isOddsSelected } = useBetting(queries.id);

    return (
        <div className={styles.dashboard}>
            <SportLayout>
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
                    <Highlight addBet={addBet} isOddsSelected={isOddsSelected}/>
                    <Matches queries={queries} addBet={addBet} isOddsSelected={isOddsSelected}/>
                </div>
                <Betslip/>
            </SportLayout>
        </div>
    )
}

export default Dashboard;