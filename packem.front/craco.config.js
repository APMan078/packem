const path = require('path');

module.exports = {
  webpack: {
    alias: {
      src: path.resolve(__dirname, 'src/'),
      '@fonts': path.resolve(__dirname, 'src/assets/fonts/'),
      '@images': path.resolve(__dirname, 'src/assets/images/'),
      '@icons': path.resolve(__dirname, 'src/assets/icons/'),
      '@pages': path.resolve(__dirname, 'src/pages/'),
      '@themes': path.resolve(__dirname, 'src/styles/themes/'),
      '@components': path.resolve(__dirname, 'src/components/'),
      '@helpers': path.resolve(__dirname, 'src/helpers/'),
      '@store': path.resolve(__dirname, 'src/store/'),
      '@contexts': path.resolve(__dirname, 'src/store/contexts/'),
      '@config': path.resolve(__dirname, 'src/config/'),
      '@api': path.resolve(__dirname, 'src/services/api/'),
      '@mui/styled-engine': path.resolve(
        __dirname,
        './node_modules/@mui/styled-engine-sc',
      ),
    },
  },
  jest: {
    configure: {
      moduleNameMapper: {
        '^src(.*)$': '<rootDir>/src$1',
        '^@fonts(.*)$': '<rootDir>/src/assets/fonts$1',
        '^@images(.*)$': '<rootDir>/src/assets/images$1',
        '^@icons(.*)$': '<rootDir>/src/assets/icons$1',
        '^@pages(.*)$': '<rootDir>/src/pages$1',
        '^@themes(.*)$': '<rootDir>/src/styles/themes$1',
        '^@components(.*)$': '<rootDir>/src/components$1',
        '^@helpers(.*)$': '<rootDir>/src/helpers$1',
        '^@store(.*)$': '<rootDir>/src/store$1',
        '^@contexts(.*)$': '<rootDir>/src/store/contexts$1',
        '^@config(.*)$': '<rootDir>/src/config$1',
        '^@api(.*)$': '<rootDir>/src/services/api$1',
      },
    },
  },
};
