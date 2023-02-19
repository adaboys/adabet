import { adabetApiUrl } from '.'
const baseHeader = {
    'Content-Type': 'application/json'
}

// const multipartFormHeader = {
//     'Content-Type': 'multipart/form-data'
// }

const apiConfig = {
    account: {
        loginSocial: {
            path: `${adabetApiUrl}api/auth/loginWithProvider`,
            method: 'POST',
            headers: baseHeader
        },
        login: {
            path: `${adabetApiUrl}api/auth/login`,
            method: 'POST',
            headers: baseHeader
        },
        logout: {
            path: `${adabetApiUrl}api/auth/logout`,
            method: 'POST',
            headers: baseHeader,
            isAuth: true
        },
        getProfile: {
            path: `${adabetApiUrl}api/user/profile`,
            method: 'GET',
            headers: baseHeader,
            isAuth: true
        },
        refreshToken: {
            path: `${adabetApiUrl}api/auth/silentLogin`,
            method: 'POST',
            headers: baseHeader
        },
        register: {
            path: `${adabetApiUrl}api/user/register/attempt`,
            method: 'POST',
            headers: baseHeader
        },
        confirmRegister: {
            path: `${adabetApiUrl}api/user/register/confirm`,
            method: 'POST',
            headers: baseHeader
        },
        getBalance: {
            path: `${adabetApiUrl}api/user/balance`,
            method: 'GET',
            headers: baseHeader,
            isAuth: true
        },
        sendConfirmCoin: {
            path: `${adabetApiUrl}api/coin/sendConfirm`,
            method: 'POST',
            headers: baseHeader,
            isAuth: true
        },
        sendActualCoin: {
            path: `${adabetApiUrl}api/coin/sendActual`,
            method: 'POST',
            headers: baseHeader,
            isAuth: true
        },
        requestLoginWallet: {
            path: `${adabetApiUrl}api/auth/requestLoginWithExternalWallet`,
            method: 'POST',
            headers: baseHeader
        },
        verifyLoginWallet: {
            path: `${adabetApiUrl}api/auth/loginWithExternalWallet`,
            method: 'POST',
            headers: baseHeader
        },
        changePassword: {
            path: `${adabetApiUrl}api/user/password/change`,
            method: 'POST',
            headers: baseHeader
        },
        requestResetPassword: {
            path: `${adabetApiUrl}api/auth/password_reset/request`,
            method: 'POST',
            headers: baseHeader
        },
        confirmResetPassword: {
            path: `${adabetApiUrl}api/auth/password_reset/confirm`,
            method: 'POST',
            headers: baseHeader
        },
    },
    sport: {
        getSportList: {
            path: `${adabetApiUrl}api/sports`,
            method: 'GET',
            headers: baseHeader,
        },
        getMatchesHighlight: {
            path: `${adabetApiUrl}api/sport/:sportId/matches/highlight`,
            method: 'GET',
            headers: baseHeader,
        },
        getMatchesTop: {
            path: `${adabetApiUrl}api/sport/:sportId/matches/top`,
            method: 'GET',
            headers: baseHeader,
        },
        getMatchesLive: {
            path: `${adabetApiUrl}api/sport/:sportId/matches/live`,
            method: 'GET',
            headers: baseHeader,
        },
        getMatchesUpcoming: {
            path: `${adabetApiUrl}api/sport/:sportId/matches/upcoming`,
            method: 'GET',
            headers: baseHeader,
        },
    },
}

export default apiConfig;
