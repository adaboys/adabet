import { useState } from 'react';
import { useSelector, useDispatch } from 'react-redux';

import { BasicModal, EmptyResult } from '@components/Common';
import Tabs from '@components/Sport/SportLayout/Tabs';
import H2H from './H2H';

import CloseIcon from '@assets/icons/close.svg';
import MenuIcon from '@assets/icons/hamburger.svg';

import styles from './index.module.scss';

const OVERVIEW_TAB = 'overview';
const H2H_TAB = 'h2h';

const tabs = [
    // { name: 'Over view', key: OVERVIEW_TAB },
    { name: 'H2H', key: H2H_TAB }
]

const MatchesHistoryModal = ({
    matchInfo,
    onClose,
}) => {

    const [activeTab, setActiveTab] = useState(H2H_TAB);
    const sports = useSelector(state => state.sport.sports || []);
    const sportName = sports.find(sport => sport.id === matchInfo.sport)?.name;

    return (
        <BasicModal
            isOpen={true}
            overlayClassName={styles.matchesHistoryModal}
            contentClassName={styles.wrapper}
        >
            <div className={styles.header}>
                <div className={styles.breadcrumb}>
                    <MenuIcon className={styles.menuIcon}/>
                    <span className="uppercase">{sportName}</span>
                    <span>/</span>
                    <span>International</span>
                    <span>/</span>
                    <span>{matchInfo.league}</span>
                </div>
                <CloseIcon className={styles.btnClose} onClick={onClose}/>
            </div>
            <Tabs
                tabs={tabs}
                className={styles.tabs}
                tabClassName={styles.tab}
                tabActiveClassName={styles.tabActive}
                onTabClick={setActiveTab} activeTab={activeTab}
            />
            {
                activeTab === H2H_TAB
                ?
                <H2H matchInfo={matchInfo}/>
                :
                <EmptyResult/>
            }
        </BasicModal>
    )
}

export default MatchesHistoryModal;
