const withReactSvg = require('next-react-svg');
const path = require('path');

module.exports = withReactSvg({
  images: {
    disableStaticImages: true
  },
  eslint: {
    // Warning: Dangerously allow production builds to successfully complete even if
    // your project has ESLint errors.
    ignoreDuringBuilds: true,
  },
  include: path.resolve(__dirname, 'src/assets/icons'),
  async rewrites() {
    return [
      {
        source: '/aircraft/:id',
        destination: '/marketplace/aircraft/:id',
      },
      {
        source: '/wingman/:id',
        destination: '/marketplace/wingman/:id',
      },
      {
        source: '/commander/:id',
        destination: '/marketplace/commander/:id',
      }
    ]
  },
  webpack: (config) => {
    const { rules } = config.module;
    // Find the array of "style rules" in the webpack config.
    // This is the array of webpack rules that:
    // - is inside a 'oneOf' block
    // - contains a rule that matches 'file.css'
    const styleRules = (rules.find((m) => m.oneOf && m.oneOf.find(({ test: reg }) => reg.test('file.css'))) || {}).oneOf;
    if (!styleRules) return config;
    // Find all the webpack rules that handle CSS modules
    // Look for rules that match '.module.css' and '.module.scss' but aren't being used to generate
    // error messages.
    const cssModuleRules = [
      styleRules.find(({ test: reg, use }) => reg.test('file.module.css') && use.loader !== 'error-loader'),
      styleRules.find(({ test: reg, use }) => reg.test('file.module.scss') && use.loader !== 'error-loader'),
    ].filter((n) => n); // remove 'undefined' values
    // Add the 'localsConvention' config option to the CSS loader config in each of these rules.
    cssModuleRules.forEach((cmr) => {
      // Find the item inside the 'use' list that defines css-loader
      const cssLoaderConfig = cmr.use.find(({ loader }) => loader.includes('css-loader'));
      if (cssLoaderConfig && cssLoaderConfig.options && cssLoaderConfig.options.modules) {
        // Patch it with the new config
        cssLoaderConfig.options.modules.exportLocalsConvention = 'camelCase';
      }
    });
    return config;
  }
})