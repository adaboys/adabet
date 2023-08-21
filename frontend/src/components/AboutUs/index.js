import React from 'react';

import Tokenomics from '@components/Dashboard/Tokenomics';
import Roadmap from '@components/Dashboard/Roadmap';
import OurTeam from './OurTeam';
import Token from './Token';

import styles from './index.module.scss';

const AboutUs = () => {
    return (
        <div className={styles.aboutUsPage}>
            <div className="container">
                <div className={styles.aboutUs}>
                    <div className={styles.info}>
                        <h3>About ADABET</h3>
                        <p>ADABET is a sports betting platform that applies Cardano blockchain technology to build betting transactions.ADBET, in conjunction with the technology and features on WEB3 and Oracle, will establish the trend of owning digital assets and conducting transparent transactions on the most secure blockchain.ADABET's mission is to develop a completely new betting platform that is distinct from traditional ones in terms of identity security, safety, transparency, and ease of property ownership in the next technological era.
                            In addition, ADABET is an open platform, allowing investors, agents to participate in stake pools or provide content, deploy and collect rewards on the platform.</p>
                        <p>Best Platform for <span className={styles.hightlight}>Prediction</span></p>
                        <p><span className={styles.hightlight}>10000+</span> member in here</p>
                        <p>Easy prosses for crate account</p>
                        <p><span className={styles.hightlight}>100%</span> secure platform</p>
                    </div>
                    <div className={styles.banner}>
                        <img src="/images/about-us-bg.png" alt="" />
                    </div>
                </div>
                <Token />
                <div className={styles.operation}>
                    <h3 className={styles.title}>Operation of the ADABET platform</h3>
                    <ul>
                        <li>Off-chain transactions are more transparent, safe, and secure than traditional betting platforms, reducing the risk of fraud.</li>
                        <li>ADA cryptocurrency or tokens developed on the ADA platform can participate in betting within the platform.</li>
                        <li>Other tokens can be exchanged for GEM in the ADABET platform to play and use as a stable currency in betting transactions.</li>
                        <li>Before entering the trading session, players can stake tokens into staking pools and cancel when the time has not expired.</li>
                        <li>The ADABET platform enables quick deposits and withdrawals through secure onchain transactions while maintaining complete privacy.</li>
                        <li>Furthermore, bonus points are accumulated after each bet and converted into GEM tokens that can be exchanged for other cryptocurrencies in the system.</li>
                    </ul>
                </div>
                <Tokenomics />
                <OurTeam />
                <div className={styles.backer}>
                    <h3 className={styles.title}>Supported by</h3>
                    <div className={styles.list}>
                        <img src="/images/catalyst.png" alt="" />
                        <img src="/images/in-out.png" alt="" />
                    </div>
                </div>
                <Roadmap />
            </div>
        </div>
    );
};

export default AboutUs;