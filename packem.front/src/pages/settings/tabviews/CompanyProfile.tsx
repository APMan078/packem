import React, { ChangeEvent, useContext, useEffect, useState } from 'react';

import Button from 'components/button';
import Card from 'components/card';
import CardWithHeader from 'components/card/CardWithHeader';
import Input from 'components/input/Input';
import { snackActions } from 'config/snackbar.js';
import {
  editCustomer,
  getCustomerById,
} from 'services/api/customer/customer.api';
import { AuthContext } from 'store/contexts/AuthContext';
import * as yup from 'yup';

import { Box, Typography } from '@mui/material';

function CompanyProfile() {
  const { currentUser, currentLocationAndFacility, updateData } =
    useContext(AuthContext);

  const initialFormErrorsState: any = {
    name: '',
    address: '',
    address2: '',
    city: '',
    stateProvince: '',
    zipPostalCode: '',
    phoneNumber: '',
  };

  const initialState = {
    customerId: '',
    name: '',
    address: '',
    address2: '',
    city: '',
    stateProvince: '',
    zipPostalCode: '',
    phoneNumber: '',
    pointOfContact: '',
  };
  const [formErrors, setFormErrors] = useState(initialFormErrorsState);
  const [form, setForm] = useState(initialState);
  const [showErrors, setShowErrors] = useState(false);
  const [error, setError] = useState('');

  const onLoadCustomerProfile = async () => {
    try {
      const customerFromAPI = await getCustomerById(
        currentUser.Claim_CustomerId,
      );

      setForm(customerFromAPI);

      return true;
    } catch (err: any) {
      return err;
    }
  };

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
  });

  useEffect(() => {
    onLoadCustomerProfile();
  }, [currentLocationAndFacility, updateData]);

  const onSaveNewCustomer = async () => {
    schema
      .validate(form, {
        abortEarly: false,
      })
      .then(async () => {
        try {
          await editCustomer(currentUser.Claim_CustomerId, form);
          snackActions.success(`Successfully edited customer.`);
          setShowErrors(false);
          setError('');
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

  return (
    <>
      {/* Company Profile */}
      <CardWithHeader
        sx={{ display: 'flex', flexDirection: 'column' }}
        header="Company Details"
        subHeader="Use corporate or headquarters information. This address will be used in on printouts and other relevant fields."
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'start',
            alignItems: 'center',
            width: { sm: '100%', md: '50%' },
            paddingY: '18px',
            gap: '18px',
          }}
        >
          <Input
            sx={{ width: '100%' }}
            placeholder="Company Name"
            value={form.name}
            error={formErrors.name}
            size="large"
            onChange={(value) => inputHandler('name', value)}
          />
          <Input
            sx={{ width: '100%' }}
            placeholder="Company Address"
            value={form.address}
            error={formErrors.address}
            size="large"
            onChange={(value) => inputHandler('address', value)}
          />
          <Input
            sx={{ width: '100%' }}
            placeholder="Company Address 2"
            value={form.address2}
            error={formErrors.address2}
            size="large"
            onChange={(value) => inputHandler('address2', value)}
          />
          <Input
            sx={{ width: '100%' }}
            placeholder="Company City"
            value={form.city}
            error={formErrors.city}
            size="large"
            onChange={(value) => inputHandler('city', value)}
          />
          <Input
            sx={{ width: '100%' }}
            placeholder="Company State or Province"
            value={form.stateProvince}
            error={formErrors.stateProvince}
            size="large"
            onChange={(value) => inputHandler('stateProvince', value)}
          />
          <Input
            sx={{ width: '100%' }}
            placeholder="Company Postal Code"
            value={form.zipPostalCode}
            error={formErrors.zipPostalCode}
            size="large"
            onChange={(value) => inputHandler('zipPostalCode', value)}
          />
          <Input
            sx={{ width: '100%' }}
            placeholder="Phone Number"
            value={form.phoneNumber}
            error={formErrors.phoneNumber}
            size="large"
            onChange={(value) => inputHandler('phoneNumber', value)}
          />
        </Box>
        <Box
          sx={{
            display: 'flex',
            width: '100%',
            justifyContent: 'end',
            gap: '8px',
          }}
        >
          <Box sx={{ display: 'flex', width: '25%', justifyContent: 'center' }}>
            <Button
              sx={{ display: 'flex', width: '100%' }}
              variant="contained"
              size="small"
              onClick={() => onSaveNewCustomer()}
            >
              SAVE
            </Button>
          </Box>
        </Box>
      </CardWithHeader>
      {/* <CardWithHeader
        sx={{ display: 'flex', flexDirection: 'column' }}
        header="Company Logo"
        subHeader="Company Logo. Upload a JPEG or PNG under 2MB in size. This will show in print material."
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'start',
            alignItems: 'center',
            width: { sm: '100%', md: '100%' },
            paddingY: '18px',
            gap: '18px',
          }}
        >
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'row',
              alignItems: 'center',
              justifyContent: 'start',
              width: '100%',
              gap: '12px',
            }}
          >
            <Box sx={{ display: 'flex', width: '50%' }}>
              <Input
                sx={{ width: '100%' }}
                placeholder="Upload Image"
                size="large"
              />
            </Box>

            <Box sx={{ display: 'flex', width: '25%' }}>
              <Button
                sx={{ display: 'flex', height: '100%' }}
                variant="contained"
                size="large"
              >
                Upload
              </Button>
            </Box>
          </Box>
        </Box>
      </CardWithHeader>
      <Card>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
          }}
        >
          <Typography variant="subtitle1" fontWeight="light">
            Last updated on 01/01/2022
          </Typography>
        </Box>
      </Card> */}
    </>
  );
}

export default React.memo(CompanyProfile);
