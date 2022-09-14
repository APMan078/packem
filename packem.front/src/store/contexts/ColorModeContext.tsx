import React, { createContext, useState, useMemo } from 'react';

import { PaletteMode } from '@mui/material';

export const ColorModeContext = createContext(null);

const ColorModeProvider = ({ children }) => {
  const [mode, setMode] = useState<PaletteMode>('light');

  const toggleColorMode = () => {
    setMode((prevMode) => (prevMode === 'light' ? 'dark' : 'light'));
  };

  const props = useMemo(
    () => ({
      mode,
      toggleColorMode,
    }),
    [mode],
  );

  return (
    <ColorModeContext.Provider value={props}>
      {children}
    </ColorModeContext.Provider>
  );
};

export default React.memo(ColorModeProvider);
