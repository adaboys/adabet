export const defaultLocale = 'en';
export const locales = ['en', 'ja', 'ge'];

export const ssrMode = typeof window === 'undefined';
export const appMode = process.env.NEXT_PUBLIC_APP_MODE;
export const adabetApiUrl = process.env.NEXT_PUBLIC_ADABET_API;

export const storageKeys = {
    USER_DATA: 'adabet-user-data',
    ACCESS_TOKEN: 'adabet-access-token',
    REFRESH_TOKEN: 'adabet-refresh-token',
    LOCALE: 'adabet-locale'
}

export const noImageUrl = '/images/no-image.jpeg';

const baseUrl = "/";
const sportUrl = `${baseUrl}sports`;

export const paths = {
    home: baseUrl,
    notFound: `${baseUrl}404`,
    sport: sportUrl
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
}

export const socialLoginTypes = {
    GOOGLE: 'google',
    FACEBOOK: 'facebook'
}

export const CLIENT_TYPE = 3;
