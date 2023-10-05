import classNames from 'classnames';
import Link from 'next/link';
import styles from './Tabs.module.scss';

const Tabs = ({ tabs, className, tabClassName, tabActiveClassName, activeTab, onTabClick }) => {
    return (
        <div className={classNames(styles.tabs, {[className]: !!className})}>
            {
                tabs.map(tab => (
                    <div
                        key={tab.key}
                        onClick={() => onTabClick(tab.key)}
                        className={
                            classNames(styles.tab, {
                                [tabClassName]: !!tabClassName,
                                [tabActiveClassName || styles.active]: activeTab === tab.key
                            })}
                        >
                        {(activeTab === tab.key && tab.activeIcon && <tab.activeIcon/>) || (tab.icon && <tab.icon/>)}
                        {
                            tab.url
                            ?
                            <Link href={tab.url}>
                                <a>{tab.name}</a>
                            </Link>
                            :
                            <a onClick={tab.onClick || null}>{tab.name}</a>
                        }
                    </div>
                ))
            }
        </div>
    )
}

export default Tabs;
