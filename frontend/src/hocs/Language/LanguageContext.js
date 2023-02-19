import React from 'react';
import { defaultLocale } from '@constants';

const LanguageContext = React.createContext({
    locale: defaultLocale,
    setLocale: () => { },
    getContentKey: () => { }
});

export default LanguageContext;
