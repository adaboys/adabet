import { BasicModal } from '@components/Common';

import CloseIcon from '@assets/icons/close.svg';

import styles from './index.module.scss';
import UserInfo from './UserInfo';
import StatisticsInfo from './StatisticsInfo';

const Statistics = ({ overlay: { hide, show, context } }) => {
    return (
        <BasicModal
            isOpen
            overlayClassName={styles.statisticsModal}
            contentClassName={styles.statisticsWrapper}
        >
            <div className={styles.header}>
                <h3 className={styles.title}>
                    User information
                </h3>
                <span onClick={hide} className={styles.btnClose}><CloseIcon /></span>
            </div>
            <UserInfo />
            <StatisticsInfo />
        </BasicModal>
    );
};

export default Statistics;