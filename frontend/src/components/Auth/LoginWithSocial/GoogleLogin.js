import { GoogleOAuthProvider } from '@react-oauth/google';
import GoogleLoginButton from './GoogleLoginButton';

const GoogleLogin = ({ onLogin, className }) => {

    return (
        <GoogleOAuthProvider clientId={process.env.NEXT_PUBLIC_GOOGLE_OAUTH_CLIENT_ID}>
            <GoogleLoginButton className={className} onLogin={onLogin} />
        </GoogleOAuthProvider>
    )
}
export default GoogleLogin;