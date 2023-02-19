import FacebookLoginWrapper from 'react-facebook-login/dist/facebook-login-render-props';
import { FormattedMessage } from 'react-intl';
import { Button } from '@components/Common';
import { socialLoginTypes } from '@constants';

import FacebookIcon from '@assets/icons/facebook.svg';

const FacebookLogin = ({ className, onLogin }) => {
    const onLoginSuccess = (response) => {
        if (response?.accessToken) {
            onLogin(socialLoginTypes.FACEBOOK, response.accessToken);
        }
    }
    return (
        <FacebookLoginWrapper
            appId={process.env.NEXT_PUBLIC_FACEBOOK_APP_ID}
            callback={onLoginSuccess}
            render={({ onClick }) => (
                <Button secondary className={className} onClick={onClick}>
                    <FacebookIcon />
                </Button>
            )}
        />
    )
}

export default FacebookLogin;
