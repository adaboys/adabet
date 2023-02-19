import * as React from "react";
import Head from "next/head";

import { metaDefaults } from "@constants";
import { cleanObject } from "@utils";

const MetaWrapper = ({ children, meta }) => {
    const { title, description, image, type, url } = { ...metaDefaults, ...cleanObject(meta) };
    return (
        <>
            <Head>
                <title>{title}</title>
                <meta name="description" content={description} />
                <meta property="og:url" content={url} />
                <meta name="og:title" property="og:title" content={title} />
                <meta property="og:type" content={type} />
                <meta
                    name="og:description"
                    property="og:description"
                    content={description}
                />
                <meta property="og:image" content={image} />
            </Head>
            {children}
        </>
    )
}

export default MetaWrapper;
