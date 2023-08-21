import { adabetApiUrl } from '.'
const baseHeader = {
    'Content-Type': 'application/json'
}

const multipartFormHeader = {
    'Content-Type': 'multipart/form-data'
}

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
            path: `${adabetApiUrl}api/user/currency/list`,
            method: 'GET',
            headers: baseHeader,
            isAuth: true
        },
        sendConfirmCoin: {
            path: `${adabetApiUrl}api/coin/withdraw/prepare`,
            method: 'POST',
            headers: baseHeader,
            isAuth: true
        },
        sendActualCoin: {
            path: `${adabetApiUrl}api/coin/withdraw/actual`,
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
            headers: baseHeader,
            isAuth: true,
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
        uploadAvatar: {
            path: `${adabetApiUrl}api/user/avatar/update`,
            method: 'POST',
            headers: multipartFormHeader,
            isAuth: true
        },
        updateProfile: {
            path: `${adabetApiUrl}api/user/identity/update`,
            method: 'POST',
            headers: baseHeader,
            isAuth: true
        },
        statistics: {
            path: `${adabetApiUrl}api/user/bet/statistics`,
            method: 'GET',
            headers: baseHeader,
            isAuth: true,
        },
        coinTransaction: {
            path: `${adabetApiUrl}api/coin/tx/history`,
            method: 'GET',
            headers: baseHeader,
            isAuth: true,
        },
        coinTransactionAll: {
            path: `${adabetApiUrl}api/coin/tx/all_history`,
            method: 'GET',
            headers: baseHeader,
            isAuth: true,
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
            isMaybeAuth: true
        },
        getMatchesLive: {
            path: `${adabetApiUrl}api/sport/:sportId/matches/live`,
            method: 'GET',
            headers: baseHeader,
            isMaybeAuth: true
        },
        getMatchesUpcoming: {
            path: `${adabetApiUrl}api/sport/:sportId/matches/upcoming`,
            method: 'GET',
            headers: baseHeader,
            isMaybeAuth: true
        },
        placeBet: {
            path: `${adabetApiUrl}api/sport/:sportId/bet/place`,
            method: 'POST',
            headers: baseHeader,
            isAuth: true
        },
        myBet: {
            path: `${adabetApiUrl}api/sport/:sportId/user/bet/histories`,
            method: 'GET',
            headers: baseHeader,
            isAuth: true
        },
        getMatchesFavorite: {
            path: `${adabetApiUrl}api/user/sport/:sportId/favorite/list`,
            method: 'GET',
            headers: baseHeader,
            isAuth: true
        },
        toogleFavoriteMatch: {
            path: `${adabetApiUrl}api/user/match/:matchId/favorite/toggle`,
            method: 'POST',
            headers: baseHeader,
            isAuth: true
        },
        totalBages: {
            path: `${adabetApiUrl}api/user/sport/:sportId/badges`,
            method: 'GET',
            headers: baseHeader,
            isAuth: true
        },
        getMatchesHistory: {
            path: `${adabetApiUrl}api/sport/match/:matchId/history`,
            method: 'GET',
            headers: baseHeader
        },
    },
    airdropEvent: {
        getMatchList: {
            path: `${adabetApiUrl}api/sport/:sportId/prediction/match/list`,
            method: 'GET',
            headers: baseHeader,
        },
        getPredictedUsers: {
            path: `${adabetApiUrl}api/sport/prediction/match/:matchId/predicted_users`,
            method: 'GET',
            headers: baseHeader,
        },
        getLeaderboard: {
            path: `${adabetApiUrl}api/sport/prediction/leaderboard`,
            method: 'GET',
            headers: baseHeader,
        },
        predict: {
            path: `${adabetApiUrl}api/sport/prediction/match/:matchId/predict`,
            method: 'POST',
            headers: baseHeader,
            isAuth: true
        },
        getPredictMatchDetails: {
            path: `${adabetApiUrl}api/sport/prediction/match/:matchId/detail`,
            method: 'GET',
            headers: baseHeader,
        },
    },
    wallet: {
        getConnectedWallet: {
            path: `${adabetApiUrl}api/user/extwallet/linked_wallets`,
            method: 'GET',
            headers: baseHeader,
            isAuth: true
        },
        requestLinkWallet: {
            path: `${adabetApiUrl}api/user/extwallet/:walletAddress/requestLink`,
            method: 'POST',
            headers: baseHeader,
            isAuth: true
        },
        verifyLinkWallet: {
            path: `${adabetApiUrl}api/user/extwallet/:walletAddress/link`,
            method: 'POST',
            headers: baseHeader,
            isAuth: true
        },
        unlinkWallet: {
            path: `${adabetApiUrl}api/user/extwallet/:walletAddress/unlink`,
            method: 'POST',
            headers: baseHeader,
            isAuth: true
        }
    },
    coin: {
        calcAmount: {
            path: `${adabetApiUrl}api/coin/swap/calc_amount`,
            method: 'GET',
            headers: baseHeader,
            isAuth: true
        },
        swap: {
            path: `${adabetApiUrl}api/coin/swap`,
            method: 'POST',
            headers: baseHeader,
            isAuth: true
        },
    },
}

export default apiConfig;
