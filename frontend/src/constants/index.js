export const defaultLocale = 'en';
export const locales = ['en', 'ja', 'ge'];

export const ssrMode = typeof window === 'undefined';
export const appMode = process.env.NEXT_PUBLIC_APP_MODE;
export const adabetApiUrl = process.env.NEXT_PUBLIC_ADABET_API;
export const dataStorageVersion = process.env.NEXT_PUBLIC_DATA_VERSION || 0;
export const casinoURL = process.env.NEXT_PUBLIC_CASINO_URL;

export const storageKeys = {
    USER_DATA: 'adabet-user-data',
    ACCESS_TOKEN: 'adabet-access-token',
    REFRESH_TOKEN: 'adabet-refresh-token',
    LOCALE: 'adabet-locale',
    BET_DATA: 'bet-data',
    BET_CURRENCY: 'bet-currency'
}

export const noImageUrl = '/images/no-image.jpeg';

export const matcheTypes = {
    TOP: 'top',
    LIVE: 'live',
    UPCOMING: 'upcoming',
    FAVORITE: 'favorite'
}

const baseUrl = "/";
const sportUrl = `${baseUrl}sports/[matchType]`;
// const accountUrl = `${baseUrl}account`;

export const paths = {
    home: baseUrl,
    notFound: `${baseUrl}404`,
    sport: sportUrl,
    account: `${baseUrl}account`,
    accountWallet: `${baseUrl}account/wallet`,
    accountDeposit: `${baseUrl}account/wallet/deposit`,
    accountWithdraw: `${baseUrl}account/wallet/withdraw`,
    accountConnectWallet: `${baseUrl}account/connect-wallet`,
    accountTransaction: `${baseUrl}account/transactions`,
    airdropEvent: `${baseUrl}airdrop-event/[tab]`,
    scoreResultDetails: `${baseUrl}airdrop-event/score-result/[id]`,
    highlightDetails: `${baseUrl}airdrop-event/highlight/[id]`,
    aboutUs: `${baseUrl}about-us`,
    myBet: `${baseUrl}my-bet`,
    highlightMatches: `${baseUrl}airdrop-event/highlight`,
    topMatches: sportUrl.replace('[matchType]', matcheTypes.TOP),
    favoriteMatches: sportUrl.replace('[matchType]', matcheTypes.FAVORITE),
    changePassword: `${baseUrl}account/change-password`,
    transaction: `${baseUrl}account/transactions`,
    bcSwap: `${baseUrl}account/wallet/bc-swap`,
}

export const numberRegExp = /[0-9]|\./;

export const metaDefaults = {
    description: "ADABET Website",
    image: "/images/logo.png",
    title: "ADABET",
    type: "website",
    url: !ssrMode ? window.location.origin : "",
};

export const overlayTypes = {
    LOGIN: 'login',
    REGISTER: 'register',
    MESSAGE: 'message',
    RESET_PASSWORD: 'resetPassword',
    STATISTICS: 'statistics',
}

export const socialLoginTypes = {
    GOOGLE: 'google',
    FACEBOOK: 'facebook'
}

export const CLIENT_TYPE = 3;

export const MIN_ADA_BET = 2;
export const CURRENCY_ADA = 1;

export const sportTypes = {
    SOCCER: 1,
    TENNIS: 2
}

export const oddsNameMapping = {
    [sportTypes.SOCCER]: {
        ['x']: 'draw',
        ['dbc']: 'Double chance',
        ['1x']: '1 or draw',
        ['2x']: '2 or draw',
        ['12']: '1 or 2'
    },
    [sportTypes.TENNIS]: {
        ['1x2']: 'Winner'
    }
}

export const betStatuses = {
    IN_PLAY: 2
}

export const SPORT_DEFAULT_ID = 1; // socccer

export const coinTypes = {
    ADA: 'ada',
    ABE: 'abe',
    GEM: 'gem'
}

export const DATE_FORMAT_DISPLAY = 'DD/MM/YYYY';
export const DATE_FORMAT_DISPLAY_US = 'YYYY-MM-DD HH:mm:ss';
