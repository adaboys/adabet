import { MetaWrapper } from '@components/Common';
import WhatWeDo from './WhatWeDo';

import styles from './index.module.scss';
import Feature from './Feature';
import AboutUs from './AboutUs';
import StartPrediction from './StartPrediction';
import Upcoming from './Upcoming';
import Roadmap from './Roadmap';
import Tokenomics from './Tokenomics';

const Dashboard = () => {

    return (
        <MetaWrapper>
            <div className={styles.dashboard}>
                <div className="container">
                    <WhatWeDo/>
                    <Feature />
                    <AboutUs />
                    <StartPrediction />
                    <Upcoming />
                    <Tokenomics />
                    <Roadmap />
                </div>
            </div>
        </MetaWrapper>
    )
}

export default Dashboard;