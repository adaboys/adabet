
import { FormattedMessage } from "react-intl";
import { useEffect } from "react";
import { useRouter } from "next/router";

import { BasicModal } from "../Modal";
import Button from "../Button";
import MainMenu from "./MainMenu";
// import GroupLanguage from "./GroupLanguage";

import { commonMessages } from "@constants/intl";
import { useAuth, useDevices } from "@hooks";

import CloseIcon from '@assets/icons/close.svg';
import AvatarIcon from '@assets/icons/avatar.svg';

import styles from "./MobileSideBarMenu.module.scss";

const MobileSideBarMenu = ({
    onClose,
    isShow,
    onShowLogin,
    onShowRegister
}) => {

    const { pathname } = useRouter();
    const { isDesktop } = useDevices();
    const { isAuthenticated, user, logout } = useAuth();

    useEffect(() => {
        if (isShow) {
            onClose();
        }
    }, [isDesktop, pathname]);

    return (
        <BasicModal
            isOpen={isShow}
            onRequestClose={onClose}
            overlayClassName={styles.mobileSidebarMenu}
            contentClassName={styles.wrapper}
        >
            <div className={styles.header}>
                <div className={styles.top}>
                    {
                        isAuthenticated
                            ?
                            <div className={styles.logged}>
                                <AvatarIcon />
                                <div className={styles.username}>{user?.wallet_address}</div>
                            </div>
                            :
                            <div></div>
                    }
                    <CloseIcon onClick={onClose} />
                </div>
                <div className={styles.bottom}>
                    {
                        isAuthenticated
                            ?
                            <a
                                onClick={() => {
                                    onClose();
                                    logout();
                                }}
                                className={styles.signOut}
                            >
                                <FormattedMessage {...commonMessages.signOut} />
                            </a>
                            :
                            <>
                                <a onClick={() => {
                                    onClose();
                                    onShowRegister();
                                }}>
                                    <FormattedMessage {...commonMessages.signUp} />
                                </a>
                                <Button onClick={() => {
                                    onClose();
                                    onShowLogin();
                                }}>
                                    <FormattedMessage {...commonMessages.login} />
                                </Button>
                            </>
                    }

                </div>
            </div>
            <div className={styles.content}>
                <MainMenu />
                {/* <GroupLanguage /> */}
            </div>
        </BasicModal>
    )
};

export default MobileSideBarMenu;

