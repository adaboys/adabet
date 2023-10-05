import { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import classNames from 'classnames';
import { NumberParam, useQueryParams } from 'use-query-params';

import { BasicSlider, EllipsisText, Flexbox } from '@components/Common';
import TeamImage from '@components/Sport/SportLayout/TeamImage';
import SportIcon from '@components/Sport/SportLayout/SportIcon';

import { sportActions } from '@redux/actions';
import { ssrMode } from '@constants';
import { getStartTime, converOddsNames } from '@utils';

import BreadcrumbDividerIcon from '@assets/icons/breadcrumb-divider.svg';

import styles from './index.module.scss';

const Highlight = ({ addBet, isOddsSelected }) => {

    const dispatch = useDispatch();
    const [queries] = useQueryParams({ id: NumberParam });
    const matches = useSelector(state => state.sport?.matchesHighlight || []);

    useEffect(() => {
        if (queries.id) {
            dispatch(sportActions.getMatchesHighlight({ id: queries.id }));
        }
    }, [queries])

    if (!matches?.length)
        return null;

    const sliderCount = ssrMode ? 3 : (document.body.clientWidth - (330 + 28)) / (340 + 16);
    return (
        <div className={styles.highlight}>

            <BasicSlider
                draggable={false}
                desktopItems={sliderCount}
                tabletItems={2}
                containerClass={styles.matchesSlider}
            // partialVisible
            >
                {
                    matches.map(item => {
                        const startTime = getStartTime(item.start_at);
                        return (
                            <div className={styles.matchBox} key={item.id}>
                                <Flexbox className={styles.breadcrumb} spacing="5px" align="center">
                                    <span className={styles.item}>
                                        <SportIcon sportType={item.sport} />
                                        International
                                    </span>
                                    <BreadcrumbDividerIcon style={{ width: '7px', height: '5px' }} />
                                    <EllipsisText className={styles.lastItem} text={item.league} />
                                </Flexbox>
                                <Flexbox
                                    className={styles.teams}
                                    justify="space-between"
                                    spacing="8px"
                                >
                                    <Flexbox className={styles.teamItem} direction="column" spacing="8px">
                                        <TeamImage id={item.img1} sportType={item.sport}/>
                                        <EllipsisText className={styles.name} text={item.t1} maxLine={2} />
                                    </Flexbox>
                                    <Flexbox className={styles.startTime} direction="column">
                                        <span className={styles.date}>
                                            {startTime.date}
                                        </span>
                                        <span className={styles.hour}>
                                            {startTime.time}
                                        </span>
                                    </Flexbox>
                                    <Flexbox
                                        className={styles.teamItem}
                                        direction="column"
                                        align="flex-end"
                                        spacing="8px"
                                    >
                                        <TeamImage id={item.img2} sportType={item.sport}/>
                                        <EllipsisText className={classNames(styles.name, styles.right)} text={item.t2} maxLine={2} />

                                    </Flexbox>
                                </Flexbox>
                                <Flexbox
                                    className={styles.odds}
                                    justify="space-between"
                                    spacing="6px"
                                >
                                    {
                                        item?.market?.odds?.length
                                            ?
                                            item.market.odds.map((odds, index) => (
                                                <Flexbox
                                                    key={index}
                                                    className={classNames(styles.oddsItem, {[styles.selected]: isOddsSelected(item, item?.market.name, odds)})}
                                                    justify="space-between"
                                                    style={{ ...(odds.s && { opacity: '0.5' })}}
                                                    disabled={odds.s}
                                                    onClick={() => addBet(item, item?.market.name, odds)}
                                                >
                                                    <span className={styles.label}>{converOddsNames(odds.k, item.sport)}</span>
                                                    <span className={styles.value}>{odds.v || '-'}</span>
                                                </Flexbox>
                                            ))
                                            :
                                            null
                                    }
                                </Flexbox>
                            </div>
                        )
                    })
                }
            </BasicSlider>
        </div>
    )
}

export default Highlight;