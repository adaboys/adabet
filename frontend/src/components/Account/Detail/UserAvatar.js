import { useDispatch, useSelector } from 'react-redux';
import { sendRequest } from '@utils/api';
import apiConfig from '@constants/apiConfig';
import { useNotification } from '@hooks';
import { accountActions } from '@redux/actions';

import AvatarIcon from '@assets/icons/avatar.svg';
import CameraIcon from '@assets/icons/camera.svg';

import styles from './UserAvatar.module.scss';

const UserAvatar = ({ user }) => {
    const { accessToken } = useSelector(state => state.account);
    const { showError, showSuccess } = useNotification();
    const dispatch = useDispatch();

    const handleFileChange = async (e) => {
        e.preventDefault();
        const file = e.target.files[0];
        const fileSizeLimit = 10 * 1024 * 1024;

        if (!file || !file.type.startsWith("image/") || file.size > fileSizeLimit) {
            showError('File is invalid');
            return;
        }

        const {
            success,
            responseData
        } = await sendRequest({...apiConfig.account.uploadAvatar, accessToken}, { avatar: file });
        if(success && responseData?.status === 200) {
            const avatar = responseData.data.avatar;
            dispatch(accountActions.updateCommonInfo({ profileData: { ...user, avatar }}));
            showSuccess('Update avatar successful!');
        }
        else {
            showError('Update avatar failed!');
        }

    }

    return (
        <div className={styles.userAvatar}>
            <div className={styles.uploadWrapper}>
                <label htmlFor='file-input' className={styles.upload}>
                    {user?.avatar ?
                        <img
                            className={styles.avatarImg}
                            src={user?.avatar}
                            alt='Avatar'
                        /> :
                        <AvatarIcon height={80} width={80} />
                    }
                    <CameraIcon className={styles.camera}/>
                </label>
                
            </div>
            <input type='file' id='file-input' onChange={handleFileChange} accept="image/*" />
        </div>
    )
}

export default UserAvatar;
