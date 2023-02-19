import { useRouter } from "next/router";
import React, { memo, useMemo } from "react";
import { QueryParamProvider as ContextProvider } from "use-query-params";

import { ssrMode } from "../constants/index";

export const NextQueryParamProviderComponent = ( props) => {
  const { children, ...rest } = props;

  const router = useRouter();
  const match = router.asPath.match(/[^?]+/);
  const pathname = match ? match[0] : router.asPath;

  const location = useMemo(
    () =>
      ssrMode? ({search: router.asPath.replace(/[^?]+/u, ""),}): window.location,
    [router.asPath]
  );

  const history = useMemo(
    () => ({
      push: ({ search }) =>
        router.push(
          { pathname: router.pathname, query: router.query },
          { search, pathname },
          { shallow: true }
        ),
      replace: ({ search }) =>
        router.replace(
          { pathname: router.pathname, query: router.query },
          { search, pathname },
          { shallow: true }
        ),
    }),
    [pathname, router]
  );

  return (
    <ContextProvider {...rest} history={history} location={location}>
      {children}
    </ContextProvider>
  );
};

export const NextQueryParamProvider = memo(NextQueryParamProviderComponent);