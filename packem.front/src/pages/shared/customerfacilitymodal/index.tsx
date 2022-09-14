import React, { useContext, useState, ChangeEvent, useEffect } from 'react';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import {
  createCustomerFacility,
  editCustomerFacility,
} from 'services/api/customerfacilities/customerfacilities.api';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface CustomerFacilityModalProps {
  facilityData?: any;
  edit?: boolean;
  callBack?: () => void;
}

export default React.memo(
  ({ facilityData, edit, callBack }: CustomerFacilityModalProps) => {
    const theme = useTheme();
    const { isFacilityModalOpen, onCloseFacilityModal, handleUpdateData } =
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
    };
    const initialState = {
      customerId: '',
      customerLocationId: '',
      customerFacilityId: '',
      name: '',
      address: '',
      address2: '',
      city: '',
      stateProvince: '',
      zipPostalCode: '',
      phoneNumber: '',
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
      customerLocationId: yup.number().required('Required.'),
      name: yup.string().required('Required.'),
      address: yup.string().required('Required.'),
      address2: yup.string(),
      city: yup.string().required('Required.'),
      stateProvince: yup.string().required('Required.'),
      zipPostalCode: yup.string().required('Required.'),
      phoneNumber: yup.string(),
    });

    const handleCloseFacilityModal = () => {
      setForm(initialState);
      setFormErrors(initialFormErrorsState);
      setError('');
      if (callBack !== undefined) {
        callBack();
      }
      onCloseFacilityModal();
    };

    const onSaveNewFacility = async () => {
      form.customerId = facilityData.customerId;
      form.customerLocationId = facilityData.customerLocationId;
      schema
        .validate(form, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            if (edit) {
              form.customerFacilityId = facilityData.customerFacilityId;
              await editCustomerFacility(facilityData.customerFacilityId, form);
              snackActions.success(`Successfully edited ${facilityData.name}.`);
            } else {
              await createCustomerFacility(form);
              snackActions.success(
                `Successfully created new facility for ${facilityData.name}.`,
              );
            }
            setShowErrors(false);
            setError('');
            setForm(initialState);
            handleCloseFacilityModal();
            handleUpdateData();
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

    useEffect(() => {
      if (edit) {
        setForm(facilityData);
      }
    }, [edit]);

    if (!isFacilityModalOpen) return null;

    if (facilityData.name === undefined) return null;

    return (
      <Modal
        open={isFacilityModalOpen}
        onClose={() => handleCloseFacilityModal()}
      >
        <ModalBox>
          <ModalContent>
            {edit ? (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Edit Facility ({facilityData.name})
              </Typography>
            ) : (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Add Facility ({facilityData.name})
              </Typography>
            )}
            {!showErrors ? (
              <>
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Name"
                  value={form.name}
                  onChange={(value) => inputHandler('name', value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') {
                      onSaveNewFacility();
                    }
                  }}
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
              </>
            ) : (
              <>
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Name"
                  value={form.name}
                  error={formErrors.name}
                  onChange={(value) => inputHandler('name', value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') {
                      onSaveNewFacility();
                    }
                  }}
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
                  value={form.phoneNumber}
                  error={formErrors.phone}
                  onChange={(value) => inputHandler('phoneNumber', value)}
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
                onClick={() => handleCloseFacilityModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="contained"
                size="large"
                onClick={() => onSaveNewFacility()}
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
