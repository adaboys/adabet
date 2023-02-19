import { useEffect, useState } from 'react';
import { IntlProvider } from 'react-intl';

import locale_JA from '@locales/ja.json';
import { locales, defaultLocale, storageKeys, paths } from '@constants';
import { getStringData, setStringData } from '@utils/localStorage';

import { LanguageContext } from '.';

const messages = {
    // en: enMessages,
    ja: locale_JA,
}

const isLocale = (language) => {
    if (locales.includes(language)) {
        return true;
    }
    return false;
}

export const getInitialLocale = () => {
    const localSetting = getStringData(storageKeys.LOCALE);
    if (localSetting && isLocale(localSetting)) {
      return localSetting;
    }
  
    // const [browserSetting] = navigator.language.split('-')
    // if (isLocale(browserSetting)) {
    //   return browserSetting
    // }
  
    return defaultLocale;
  }

const LanguageProvider = ({
    children
}) => {
    // const locale = getInitialLocale();
    const [locale, setLocale] = useState();

    const changeLocale = (langCode) => {
        setStringData(storageKeys.LOCALE, langCode);
        setLocale(langCode);
        // window.location.replace(paths.home);
    }

    useEffect(() => {
        setLocale(getInitialLocale());
    },[])


    return (
        <LanguageContext.Provider value={{locale, changeLocale}}>
            <IntlProvider locale={locale} messages={messages[locale]} defaultLocale={defaultLocale}>
                {children}
            </IntlProvider>
        </LanguageContext.Provider>
    )
}

export default LanguageProvider;

