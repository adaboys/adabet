import { useState } from 'react';

import OverlayContext from './OverlayContext';
import { OverlayManager } from '@components/Common';

const OverlayProvider = ({ children }) => {
    const [overlayConfig, setOverlayConfig] = useState({ type: null, context: null });

    const show = (type, context) => {
        document.body.style.overflow = 'hidden';
        setOverlayConfig({ type, context });
    };

    const hide = () => {
        document.body.style.overflow = '';
        setOverlayConfig({});
    };

    return (
        <OverlayContext.Provider value={{ ...overlayConfig, hide, show }}>
            {children}
            <OverlayManager/>
        </OverlayContext.Provider>
    );
}

export default OverlayProvider;
