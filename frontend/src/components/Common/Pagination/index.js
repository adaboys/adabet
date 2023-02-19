import React, { useEffect, useImperativeHandle, useState } from "react";
import Pagination from "rc-pagination";
import { StringParam, useQueryParams } from "use-query-params";
import { useRouter } from "next/router";
import classNames from "classnames";
import * as Scroll from "react-scroll";

import styles from "./index.module.scss";

const MyPagination = ({
    changePage,
    totalRecord,
    pageSize,
    className,
    disableScrollTopOnPageChange
}, ref) => {
    //get query param when change page
    const queryParams = useQueryParams({
        page: StringParam,
    });
    // use query prop of useRouter to check refresh page
    const { query } = useRouter();
    const [current, setCurrentPage] = useState(1);

    //when refresh page then setCurrent = page param in url
    useEffect(() => {
        setCurrentPage(parseInt(queryParams[0]?.page) || 1);
    }, [query]);

    useImperativeHandle(ref, () => ({
        setCurrentPage,
    }), []);

    const scrollToTop = () => {
        let scroll = Scroll.animateScroll;
        scroll.scrollToTop({ smooth: true });
    };

    const onChangePage = (currentPage) => {
        !disableScrollTopOnPageChange && scrollToTop();
        changePage(currentPage);
        setCurrentPage(currentPage);
    };

    return (
        <div className={classNames(styles.orderPagination, { [className]: !!className })}>
            <Pagination
                total={totalRecord}
                current={current}
                onChange={(currentPage) => onChangePage(currentPage)}
                pageSize={pageSize}
                showTitle={false}
            />
        </div>
    );
}

export default React.forwardRef(MyPagination);
