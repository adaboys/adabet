import React from "react";

import { ssrMode } from "@constants";

const useNetworkStatus = (callBack) => {
  const [online, setOnline] = React.useState(
    !ssrMode && "onLine" in navigator ? navigator.onLine : true
  );

  const updateOnlineStatus = () => {
    const status = navigator.onLine;

    if (callBack) {
      callBack(status);
    }
    setOnline(navigator.onLine);
  };

  React.useEffect(() => {
    addEventListener("offline", updateOnlineStatus);
    addEventListener("online", updateOnlineStatus);

    return () => {
      removeEventListener("offline", updateOnlineStatus);
      removeEventListener("online", updateOnlineStatus);
    };
  }, []);

  return { online };
};

export default useNetworkStatus;
