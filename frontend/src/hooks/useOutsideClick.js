import { useEffect, useRef } from 'react';

// Description: function track event click outside element
// Rule:
// onClose: function onClose trigger function of parent
// flag: enable or disable function click outside
const useOutsideClick = (handler, activeRef) => {
  const ref = useRef(null);
  useEffect(
    () => {
      const listener = (event) => {
        // Do nothing if clicking ref's element or descendent elements
        if (!ref.current || ref.current.contains(event.target)
        || (activeRef && activeRef.current.contains(event.target))) {
          return;
        }
        handler(event);
      };
      document.addEventListener("mousedown", listener);
      document.addEventListener("touchstart", listener);
      return () => {
        document.removeEventListener("mousedown", listener);
        document.removeEventListener("touchstart", listener);
      };
    },
    // Add ref and handler to effect dependencies
    // It's worth noting that because passed in handler is a new ...
    // ... function on every render that will cause this effect ...
    // ... callback/cleanup to run every render. It's not a big deal ...
    // ... but to optimize you can wrap handler in useCallback before ...
    // ... passing it into this hook.
    [ref, handler]
  );
  return ref;
};

export default useOutsideClick;