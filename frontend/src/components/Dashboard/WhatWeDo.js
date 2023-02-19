import classNames from 'classnames';

import RightArrowIcon from '@assets/icons/right-arrow.svg';

import styles from './WhatWeDo.module.scss';

const WhatWeDo = () => {
    return (
        <div className={styles.whatWeDo}>
            <div className="col-6">
                <h3 className={styles.description}>
                    Attention! We Have Restricted Sports Betting In Some Countrie. And The Participating Age Will Be Verified By Our KYC At The Age Of 20+. Many Thanks For The Cooperation On The Innovation Platform Of ADABET.IO
                </h3>
                <div className={styles.highlight}>
                    <ul className={styles.menu}>
                        <li className={classNames(styles.item, styles.active)}>
                            <RightArrowIcon/>
                            Rugby
                        </li>
                        <li className={styles.item}>
                            <RightArrowIcon/>
                            Football
                        </li>
                        <li className={styles.item}>
                            <RightArrowIcon/>
                            Baseball
                        </li>
                        <li className={styles.item}>
                            <RightArrowIcon/>
                            Tennis
                        </li>
                        <li className={styles.item}>
                            <RightArrowIcon/>
                            Golf
                        </li>
                        <li className={styles.item}>
                            <RightArrowIcon/>
                            Racing
                        </li>
                    </ul>
                    <img className={styles.video} src="/images/video-thumbnail.png" alt="Highlight video"/>
                </div>
            </div>
            <img className={styles.sportBanner} src="/images/sport-banner.png" alt="Sport Game"/>
        </div>
    )
}

export default WhatWeDo;