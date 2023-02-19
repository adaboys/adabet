import { MetaWrapper } from '@components/Common';
import WhatWeDo from './WhatWeDo';

import styles from './index.module.scss';

const Dashboard = () => {
    
    return (
        <MetaWrapper>
            <div className={styles.dashboard}>
                <div className="container">
                    <WhatWeDo/>
                </div>
            </div>
        </MetaWrapper>
    )
}

export default Dashboard;