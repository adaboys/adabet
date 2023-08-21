import { useSelector, useDispatch } from 'react-redux';
import { betActions } from '@redux/actions';
import { MIN_ADA_BET } from '@constants';

const useBetting = (sportId) => {
    const dispatch = useDispatch();
    const { data } = useSelector(state => state.bet || {});
    const betData = data[sportId] || [];

    const addBet = (matcheItem, market, oddsItem, callback) => {
        let betExist = false;
        const newBetData = betData?.filter(item => {
            if (item.matchId === matcheItem.id && item.market === market && item.oddsItem.k === oddsItem.k) {
                betExist = true;
                return false;
            }
            return true;
        });

        if (!betExist) {
            newBetData.push({
                matchId: matcheItem.id,
                t1: matcheItem.t1,
                t2: matcheItem.t2,
                league: matcheItem.league,
                sport: matcheItem.sport,
                market,
                oddsItem,
                amount: MIN_ADA_BET
            });
            callback && callback();
        }
        dispatch(betActions.updateBet({
            ...data,
            [sportId]: newBetData
        }));
    }

    const removeBet = (betItem) => {
        const { matchId, market, oddsItem } = betItem || {};
        const newBetData = betData?.filter(item => !(item.matchId === matchId && item.market === market && item.oddsItem.k === oddsItem.k));
        dispatch(betActions.updateBet({
            ...data,
            [sportId]: newBetData
        }));
    }

    const removeAllBet = () => {
        delete data[sportId];
        dispatch(betActions.updateBet(data));
    }

    const updateBet = (betItem) => {
        const { matchId, market, oddsItem } = betItem || {};
        const newBetData = betData?.map(item => {
            if (item.matchId === matchId && item.market === market && item.oddsItem.k === oddsItem.k) {
                return betItem;
            }
            return item;
        });
        dispatch(betActions.updateBet({
            ...data,
            [sportId]: newBetData
        }));
    }

    const updateAmountsBet = (amount) => {
        if (betData?.length) {
            const newBetData = betData?.map(item => ({...item, amount}));
            dispatch(betActions.updateBet({
                ...data,
                [sportId]: newBetData
            }));
        }
    }

    const isOddsSelected = (matcheItem, market, oddsItem) => {
        return betData?.some(item => item.matchId === matcheItem.id && item.market === market && item.oddsItem.k === oddsItem.k);
    }

    return {
        betData,
        addBet,
        updateBet,
        updateAmountsBet,
        removeBet,
        removeAllBet,
        isOddsSelected
    }
}

export default useBetting;
