import { useSelector, useDispatch } from 'react-redux';
import { useIntl } from 'react-intl';

import { accountActions, accountActionTypes } from '@redux/actions';
import { useNotification } from '@hooks';
import { commonMessages } from '@constants/intl';

const useAuth = () => {
    const dispatch = useDispatch();
    const intl = useIntl();
    const { showSuccess } = useNotification();

    const { profileData, accessToken: accessTokenState, currency } = useSelector(state => state.account);
    const isGetttingProfile = useSelector(state => state.loading[accountActionTypes.GET_PROFILE]);
    
    const logout = (callbackFunc) => {
        dispatch(accountActions.logout());
        showSuccess(intl.formatMessage(commonMessages.logoutSuccessful));
        callbackFunc && callbackFunc();
    }

    return {
        user: profileData,
        currency,
        isAuthenticated: !!accessTokenState,
        loaded: isGetttingProfile === false,
        logout
    }
}

export default useAuth;
