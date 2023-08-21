import { useMemo } from 'react';

import { useAuth } from '@hooks';

import AvatarIcon from '@assets/icons/avatar.svg';
import LinkIcon from '@assets/icons/link.svg';

import styles from './index.module.scss';

const UserInfo = () => {
    const { user } = useAuth();

    const referalLink = useMemo(() => {
        return `${process.env.NEXT_PUBLIC_SITE_URL}/refCode=${user?.referral_code}`
    }, [user?.referral_code]);

    return (
        <div className={styles.userInfo}>
            {user?.avatar ?
                <img
                    className={styles.avatarImg}
                    src={user?.avatar}
                    alt='Avatar'
                /> :
                <AvatarIcon height={96} width={96} />
            }
            <div className={styles.userPoint}>
                <label className={styles.level}>VIP {user.vip_level}</label>
                <span className={styles.nextLevel}>{user.next_vip_point} points to vip 2</span>
            </div>
            <div className={styles.userCode}>
                <label className={styles.label}>Your ID:</label>
                <span className={styles.value}>{user.code}</span>
            </div>
            <div className={styles.referal}>
                <LinkIcon />
                <span>Your referral link: <a className={styles.link}>{referalLink}</a></span>
            </div>
        </div>
    );
};

export default UserInfo;