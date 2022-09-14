import React, { createContext, useContext, useMemo } from 'react';

import ColorModeProvider, {
  ColorModeContext,
} from 'store/contexts/ColorModeContext';

import {
  ThemeProvider as MUIThemeProvider,
  createTheme,
} from '@mui/material/styles';

import { getDesignTokens } from '../../styles/themes';

const ThemeProvider = ({ children }) => {
  const { mode } = useContext(ColorModeContext);

  // Update the theme only if the mode changes
  const theme = useMemo(() => createTheme(getDesignTokens(mode)), [mode]);

  return <MUIThemeProvider theme={theme}>{children}</MUIThemeProvider>;
};

export default React.memo(ThemeProvider);
