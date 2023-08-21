import MatchesItem from './MatchesItem';

import styles from './index.module.scss';

const MatchesList = ({ matches, onToggleMatch, onShowhistory, addBet, isOddsSelected }) => {
    return (
        <div className={styles.matchesList}>
            {
                matches.map(matchesItem => (
                    <MatchesItem
                        key={matchesItem.id}
                        matchesItem={matchesItem}
                        addBet={addBet}
                        isOddsSelected={isOddsSelected}
                        onToggleMatch={onToggleMatch}
                        onShowhistory={onShowhistory}
                    />
                ))
            }
        </div>
    )
}

export default MatchesList;
