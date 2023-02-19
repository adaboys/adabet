import LoginModal from '@components/Auth/LoginModal';
import RegisterModal from '@components/Auth/RegisterModal';
import { MessageModal } from '../Modal';

import OverlayContext from '@hocs/Overlay/OverlayContext';
import { overlayTypes } from '@constants';
import ResetPasswordModal from '@components/Auth/ResetPassword/ResetPasswordModal';

const OverlayManager = () => (
    <OverlayContext.Consumer>
        {overlay => {
            switch (overlay.type) {
                case overlayTypes.LOGIN:
                    return <LoginModal overlay={overlay} />;
                case overlayTypes.REGISTER:
                    return <RegisterModal overlay={overlay} />;
                case overlayTypes.MESSAGE:
                    return <MessageModal overlay={overlay} />;
                case overlayTypes.RESET_PASSWORD:
                    return <ResetPasswordModal overlay={overlay} />;
                default:
                    return null;
            }
        }}
    </OverlayContext.Consumer>
);

export default OverlayManager;
