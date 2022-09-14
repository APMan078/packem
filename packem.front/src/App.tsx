import { BrowserRouter as Router } from 'react-router-dom';

import { SnackbarConfig } from 'config/snackbar.js';
import { SnackbarProvider } from 'notistack';
import AuthProvider from 'store/contexts/AuthContext';
import ColorModeProvider from 'store/contexts/ColorModeContext';
import GlobalProvider from 'store/contexts/GlobalContext';
import ThemeProvider from 'store/contexts/ThemeContext';

import { CssBaseline } from '@mui/material/';

import MainRoutes from './routes';

function App() {
  return (
    <SnackbarProvider maxSnack={3}>
      <SnackbarConfig />
      <AuthProvider>
        <GlobalProvider>
          <ColorModeProvider>
            <Router>
              <ThemeProvider>
                <CssBaseline />
                <MainRoutes />
              </ThemeProvider>
            </Router>
          </ColorModeProvider>
        </GlobalProvider>
      </AuthProvider>
    </SnackbarProvider>
  );
}

export default App;
