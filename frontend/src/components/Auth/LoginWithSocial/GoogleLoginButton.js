import { useGoogleLogin } from '@react-oauth/google';
import { FormattedMessage } from 'react-intl';

import { Button } from '@components/Common';
import { socialLoginTypes } from '@constants';

import GoogleIcon from '@assets/icons/google.svg';

const GoogleLoginButton = ({ onLogin, className }) => {

    const login = useGoogleLogin({
        onSuccess: response => {
            if (response?.access_token) {
                onLogin(socialLoginTypes.GOOGLE, response.access_token);
            }
        },
    });

    return (
        <Button secondary onClick={login} className={className}>
            <GoogleIcon />
        </Button>
    )
}
export default GoogleLoginButton;