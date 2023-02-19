import { useEffect, useRef, useState } from "react";
import classNames from "classnames";
import Link from "next/link";

import { useOutsideClick } from "@hooks";

import styles from './index.module.scss';

const DropdownMenu = ({ menuList }) => {
    const [isShow, setIsShow] = useState(false);
    const refMenu = useRef(null);
    const selectedMenu = menuList.find(menu => menu.isActive);
   
    const onCloseModal = () => {
        setIsShow(false);
    }
    const ref = useOutsideClick(onCloseModal);

    useEffect(() => {
        if (isShow) {
            refMenu.current.style.display = "flex";
        } else {
            refMenu.current.style.display = "none";
        }
    }, [isShow])

    return (
        <div className={styles.dropdownMenu} ref={ref}>
            {
                selectedMenu
                ?
                <div className={styles.selectedMenu} onClick={() => setIsShow(!isShow)}>
                    {selectedMenu.name}
                </div>
                :
                null
            }
            <div className={styles.dropdown} ref={refMenu}>
                {
                    menuList.map(menu => (
                        menu.url
                        ?
                        <Link
                            key={menu.url}
                            href={menu.url}
                        >
                            <a className={classNames(styles.menuItem, {[styles.active]: menu.isActive})}>{menu.name}</a>
                        </Link>
                        :
                        <a onClick={menu.onClick || null} className={classNames(styles.menuItem, {[styles.active]: menu.isActive})}>{menu.name}</a>
                    ))
                }
            </div>
        </div>
    );
}

export default DropdownMenu;