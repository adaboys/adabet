import React from 'react';

import styles from './AboutUs.module.scss';

import ArrowTailRight from '@assets/icons/arrow-tail-right.svg';
import Link from 'next/link';
import { paths } from '@constants';

const AboutUs = () => {
    return (
        <div className={styles.aboutUs}>
            <div className={styles.banner}>
                <img src="/images/about-us-bg.png" alt="" />
            </div>
            <div className={styles.info}>
                <h3>About ADABET</h3>
                <p>ADABET is a sports betting platform that applies Cardano blockchain technology to build betting transactions.ADBET, in conjunction with the technology and features on WEB3 and Oracle, will establish the trend of owning digital assets and conducting transparent transactions on the most secure blockchain.ADABET's mission is to develop a completely new betting platform that is distinct from traditional ones in terms of identity security, safety, transparency, and ease of property ownership in the next technological era.
                    In addition, ADABET is an open platform, allowing investors, agents to participate in stake pools or provide content, deploy and collect rewards on the platform.</p>
                <p>Best Platform for <span className={styles.hightlight}>Prediction</span></p>
                <p><span className={styles.hightlight}>10000+</span> member in here</p>
                <p>Easy prosses for crate account</p>
                <p><span className={styles.hightlight}>100%</span> secure platform</p>
                <Link href={paths.aboutUs}>
                    <button>
                        Read more
                        <ArrowTailRight />
                    </button>
                </Link>
            </div>
        </div>
    );
};

export default AboutUs;