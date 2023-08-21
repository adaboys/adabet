import { useEffect } from 'react';
import classNames from 'classnames';
import { useDispatch, useSelector } from 'react-redux';
import { NumberParam, useQueryParams } from 'use-query-params';
import { useRouter } from 'next/router';

import { CollapseTile } from '@components/Common';
import { sportActions } from '@redux/actions';
import { sportIcons } from '@constants/icons';
import { generateSportUrl } from '@utils';

import SortIcon from '@assets/icons/sort.svg';

import styles from './SideLeftContent.module.scss';

const SideLeftContent = () => {
    const dispatch = useDispatch();
    const [queries, setQueries] = useQueryParams({ id: NumberParam });
    const { push, query } = useRouter();
    const sports = useSelector(state => state.sport.sports) || [];

    useEffect(() => {
        dispatch(sportActions.getSportList());
    }, [])

    useEffect(() => {
        if(sports?.length && !queries.id) {
            setQueries({ id: sports[0].id});
        }
    }, [sports])
   
    return (
        <div className={styles.sideLeftContent}>
            <div className={styles.title}>
                <h3>Sports</h3>
                <SortIcon/>
            </div>
            <div className={styles.menus}>
                {
                    sports.map(sport => (
                        <CollapseTile
                            key={sport.id}
                            title={
                                <div className={styles.groupName}>
                                    <span className={styles.name}>
                                        {sportIcons[sport.name]}
                                        {sport.name}
                                    </span>
                                    <span className={styles.count}>{sport?.country_count}</span>
                                </div>}
                            className={classNames(styles.item, {[styles.active]: sport.id === queries.id})}
                            onClick={() => push(generateSportUrl({...query, id: sport.id}))}
                        >

                        </CollapseTile>
                    ))
                }
            </div>
        </div>
    )
}

export default SideLeftContent;
