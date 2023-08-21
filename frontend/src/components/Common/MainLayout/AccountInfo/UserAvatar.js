import AvatarIcon from '@assets/icons/avatar.svg';

import styles from './UserAvatar.module.scss';

const UserAvatar = ({ user, onClick }) => {
    return (
        <a className={styles.userAvatar} onClick={onClick}>
            {
                user?.avatar
                ?
                <img className={styles.avatarImg} src={user.avatar}/>
                :
                <AvatarIcon />
            }
            <span className={styles.info}>
                <p className={styles.name}>{user?.name}</p>
                <p className={styles.level}>VIP {user?.vip_level}</p>
            </span>
        </a>
    )
}

export default UserAvatar;
