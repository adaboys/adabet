import { useAlert } from 'react-alert';
import { useContext } from 'react';

import { OverlayContext } from '@hocs';
import { overlayTypes } from '@constants';

const useNotification = () => {
    const alert = useAlert();
    const overlay = useContext(OverlayContext);

    const showSuccess = (message) => {
        alert.removeAll();
        alert.success(message);
    }

    const showError = (message) => {
        alert.removeAll();
        alert.error(message);
    }

    const showPopupSuccess = (message) => {
        overlay.show(overlayTypes.MESSAGE, { type: 'success', message });
    }

    const showPopupError = (message) => {
        overlay.show(overlayTypes.MESSAGE, { type: 'error', message });
    }

    const showPopupWarning = (message) => {
        overlay.show(overlayTypes.MESSAGE, { type: 'warning', message });
    }

    return {
        showError,
        showSuccess,
        showPopupError,
        showPopupSuccess,
        showPopupWarning
    }
}

export default useNotification;