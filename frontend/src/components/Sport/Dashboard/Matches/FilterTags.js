import { Flexbox } from '@components/Common';
import { sportIcons } from '@constants/icons';
import classNames from 'classnames';
import styles from './FilterTags.module.scss';

const FilterTags = () => {
    return (
        <Flexbox className={styles.filterTags} spacing="20px">
            {
                Object.keys(sportIcons).map(name => (
                    <Flexbox
                        className={classNames(styles.item, {[styles.active]: name === 'Football'})}
                        align="center"
                        spacing="5px"
                    >
                        {sportIcons[name]}
                        {name}
                    </Flexbox>
                ))
            }
        </Flexbox>
    )
}

export default FilterTags;