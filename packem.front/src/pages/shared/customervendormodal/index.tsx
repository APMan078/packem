import React, { useContext, useState, ChangeEvent, useEffect } from 'react';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import { createCustomerVendor } from 'services/api/customer/customer.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface VendorModalProps {
  vendor?: any;
}

export default React.memo(({ vendor }: VendorModalProps) => {
  const theme = useTheme();
  const { currentUser } = useContext(AuthContext);
  const { isVendorModalOpen, onCloseVendorModal, handleUpdateData } =
    useContext(GlobalContext);
  const [showErrors, setShowErrors] = useState(false);
  const [error, setError] = useState('');
  const initialFormErrorsState: any = {
    vendorId: '',
    customerId: '',
    account: '',
    name: '',
    contact: '',
    address: '',
    city: '',
    stateProvince: '',
    zipPostalCode: '',
    phone: '',
  };
  const initialState: any = {
    vendorId: '',
    customerId: '',
    account: '',
    name: '',
    contact: '',
    address: '',
    city: '',
    stateProvince: '',
    zipPostalCode: '',
    phone: '',
  };
  const [formErrors, setFormErrors] = useState<any>(initialFormErrorsState);
  const [form, setForm] = useState<any>(initialState);

  const onForm = (key, text) => {
    setForm(() => ({
      ...form,
      [key]: text,
    }));
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) =>
    onForm(key, event.target.value);

  const schema = yup.object().shape({
    customerId: yup.number().required('Required.'),
    account: yup.string().required('Required.'),
    name: yup.string().required('Required.'),
    contact: yup.string().required('Required'),
    address: yup.string().required('Required'),
    city: yup.string().required('Required'),
    stateProvince: yup.string().required('Required'),
    zipPostalCode: yup.string().required('Required'),
    phone: yup.string().required('Required'),
  });

  const handleCloseVendorModal = () => {
    setForm(initialState);
    setFormErrors(initialFormErrorsState);
    setError('');
    onCloseVendorModal();
  };

  const onSaveNewVendor = async () => {
    form.customerId = currentUser.Claim_CustomerId;
    schema
      .validate(form, {
        abortEarly: false,
      })
      .then(async () => {
        try {
          const newVendor = await createCustomerVendor(form);
          snackActions.success(`Successfully added new vendor.`);
          handleUpdateData();
          handleCloseVendorModal();
        } catch (err: any) {
          setError(err);
          setFormErrors(initialFormErrorsState);
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

  const onLoadEditVendor = () => {
    setForm(vendor);
  };

  useEffect(() => {
    setForm(initialState);
    onLoadEditVendor();
  }, [vendor]);

  if (!isVendorModalOpen) return null;

  return (
    <Modal open={isVendorModalOpen} onClose={() => handleCloseVendorModal()}>
      <ModalBox>
        <ModalContent>
          {vendor.vendorId === '' ? (
            <Typography
              sx={{ marginBottom: '16px' }}
              variant="h6"
              fontWeight="bold"
            >
              Add Vendor
            </Typography>
          ) : (
            <Typography
              sx={{ marginBottom: '16px' }}
              variant="h6"
              fontWeight="bold"
            >
              Edit Vendor
            </Typography>
          )}
          {!showErrors ? (
            <>
              <Input
                sx={{ width: '100%' }}
                placeholder="Account No."
                value={form.account}
                onChange={(value) => inputHandler('account', value)}
              />
              <Input
                sx={{ width: '100%' }}
                placeholder="Name"
                value={form.name}
                onChange={(value) => inputHandler('name', value)}
              />
              <Input
                sx={{ width: '100%' }}
                placeholder="Point of Contact"
                value={form.contact}
                onChange={(value) => inputHandler('contact', value)}
              />
              <Input
                sx={{ width: '100%' }}
                placeholder="Address"
                value={form.address}
                onChange={(value) => inputHandler('address', value)}
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
                value={form.phone}
                onChange={(value) => inputHandler('phone', value)}
              />
            </>
          ) : (
            <>
              <Input
                sx={{ width: '100%' }}
                placeholder="Account No."
                value={form.account}
                error={formErrors.account}
                onChange={(value) => inputHandler('account', value)}
              />
              <Input
                sx={{ width: '100%' }}
                placeholder="Name"
                value={form.name}
                error={formErrors.name}
                onChange={(value) => inputHandler('name', value)}
              />
              <Input
                sx={{ width: '100%' }}
                placeholder="Point of Contact"
                value={form.contact}
                error={formErrors.contact}
                onChange={(value) => inputHandler('contact', value)}
              />
              <Input
                sx={{ width: '100%' }}
                placeholder="Address"
                value={form.address}
                error={formErrors.address}
                onChange={(value) => inputHandler('address', value)}
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
                value={form.phone}
                error={formErrors.phone}
                onChange={(value) => inputHandler('phone', value)}
              />
            </>
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
              onClick={() => handleCloseVendorModal()}
            >
              Cancel
            </Button>
            <Button
              sx={{ minHeight: '48px', maxWidth: '91px' }}
              variant="contained"
              size="large"
              onClick={() => onSaveNewVendor()}
            >
              Submit
            </Button>
          </Box>
        </ModalContent>
      </ModalBox>
    </Modal>
  );
});
