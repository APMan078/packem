import React, { useContext, useState, ChangeEvent, useEffect } from 'react';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import {
  createCustomer,
  editCustomer,
} from 'services/api/customer/customer.api';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface CustomerModalProps {
  customerData?: any;
  edit?: boolean;
  callBack?: () => void;
}

export default React.memo(
  ({ customerData, edit, callBack }: CustomerModalProps) => {
    const theme = useTheme();
    const { isCustomerModalOpen, onCloseCustomerModal, handleUpdateData } =
      useContext(GlobalContext);

    const [showErrors, setShowErrors] = useState(false);
    const [error, setError] = useState('');
    const initialFormErrorsState: any = {
      name: '',
      address: '',
      address2: '',
      city: '',
      stateProvince: '',
      zipPostalCode: '',
      phoneNumber: '',
      pointOfContact: '',
      contactEmail: '',
    };
    const initialState = {
      name: '',
      address: '',
      address2: '',
      city: '',
      stateProvince: '',
      zipPostalCode: '',
      phoneNumber: '',
      pointOfContact: '',
      contactEmail: '',
    };
    const [formErrors, setFormErrors] = useState(initialFormErrorsState);
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
      name: yup.string().required('Required.'),
      address: yup.string().required('Required.'),
      address2: yup.string(),
      city: yup.string().required('Required.'),
      stateProvince: yup.string().required('Required.'),
      zipPostalCode: yup.string().required('Required.'),
      phoneNumber: yup.string().required('Required.'),
      pointOfContact: yup.string().required('Required.'),
      contactEmail: yup.string().required('Required.'),
    });

    const handleCloseCustomerModal = () => {
      setForm(initialState);
      setFormErrors(initialFormErrorsState);
      setError('');
      if (edit) {
        callBack();
      }
      onCloseCustomerModal();
    };

    const onSaveNewCustomer = async () => {
      schema
        .validate(form, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            if (edit) {
              await editCustomer(customerData.customerId, form);
              snackActions.success(`Successfully edited customer.`);
            } else {
              await createCustomer(form);
              snackActions.success(`Successfully created new customer.`);
            }
            setShowErrors(false);
            setError('');
            handleCloseCustomerModal();
            handleUpdateData();
          } catch (err: any) {
            setError(err);
            setFormErrors(initialFormErrorsState);
            setShowErrors(true);
            snackActions.error('Unable to create customer.');
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
        setForm(customerData);
      }
    }, [customerData]);

    if (!isCustomerModalOpen) return null;

    return (
      <Modal
        open={isCustomerModalOpen}
        onClose={() => handleCloseCustomerModal()}
      >
        <ModalBox>
          <ModalContent>
            {edit ? (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Edit Customer
              </Typography>
            ) : (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Create Customer
              </Typography>
            )}
            {!showErrors ? (
              <Box
                sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}
              >
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Company Name"
                  value={form.name}
                  onChange={(value) => inputHandler('name', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Address"
                  value={form.address}
                  onChange={(value) => inputHandler('address', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Address 2"
                  value={form.address2}
                  onChange={(value) => inputHandler('address2', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="City"
                  value={form.city}
                  onChange={(value) => inputHandler('city', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="State/Province"
                  value={form.stateProvince}
                  onChange={(value) => inputHandler('stateProvince', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="ZIP"
                  value={form.zipPostalCode}
                  onChange={(value) => inputHandler('zipPostalCode', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Phone Number"
                  value={form.phoneNumber}
                  onChange={(value) => inputHandler('phoneNumber', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Point of Contact"
                  value={form.pointOfContact}
                  onChange={(value) => inputHandler('pointOfContact', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Contact Email"
                  value={form.contactEmail}
                  onChange={(value) => inputHandler('contactEmail', value)}
                />
              </Box>
            ) : (
              <Box
                sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}
              >
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Company Name"
                  error={formErrors.name}
                  value={form.name}
                  onChange={(value) => inputHandler('name', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Address"
                  error={formErrors.address}
                  value={form.address}
                  onChange={(value) => inputHandler('address', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Address 2"
                  value={form.address2}
                  error={formErrors.address2}
                  onChange={(value) => inputHandler('address2', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="City"
                  value={form.city}
                  error={formErrors.city}
                  onChange={(value) => inputHandler('city', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="State/Province"
                  value={form.stateProvince}
                  error={formErrors.stateProvince}
                  onChange={(value) => inputHandler('stateProvince', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="ZIP"
                  value={form.zipPostalCode}
                  error={formErrors.zipPostalCode}
                  onChange={(value) => inputHandler('zipPostalCode', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Phone Number"
                  error={formErrors.phoneNumber}
                  value={form.phoneNumber}
                  onChange={(value) => inputHandler('phoneNumber', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Point of Contact"
                  error={formErrors.pointOfContact}
                  value={form.pointOfContact}
                  onChange={(value) => inputHandler('pointOfContact', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Contact Email"
                  error={formErrors.contactEmail}
                  value={form.contactEmail}
                  onChange={(value) => inputHandler('contactEmail', value)}
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
                onClick={() => handleCloseCustomerModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="contained"
                size="large"
                onClick={() => onSaveNewCustomer()}
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
