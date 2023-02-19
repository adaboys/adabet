import { locales, defaultLocale, storageKeys } from '../constants';
import { getStringData } from './localStorage';

export const isLocale = tested => {
    return locales.some(locale => locale === tested)
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