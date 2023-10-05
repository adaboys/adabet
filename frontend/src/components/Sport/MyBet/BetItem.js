import { useMemo } from 'react';
import moment from 'moment';
import { FormattedNumber } from 'react-intl';

import { Button, Flexbox, EllipsisText } from '@components/Common';
import SportIcon from '../SportLayout/SportIcon';

import { converOddsNames } from '@utils';

import BreadcrumbDividerIcon from '@assets/icons/breadcrumb-divider.svg';
import ExportIcon from '@assets/icons/export.svg';
import RadarIcon from '@assets/icons/radar.svg';

import styles from './BetItem.module.scss';

const BET_OPEN = 0;
const BET_WON = 1;
const BET_DRAW = 2;
const BET_LOSED = 3;

const REWARD_STATUS_PROCESSING = 1;
const REWARD_STATUS_SUBMITTED = 2;
const REWARD_STATUS_SUBMIT_FAILED = 3;

const betResults = {
    [BET_OPEN]: {
        text: 'OPEN',
        background: '#225FED'
    },
    [BET_LOSED]: {
        text: 'LOST',
        background: 'rgba(247, 64, 64, 0.8)'
    },
    [BET_WON]: {
        text: 'WON',
        background: 'rgba(50, 198, 83, 0.8)'
    },
    [BET_DRAW]: {
        text: 'DRAW',
        background: '#F7C927CC'
    },
}

const rewardStatuses = {
    [REWARD_STATUS_PROCESSING]: 'Processing',
    [REWARD_STATUS_SUBMITTED]: 'Submitted',
    [REWARD_STATUS_SUBMIT_FAILED]: 'Submit failed'
}

const BetItem = ({ bet }) => {
    const betResult = betResults[bet.bet_result] || {};
    const isLive = bet?.bet_result === BET_OPEN && bet?.timer > 0;
    let timeLeft;
    if(isLive) {
        const minute = Math.floor(bet.timer / 60);
        timeLeft = `${minute}'`;
    }

    const startTime = useMemo(() => {
        if (bet?.start_at) {
            return moment.utc(bet.start_at).local().format('MMM D, YYYY HH:mm');
        }
        return null;
    }, [bet?.start_at])

    return (
        <div className={styles.betItem}>
            <div className={styles.header}>
                <div className={styles.left}>
                    <span className={styles.type}>Single</span>
                    {
                        !isLive
                        ?
                            <span className={styles.startTime}>{startTime}</span>
                        :
                        null
                    }
                    
                </div>
                <div className={styles.right}>
                    {/* <Button style={{ background: bet.reward_status === 2 ? '#F7C927' : betResult.background }} >{bet.reward_status === 2 ? 'CASHED OUT' : betResult.text}</Button> */}
                    {
                        isLive
                        ?
                        <>
                            <span>{timeLeft}</span>
                            <RadarIcon/>
                        </>
                        :
                        null
                    }
                    <Button style={{ background: betResult.background }} >{betResult.text}</Button>
                </div>
            </div>
            <div className={styles.betInfo}>
                <Flexbox className={styles.league} spacing="12px" align="center">
                    <Flexbox className={styles.item} spacing="12px" align="center">
                        <SportIcon sportType={bet.sport} />
                        International
                    </Flexbox>
                    <BreadcrumbDividerIcon className={styles.divider} />
                    <EllipsisText className={styles.item} text={bet.league} />
                </Flexbox>
                <Flexbox className={styles.teams} align="center">
                    <Flexbox spacing="4px" align="center">
                        <span className={styles.team}>{bet.team1}</span>
                        vs
                        <span className={styles.team}>{bet.team2}</span>
                    </Flexbox>
                </Flexbox>
                <Flexbox className={styles.odds} align="center" justify="space-between">
                    <Flexbox align="center">
                        <span>{converOddsNames(bet.market, bet.sport)} {bet.team1}</span>
                        <span className={styles.score}>{bet.score1} : {bet.score2}</span>
                    </Flexbox>
                    <span className={styles.odd}>{bet.odd}</span>
                </Flexbox>
            </div>
            <Flexbox justify="space-between">
                <span className={styles.ticket}>Ticket ID: {bet.ticket}</span>
                {
                    bet?.bet_tx_id
                    ?
                    <a 
                        className={styles.viewOnChain}
                        href={`${process.env.NEXT_PUBLIC_CADARNOSCAN_URL}transaction/${bet.bet_tx_id}`}
                        target="_blank"
                    >
                        View bet on chain <ExportIcon/>
                    </a>
                    :
                    null
                }
            </Flexbox>
            <Flexbox justify="space-between" className={styles.stake}>
                <span>Stake</span>
                <span><FormattedNumber value={bet.staked || 0} maximumFractionDigits="2" /> {bet.coin}</span>
            </Flexbox>

            {
                bet.bet_result === BET_WON
                    ?
                    <>
                        <Flexbox justify="space-between" className={styles.reward}>
                            <span>Protential Win</span>
                            <span><FormattedNumber value={(bet.staked || 0) * (bet.odd || 0)} maximumFractionDigits="2" /> {bet.coin}</span>
                        </Flexbox>
                        <Flexbox justify="space-between" className={styles.rewardStatus}>
                            <span>Reward status</span>
                            <Flexbox spacing="4px" align="center">
                                {rewardStatuses[bet.reward_status]}
                                {
                                    bet.reward_status === REWARD_STATUS_SUBMITTED
                                        ?
                                        <a
                                            className={styles.cadarnoscan}
                                            href={`${process.env.NEXT_PUBLIC_CADARNOSCAN_URL}transaction/${bet.reward_tx_id}`}
                                            target="_blank"
                                        >
                                            <img src="/images/cadarnoscan.png" alt="Cadarnoscan" />
                                        </a>
                                        :
                                        null
                                }

                            </Flexbox>
                        </Flexbox>
                    </>
                    :
                    null

            }


        </div>
    )
}

export default BetItem;