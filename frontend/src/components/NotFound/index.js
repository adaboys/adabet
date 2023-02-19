import Link from "next/link";
import * as React from "react";

import { paths } from "@constants";
import { Button } from "@components/Common";

import styles from "./index.module.scss";


const NotFound = ({ message }) => (
    <div className={styles.notFoundPage}>
        <h2 className={styles.header}>
            404
        </h2>
        <div className={styles.ruler} />
        <div className={styles.message}>
            {message}
        </div>
        <div className={styles.buttonWrapper}>
            <Link href={paths.home}>
                <a>
                    <Button large className={styles.button}>
                        Back to home
                    </Button>
                </a>
            </Link>
        </div>
    </div>
);

export default NotFound;
