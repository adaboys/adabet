import { useRouter } from "next/router";
import { FormattedMessage } from "react-intl";

import Tabs from "@components/Sport/SportLayout/Tabs";
import AccountLayout from '../AccountLayout';

import styles from './index.module.scss';

const TabLayout = ({ children, tabs, title, icon }) => {
    const { pathname, push } = useRouter();

    return (
        <AccountLayout>
            <div className={styles.tabLayout}>
                <h3 className={styles.title}>
                    {icon}
                    <FormattedMessage key="title" defaultMessage={title} />
                </h3>
                <Tabs
                    className={styles.tabs}
                    tabs={tabs}
                    tabClassName={styles.tabItem}
                    tabActiveClassName={styles.tabActive}
                    onTabClick={push}
                    activeTab={pathname}
                />
                {children}
            </div>
        </AccountLayout>
    )
}

export default TabLayout;