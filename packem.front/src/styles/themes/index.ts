import { PaletteMode } from '@mui/material';
import { grey } from '@mui/material/colors';

export const getDesignTokens = (mode: PaletteMode) => ({
  breakpoints: {
    values: {
      xs: 0,
      sm: 600,
      md: 960,
      lg: 1280,
      xl: 1920,
    },
  },
  palette: {
    mode,
    primary: {
      main: mode === 'dark' ? '#D1495B' : '#1C9DCC',
      light: mode === 'dark' ? '#D1495B0A' : '#1C9DCC0A',
    },
    secondary: {
      main: mode === 'dark' ? '#00798C' : '#DE4A50',
    },
    background: {
      default: mode === 'dark' ? '#17262f' : '#EFF2F7',
      paper: mode === 'dark' ? '#17262f' : '#EFF2F7',
    },
    text: {
      primary: mode === 'dark' ? grey[300] : grey[900],
      secondary: mode === 'dark' ? grey[500] : grey[800],
    },
  },
  transitions: {
    duration: {
      shortest: 150,
      shorter: 200,
      short: 250,
      standard: 300,
      complex: 375,
      enteringScreen: 225,
      leavingScreen: 195,
    },
  },
});
