import React, { useContext, useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { AuthContext } from 'store/contexts/AuthContext';

import AdminPanelSettingsOutlinedIcon from '@mui/icons-material/AdminPanelSettingsOutlined';
import DashboardIcon from '@mui/icons-material/Dashboard';
import DomainIcon from '@mui/icons-material/Domain';
import GradingOutlinedIcon from '@mui/icons-material/Grading';
import Inventory2OutlinedIcon from '@mui/icons-material/Inventory2Outlined';
import InventoryOutlinedIcon from '@mui/icons-material/InventoryOutlined';
import ListAltIcon from '@mui/icons-material/ListAlt';
import ManageAccountsOutlinedIcon from '@mui/icons-material/ManageAccountsOutlined';
import PowerSettingsNewOutlinedIcon from '@mui/icons-material/PowerSettingsNewOutlined';
import ReceiptLongIcon from '@mui/icons-material/ReceiptLong';
import SettingsEthernetOutlinedIcon from '@mui/icons-material/SettingsEthernetOutlined';
import TapAndPlayOutlinedIcon from '@mui/icons-material/TapAndPlayOutlined';
import {
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Box,
} from '@mui/material';
import MuiDrawer from '@mui/material/Drawer';
import { styled, Theme, CSSObject } from '@mui/material/styles';

import { NavBarLogo } from '../styles';

const drawerWidth = 296;

const openedMixin = (theme: Theme): CSSObject => ({
  width: drawerWidth,
  transition: theme.transitions.create('width', {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.enteringScreen,
  }),
  overflowX: 'hidden',
});

const closedMixin = (theme: Theme): CSSObject => ({
  transition: theme.transitions.create('width', {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.leavingScreen,
  }),
  overflowX: 'hidden',
  width: `calc(${theme.spacing(7)} + 1px)`,
  [theme.breakpoints.up('sm')]: {
    width: `calc(${theme.spacing(8)} + 1px)`,
  },
});

const NavBar = styled(MuiDrawer, {
  shouldForwardProp: (prop) => prop !== 'open',
})(({ theme, open }) => ({
  width: drawerWidth,
  flexShrink: 0,
  whiteSpace: 'nowrap',
  boxSizing: 'border-box',
  ...(open && {
    ...openedMixin(theme),
    '& .MuiDrawer-paper': openedMixin(theme),
  }),
  ...(!open && {
    ...closedMixin(theme),
    '& .MuiDrawer-paper': closedMixin(theme),
  }),
}));

function Navbar() {
  const [open, setOpen] = React.useState(false);
  const navigate = useNavigate();
  const { logout, isSuperAdmin, isAdmin, isInventoryViewer } =
    useContext(AuthContext);

  const handleDrawerOpen = () => {
    setOpen(true);
  };

  const handleDrawerClose = () => {
    setOpen(false);
  };

  const onLogout = async () => {
    logout();
    navigate('/login');
  };

  const buttonClickAdmin = {
    0: () => navigate(`/customer-management`),
    1: () => onLogout(),
  };

  const buttonClickViewer = {
    0: () => navigate(`/customer-inventory`),
    1: () => onLogout(),
  };

  const buttonClick = {
    0: () => navigate('/dashboard'),
    1: () => navigate('/inventory'),
    2: () => navigate('/bins'),
    3: () => navigate('/receiving'),
    4: () => navigate('/sales'),
    5: () => navigate('/facilities'),
    6: () => navigate('/devices'),
    7: () => navigate('/users'),
    8: () => navigate('/customers'),
    9: () => navigate('/picking'),
    10: () => navigate('/put-away'),
    11: () => onLogout(),
  };

  return (
    <NavBar
      sx={{
        '& .MuiDrawer-paper': { background: '#17262f' },
        display: {
          xs: 'none',
          sm: 'none',
          md: 'none',
          lg: 'flex',
          xl: 'flex',
        },
      }}
      onMouseEnter={() => handleDrawerOpen()}
      onMouseLeave={() => handleDrawerClose()}
      variant="permanent"
      open={open}
    >
      <NavBarLogo />
      <List sx={{ display: 'flex', flexDirection: 'column' }}>
        {isSuperAdmin && (
          <Box>
            {['Customer Management', 'Log Out'].map((text, index) => (
              <ListItem key={text} disablePadding sx={{ display: 'block' }}>
                <ListItemButton
                  sx={{
                    minHeight: 48,
                    justifyContent: open ? 'initial' : 'center',
                    px: 2.5,
                  }}
                  onClick={buttonClickAdmin[index]}
                >
                  <ListItemIcon
                    sx={{
                      minWidth: 0,
                      mr: open ? 3 : 'auto',
                      justifyContent: 'center',
                    }}
                  >
                    {index === 0 && (
                      <AdminPanelSettingsOutlinedIcon
                        sx={{ color: '#ffffff' }}
                      />
                    )}
                    {index === 1 && (
                      <PowerSettingsNewOutlinedIcon sx={{ color: '#ffffff' }} />
                    )}
                  </ListItemIcon>
                  <ListItemText
                    primary={text}
                    sx={{
                      color: 'white',
                      textTransform: 'uppercase',
                      opacity: open ? 1 : 0,
                    }}
                  />
                </ListItemButton>
              </ListItem>
            ))}
          </Box>
        )}
        {isInventoryViewer && (
          <Box>
            {['Inventory', 'Log Out'].map((text, index) => (
              <ListItem key={text} disablePadding sx={{ display: 'block' }}>
                <ListItemButton
                  sx={{
                    minHeight: 48,
                    justifyContent: open ? 'initial' : 'center',
                    px: 2.5,
                  }}
                  onClick={buttonClickViewer[index]}
                >
                  <ListItemIcon
                    sx={{
                      minWidth: 0,
                      mr: open ? 3 : 'auto',
                      justifyContent: 'center',
                    }}
                  >
                    {index === 0 && (
                      <AdminPanelSettingsOutlinedIcon
                        sx={{ color: '#ffffff' }}
                      />
                    )}
                    {index === 1 && (
                      <PowerSettingsNewOutlinedIcon sx={{ color: '#ffffff' }} />
                    )}
                  </ListItemIcon>
                  <ListItemText
                    primary={text}
                    sx={{
                      color: 'white',
                      textTransform: 'uppercase',
                      opacity: open ? 1 : 0,
                    }}
                  />
                </ListItemButton>
              </ListItem>
            ))}
          </Box>
        )}
        {!isSuperAdmin && isAdmin && (
          <Box>
            {[
              'Dashboard',
              'Inventory',
              'Locations and Areas',
              'Receiving',
              'Sales',
              'Facilities',
              'Devices',
              'Users',
              'Customers',
              'Picking',
              'Put Away',
              'Log Out',
            ].map((text, index) => (
              <ListItem key={text} disablePadding sx={{ display: 'block' }}>
                <ListItemButton
                  sx={{
                    minHeight: 48,
                    justifyContent: open ? 'initial' : 'center',
                    px: 2.5,
                  }}
                  onClick={buttonClick[index]}
                >
                  <ListItemIcon
                    sx={{
                      minWidth: 0,
                      mr: open ? 3 : 'auto',
                      justifyContent: 'center',
                    }}
                  >
                    {index === 0 && <DashboardIcon sx={{ color: '#ffffff' }} />}
                    {index === 1 && <ListAltIcon sx={{ color: '#ffffff' }} />}
                    {index === 2 && (
                      <Inventory2OutlinedIcon sx={{ color: '#ffffff' }} />
                    )}
                    {index === 3 && (
                      <InventoryOutlinedIcon sx={{ color: '#ffffff' }} />
                    )}
                    {index === 4 && (
                      <ReceiptLongIcon sx={{ color: '#ffffff' }} />
                    )}
                    {index === 5 && <DomainIcon sx={{ color: '#ffffff' }} />}
                    {index === 6 && (
                      <TapAndPlayOutlinedIcon sx={{ color: '#ffffff' }} />
                    )}
                    {index === 7 && (
                      <ManageAccountsOutlinedIcon sx={{ color: '#ffffff' }} />
                    )}
                    {index === 8 && (
                      <SettingsEthernetOutlinedIcon sx={{ color: '#ffffff' }} />
                    )}
                    {index === 9 && (
                      <Inventory2OutlinedIcon sx={{ color: '#ffffff' }} />
                    )}
                    {index === 10 && (
                      <InventoryOutlinedIcon sx={{ color: '#ffffff' }} />
                    )}
                    {index === 11 && (
                      <PowerSettingsNewOutlinedIcon sx={{ color: '#ffffff' }} />
                    )}
                  </ListItemIcon>
                  <ListItemText
                    primary={text}
                    sx={{
                      color: 'white',
                      textTransform: 'uppercase',
                      opacity: open ? 1 : 0,
                    }}
                  />
                </ListItemButton>
              </ListItem>
            ))}
          </Box>
        )}
      </List>
    </NavBar>
  );
}

export default React.memo(Navbar);
