import React from "react";

import { noImageUrl } from '@constants';

const PlaceholderImage = ({
    alt = "placeholder",
}) => {
    return <img src={noImageUrl} alt={alt} />;
};

export default PlaceholderImage;

