import React, { useRef, useState, useContext, useEffect } from 'react';
import { useJwt } from 'react-jwt';
import { useNavigate, useLocation } from 'react-router-dom';

import Button from 'components/button';
import DropDownMenu from 'components/header/menu';
import moment from 'moment';
import SelectLocationAndFacilityModal from 'pages/shared/selectlocationandfacilitymodal';
import { getCustomerFacilityByFacilityId } from 'services/api/customerfacilities/customerfacilities.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { ColorModeContext } from 'store/contexts/ColorModeContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import Brightness4Icon from '@mui/icons-material/Brightness4';
import Brightness7Icon from '@mui/icons-material/Brightness7';
import SettingsOutlinedIcon from '@mui/icons-material/SettingsOutlined';
import { Typography, Box } from '@mui/material';
import IconButton from '@mui/material/IconButton';
import { useTheme } from '@mui/material/styles';

import {
  WMSRedContainer,
  HeaderBox,
  LowHeaderBox,
  TopHeader,
  MenuButton,
} from '../styles';

interface HeaderProps {
  customerLocations?: any[];
  orderCustomerDetails?: any;
  printSOTicketsCallBack?: () => void;
}

function Header({
  customerLocations,
  orderCustomerDetails,
  printSOTicketsCallBack,
}: HeaderProps) {
  const theme = useTheme();
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = useState(null);
  const [openMenu, setOpenMenu] = useState(false);
  const location = useLocation();
  const currentLoc = location.pathname.split('/')[1];
  const menuButton = useRef();
  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  const { toggleColorMode } = useContext(ColorModeContext);
  const [customerFacility, setCustomerFacility] = useState([]);
  const {
    updateData,
    onOpenLocationAndFacilityModal,
    onOpenItemModal,
    onOpenVendorModal,
    onOpenCustomerModal,
    onOpenCustomerLocationModal,
    onOpenFacilityZoneModal,
    onOpenDeviceModal,
    onOpenDeviceTokenModal,
    onOpenUserModal,
    onOpenCreateBinModal,
    onOpenBinModal,
    onOpenManualReceiptModal,
    onOpenPurchaseOrderModal,
    onOpenPurchaseOrderItemModal,
    onOpenFileInputModal,
    onOpenManualSalesOrderModal,
    onOpenOrderCustomerModal,
    onOpenAddOrderLineItemModal,
    onOpenOrderCustomerAddressModal,
  } = useContext(GlobalContext);

  const userInitials = () => {
    const parsedName = currentUser.Claim_Fullname.split(' ');
    return parsedName[0].charAt(0) + parsedName[1].charAt(0);
  };

  const handleClickMenu = () => {
    if (!anchorEl) setAnchorEl(menuButton.current);
    setOpenMenu(!openMenu);
  };

  const onMenuClose = () => {
    setAnchorEl(null);
    setOpenMenu(!openMenu);
  };

  const onLoadCustomerFacility = async () => {
    try {
      const customerFacilityFromApi = await getCustomerFacilityByFacilityId(
        location.pathname.split('/')[3],
      );

      setCustomerFacility(customerFacilityFromApi.name);
      return true;
    } catch (error) {
      return 'Facility';
    }
  };

  const handleGetRoute = () => {
    switch (currentLoc) {
      case 'receiving':
        if (location.pathname.includes('/item')) {
          return `Receiving / PO ${location.pathname.split('/')[3]}`;
        }
        return 'Receiving';

      case 'customer-inventory':
        if (location.pathname.includes('/vendors')) {
          return 'Inventory / Vendors';
        }
        if (location.pathname.includes('/item')) {
          return `Inventory / Item ${location.pathname.split('/')[3]}`;
        }
        return 'Inventory';
      case 'inventory':
        if (location.pathname.includes('/vendors')) {
          return 'Inventory / Vendors';
        }
        if (location.pathname.includes('/item')) {
          return `Inventory / Item ${location.pathname.split('/')[3]}`;
        }
        return 'Inventory';
      case 'put-away':
        return 'Put Away Queue';
      case 'customers':
        if (location.pathname.includes(`/customers/`)) {
          if (orderCustomerDetails.customerName !== undefined) {
            return `Customer / ${
              orderCustomerDetails.customerName
            } - Last Order Date: ${moment(
              orderCustomerDetails.lastDateOrdered,
            ).format('MM/DD/YYYY')}`;
          }
          return 'Loading...';
        }
        return 'Customer Management';
      case 'bins':
        if (location.pathname.includes(`transfer-queue`)) {
          return 'Transfers';
        }
        if (location.pathname.includes(`bins/bin`)) {
          return `Storage Management / ${location.pathname.split('/')[3]}`;
        }
        return 'Storage Management';
      case 'sales':
        if (location.pathname.includes('/order')) {
          return `Sales Order / ${location.pathname.split('/')[3]}`;
        }
        return 'Sales';
      case 'facilities':
        if (location.pathname.includes('/facility')) {
          return `Facility / ${customerFacility}`;
        }
        return 'Locations and Facilities';
      case 'devices':
        if (location.pathname.includes('devices/device')) {
          return `Devices / ${location.pathname.split('/')[3]}`;
        }
        return 'Devices';
      default:
        return currentLoc;
    }
  };

  useEffect(() => {
    if (location.pathname.includes(`/facilities/facility`)) {
      onLoadCustomerFacility();
    }
  }, [updateData, currentLocationAndFacility, orderCustomerDetails]);

  return (
    <>
      <SelectLocationAndFacilityModal />
      <HeaderBox>
        <TopHeader>
          <MenuButton ref={menuButton} onClick={() => handleClickMenu()} />
          <DropDownMenu
            open={openMenu}
            onClose={onMenuClose}
            anchorEl={anchorEl}
          />
          <Typography sx={{ fontWeight: 'medium' }} variant="overline">
            {currentUser.Claim_CustomerName}
          </Typography>
          {currentLocationAndFacility === null ? (
            <Box />
          ) : (
            <Box sx={{ display: 'flex', flexDirection: 'row' }}>
              <Typography
                sx={{
                  fontWeight: 'medium',
                  color: [theme.palette.primary.main],
                }}
                variant="overline"
                onClick={() => onOpenLocationAndFacilityModal()}
              >
                {currentLocationAndFacility.locationName} •{' '}
                {currentLocationAndFacility.facilityName}
              </Typography>
            </Box>
          )}
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'row',
              alignItems: 'center',
              gap: '12px',
            }}
          >
            <IconButton
              sx={{ ml: 1 }}
              onClick={toggleColorMode}
              color="inherit"
            >
              {theme.palette.mode === 'dark' ? (
                <Brightness7Icon />
              ) : (
                <Brightness4Icon />
              )}
            </IconButton>
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'end',
              }}
            >
              <Typography sx={{ fontWeight: 'bold' }} variant="caption">
                {currentUser.Claim_Fullname}
              </Typography>
              <Typography variant="caption">
                {currentUser.Claim_Email}
              </Typography>
            </Box>
            <WMSRedContainer>
              <Typography variant="h5" color="white">
                {userInitials()}
              </Typography>
            </WMSRedContainer>
            <Box>
              <IconButton
                sx={{ ml: 1 }}
                onClick={() => navigate('/settings')}
                color="inherit"
              >
                <SettingsOutlinedIcon />
              </IconButton>
            </Box>
          </Box>
        </TopHeader>
      </HeaderBox>
      <LowHeaderBox>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            justifyContent: 'space-between',
            width: '100%',
            gap: '8px',
          }}
        >
          <Box
            sx={{
              width: '100%',
            }}
          >
            <Typography
              variant="h6"
              fontWeight="bold"
              textTransform="capitalize"
            >
              {handleGetRoute()}
            </Typography>
          </Box>
          {currentLoc === 'receiving' &&
            !location.pathname.includes('receiving/') && (
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'row',
                  justifyContent: 'flex-end',
                  width: '100%',
                  gap: '8px',
                }}
              >
                <Button
                  variant="text"
                  size="large"
                  onClick={() => onOpenManualReceiptModal()}
                >
                  MANUAL RECEIPT
                </Button>
                <Button
                  variant="text"
                  size="large"
                  onClick={() => onOpenFileInputModal()}
                >
                  IMPORT
                </Button>
                <Button
                  variant="text"
                  size="large"
                  onClick={() => onOpenPurchaseOrderModal()}
                >
                  ADD PO
                </Button>
              </Box>
            )}
          {currentLoc === 'receiving' && location.pathname.includes('/item') && (
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                justifyContent: 'flex-end',
                width: '100%',
                gap: '8px',
              }}
            >
              <Button
                variant="text"
                size="large"
                onClick={() => onOpenPurchaseOrderItemModal()}
              >
                ADD ITEMS
              </Button>
            </Box>
          )}
          {currentLoc === 'customer-management' &&
            !location.pathname.includes('customer-management/customer') && (
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'row',
                  justifyContent: 'flex-end',
                  width: '100%',
                  gap: '8px',
                }}
              >
                <Button
                  variant="text"
                  size="large"
                  onClick={() => onOpenCustomerModal()}
                >
                  Add Customer
                </Button>
              </Box>
            )}
          {currentLoc === 'inventory' &&
            !location.pathname.includes('inventory/') && (
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'row',
                  justifyContent: 'flex-end',
                  width: '100%',
                  gap: '8px',
                }}
              >
                <Button
                  variant="text"
                  size="large"
                  onClick={() => navigate('/inventory/vendors')}
                >
                  Vendors
                </Button>
                <Button
                  variant="text"
                  size="large"
                  onClick={() => onOpenItemModal()}
                >
                  Add Item
                </Button>
              </Box>
            )}
          {currentLoc === 'inventory' && location.pathname.includes('/item') && (
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                justifyContent: 'flex-end',
                width: '100%',
                gap: '8px',
              }}
            >
              <Button
                variant="text"
                size="large"
                onClick={() => onOpenBinModal()}
              >
                Add Bin & Qty
              </Button>
            </Box>
          )}
          {currentLoc === 'inventory' &&
            location.pathname.includes('/vendors') && (
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'row',
                  justifyContent: 'flex-end',
                  width: '100%',
                  gap: '8px',
                }}
              >
                <Button
                  variant="text"
                  size="large"
                  onClick={() => onOpenVendorModal()}
                >
                  Add
                </Button>
              </Box>
            )}
          {currentLoc === 'devices' &&
            !location.pathname.includes('devices/device') && (
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'row',
                  justifyContent: 'flex-end',
                  width: '100%',
                  gap: '8px',
                }}
              >
                <Button
                  variant="text"
                  size="large"
                  onClick={() => onOpenDeviceModal()}
                >
                  Register Device
                </Button>
              </Box>
            )}
          {currentLoc === 'bins' &&
            !location.pathname.includes('/transfer-queue') &&
            !location.pathname.includes('bins/bin') && (
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'row',
                  justifyContent: 'flex-end',
                  width: '100%',
                  gap: '8px',
                }}
              >
                <Button
                  variant="text"
                  size="large"
                  onClick={() => navigate('/bins/transfer-queue')}
                >
                  Transfers
                </Button>
                <Button
                  variant="text"
                  size="large"
                  onClick={() => onOpenFileInputModal()}
                >
                  Import
                </Button>
                <Button
                  variant="text"
                  size="large"
                  onClick={() => onOpenCreateBinModal()}
                >
                  Add Location
                </Button>
                <Button
                  variant="text"
                  size="large"
                  onClick={() => onOpenFacilityZoneModal()}
                >
                  Add Area
                </Button>
              </Box>
            )}
          {currentLoc === 'sales' && !location.pathname.includes(`/order`) && (
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                justifyContent: 'flex-end',
                width: '100%',
                gap: '8px',
              }}
            >
              <Button
                sx={{
                  display: 'flex',
                  [theme.breakpoints.up('lg')]: {
                    minWidth: '140px',
                  },
                }}
                variant="text"
                size="large"
                onClick={printSOTicketsCallBack}
              >
                Print Pick Tickets
              </Button>
              <Button
                variant="text"
                size="large"
                onClick={() => onOpenFileInputModal()}
              >
                Import
              </Button>
              <Button
                variant="text"
                size="large"
                onClick={() => onOpenManualSalesOrderModal()}
              >
                Add Order
              </Button>
            </Box>
          )}
          {currentLoc === 'sales' && location.pathname.includes('/order') && (
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                justifyContent: 'flex-end',
                width: '100%',
                gap: '8px',
              }}
            >
              <Button
                variant="text"
                size="large"
                onClick={() => onOpenManualReceiptModal()}
              >
                Archive
              </Button>
              <Button variant="text" size="large">
                Pick Ticket
              </Button>
              <Button
                variant="text"
                size="large"
                onClick={() => onOpenAddOrderLineItemModal()}
              >
                Add Items
              </Button>
            </Box>
          )}
          {location.pathname.includes('devices/device') && (
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                justifyContent: 'flex-end',
                width: '100%',
                gap: '8px',
              }}
            >
              <Button
                variant="text"
                size="large"
                onClick={() => onOpenDeviceTokenModal()}
              >
                Create Token
              </Button>
            </Box>
          )}
          {currentLoc === 'facilities' && (
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                justifyContent: 'flex-end',
                width: '100%',
                gap: '8px',
              }}
            >
              <Button
                variant="text"
                size="large"
                onClick={() => onOpenCustomerLocationModal()}
              >
                Add Location
              </Button>
            </Box>
          )}
          {currentLoc === 'users' && (
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                justifyContent: 'flex-end',
                width: '100%',
                gap: '8px',
              }}
            >
              <Button
                variant="text"
                size="large"
                onClick={() => onOpenUserModal()}
              >
                Add User
              </Button>
            </Box>
          )}
          {currentLoc === 'customers' &&
            !location.pathname.includes('customers/') && (
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'row',
                  justifyContent: 'flex-end',
                  width: '100%',
                  gap: '8px',
                }}
              >
                <Button
                  variant="text"
                  size="large"
                  onClick={() => onOpenOrderCustomerModal()}
                >
                  Add Customer
                </Button>
              </Box>
            )}
          {currentLoc === 'customers' &&
            location.pathname.includes('customers/') && (
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'row',
                  justifyContent: 'flex-end',
                  width: '100%',
                  gap: '8px',
                }}
              >
                <Button
                  variant="text"
                  size="large"
                  onClick={() => onOpenManualSalesOrderModal()}
                >
                  Add Sale Order
                </Button>
                <Button
                  variant="text"
                  size="large"
                  onClick={() => onOpenOrderCustomerAddressModal()}
                >
                  Add Address
                </Button>
              </Box>
            )}
        </Box>
      </LowHeaderBox>
    </>
  );
}

export default React.memo(Header);
