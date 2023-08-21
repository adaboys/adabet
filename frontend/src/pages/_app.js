import { END } from 'redux-saga';
import { useEffect } from 'react';

import { wrapper } from '../redux/store';
import { accountActions } from '@redux/actions';
import { getCookie, getObjectData, removeItem } from '@utils/localStorage';
import { getInt, isMobileOnUserAgent } from '@utils';
import { storageKeys, dataStorageVersion, CURRENCY_ADA } from '@constants';

import { MainLayout } from '../components/Common';
import { NextQueryParamProvider, OverlayProvider, LanguageProvider, AlertProvider } from '@hocs';

import '../assets/scss/index.scss';

const App = ({
    Component,
    pageProps
}) => {

    useEffect(() => {
        const betData = getObjectData(storageKeys.BET_DATA);
        if (betData) {
            try {
                const currentVersion = betData.version || 0;
                if (dataStorageVersion > currentVersion) {
                    removeItem(storageKeys.BET_DATA);
                }
            }
            catch (ex) {
                console.log(ex);
            }
        }
    }, [])

    const getLayout = Component.getLayout || ((page) => <MainLayout>{page}</MainLayout>);

    return (
        <LanguageProvider>
            <AlertProvider>
                <NextQueryParamProvider>
                    <OverlayProvider>
                        {getLayout(<Component {...pageProps} />)}
                    </OverlayProvider>
                </NextQueryParamProvider>
            </AlertProvider>
        </LanguageProvider>
    )
}

App.getInitialProps = wrapper.getInitialAppProps(store => async ({ Component, ctx }) => {
    // const pageProps = {
    //     ...(Component.getInitialProps ? await Component.getInitialProps(ctx) : {}),
    // };
    if (Component.getInitialProps) {
        await Component.getInitialProps(ctx);
    }
    // 2. Stop the saga if on server
    if (ctx.req) {
        await store.dispatch(accountActions.updateCommonInfo({
            isMobile: isMobileOnUserAgent(ctx.req.headers['user-agent']),
            accessToken: getCookie(storageKeys.ACCESS_TOKEN, ctx.req.headers.cookie),
            refreshToken: getCookie(storageKeys.REFRESH_TOKEN, ctx.req.headers.cookie),
            currency: getInt(getCookie(storageKeys.BET_CURRENCY, ctx.req.headers.cookie)) || CURRENCY_ADA,
        }))
        await store.dispatch(END);
        await store.sagaTask.toPromise();
    }

    // 3. Return props
    // return {
    //     ...pageProps,
    // };
});
// export default withLocale(wrapper.withRedux(App));

export default wrapper.withRedux(App);


