import classNames from 'classnames';

import { EllipsisText, Flexbox } from '@components/Common';
import TeamImage from '@components/Sport/SportLayout/TeamImage';

import { getStartTime } from '@utils';

import FootballIcon from '@assets/icons/football.svg';
import BreadcrumbDividerIcon from '@assets/icons/breadcrumb-divider.svg';

import styles from './index.module.scss';

const MatchHighlightItem = ({ item }) => {
    const startTime = getStartTime(item.start_at);

    return (
        <div className={styles.matchBox}>
            <Flexbox className={styles.breadcrumb} spacing="5px" align="center">
                <span className={styles.item}>
                    <FootballIcon />
                    International
                </span>
                <BreadcrumbDividerIcon style={{ width: '7px', height: '5px' }} />
                <EllipsisText className={styles.lastItem} text={item.league} />
            </Flexbox>
            <Flexbox
                className={styles.teams}
                justify="space-between"
                spacing="8px"
            >
                <Flexbox className={styles.teamItem} direction="column" spacing="8px">
                    <TeamImage id={item.img1} sportType={item.sport}/>
                    <EllipsisText className={styles.name} text={item.t1} maxLine={2} />
                </Flexbox>
                <Flexbox className={styles.startTime} direction="column">
                    <span className={styles.date}>
                        {startTime.date}
                    </span>
                    <span className={styles.hour}>
                        {startTime.time}
                    </span>
                </Flexbox>
                <Flexbox
                    className={styles.teamItem}
                    direction="column"
                    align="flex-end"
                    spacing="8px"
                >
                    <TeamImage id={item.img2} sportType={item.sport}/>
                    <EllipsisText className={classNames(styles.name, styles.right)} text={item.t2} maxLine={2} />

                </Flexbox>
            </Flexbox>
            <Flexbox
                className={styles.odds}
                justify="space-between"
                spacing="6px"
            >
                {!!item?.market?.odds?.length && (
                    item.market.odds.map((odds, index) => (
                        <Flexbox className={styles.oddsItem} justify="space-between" key={index}>
                            <span className={styles.label}>{odds.k.replace('w', '')}</span>
                            <span className={styles.value}>{odds.v}</span>
                        </Flexbox>
                    ))
                )}
            </Flexbox>
        </div>
    );
};

export default MatchHighlightItem;