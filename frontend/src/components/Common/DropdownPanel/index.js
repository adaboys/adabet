import classNames from 'classnames';

import { useOutsideClick } from '@/hooks';

import styles from './index.module.scss';

const DropdownPanel = ({ className, activeRef, children, onClickOutside }) => {
    const ref = useOutsideClick(onClickOutside, activeRef);

    return (
        <div className={classNames(styles.dropdownPanel, className)} ref={ref}>
            {children}
        </div>
    )
}

export default DropdownPanel;