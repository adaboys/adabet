import { useRef } from 'react';
import classNames from 'classnames';
import { useDraggable } from 'react-use-draggable-scroll';
import { useRouter } from 'next/router';

import { Flexbox, EllipsisText } from '@components/Common';
import TeamImage from '@components/Sport/SportLayout/TeamImage';
import SportIcon from '@components/Sport/SportLayout/SportIcon';

import { getStartTime, converOddsNames, paddingText } from '@utils';
import { matcheTypes, paths } from '@constants';


import PeopleIcon from '@assets/icons/people.svg';
import StarIcon from '@assets/icons/star.svg';
import StarActiveIcon from '@assets/icons/star-active.svg';
import BreadcrumbDividerIcon from '@assets/icons/breadcrumb-divider.svg';
import VSIcon from '@assets/icons/vs.svg';
import RadarIcon from '@assets/icons/radar.svg';
import ReportIcon from '@assets/icons/report.svg';

import styles from './MatchesItem.module.scss';

const MatchesItem = ({ matchesItem, addBet, isOddsSelected, onToggleMatch, onShowhistory }) => {
    const { pathname, query } = useRouter()
    const startTime = getStartTime(matchesItem.start_at);
    const listMarketRef = useRef(null);
    const { events } = useDraggable(listMarketRef);
    const { t: startTimeSecond, tt: totalTime, b: isBreakTime, i: extraTime } = matchesItem.timer || {}
    const isLive = startTimeSecond > 0;
    // const isHafttime = isLive && isBreakTime;
    const isSportPage = pathname === paths.sport;

    let timeLeft;
    if(isLive) {
        // const half = startTimeSecond < totalTime/2 + extraTime ? '1st' : '2nd';
        const minute = Math.floor(startTimeSecond / 60);
        // timeLeft = `${minute}' ${half} haft`;
        timeLeft = `${minute}'`;
    }

    return (
        <div className={styles.matchesItem}>
            <div className={styles.matchInfo}>
                <Flexbox className={styles.startTime} spacing="4px" direction="column">
                    {
                    isLive
                    ?
                        <>
                            <RadarIcon/>
                            {/* <span className={styles.timeLeft}>{isHafttime ? 'Hafttime' : timeLeft}</span> */}
                            <span className={styles.timeLeft}>{paddingText(3, timeLeft.toString(), '0')}</span>
                            <span className={styles.score}>{matchesItem.s1}-{matchesItem.s2}</span>
                        </>
                    :
                        <>
                            <span className={styles.time}>{startTime.time}</span>
                            <span>{startTime.date}</span>
                        </>
                    }
                </Flexbox>
                <Flexbox direction="column" className={styles.info}>
                    <Flexbox justify="space-between">
                        <Flexbox className={styles.league} spacing="12px" align="center">
                            <Flexbox className={styles.item} spacing="12px" align="center">
                                <SportIcon sportType={matchesItem.sport || 1}/>
                                International
                            </Flexbox>
                            <BreadcrumbDividerIcon className={styles.divider} />
                            <EllipsisText className={styles.item} text={matchesItem.league} />
                        </Flexbox>
                        <Flexbox className={styles.actions} align="center" spacing="8px">
                            <ReportIcon className="cursor-pointer" onClick={() => matchesItem.sport === 1 ? onShowhistory(matchesItem) : null}/>
                            <PeopleIcon/>
                            {
                                query?.matchType === matcheTypes.FAVORITE || matchesItem.favorited
                                ?
                                <StarActiveIcon
                                    onClick={!isSportPage ? null : () => onToggleMatch(matchesItem.id, false)}
                                    className="cursor-pointer"
                                />
                                :
                                <StarIcon
                                    onClick={!isSportPage ? null : () => onToggleMatch(matchesItem.id, true)}
                                    className={styles.starIcon}
                                />
                            }
                        </Flexbox>
                    </Flexbox>

                    <Flexbox
                        className={styles.teams}
                        spacing="16px"
                        align="center"
                    >
                        <Flexbox className={classNames(styles.teamItem, styles.team1)} spacing="12px" align="center">
                            <div className={styles.name}>
                                {matchesItem.t1}
                            </div>
                            <TeamImage id={matchesItem.img1} sportType={matchesItem.sport || 1}/>

                        </Flexbox>
                        {/* <span className={styles.divider}>vs</span> */}
                        <VSIcon/>
                        <Flexbox className={classNames(styles.teamItem, styles.team2)} spacing="8px" align="center">
                            <TeamImage id={matchesItem.img2} sportType={matchesItem.sport} />
                            <EllipsisText className={styles.name} text={matchesItem.t2} basedOn="words" />

                        </Flexbox>
                    </Flexbox>

                </Flexbox>
            </div>
            <div ref={listMarketRef} className={styles.markets} {...events}>
                {
                    matchesItem?.markets?.length
                    ?
                    matchesItem.markets?.map((market, marketIndex) => (
                        <Flexbox key={marketIndex} direction="column" spacing="8px" className={styles.marketItem}>
                            <span className={styles.name}>{converOddsNames(market?.name, matchesItem.sport)}</span>
                            <Flexbox spacing="2px" className={styles.odds}>
                                {
                                    market?.odds?.map((oddsItem, oddsIndex) => (
                                        <Flexbox
                                            key={oddsIndex}
                                            spacing="2px"
                                            className={classNames(styles.oddsItem, {[styles.selected]: isSportPage && isOddsSelected(matchesItem, market.name , oddsItem)})}
                                            direction="column"
                                            align="center"
                                            disabled={oddsItem.s}
                                            style={{ ...(oddsItem.s && { opacity: '0.5' })}}
                                            onClick={!isSportPage ? null : () => addBet(matchesItem, market.name, oddsItem)}
                                        >
                                            <span className={styles.label}>{converOddsNames(oddsItem.k, matchesItem.sport)}</span>
                                            <span className={styles.value}>{oddsItem.v || '-'}</span>
                                        </Flexbox>
                                    ))
                                }
                            </Flexbox>
                        </Flexbox>
                    ))
                    :
                    <div className={styles.emptyMarket}>No Market Available</div>
                }
            </div>
        </div>
    )

}

export default MatchesItem;
