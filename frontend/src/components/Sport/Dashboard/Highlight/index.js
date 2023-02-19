import { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import moment from 'moment';
import classNames from 'classnames';
import { NumberParam, useQueryParams } from 'use-query-params';

import { BasicSlider, EllipsisText, Flexbox } from '@components/Common';
import { sportActions } from '@redux/actions';
import { ssrMode } from '@constants';
import { getStartTime } from '@utils';

import FootballIcon from '@assets/icons/football.svg';
import BreadcrumbDividerIcon from '@assets/icons/breadcrumb-divider.svg';

import styles from './index.module.scss';

const Highlight = () => {

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

    const sliderCount = ssrMode ? 3 : (document.body.clientWidth - 330) / 374;
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
                                        <FootballIcon />
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
                                        <img src="/images/flags/france.png" />
                                        <EllipsisText className={styles.name} text={item.team1} maxLine={2} />
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
                                        <img src="/images/flags/england.png" />
                                        <EllipsisText className={classNames(styles.name, styles.right)} text={item.team2} maxLine={2} />

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
                                            item.market.odds.map(odds => (
                                                <Flexbox className={styles.oddsItem} justify="space-between">
                                                    <span className={styles.label}>{odds.k.replace('w', '')}</span>
                                                    <span className={styles.value}>{odds.v}</span>
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