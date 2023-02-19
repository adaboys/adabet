import { useEffect, useState } from 'react';
import { useSelector } from 'react-redux';
import { ssrMode } from '../constants';

const calcDevices = width => {
    const isMobile = width < 768;
    const isTablet = width >= 768 && width <= 1023;
    const isDesktop = width > 1023;
    const isWideDesktop = width > 1336;
    return { isMobile, isTablet, isDesktop, isWideDesktop };
}

const useDevices = () => {
    const { isMobile } = useSelector(state => state.account);
    
    const windowInnerWidth = ssrMode ? 1336 : window.innerWidth;
    const [devices, setDevices] = useState(() => {
        if(ssrMode) {
            return {
                isMobile,
                isTablet: false,
                isDesktop: !isMobile,
                isWideDesktop: false
            }
        }
        return calcDevices(windowInnerWidth);
    })
    const handleResize = e => {
        setDevices(calcDevices(e.target.innerWidth));
    }

    useEffect(() => {
        window.addEventListener('resize', handleResize);
        return () => {
            window.removeEventListener('resize', handleResize);
        }
    }, [])

    return devices;
}

export default useDevices;
