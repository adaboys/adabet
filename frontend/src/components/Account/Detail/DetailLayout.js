import OtherInfo from "./OtherInfo";

import { paths } from "@constants";
import { useAuth } from "@hooks";

import UserIcon from '@assets/icons/user.svg';

import TabLayout from "../TabLayout";
import styles from './DetailLayout.module.scss';

const tabs = [
    { name: 'User Information', key: paths.account },
    { name: 'Change password', key: paths.changePassword },
]

const DetailLayout = ({ children }) => {
    const { user } = useAuth();

    return (
        <TabLayout tabs={tabs} icon={<UserIcon />} title="Information & Sercure">
            <div className={styles.detailLayoutContent}>
                <OtherInfo user={user} />
                {children}
            </div>
        </TabLayout>
    )
}

export default DetailLayout;