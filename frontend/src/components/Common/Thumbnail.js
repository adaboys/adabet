import React from "react";

import { maybe } from "@utils";

import CachedImage from './CachedImage';
import PlaceholderImage from './PlaceholderImage';


const Thumbnail = ({
  source,
  children,
  ...props
}) => {
  const { thumbnail, thumbnail2x } = source;

  if (!thumbnail && !thumbnail2x) {
    return <PlaceholderImage />;
  }

  return (
    <CachedImage
      {...props}
      url={maybe(() => thumbnail.url)}
      url2x={maybe(() => thumbnail2x.url)}
      alt={maybe(() => (thumbnail.alt ? thumbnail.alt : ""), "")}
    >
      {children}
    </CachedImage>
  );
};

export default Thumbnail;

