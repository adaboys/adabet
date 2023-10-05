import moment from 'moment';
import { useMemo, useState, useCallback } from 'react';
import { CircularProgressbarWithChildren, buildStyles } from 'react-circular-progressbar';
import 'react-circular-progressbar/dist/styles.css';
import classNames from 'classnames';

import { Flexbox, EllipsisText, Button } from '@components/Common';
import TeamImage from '@components/Sport/SportLayout/TeamImage';

import RadarIcon from '@assets/icons/radar.svg';
import RefreshIcon from '@assets/icons/refresh.svg';
import MatchDividerIcon from '@assets/icons/match-divider.svg';
import ArrowIcon from '@assets/icons/per-arrow.svg';

import styles from './h2h.module.scss';

const PAGING_SIZE = 5;

const H2H = ({ matchInfo }) => {
    const [isSwap, setIsSwap] = useState(false);
    const [nextMeetingPage, setNextMeetingPage] = useState(1);
    const [matchesPages, setMatchesPages] = useState({
        next_matches_homes: 1,
        next_matches_aways: 1,
        last_matches_homes: 1,
        last_matches_aways: 1
    });
    const { t: startTimeSecond } = matchInfo.timer || {}
    const isLive = startTimeSecond > 0;
    const startAt = moment.utc(matchInfo.start_at).local().format('YYYY/MM/DD - HH:mm');

    let timeLeft;
    if (isLive) {
        const minute = Math.floor(startTimeSecond / 60);
        timeLeft = `${minute}'`;
    }

    const nextMatch = useMemo(() => {
        if (matchInfo?.history?.next_meetings?.list.length) {
            return matchInfo.history.next_meetings.list[matchInfo.history.next_meetings.list.length - 1];
        }
        return null;
    }, [matchInfo])

    const firstMatch = useMemo(() => {
        if (matchInfo?.history?.last_meetings?.list.length) {
            return matchInfo.history.last_meetings.list[0];
        }
        return null;
    }, [matchInfo]);

    const totalMatch = useMemo(() => {
        if (matchInfo?.history?.summary) {
            return matchInfo.history.summary.away.victories + matchInfo.history.summary.home.victories;
        }
        return 0;
    }, [matchInfo]);



    const getSummaryOfTeam = useCallback((isLeft) => {
        const teamKey = isSwap === isLeft ? 'away' : 'home';
        if (matchInfo?.history?.summary) {
            return matchInfo.history.summary[teamKey];
        }
        return null;
    }, [matchInfo, isSwap]);

    const getVictoryOfTeam = useCallback((isLeft) => {
        const summary = getSummaryOfTeam(isLeft);
        if (summary) {
            return summary.victories;
        }
        return 0;
    }, [getSummaryOfTeam]);

    const renderSummaryOfTeam = useCallback((isLeft) => {
        const summary = getSummaryOfTeam(isLeft);
        let perTxt = 'N/A';
        let per = 0;
        let performances = [];
        if (summary?.performance?.length) {
            performances = summary.performance.slice(-6);
            const totalPoint = performances.reduce((total, item) => {
                if (item === 'W') return total + 2;
                else if (item === 'D') return total + 1;
                else return total;
            }, 0);
            per = Math.round(100 * totalPoint / (performances.length * 2));
            perTxt = `${per} %`;
        }
        return (
            <div className={styles.sumTeam}>
                <div className={styles.chart}>
                    <div className={styles.circle}>
                        <CircularProgressbarWithChildren
                            value={per}
                            strokeWidth={4}
                            circleRatio={getVictoryOfTeam(isLeft) / totalMatch}
                            styles={buildStyles({
                                pathColor: "#fff",
                                // trailColor: "#eee",
                                trailColor: "transparent",
                                // strokeLinecap: "#000"
                            })}
                        >
                            <span className={styles.total}>{perTxt}</span>
                            <span className={styles.label}>Form</span>
                        </CircularProgressbarWithChildren>
                    </div>
                </div>
                <p className={styles.perTitle}>Performance</p>
                <div className={styles.performance}>
                    {
                        performances.map((perform, index) => (
                            <>
                                <span className={classNames(styles.item, {
                                    [styles.draw]: perform === 'D',
                                    [styles.lose]: perform === 'L',
                                    [styles.win]: perform === 'W'
                                })}>{perform}</span>
                                {
                                    index === performances.length - 1
                                        ?
                                        <ArrowIcon />
                                        :
                                        <span className={styles.divider}></span>
                                }

                            </>
                        ))
                    }
                </div>
            </div>
        )
    }, [getSummaryOfTeam]);

    const renderLastMatchTable = useCallback((isLeft, dataKey) => {
        const teamKey = isSwap === isLeft ? 'aways' : 'homes';
        const matches = matchInfo?.history?.[dataKey]?.[teamKey] || [];
        const pagingKey = `${dataKey}_${teamKey}`;

        return (
            <Flexbox direction="column" style={{ flex: 1}}>
                <table className={styles.matchTable}>
                    <thead>
                        <tr>
                            <td align="center">Match</td>
                        </tr>
                    </thead>
                    <tbody>
                        {
                        matches.slice(0, matchesPages[pagingKey] * PAGING_SIZE).map((met) => {
                                const [homeScore, awayScore] = met.ss.split('-');
                                return (
                                    <tr>
                                        <td>
                                            <Flexbox spacing="32px">
                                                <Flexbox spacing="8px" justify="flex-end" style={{ flex: 1 }} align="center">
                                                    <EllipsisText className={classNames("text-right", {[styles.active]: homeScore > awayScore && dataKey !== 'next_matches' })} text={met.home.name} />
                                                    <TeamImage id={met.home.img} sportType={matchInfo.sport} />
                                                </Flexbox>
                                                <Flexbox direction="column">
                                                    <span style={{fontSize: '12px'}}>{met.time ? moment.unix(met.time).local().format("YYYY/MM/DD") : '-'}</span>
                                                    <Flexbox spacing="10px" justify="center">
                                                        {
                                                            dataKey === 'next_matches'
                                                            ?
                                                            '-'
                                                            :
                                                            <span className={classNames({ [styles.active]: homeScore > awayScore })}>{homeScore}</span>
                                                        }
                                                        
                                                        <span>:</span>
                                                        {
                                                            dataKey === 'next_matches'
                                                            ?
                                                            '-'
                                                            :
                                                            <span className={classNames({ [styles.active]: homeScore < awayScore })}>{awayScore}</span>
                                                        }
                                                        {
                                                            dataKey === 'next_matches'
                                                            ?
                                                            null:
                                                            <span>(FT)</span>
                                                        }
                                                    </Flexbox>
                                                </Flexbox>
                                                <Flexbox spacing="8px" justify="flex-start" style={{ flex: 1 }} align="center">
                                                    <TeamImage id={met.away.img} sportType={matchInfo.sport} />
                                                    <EllipsisText className={classNames({[styles.active]: homeScore < awayScore && dataKey !== 'next_matches' })} text={met.away.name} />
                                                </Flexbox>
                                            </Flexbox>
                                        </td>
                                    </tr>
                                )
                            })
                        }

                    </tbody>
                </table>
                {
                    matches.length > matchesPages[pagingKey] * PAGING_SIZE
                    ?
                    <div className={styles.actions}>
                        <Button onClick={() => setMatchesPages({...matchesPages, [pagingKey]: matchesPages[pagingKey] + 1})} className={styles.btnViewMore}>View more</Button>
                    </div>
                    :
                    null
                }
            </Flexbox>
        )
    }, [matchInfo, isSwap, matchesPages]);

    return (
        <div className={styles.h2h}>
            <div className={styles.summary}>
                <div className={styles.header}>
                    {matchInfo.league}
                </div>
                <Flexbox className={styles.content} justify="space-between">
                    <Flexbox className={styles.team}>
                        <TeamImage size="b" id={isSwap ? matchInfo.img2 : matchInfo.img1} sportType={matchInfo.sport}/>
                        <Flexbox>
                            <span className={styles.name}>{isSwap ? matchInfo.t2 : matchInfo.t1}</span>
                        </Flexbox>
                    </Flexbox>
                    <Flexbox className={styles.info} direction="column" align="center" spacing="8px">
                        {
                            isLive
                                ?
                                <>
                                    <div className={styles.timeLeft}>{timeLeft} <RadarIcon /></div>
                                    <span>{`${!isSwap ? matchInfo.s1 : matchInfo.s2} : ${!isSwap ? matchInfo.s2 : matchInfo.s1}`}</span>
                                </>
                                :
                                <span className={styles.timeStart}>{startAt}</span>
                        }
                        <RefreshIcon onClick={() => setIsSwap(!isSwap)} className="cursor-pointer" />
                    </Flexbox>
                    <Flexbox className={styles.team} justify="flex-end">
                        <Flexbox>
                            <span className={styles.name}>{isSwap ? matchInfo.t1 : matchInfo.t2}</span>
                        </Flexbox>
                        <TeamImage size="b" id={isSwap ? matchInfo.img1 : matchInfo.img2} sportType={matchInfo.sport}/>
                        {/* <img src="/images/sports/shirt.png" alt="Shirt"/> */}
                    </Flexbox>
                </Flexbox>
            </div>
            <div className={styles.nextMatch}>
                <div className={styles.header}>
                    Next match
                </div>
                <div className={styles.info}>
                    <Flexbox className={styles.col} direction="column" align="center">
                        <span className={styles.title}>Date</span>
                        <span>{nextMatch ? moment.unix(nextMatch.time).local().format("YYYY/MM/DD") : '-'}</span>
                    </Flexbox>
                    <Flexbox className={styles.col} direction="column" align="center">
                        <span className={styles.title}>Tournament</span>
                        <span>{nextMatch ? nextMatch.league : '-'}</span>
                    </Flexbox>
                    <Flexbox className={styles.col} direction="column" align="center">
                        <span className={styles.title}>Match</span>
                        <span>{nextMatch?.home?.name} <MatchDividerIcon /> {nextMatch?.away?.name}</span>
                    </Flexbox>
                </div>
                <div className={styles.statistics}>
                    {renderSummaryOfTeam(true)}
                    <Flexbox direction="column" spacing="16px">
                        <Flexbox className={styles.victories} direction="column" align="center">
                            <span className={styles.total}>{getVictoryOfTeam(true)}</span>
                            <span className={styles.lable}>victories</span>
                        </Flexbox>
                        <Flexbox className={styles.generalInfo} direction="column" align="flex-end">
                            <span className={styles.label}>Highest win</span>
                            <span className={styles.value}>{getSummaryOfTeam(true).highest_win}</span>
                        </Flexbox>
                        <Flexbox className={styles.generalInfo} direction="column" align="flex-end">
                            <span className={styles.label}>Total goal(s)</span>
                            <span className={styles.value}>{getSummaryOfTeam(true).tt_goals}</span>
                        </Flexbox>
                        <Flexbox className={styles.generalInfo} direction="column" align="flex-end">
                            <span className={styles.label}>Avg. goals/match</span>
                            <span className={styles.value}>{getSummaryOfTeam(true).avg_goal_match}</span>
                        </Flexbox>
                    </Flexbox>
                    <div className={styles.sumMeeting}>
                        <div className={styles.win}>
                            <h3 className={styles.total}>{totalMatch}</h3>
                            <div className={styles.played}>
                                {
                                    firstMatch
                                        ?
                                        <Flexbox direction="column">
                                            <span>Played since</span>
                                            <span>{moment.unix(nextMatch.time).local().format("YYYY")}</span>
                                        </Flexbox>
                                        :
                                        <span>Played</span>
                                }
                            </div>
                            <div className={styles.home}>
                                <CircularProgressbarWithChildren
                                    value={100}
                                    strokeWidth={4}
                                    circleRatio={totalMatch === 0 ? 0 : getVictoryOfTeam(true) / totalMatch}
                                    styles={buildStyles({
                                        pathColor: "rgba(255, 255, 255, 0.50)",
                                        // trailColor: "#eee",
                                        trailColor: "transparent",
                                        // strokeLinecap: "#000"
                                    })}
                                >

                                </CircularProgressbarWithChildren>
                            </div>
                            <div className={styles.away}>
                                <CircularProgressbarWithChildren
                                    value={100}
                                    strokeWidth={4}
                                    circleRatio={totalMatch === 0 ? 0 : getVictoryOfTeam(false) / totalMatch}
                                    styles={buildStyles({
                                        // pathColor: "rgba(255, 255, 255, 0.50)",
                                        pathColor: "#fff",
                                        trailColor: "transparent",
                                        // strokeLinecap: "#000"
                                    })}
                                >

                                </CircularProgressbarWithChildren>
                            </div>
                        </div>
                        <Flexbox direction="column" align="center" className={styles.draw}>
                            <span className={styles.value}>{matchInfo?.history?.summary?.draw || 0}</span>
                            <span className={styles.label}>Draw</span>
                        </Flexbox>
                    </div>
                    <Flexbox direction="column" spacing="16px">
                        <Flexbox className={styles.victories} direction="column" align="center">
                            <span className={styles.total}>{getVictoryOfTeam(false)}</span>
                            <span className={styles.lable}>victories</span>
                        </Flexbox>
                        <Flexbox className={styles.generalInfo} direction="column" align="flex-start">
                            <span className={styles.label}>Highest win</span>
                            <span className={styles.value}>{getSummaryOfTeam(false).highest_win}</span>
                        </Flexbox>
                        <Flexbox className={styles.generalInfo} direction="column" align="flex-start">
                            <span className={styles.label}>Total goal(s)</span>
                            <span className={styles.value}>{getSummaryOfTeam(false).tt_goals}</span>
                        </Flexbox>
                        <Flexbox className={styles.generalInfo} direction="column" align="flex-start">
                            <span className={styles.label}>Avg. goals/match</span>
                            <span className={styles.value}>{getSummaryOfTeam(false).avg_goal_match}</span>
                        </Flexbox>
                    </Flexbox>
                    {renderSummaryOfTeam(false)}
                </div>
            </div>
            <div className={styles.lastMeetings}>
                <div className={styles.header}>
                    Last meetings
                </div>
                <table className={styles.info}>
                    <thead>
                        <tr>
                            <td style={{width: '12%'}}>Date</td>
                            <td align="center" style={{width: '60%'}}>Match</td>
                            <td>Tournament</td>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            matchInfo?.history?.last_meetings?.list?.slice(0, nextMeetingPage * PAGING_SIZE)?.map((met) => {
                                const [homeScore, awayScore] = met.ss.split('-');
                                return (
                                    <tr>
                                        <td  style={{width: '12%'}}>{met.time ? moment.unix(met.time).local().format("YYYY/MM/DD") : '-'}</td>
                                        <td style={{width: '60%'}}>
                                            <Flexbox spacing="16px">
                                                <Flexbox spacing="8px" justify="flex-end" style={{flex: 1}}>
                                                    {/* <span className={classNames({ [styles.active]: homeScore > awayScore })}>{met.home.name}</span> */}
                                                    <EllipsisText className={classNames("text-right", {[styles.active]: homeScore > awayScore })} text={met.home.name} />
                                                    <TeamImage id={met.home.img} sportType={matchInfo.sport} />
                                                </Flexbox>
                                                <Flexbox spacing="10px">
                                                    <span className={classNames({ [styles.active]: homeScore > awayScore })}>{homeScore}</span>
                                                    <span>:</span>
                                                    <span className={classNames({ [styles.active]: homeScore < awayScore })}>{awayScore}</span>
                                                    <span>(FT)</span>
                                                </Flexbox>
                                                <Flexbox spacing="8px" justify="flex-start" style={{flex: 1}}>
                                                    <TeamImage id={met.away.img} sportType={matchInfo.sport} />
                                                    <EllipsisText className={classNames({ [styles.active]: homeScore < awayScore })} text={met.away.name} />
                                                    {/* <span className={classNames({ [styles.active]: homeScore < awayScore })}>{met.away.name}</span> */}

                                                </Flexbox>
                                            </Flexbox>
                                        </td>
                                        <td><EllipsisText text={met.league}/></td>
                                    </tr>
                                )
                            })
                        }

                    </tbody>
                </table>
                {
                    matchInfo?.history?.last_meetings?.list?.length > nextMeetingPage * PAGING_SIZE
                    ?
                    <div className={styles.actions}>
                        <Button onClick={() => setNextMeetingPage(nextMeetingPage + 1)} className={styles.btnViewMore}>View more</Button>
                    </div>
                    :
                    null
                }
                
            </div>
            <div className={styles.matches}>
                <div className={styles.header}>
                    Last matches
                </div>
                <Flexbox spacing="16px" style={{padding: '16px 16px 0 16px'}}>
                    {renderLastMatchTable(true, 'last_matches')}
                    {renderLastMatchTable(false, 'last_matches')}
                </Flexbox>
            </div>
            <div className={styles.matches}>
                <div className={styles.header}>
                    Next matches
                </div>
                <Flexbox spacing="16px" style={{padding: '16px 16px 0 16px'}}>
                    {renderLastMatchTable(true, 'next_matches')}
                    {renderLastMatchTable(false, 'next_matches')}
                </Flexbox>
            </div>
        </div>
    )
}

export default H2H;
