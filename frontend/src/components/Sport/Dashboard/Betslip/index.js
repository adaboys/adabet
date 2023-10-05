import { useState, useEffect } from 'react';
import classNames from 'classnames';
import { NumberParam, useQueryParams } from 'use-query-params';

import { EmptyResult } from '@components/Common';
import Tabs from '../../SportLayout/Tabs';
import Single from './Single';

import NoteIcon from '@assets/icons/note.svg';
import BreadcrumbDividerIcon from '@assets/icons/breadcrumb-divider.svg';
import { useBetting } from '@hooks';

import styles from './index.module.scss';

const SINGLE_TAB = 'single';
const COMBO_TAB = 'combo';
const SYSTEM_TAB = 'system';

const tabs = [
    { name: 'Single', key: SINGLE_TAB },
    { name: 'Combo', key: COMBO_TAB },
    { name: 'System', key: SYSTEM_TAB }
]

const Betslip = () => {

    const [queries] = useQueryParams({ id: NumberParam });
    const [activeTab, setActiveTab] = useState(SINGLE_TAB);
    const [isOpen, setIsOpen] = useState(false);
    const sportId = queries.id;
    const { betData, removeBet, updateBet, updateAmountsBet, removeAllBet } = useBetting(sportId);
    useEffect(() => {
        if(betData?.length) {
            setIsOpen(true);
        }
        else if(isOpen) {
            setIsOpen(false);
        }
    }, [betData?.length])

    return (
        <div className={styles.betslip}>
            <div className={styles.header} onClick={() => setIsOpen(!isOpen)}>
                {
                    betData?.length
                    ?
                    <span className={styles.count}>{betData?.length}</span>
                    :
                    <NoteIcon />
                }
                <span className={styles.title}>My bets</span>
                <BreadcrumbDividerIcon className={classNames(styles.chevronDown, {[styles.isOpen]: isOpen})} />
            </div>
            {
                isOpen
                    ?
                    <div className={styles.content}>
                        <Tabs
                            tabs={tabs}
                            className={styles.tabs}
                            tabClassName={styles.tab}
                            tabActiveClassName={styles.tabActive}
                            onTabClick={setActiveTab} activeTab={activeTab}
                        />
                        <div className={styles.matches}>
                            {
                                activeTab !== SINGLE_TAB || !betData?.length
                                    ?
                                    <EmptyResult />
                                    :
                                    <Single
                                        sportId={sportId}
                                        betData={betData}
                                        removeBet={removeBet}
                                        updateBet={updateBet}
                                        updateAmountsBet={updateAmountsBet}
                                        removeAllBet={removeAllBet}
                                    />
                            }
                        </div>
                    </div>
                    :
                    null
            }

        </div>
    )
}

export default Betslip;