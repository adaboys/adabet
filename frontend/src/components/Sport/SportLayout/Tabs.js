import classNames from 'classnames';
import Link from 'next/link';
import styles from './Tabs.module.scss';

const Tabs = ({ tabs, className, tabActiveClassName, activeTab, onTabClick }) => {
    return (
        <div className={classNames(styles.tabs, {[className]: !!className})}>
            {
                tabs.map(tab => (
                    <div
                        onClick={() => onTabClick(tab.key)}
                        className={classNames(styles.tab, { [tabActiveClassName || styles.active]: activeTab === tab.key })}>
                        <tab.icon/>
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
