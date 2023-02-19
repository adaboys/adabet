import { useState } from 'react';

import { Collapse } from 'react-collapse';
import classNames from 'classnames';

import ArrowDownIcon from '@assets/icons/arrow-down.svg';

import styles from './index.module.scss';


const CollapseTile = ({
    title,
    children,
    className,
    activeClassName,
    defaultOpen,
    onClick
}) => {
    const [isShow, setIsShow] = useState(defaultOpen);

    const handleClick = () => {
        setIsShow(!isShow);
        onClick && onClick();
    }

    const handleArrowClick = e => {
        e.stopPropagation();
        setIsShow(!isShow);
    }

    return (
        <div className={classNames(styles.collapseTile, { [className]: !! className, [activeClassName]: isShow && !!activeClassName })}>
            <a className={styles.title} onClick={handleClick}>
                {title}
                <ArrowDownIcon
                    className={classNames({ [styles.open]: isShow })}
                    onClick={handleArrowClick}
                />
            </a>
            <Collapse isOpened={isShow}>
                {children}
            </Collapse>
        </div>
    );
};

export default CollapseTile;