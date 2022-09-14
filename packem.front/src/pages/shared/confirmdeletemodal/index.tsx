import React, { useContext, useState } from 'react';

import Button from 'components/button';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import {
  deleteCustomerFacility,
  deleteCustomerFacilityZone,
} from 'services/api/customerfacilities/customerfacilities.api';
import { deleteCustomerLocation } from 'services/api/customerlocations/customerlocations.api';
import { deleteItem } from 'services/api/item/item.api';
import {
  deleteOrderCustomer,
  deleteOrderCustomerAddress,
} from 'services/api/ordercustomers';
import { deletePurchaseOrder } from 'services/api/purchaseOrders/purchaseOrders.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface ConfirmationDialogProps {
  deleteInventoryItem?: boolean;
  deleteFacility?: boolean;
  deleteLocation?: boolean;
  deleteZone?: boolean;
  deletePO?: boolean;
  orderCustomerDelete?: boolean;
  orderCustomerAddressDelete?: boolean;
  orderCustomerAddressData?: any;
  itemData?: any;
  facilityData?: any;
  locationData?: any;
  orderCustomerData?: any;
  zoneData?: any;
  poData?: any;
  callBack?: () => void;
}

export default React.memo(
  ({
    deleteInventoryItem,
    deleteFacility,
    deleteLocation,
    deleteZone,
    deletePO,
    orderCustomerDelete,
    orderCustomerAddressDelete,
    orderCustomerData,
    orderCustomerAddressData,
    itemData,
    facilityData,
    locationData,
    zoneData,
    poData,
    callBack,
  }: ConfirmationDialogProps) => {
    const theme = useTheme();
    const {
      isConfirmDeleteDialogOpen,
      onCloseConfirmDeleteDialog,
      handleUpdateData,
    } = useContext(GlobalContext);
    const { currentUser } = useContext(AuthContext);
    const [showErrors, setShowErrors] = useState(false);
    const [error, setError] = useState('');

    const handleCloseConfirmationModal = () => {
      callBack();
      onCloseConfirmDeleteDialog();
    };

    const deleteContent = async () => {
      try {
        if (orderCustomerDelete) {
          await deleteOrderCustomer({
            customerId: currentUser.Claim_CustomerId,
            orderCustomerId: orderCustomerData.orderCustomerId,
          });
        }
        if (orderCustomerAddressDelete) {
          await deleteOrderCustomerAddress({
            orderCustomerAddressId:
              orderCustomerAddressData.orderCustomerAddressId,
          });
        }
        if (deleteInventoryItem) {
          await deleteItem({
            itemId: itemData.itemId,
          });
        }
        if (deleteFacility) {
          await deleteCustomerFacility({
            customerFacilityId: facilityData.customerFacilityId,
          });
        }
        if (deleteZone) {
          await deleteCustomerFacilityZone({ zoneId: zoneData.zoneId });
        }
        if (deleteLocation) {
          await deleteCustomerLocation({
            customerLocationId: locationData.customerLocationId,
          });
        }
        if (deletePO) {
          await deletePurchaseOrder({
            purchaseOrderId: poData.purchaseOrderId,
          });
        }
        snackActions.success(`Successfully removed data.`);
        setShowErrors(false);
        setError('');
        handleCloseConfirmationModal();
        handleUpdateData();
      } catch (err: any) {
        setError(err);
        snackActions.error('Delete Operation Failed.');
      }
    };

    if (!isConfirmDeleteDialogOpen) return null;

    return (
      <Modal
        open={isConfirmDeleteDialogOpen}
        onClose={() => handleCloseConfirmationModal()}
      >
        <ModalBox>
          <ModalContent>
            {deleteInventoryItem && (
              <Box
                sx={{
                  width: '100%',
                  display: 'flex',
                  flexDirection: 'column',
                  gap: '16px',
                }}
              >
                <Typography
                  sx={{ marginBottom: '16px' }}
                  variant="h6"
                  fontWeight="bold"
                >
                  Delete Item {itemData.itemSKU}
                </Typography>
                <Typography variant="subtitle1">
                  This action cannot be undone.
                  <br />
                  Please confirm that you wish to delete this item from your
                  inventory.
                </Typography>
              </Box>
            )}
            {orderCustomerDelete && (
              <Box
                sx={{
                  width: '100%',
                  display: 'flex',
                  flexDirection: 'column',
                  gap: '16px',
                }}
              >
                <Typography
                  sx={{ marginBottom: '16px' }}
                  variant="h6"
                  fontWeight="bold"
                >
                  Delete Customer {orderCustomerData.customerName}
                </Typography>
                <Typography variant="subtitle1">
                  This action cannot be undone.
                  <br />
                  Please confirm that you wish to delete customer and their
                  information.
                </Typography>
              </Box>
            )}
            {orderCustomerAddressDelete && (
              <Box
                sx={{
                  width: '100%',
                  display: 'flex',
                  flexDirection: 'column',
                  gap: '16px',
                }}
              >
                <Typography
                  sx={{ marginBottom: '16px' }}
                  variant="h6"
                  fontWeight="bold"
                >
                  Delete Customer Address
                </Typography>
                <Typography variant="subtitle1">
                  This action cannot be undone.
                  <br />
                  Please confirm that you wish to delete customer address.
                </Typography>
              </Box>
            )}
            {deleteFacility && (
              <Box
                sx={{
                  width: '100%',
                  display: 'flex',
                  flexDirection: 'column',
                  gap: '16px',
                }}
              >
                <Typography
                  sx={{ marginBottom: '16px' }}
                  variant="h6"
                  fontWeight="bold"
                >
                  Delete Facility {facilityData.name}
                </Typography>
                <Typography variant="subtitle1">
                  This action cannot be undone, and all data associated to this
                  facility will be lost.
                  <br />
                  Please confirm that you wish to delete this facility.
                </Typography>
              </Box>
            )}
            {deleteLocation && (
              <Box
                sx={{
                  width: '100%',
                  display: 'flex',
                  flexDirection: 'column',
                  gap: '16px',
                }}
              >
                <Typography
                  sx={{ marginBottom: '16px' }}
                  variant="h6"
                  fontWeight="bold"
                >
                  Delete Location {locationData.name}
                </Typography>
                <Typography variant="subtitle1">
                  This action cannot be undone, and all data associated to this
                  location will be lost.
                  <br />
                  Please confirm that you wish to delete this location.
                </Typography>
              </Box>
            )}
            {deleteZone && (
              <Box
                sx={{
                  width: '100%',
                  display: 'flex',
                  flexDirection: 'column',
                  gap: '16px',
                }}
              >
                <Typography
                  sx={{ marginBottom: '16px' }}
                  variant="h6"
                  fontWeight="bold"
                >
                  Delete Zone {zoneData.name}
                </Typography>
                <Typography variant="subtitle1">
                  This action cannot be undone, and all data associated to this
                  location will be lost.
                  <br />
                  Please confirm that you wish to delete this zone.
                </Typography>
              </Box>
            )}
            {deletePO && (
              <Box
                sx={{
                  width: '100%',
                  display: 'flex',
                  flexDirection: 'column',
                  gap: '16px',
                }}
              >
                <Typography
                  sx={{ marginBottom: '16px' }}
                  variant="h6"
                  fontWeight="bold"
                >
                  Delete Purchase Order {poData.purchaseOrderNo}
                </Typography>
                <Typography variant="subtitle1">
                  This action cannot be undone, and all data associated to this
                  location will be lost.
                  <br />
                  Please confirm that you wish to delete this purchase order.
                </Typography>
              </Box>
            )}
            <Box
              sx={{
                display: 'flex',
                width: '100%',
                [theme.breakpoints.up('sm')]: {
                  justifyContent: 'flex-end',
                },
                [theme.breakpoints.down('sm')]: {
                  justifyContent: 'center',
                },
                gap: '8px',
              }}
            >
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="text"
                size="large"
                onClick={() => handleCloseConfirmationModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="contained"
                size="large"
                onClick={() => deleteContent()}
              >
                Confirm
              </Button>
            </Box>
          </ModalContent>
        </ModalBox>
      </Modal>
    );
  },
);
