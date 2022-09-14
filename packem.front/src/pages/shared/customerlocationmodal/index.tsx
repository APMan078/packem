import React, { useContext, useState, ChangeEvent, useEffect } from 'react';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import {
  createCustomerLocation,
  editCustomerLocation,
} from 'services/api/customerlocations/customerlocations.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface CustomerLocationProps {
  customer?: any;
  customerManagement?: boolean;
  edit?: boolean;
  callBack?: () => void;
}

export default React.memo(
  ({ customer, customerManagement, edit, callBack }: CustomerLocationProps) => {
    const theme = useTheme();
    const {
      isCustomerLocationModalOpen,
      onCloseCustomerLocationModal,
      handleUpdateData,
    } = useContext(GlobalContext);
    const { currentUser } = useContext(AuthContext);
    const [showErrors, setShowErrors] = useState(false);
    const [error, setError] = useState('');
    const [formErrors, setFormErrors] = useState({
      name: '',
    });
    const [form, setForm] = useState({
      customerId: currentUser.Claim_CustomerId,
      name: '',
      customerLocationId: '',
    });

    const onForm = (key, text) => {
      setForm(() => ({
        ...form,
        [key]: text,
      }));
    };

    const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) =>
      onForm(key, event.target.value);

    const schema = yup.object().shape({
      customerId: yup.number().required('Required'),
      name: yup.string().required('Required.'),
      customerLocationId: yup.string(),
    });

    const handleCloseCustomerLocationModal = () => {
      setForm({ customerId: '', name: '', customerLocationId: '' });
      if (callBack !== undefined) {
        callBack();
      }
      onCloseCustomerLocationModal();
    };

    const onSaveNewCustomerLocation = async () => {
      if (!customerManagement) {
        form.customerId = currentUser.Claim_CustomerId;
      } else {
        form.customerId = customer.customerId;
      }
      schema
        .validate(form, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            if (edit) {
              form.customerLocationId = customer.customerLocationId;
              await editCustomerLocation(customer.customerLocationId, form);
              snackActions.success(`Successfully created edited location.`);
            } else {
              await createCustomerLocation(form);
              snackActions.success(
                `Successfully created new customer location.`,
              );
            }
            setShowErrors(false);
            setError('');
            handleCloseCustomerLocationModal();
            handleUpdateData();
          } catch (err: any) {
            setError(err);
            setFormErrors({
              name: '',
            });
            setShowErrors(true);
            snackActions.error('Unable to create customer location.');
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
      if (edit) {
        setForm(customer);
      }
    }, [edit]);

    if (!isCustomerLocationModalOpen) return null;

    if (customer === undefined) return null;

    return (
      <Modal
        open={isCustomerLocationModalOpen}
        onClose={() => handleCloseCustomerLocationModal()}
      >
        <ModalBox>
          <ModalContent>
            {customerManagement ? (
              <Box>
                {edit ? (
                  <Typography
                    sx={{ marginBottom: '16px' }}
                    variant="h6"
                    fontWeight="bold"
                  >
                    Edit Customer Location ({customer.name})
                  </Typography>
                ) : (
                  <Typography
                    sx={{ marginBottom: '16px' }}
                    variant="h6"
                    fontWeight="bold"
                  >
                    Add Customer Location ({customer.name})
                  </Typography>
                )}
              </Box>
            ) : (
              <Box>
                {edit ? (
                  <Typography
                    sx={{ marginBottom: '16px' }}
                    variant="h6"
                    fontWeight="bold"
                  >
                    Edit Location
                  </Typography>
                ) : (
                  <Typography
                    sx={{ marginBottom: '16px' }}
                    variant="h6"
                    fontWeight="bold"
                  >
                    Add Location
                  </Typography>
                )}
              </Box>
            )}
            {!showErrors ? (
              <Input
                sx={{ width: '100%' }}
                placeholder="Location Name"
                value={form.name}
                onChange={(value) => inputHandler('name', value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onSaveNewCustomerLocation();
                  }
                }}
              />
            ) : (
              <Input
                sx={{ width: '100%' }}
                placeholder="Location Name"
                value={form.name}
                error={formErrors.name}
                onChange={(value) => inputHandler('name', value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onSaveNewCustomerLocation();
                  }
                }}
              />
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
                onClick={() => handleCloseCustomerLocationModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="contained"
                size="large"
                onClick={() => onSaveNewCustomerLocation()}
              >
                Submit
              </Button>
            </Box>
          </ModalContent>
        </ModalBox>
      </Modal>
    );
  },
);
