import React, { useContext, useState, ChangeEvent, useEffect } from 'react';
import { useLocation } from 'react-router-dom';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import { adjustCustomerBinQty } from 'services/api/customerfacilities/customerfacilities.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface AdjustQtyBinModalProps {
  locationDetails?: any;
  itemDetails?: any;
}

export default React.memo(
  ({ locationDetails, itemDetails }: AdjustQtyBinModalProps) => {
    const theme = useTheme();
    const getLocation = useLocation();
    const { currentUser } = useContext(AuthContext);
    const { isAdjustModalOpen, onCloseAdjustModal, handleUpdateData } =
      useContext(GlobalContext);
    const [showErrors, setShowErrors] = useState(false);
    const [error, setError] = useState('');
    const [formErrors, setFormErrors] = useState({
      customerId: '',
      customerLocationId: '',
      itemId: '',
      binId: '',
      newQty: '',
    });
    const initialState = {
      customerId: currentUser.Claim_CustomerId,
      customerLocationId: locationDetails.customerLocationId,
      itemId: itemDetails,
      binId: locationDetails.binId,
      newQty: '',
    };
    const [form, setForm] = useState(initialState);

    const onForm = (key, text) => {
      setForm(() => ({
        ...form,
        [key]: text,
      }));
    };

    const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) =>
      onForm(key, event.target.value);

    const schema = yup.object().shape({
      customerId: yup.string().required('Required.'),
      customerLocationId: yup.number().required('Required.'),
      itemId: yup.string().required('Required.'),
      binId: yup.number().required('Required.'),
      newQty: yup.string().required('Required.'),
    });

    const handleCloseAdjustModal = () => {
      setForm({
        customerId: '',
        customerLocationId: '',
        itemId: '',
        binId: '',
        newQty: '',
      });
      onCloseAdjustModal();
    };

    const onAdjustBinQty = async () => {
      schema
        .validate(form, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            const newAdjustQtyEntity = await adjustCustomerBinQty(form);
            snackActions.success(`Successfully queued item for bin transfer.`);
            setForm(initialState);
            handleCloseAdjustModal();
            handleUpdateData();
          } catch (err: any) {
            setError(err);
            setFormErrors({
              customerId: currentUser.customerId,
              customerLocationId: locationDetails.customerLocationId,
              itemId: itemDetails,
              binId: locationDetails.binId,
              newQty: '',
            });
            setShowErrors(true);
            snackActions.error(`${err}`);
          }
        })
        .catch((err) => {
          const errorsFound = err.inner.reduce((acc, curr) => {
            if (!acc[curr.path]) acc[curr.path] = curr.message;
            return acc;
          }, {});
          setFormErrors(errorsFound);
          setError('');
          setShowErrors(true);
        });
    };

    useEffect(() => {
      setForm(initialState);
    }, [itemDetails, locationDetails]);

    if (!isAdjustModalOpen) return null;

    return (
      <Modal open={isAdjustModalOpen} onClose={() => handleCloseAdjustModal()}>
        <ModalBox>
          <ModalContent>
            <Typography
              sx={{ marginBotton: '16px' }}
              variant="h6"
              fontWeight="bold"
            >
              Adjust Item Quantity
            </Typography>
            <Typography variant="subtitle1" fontWeight="bold">
              Change known quantities for item{' '}
              {getLocation.pathname.split('/')[3]} in {locationDetails.bin}.
            </Typography>
            {!showErrors ? (
              <Box
                sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}
              >
                <Input
                  sx={{ width: '100% ' }}
                  type="number"
                  placeholder="New Quantity"
                  value={form.newQty}
                  onChange={(value) => inputHandler('newQty', value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') {
                      onAdjustBinQty();
                    }
                  }}
                />
              </Box>
            ) : (
              <Box
                sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}
              >
                <Input
                  sx={{ width: '100% ' }}
                  type="number"
                  placeholder="Quantity to Transfer"
                  value={form.newQty}
                  error={formErrors.newQty}
                  onChange={(value) => inputHandler('newQty', value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') {
                      onAdjustBinQty();
                    }
                  }}
                />
              </Box>
            )}
            <Typography variant="subtitle2" color="#d32f2f">
              {error}
            </Typography>
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
                onClick={() => handleCloseAdjustModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="contained"
                size="large"
                onClick={() => onAdjustBinQty()}
              >
                Queue
              </Button>
            </Box>
          </ModalContent>
        </ModalBox>
      </Modal>
    );
  },
);
