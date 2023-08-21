import classNames from 'classnames';

import RightArrowIcon from '@assets/icons/right-arrow.svg';

import styles from './WhatWeDo.module.scss';

const WhatWeDo = () => {
    return (
        <div className={styles.whatWeDo}>
            <div className="row">
                <div className="col-6">
                    <h3 className={styles.description}>
                        Attention! We have restricted sports betting in some countries. And the participating age will be verified by our KYC at the age of 20+. Many thanks for the cooperation on the innovation platform of ADABET.IO
                    </h3>
                    <div className={styles.highlight}>
                        <ul className={styles.menu}>
                            <li className={classNames(styles.item, styles.active)}>
                                <RightArrowIcon/>
                                Rugby
                            </li>
                            <li className={styles.item}>
                                <RightArrowIcon/>
                                Soccer
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
                <div className="col-6">
                    <img className={styles.sportBanner} src="/images/main-banner.png" alt="Sport Game"/>
                </div>
            </div>
        </div>
    )
}

export default WhatWeDo;