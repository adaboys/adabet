

import { Flexbox } from '@components/Common';
import SportIcon from '@components/Sport/SportLayout/SportIcon';

import { MIN_ADA_BET } from '@constants';
import { converOddsNames } from '@utils'

import CloseIcon from '@assets/icons/close.svg';

import styles from './OddsItem.module.scss';

const regexp = /^\d*(\.\d{0,2})?$/;

const OddsItem = ({ betItem, balanceInfo, removeBet, updateBet }) => {

    const onChangeAmount = (evt) => {
        const newValue = evt.target.value;
        if (regexp.test(newValue)) {
            updateBet({
                ...betItem,
                amount: newValue
            })
        }
    }

    const onBlurAmount = (evt) => {
        const newValue = evt.target.value;
        if(!newValue || newValue < MIN_ADA_BET) {
            updateBet({
                ...betItem,
                amount: MIN_ADA_BET
            })
        }
    }
    const oddName = betItem?.oddsItem.k.replace('x', '');
    return (
        <Flexbox className={styles.oddsItem} direction="column">
            <div className={styles.league}>
                <button onClick={() => removeBet(betItem)} className={styles.close}><CloseIcon /></button>
                <Flexbox
                    className={styles.name}
                    spacing="6px"
                    align="center"
                >
                    <SportIcon sportType={betItem.sport} />
                    {oddName === '1' ? betItem.t1 : oddName === '2' ? betItem.t2 : 'Draw'}
                </Flexbox>
                <div className={styles.events}>
                    <p>{betItem.t1} vs {betItem.t2}</p>
                    <p>{converOddsNames(betItem.market, betItem.sport)}</p>
                </div>
            </div>
            <Flexbox
                spacing="12px"
                align="center"
                justify="space-between"
                className={styles.currentStatus}
            >
                <span className={styles.oodsValue}>{betItem?.oddsItem?.v}</span>
                <div className={styles.amount}>
                    <input
                        onChange={onChangeAmount}
                        onBlur={onBlurAmount}
                        value={betItem?.amount}
                    />
                    <span>{balanceInfo?.name}</span>
                </div>
            </Flexbox>
        </Flexbox>
    )
}

export default OddsItem;