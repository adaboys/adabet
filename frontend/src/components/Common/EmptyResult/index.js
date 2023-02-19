import classNames from "classnames";
import React from "react";
import { useIntl } from "react-intl";

import { commonMessages } from "@constants/intl";

import styles from "./index.module.scss";

const EmptyResult = ({
    message,
    style = {},
    className,
    boxClassName,
    imageSrc
}) => {
    const intl = useIntl();

    return (
        <div className={classNames(styles.emptyResult, { [className]: !!className })} style={style}>
            <div className="container">
                <div className={classNames(styles.emptyBox, { [boxClassName]: boxClassName })}>
                    <img src={imageSrc || '/images/empty.png'} alt="empty" />
                    {message || intl.formatMessage(commonMessages.noContent)}
                </div>
            </div>
        </div>
    )
}

export default EmptyResult;