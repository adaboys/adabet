import { useMemo } from 'react';
import UserAvatar from './UserAvatar';

import LinkIcon from '@assets/icons/link.svg';

import styles from './OtherInfo.module.scss';

const OtherInfo = ({ user }) => {

    const referalLink = useMemo(() => {
        return `${process.env.NEXT_PUBLIC_SITE_URL}/refCode=${user?.referral_code}`
    }, [user?.referral_code]);

    return (
        <div className={styles.otherInfo}>
            <UserAvatar user={user}/>
            <div className={styles.userPoint}>
                <label className={styles.level}>VIP {user.vip_level}</label>
                <span className={styles.nextLevel}>{user.next_vip_point} points to vip 2</span>
            </div>
            <div className={styles.userCode}>
                <label className={styles.label}>Your ID:</label>
                <span className={styles.value}>{user.code}</span>
            </div>
            <div className={styles.referal}>
                <label className={styles.label}>
                    <LinkIcon/>
                    Your referral link:
                </label>
                <a className={styles.link}>{referalLink}</a>
            </div>
        </div>
    )
}

export default OtherInfo;
