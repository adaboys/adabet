import { useContext, useState, useMemo } from 'react';
import classNames from 'classnames';
import { useDispatch, useSelector } from 'react-redux';
import { FormattedNumber } from 'react-intl';

import { Button, Flexbox } from '@components/Common';
import OddsItem from './OddsItem';

import { OverlayContext } from '@hocs';
import { useAuth, useNotification } from '@hooks';
import { overlayTypes, CURRENCY_ADA } from '@constants';
import { betActions } from '@redux/actions';

import ShareIcon from '@assets/icons/share.svg';

import styles from './Single.module.scss';

const suggestionAmountsBest = [2, 10, 20, 50, 100]

const Single = ({ sportId, betData, removeBet, updateBet, updateAmountsBet, removeAllBet }) => {
    const [betValueSelected, setBetValueSelected] = useState();
    const overlay = useContext(OverlayContext);
    const dispatch = useDispatch();
    const [isSubmitting, setIsSubmitting] = useState();
    const { isAuthenticated, currency } = useAuth();
    const { showError, showSuccess } = useNotification();
    const { balances } = useSelector(state => state.account);

    const balanceInfo = useMemo(() => {
        if (balances?.length > 0) {
            return balances.find(item => item.id === currency)
        }
        return null
    }, [balances, currency]);

    const onShowLogin = () => {
        overlay.show(overlayTypes.LOGIN);
    }

    const onShowRegister = () => {
        overlay.show(overlayTypes.REGISTER);
    }

    const setAmountsBet = amount => {
        setBetValueSelected(amount);
        updateAmountsBet(amount);
    }

    const placeBet = () => {
        if(sportId && betData?.length) {
            const bets = betData.reduce(
                (result, item) => {
                    const oddsItem = {
                        name: item.oddsItem.k,
                        value: item.oddsItem.v,
                        bet_amount: item.amount
                    }
                    const betExists = result.findIndex(bet => bet.match_id === item.matchId);
                    if(betExists > -1) {
                        const marketExists = result[betExists].markets.findIndex(market => market.name === item.market);
                        if(marketExists > -1) {
                            result[betExists].markets[marketExists].odds.push(oddsItem)
                        }
                        else {
                            result[betExists].markets.push({
                                name: item.market,
                                odds: [oddsItem]
                            })
                        }
                        return result;
                    }
                    else {
                        return [
                            ...result,
                            {
                                match_id: item.matchId,
                                markets: [{
                                    name: item.market,
                                    odds: [oddsItem]
                                }]
                            }
                        ]
                    }
                },
                [],
            );
            setIsSubmitting(true);
            dispatch(betActions.placeBet({
                sportId,
                params: {
                    bets,
                    bet_currency: currency || CURRENCY_ADA
                },
                onCompleted: response => {
                    setIsSubmitting(false);
                    if(response?.status === 200) {
                        removeAllBet();
                        showSuccess('Place successful!');
                    }
                    else if(response?.code === 'balance_not_enough') {
                        showError('Balance not enough!');
                    }
                    else {
                        showError('Place failed!');
                    }
                },
                onError: err => {
                    console.log(err);
                    setIsSubmitting(false);
                    showError('Place failed!');
                }
            }));
        }
    }

    const totalAmount = useMemo(() => {
        if (betData) {
            return betData.reduce((total, betItem) => {
                if (betItem?.amount) {
                    return total + parseFloat(betItem.amount);
                }
                return total;
            }, 0);
        }
        return 0;
    }, [betData])

    const totalPotential = useMemo(() => {
        if (betData) {
            return betData.reduce((total, betItem) => {
                if (betItem?.amount && betItem?.oddsItem) {
                    return total + parseFloat(betItem.amount)*parseFloat(betItem.oddsItem.v);
                }
                return total;
            }, 0);
        }
        return 0;
    }, [betData])

    return (
        <div className={styles.single}>
            {
                betData?.map((betItem, index) => <OddsItem key={index} betItem={betItem} removeBet={removeBet} updateBet={updateBet} balanceInfo={balanceInfo}/>)
            }
            <Flexbox
                className={styles.amount}
                spacing="12px"
                align="center"
            >
                {
                    suggestionAmountsBest.map(amount =>
                        <div key={amount} onClick={() => setAmountsBet(amount)} className={classNames(styles.betValue, betValueSelected === amount && styles.active)}>{amount}</div>
                    )
                }
            </Flexbox>

            <div className={styles.summary}>
                <Flexbox justify="space-between" className={styles.total}>
                    <span>Total bet</span>
                    <span>{totalAmount} {balanceInfo?.name}</span>
                </Flexbox>
                <Flexbox justify="space-between" className={styles.potential}>
                    <span>POTENTIAL WIN</span>
                    <span><FormattedNumber value={totalPotential} maximumFractionDigits="2"/> {balanceInfo?.name}</span>
                </Flexbox>
                <Flexbox justify="space-between" className={styles.actions}>
                    <button onClick={removeAllBet}>Clear all</button>
                    <button>
                        <Flexbox align="center" spacing="3px">
                            share bet <ShareIcon width={12} height={12} />
                        </Flexbox>
                    </button>
                </Flexbox>
            </div>
            <div className={styles.footer}>
                {
                    isAuthenticated
                        ?
                        <Button onClick={placeBet} loading={isSubmitting}>Place Bet</Button>
                        :
                        <>
                            <Button onClick={onShowLogin}>Log in</Button>
                            <div className={styles.dontHaveAccount}>
                                Don't you have an account? <a href="#" onClick={onShowRegister}>Join Now!</a>
                            </div>
                        </>
                }
            </div>
        </div>
    )
}

export default Single;
