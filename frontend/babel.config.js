module.exports = function (api) {
    api.cache(true)

    return {
        presets: [
            'next/babel',
            // '@babel/preset-react'
        ],
        plugins: [
            [
                'react-intl-auto',
                {
                    removePrefix: 'src/',
                    useKey: true,
                    filebase: true,
                    includeExportName: true
                },
            ],
        ],
    }
}
