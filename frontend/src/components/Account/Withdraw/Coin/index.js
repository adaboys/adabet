import { coinTypes } from '@constants';
import AdaIcon from '@assets/icons/ada.svg';

import styles from './index.module.scss';
import classNames from 'classnames';

const Coin = ({
    type = coinTypes.ISKY,
    style = {},
    className
}) => {
    return (
        <span className={classNames(styles.coin, {[className]: !!className})} style={style}>
            {
                type === coinTypes.ADA
                    ?
                    <><AdaIcon style={{ width: '20px', height: '20px'}} /> ADA</>
                    :
                    type === coinTypes.GEM
                    ?
                    <><img src="/images/tokens/GEM.png" alt="GEM" /> GEM</>
                    :
                    <><img src="/images/tokens/ABE.png" alt="ABE" /> ABE</>
                    
            }
        </span>
    )
}

export default Coin;
