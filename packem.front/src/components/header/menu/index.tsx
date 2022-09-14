import React, { useContext } from 'react';
import { useNavigate } from 'react-router-dom';

import { MenuBase } from 'components/styles';
import { AuthContext } from 'store/contexts/AuthContext';

import AdminPanelSettingsOutlinedIcon from '@mui/icons-material/AdminPanelSettingsOutlined';
import DomainIcon from '@mui/icons-material/Domain';
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
} from '@mui/material';

function DropDownMenu({ open, onClose, anchorEl }) {
  const navigate = useNavigate();
  const { logout, isSuperAdmin } = useContext(AuthContext);
  const onLogout = async () => {
    logout();
    navigate('/login');
  };

  const buttonClick = {
    0: () => navigate('/inventory'),
    1: () => navigate('/bins'),
    2: () => navigate('/receiving'),
    3: () => navigate('/sales'),
    4: () => navigate('/facilities'),
    5: () => navigate('/devices'),
    6: () => navigate('/users'),
    7: () => navigate('/customers'),
    8: () => onLogout(),
  };

  return (
    <MenuBase
      anchorEl={anchorEl}
      open={open}
      onClose={onClose}
      anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      transformOrigin={{ vertical: 'top', horizontal: 'center' }}
    >
      <List sx={{ display: 'flex', flexDirection: 'column' }}>
        {isSuperAdmin && (
          <ListItem disablePadding sx={{ display: 'block' }}>
            <ListItemButton
              sx={{
                minHeight: 48,
                justifyContent: open ? 'initial' : 'center',
                px: 2.5,
              }}
              onClick={() => navigate('/customer-management')}
            >
              <ListItemIcon
                sx={{
                  minWidth: 0,
                  mr: open ? 3 : 'auto',
                  justifyContent: 'center',
                }}
              >
                <AdminPanelSettingsOutlinedIcon />
              </ListItemIcon>
              <ListItemText
                primary="Customer Management"
                sx={{
                  textTransform: 'uppercase',
                  opacity: open ? 1 : 0,
                }}
              />
            </ListItemButton>
          </ListItem>
        )}
        {[
          'Inventory',
          'Bins',
          'Receiving',
          'Sales',
          'Facilities',
          'Devices',
          'Users',
          'Customers',
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
                {index === 0 && <ListAltIcon />}
                {index === 1 && <Inventory2OutlinedIcon />}
                {index === 2 && <InventoryOutlinedIcon />}
                {index === 3 && <ReceiptLongIcon />}
                {index === 4 && <DomainIcon />}
                {index === 5 && <TapAndPlayOutlinedIcon />}
                {index === 6 && <ManageAccountsOutlinedIcon />}
                {index === 7 && <SettingsEthernetOutlinedIcon />}
                {index === 8 && <PowerSettingsNewOutlinedIcon />}
              </ListItemIcon>
              <ListItemText
                primary={text}
                sx={{
                  textTransform: 'uppercase',
                  opacity: open ? 1 : 0,
                }}
              />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </MenuBase>
  );
}

export default React.memo(DropDownMenu);
