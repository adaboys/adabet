import React from 'react';

import DividerIcon from '@assets/icons/breadcrumb-divider.svg';

import styles from './index.module.scss';
import classNames from 'classnames';

const PaginationNextPrev = ({
    currentPage,
    onPrev,
    onNext,
    totalPage,
    label = 'Page',
    className,
    ...rest
}) => {
    const disablePrev = rest.disablePrev ?? currentPage <= 1;
    const disableNext = rest.disableNext ?? currentPage >= totalPage;

    return (
        <div className={classNames(styles.paginationContainer, className)}>
            {!!totalPage && <div className={styles.total}>Total {totalPage} page</div>}
            <div className={styles.paginationNextPrev}>
                <button
                    disabled={disablePrev}
                    onClick={() => !disablePrev && onPrev?.()}
                >
                    <DividerIcon />
                </button>
                <span className={styles.current}>{label} <b>{currentPage}</b></span>
                <button
                    disabled={disableNext}
                    onClick={() => !disableNext && onNext?.()}
                >
                    <DividerIcon />
                </button>
            </div>
        </div>
    );
};

export default PaginationNextPrev;