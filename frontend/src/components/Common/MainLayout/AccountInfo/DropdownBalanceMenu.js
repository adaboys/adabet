
import { FormattedNumber } from 'react-intl';
import { DropdownPanel } from '@components/Common';

import { CURRENCY_ADA } from '@constants';

import AdaIcon from '@assets/icons/ada.svg';

import styles from './DropdownBalanceMenu.module.scss'
import classNames from 'classnames';


const DropdownBalanceMenu = ({ activeRef, balances, currency, onClose, onChangeCurrency }) => {
    return (
        <DropdownPanel activeRef={activeRef} onClickOutside={onClose} className={styles.dropdownBalanceMenu}>
            <div className={styles.menuList}>
                {
                    balances.map(balanceItem => (


                        <a
                            key={balanceItem.id}
                            className={classNames(styles.menuItem, {[styles.active]: balanceItem.id === currency})}
                            onClick={() => onChangeCurrency(balanceItem.id)}
                        >
                            {
                                balanceItem.id === CURRENCY_ADA
                                    ?
                                    <span className={styles.currency}>
                                        <AdaIcon /> ADA
                                    </span>
                                    :
                                    <span className={styles.currency}>
                                        <img src={`/images/tokens/${balanceItem.name}.png`} /> {balanceItem.name}
                                    </span>
                            }

                            <FormattedNumber value={balanceItem?.amount || 0} maximumFractionDigits="2" />
                        </a>

                    ))
                }
            </div>
        </DropdownPanel>
    )
}

export default DropdownBalanceMenu;