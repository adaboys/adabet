import classNames from 'classnames';
import { Flexbox, EllipsisText } from '@components/Common';
import { getStartTime } from '@utils';

import PeopleIcon from '@assets/icons/people.svg';
import StarIcon from '@assets/icons/star.svg';
import FootballIcon from '@assets/icons/football.svg';
import BreadcrumbDividerIcon from '@assets/icons/breadcrumb-divider.svg';

import styles from './MatchesList.module.scss';

const MatchesList = ({ matches }) => {
    return (
        <div className={styles.matchesList}>
            {
                matches.map(matchesItem => {
                    const startTime = getStartTime(matchesItem.start_at);
                    return (
                        <div className={styles.matchesItem}>
                            <div className={styles.header}>
                                <Flexbox className={styles.startTime} spacing="12px">
                                    <span>{startTime.date}</span>
                                    <span>&#8226;</span>
                                    <span className={styles.time}>{startTime.time}</span>
                                </Flexbox>
                                <Flexbox className={styles.league} spacing="12px" align="center">
                                    <Flexbox className={styles.item} spacing="12px" align="center">
                                        <FootballIcon />
                                        International
                                    </Flexbox>
                                    <BreadcrumbDividerIcon style={{ width: '7px', height: '5px' }} />
                                    <EllipsisText className={styles.lastItem} text={matchesItem.league} />
                                </Flexbox>
                                <Flexbox
                                    className={styles.teams}
                                    spacing="24px"
                                    align="center"
                                >
                                    <Flexbox className={classNames(styles.teamItem, styles.team1)} spacing="12px" align="center">
                                        <img src="/images/flags/aston.png" />
                                        <div className={styles.name}>
                                            {matchesItem.team1}
                                        </div>

                                    </Flexbox>
                                    <span className={styles.divider}>vs</span>
                                    <Flexbox className={classNames(styles.teamItem, styles.team2)} spacing="8px" align="center">
                                        <img src="/images/flags/liverpool.png" />
                                        <EllipsisText className={styles.name} text={matchesItem.team2} basedOn="words" />

                                    </Flexbox>
                                </Flexbox>
                                <Flexbox className={styles.actions} align="center" spacing="16px">
                                    <PeopleIcon />
                                    <StarIcon className={styles.starIcon} />
                                </Flexbox>
                            </div>
                            <Flexbox className={styles.markets} spacing="12px">
                                {
                                    matchesItem.markets?.map(market => (
                                        <Flexbox direction="column" spacing="8px" className={styles.marketItem}>
                                            <span className={styles.name}>{market?.name}</span>
                                            <Flexbox spacing="2px" className={styles.odds}>
                                                {
                                                    market?.odds?.map(odds => (
                                                        <Flexbox
                                                            spacing="2px"
                                                            className={styles.oddsItem}
                                                            direction="column"
                                                            align="center"
                                                        >
                                                            <span className={styles.label}>{odds.k.replace('w', '')}</span>
                                                            <span className={styles.value}>{odds.v}</span>
                                                        </Flexbox>
                                                    ))
                                                }
                                            </Flexbox>
                                        </Flexbox>
                                    ))
                                }
                            </Flexbox>
                        </div>
                    )
                })
            }
        </div>
    )
}

export default MatchesList;
