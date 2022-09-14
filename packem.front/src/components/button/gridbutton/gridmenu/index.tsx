import React, { useContext } from 'react';

import { GlobalContext } from 'store/contexts/GlobalContext';

import {
  Menu,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
} from '@mui/material';

interface GridMenuProps {
  open: boolean;
  onClose: any;
  anchorEl: any;
  devices?: boolean;
  customerManagement?: boolean;
  users?: boolean;
  locations?: boolean;
  facilities?: boolean;
  customers?: boolean;
}
function GridMenu({
  open,
  onClose,
  anchorEl,
  devices,
  customerManagement,
  users,
  locations,
  facilities,
  customers,
}: GridMenuProps) {
  const {
    onOpenCustomerModal,
    onOpenCustomerLocationModal,
    onOpenUserModal,
    onOpenFacilityModal,
    onOpenDeviceModal,
    onOpenDeviceTokenModal,
  } = useContext(GlobalContext);

  const handleButtonClick = {
    0: () => onOpenUserModal(),
    1: () => onOpenFacilityModal(),
  };

  return (
    <Menu
      anchorEl={anchorEl}
      open={open}
      onClose={onClose}
      anchorOrigin={{ vertical: 'center', horizontal: 'left' }}
      transformOrigin={{ vertical: 'center', horizontal: 'left' }}
    >
      {devices && (
        <List sx={{ display: 'flex', flexDirection: 'column' }}>
          <ListItem disablePadding sx={{ display: 'block' }}>
            <ListItemButton sx={{ minHeight: 48, px: 2.5 }}>
              <ListItemText
                primary="Register Device"
                sx={{ textTransform: 'Capitalize' }}
              />
            </ListItemButton>
          </ListItem>
        </List>
      )}
      {customerManagement && customers && (
        <List sx={{ display: 'flex', flexDirection: 'column' }}>
          {['Add Location'].map((text) => (
            <ListItem key={text} disablePadding sx={{ display: 'block' }}>
              <ListItemButton
                sx={{
                  minHeight: 48,
                  px: 2.5,
                }}
                onClick={() => onOpenCustomerLocationModal()}
              >
                <ListItemText
                  primary={text}
                  sx={{
                    textTransform: 'Capitalize',
                  }}
                />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      )}
      {customerManagement && locations && (
        <List sx={{ display: 'flex', flexDirection: 'column' }}>
          {['Add User', 'Add Facility'].map((text, index) => (
            <ListItem key={text} disablePadding sx={{ display: 'block' }}>
              <ListItemButton
                sx={{
                  minHeight: 48,
                  px: 2.5,
                }}
                onClick={handleButtonClick[index]}
              >
                <ListItemText
                  primary={text}
                  sx={{
                    textTransform: 'Capitalize',
                  }}
                />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      )}
      {locations && !customerManagement && (
        <List sx={{ display: 'flex', flexDirection: 'column' }}>
          {['Add Facility'].map((text) => (
            <ListItem key={text} disablePadding sx={{ display: 'block' }}>
              <ListItemButton
                sx={{
                  minHeight: 48,
                  px: 2.5,
                }}
                onClick={() => onOpenFacilityModal()}
              >
                <ListItemText
                  primary={text}
                  sx={{
                    textTransform: 'Capitalize',
                  }}
                />
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      )}
    </Menu>
  );
}

export default React.memo(GridMenu);
