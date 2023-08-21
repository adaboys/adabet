import React from 'react';

import styles from './OurTeam.module.scss';

import UserIcon from '@assets/icons/user.svg';

const groups = [
    {
        name: 'Technical',
        members: [
            {
                image: '',
                name: 'ADABoys LAB',
                desc: 'Lorem ipsum dolor sit amet, consectetur adipisci elit, sed eiusmod tempor incidunt ut labore et dolore magna aliqua.'
            },
            {
                image: '',
                name: 'Alex',
                desc: 'Ut enim ad minim veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam.'
            },
        ]
    },
    {
        name: 'Advisor',
        members: [
            {
                image: '',
                name: 'Gonzalo',
                desc: 'Voluptate velit esse cillum dolore eu fugiat nulla pariatur. '
            },
            {
                image: '',
                name: 'Mie',
                desc: 'Nisi ut aliquid ex ea commodi consequatur. Quis aute iure reprehenderit in.'
            },
        ]
    },
    {
        name: 'Marketing',
        members: [
            {
                image: '',
                name: 'Vicenzo',
                desc: 'Excepteur sint obcaecat cupiditat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.'
            },
            {
                image: '',
                name: 'Ly',
                desc: 'Sint obcaecat cupiditat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.'
            },
        ]
    },
]

const OurTeam = () => {
    return (
        <div className={styles.ourTeam}>
            <h3 className={styles.title}>Our team</h3>
            <div className={styles.groups}>
                {groups.map((g, index) => (
                    <div className={styles.item} key={index}>
                        <h4 className={styles.groupName}>{g.name}</h4>
                        <div className={styles.members}>
                            {g.members.map((mem, indexMem) => (
                                <div className={styles.member} key={indexMem}>
                                    <div className={styles.img}>
                                        {/* <img src="" alt="" /> */}
                                        <UserIcon />
                                    </div>
                                    <div className={styles.info}>
                                        <div className={styles.name}>{mem.name}</div>
                                        <div className={styles.desc}>{mem.desc}</div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default OurTeam;