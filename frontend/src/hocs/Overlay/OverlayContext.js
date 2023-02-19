import React from 'react';

const OverlayContext = React.createContext({
  context: null,
  hide: () => {},
  show: type => {},
  theme: null,
  type: null,
});

export default OverlayContext;
