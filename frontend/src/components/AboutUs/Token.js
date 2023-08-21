import React from 'react';

import { formatNumber } from '@utils';

import CopyWrapper from '@components/Common/CopyWrapper';

import IconCopy from '@assets/icons/copy.svg';

import styles from './Token.module.scss';

const tokens = [
    {
        name: 'ABE',
        image: '/images/tokens/ABE.png',
        desc: `ABE is a token that represents the ADABET platform's share of value for investors and is tradable on the external market.`,
        decimals: 6,
        totalSupply: 1000000000,
        policyId: 'acc37b7ef2b3bb7855a7ba66ad3a242771092463e184d26f63217208',
        fingerprint: 'asset1rjemq922zhpnwsm3wf683dfzevr9khxrhskem2',
        assetName: 'ABE (414245)',
    },
    {
        name: 'GEM',
        image: '/images/tokens/GEM.png',
        desc: `GEM for game play in the platform.`,
        decimals: 6,
        totalSupply: 1000000000,
        policyId: '4d0f8074126d4adf93bdcf13c88ea25e25eb002d681e4d0d9e554f12',
        fingerprint: 'asset1hvpxpvdch7fjphcvuzl896uvawc7652u35tzq7',
        assetName: 'GEM (47454d)',
    },
]

const Token = () => {
    return (
        <div className={styles.token}>
            <h3 className={styles.title}>Token</h3>
            <div className={styles.desc}>There are 2 types of token in ADABET:</div>
            <div className={styles.shorts}>
                {tokens.map((token, index) => (
                    <div className={styles.item}>
                        <img src={token.image} alt="" />
                        <h4 className={styles.name}>{token.name}</h4>
                        <div className={styles.desc}>{token.desc}</div>
                    </div>
                ))}
            </div>
            <div className={styles.details}>
                {tokens.map((token, index) => (
                    <div className={styles.item}>
                        <div className={styles.head}>
                            <img src={token.image} alt="" />
                            <span>{token.name}</span>
                        </div>
                        <div className={styles.content}>
                            <div className={styles.row}>
                                <div className={styles.info}>
                                    <div className={styles.name}>Adabet betting platform token</div>
                                    <div className={styles.desc}>A currency for Adabet betting platform</div>
                                    <a href="https://adabet.io">https://adabet.io</a>
                                </div>
                            </div>
                            <div className={styles.row}>
                                <div className={styles.label}>Total Supply</div>
                                <div className={styles.infoSecond}>{token.decimals}</div>
                            </div>
                            <div className={styles.row}>
                                <div className={styles.label}>Total Supply</div>
                                <div>{formatNumber(token.totalSupply)}</div>
                            </div>
                            <div className={styles.row}>
                                <div className={styles.label}>Policy Id</div>
                                <div className={styles.copyAble}>
                                    <div className={styles.copyValue}>{token.policyId}</div>
                                    <CopyWrapper text={token.policyId}><IconCopy /></CopyWrapper>
                                </div>
                            </div>
                            <div className={styles.row}>
                                <div className={styles.label}>Fingerprint</div>
                                <div className={styles.copyAble}>
                                    <div className={styles.copyValue}>{token.fingerprint}</div>
                                    <CopyWrapper text={token.fingerprint}><IconCopy /></CopyWrapper>
                                </div>
                            </div>
                            <div className={styles.row}>
                                <div className={styles.label}>Asset Name</div>
                                <div className={styles.infoSecond}>{token.assetName}</div>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default Token;