import { useEffect } from 'react';
import classNames from 'classnames';
import { useDispatch, useSelector } from 'react-redux';
import { NumberParam, useQueryParams } from 'use-query-params';
import { useRouter } from 'next/router';

import { Flexbox } from '@components/Common';
import { generateSportUrl } from '@utils';

import { sportIcons } from '@constants/icons';

import styles from './FilterTags.module.scss';

const FilterTags = () => {

    const [queries, setQueries] = useQueryParams({ id: NumberParam });
    const { push, query } = useRouter();
    const sports = useSelector(state => state.sport.sports || []);

    return (
        <Flexbox className={styles.filterTags} spacing="20px">
            {/* {
                Object.keys(sportIcons).map(name => (
                    <Flexbox
                        className={classNames(styles.item, {[styles.active]: name === 'Football' || name === 'Soccer'})}
                        align="center"
                        spacing="5px"
                        key={name}
                    >
                        {sportIcons[name]}
                        {name}
                    </Flexbox>
                ))
            } */}
            {
                sports.map(sport => (
                    <Flexbox
                        className={classNames(styles.item, {[styles.active]: sport.id === queries.id})}
                        align="center"
                        spacing="5px"
                        key={sport.id}
                        onClick={() => push(generateSportUrl({...query, id: sport.id}))}
                    >
                        {sportIcons[sport.name]}
                        {sport.name}
                    </Flexbox>
                ))
            }
        </Flexbox>
    )
}

export default FilterTags;