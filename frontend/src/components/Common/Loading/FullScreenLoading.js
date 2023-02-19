import React, { useEffect } from 'react';
import { useSelector } from 'react-redux';
import MoonLoader from "react-spinners/MoonLoader";

import styles from './FullScreenLoading.module.scss';

const FullScreenLoading = () => {
    const fullScreenLoading = useSelector(state => state.loading.fullScreenLoading)

    useEffect(() => {
        if (fullScreenLoading) {
            document.body.style.overflow = 'hidden';
        }

        return () => {
            document.body.style.overflow = ''
        }
    }, [fullScreenLoading])

    if (!fullScreenLoading) {
        return null
    }

    return (
        <div className={styles.fullScreenLoading}>
            <MoonLoader color="white" loading size={40}/>
        </div>
    );
};

export default FullScreenLoading;