import React from 'react';

import IconCheck from '@assets/icons/check.svg';

import styles from './Roadmap.module.scss';
import classNames from 'classnames';

const Stage = ({ stage, year, tasks = [] }) => {
    return (
        <div className={styles.stage}>
            <div className={styles.title}>
                <div className={styles.dot} />
                <div className={styles.stageCount}>Stage {stage}</div>
                <div className={styles.separate}>/</div>
                <div className={styles.year}>{year}</div>
            </div>
            <div className={styles.tasks}>
                {tasks.map((el, index) => (
                    <div className={classNames(styles.item, el.done && styles.done)} key={index}>
                        <div className={styles.check}><IconCheck /></div>
                        {el.name}
                    </div>
                ))}
            </div>
        </div>
    );
}

const data = [
    {
        year: 2023,
        stage: 1,
        tasks: [
            { name: 'Initialization of the project and the first ideas for the blockchain betting platform', done: true  },
            { name: 'Successfully raised capital on Catalyst from the Cardano Foundation', done: true  },
            { name: 'Construction and design of technology infrastructure system', done: true },
            { name: `ADABET . UI design- Establish project's community and information exchange channels`, done: true },
            { name: `Design software platform ADABET and Database`, done: true },
            { name: `Litepaper and docs`, done: true },
        ]
    },
    {
        year: 2023,
        stage: 2,
        tasks: [
            { name: `Create a Db-sync-Cardano blockchain node`, done: true },
            { name: `Create APIs, White label for betting`, done: true },
            { name: `Validate API operations using Cardano blockchain metadata`, done: true },
            { name: `Create a cryptocurrency payment method, namely the first ADA token`, done: true },
            { name: `Plan community activities by organizing events`, done: true },
            { name: `Create tokenomics and an ecosystem of points, coins, and tokens for use in the ADABET system`, done: true },
        ]
    },
    {
        year: 2023,
        stage: 3,
        tasks: [
            { name: `Whitepaper and docs`, done: true },
            { name: `Implement betting on ADABET . platform`, done: true },
            { name: `Build GEM and ABE tokens for the operation of the ADABET platform`, done: true},
            { name: `Test-net on the Cardano blockchain for betting transactions`, done: true},
            { name: `Create content and bet forms for players on the platform`, done: true},
            { name: `Airdrop tokens and ISPO to the community of participants`, done: true},
        ]
    },
    {
        year: 2023,
        stage: 4,
        tasks: [
            { name: `Token sales to individuals and organizations (seed, private sale)`},
            { name: `Player feature development`},
            { name: `Deployment and development of the main-testing system`},
            { name: `Expansion to cryptocurrencies other than Cardano`},
            { name: `Listing dcex cryptocurrency for trading`},
        ]
    },
    {
        year: 2024,
        stage: 1,
        tasks: [
            { name: `Development of casino game content`},
            { name: `Develop of a peer-to-peer (P2P) feature for investors or players on participating platforms`},
            { name: `Develop an affiliate feature`},
            { name: `Create a reward system for users based on their ranks`},
        ]
    },
]

const Roadmap = () => {
    return (
        <div className={styles.roadmap}>
            <h3 className={styles.title}>Roadmap</h3>
            <div className={styles.list}>
                {data.map((item, index) => (
                    <Stage
                        key={index}
                        count={index + 1}
                        {...item}
                    />
                ))}
            </div>
        </div>
    );
};

export default Roadmap;