import React, { useContext, useState, ChangeEvent, useEffect } from 'react';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import {
  registerDevice,
  editDeviceData,
} from 'services/api/devices/devices.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface DeviceRegisterProps {
  customer?: any;
  edit?: boolean;
  callBack?: () => void;
}

export default React.memo(
  ({ customer, edit, callBack }: DeviceRegisterProps) => {
    const theme = useTheme();
    const { isDeviceModalOpen, onCloseDeviceModal, handleUpdateData } =
      useContext(GlobalContext);
    const { currentLocationAndFacility } = useContext(AuthContext);
    const [showErrors, setShowErrors] = useState(false);
    const [error, setError] = useState('');
    const [formErrors, setFormErrors] = useState({
      customerLocationId: '',
      serialNumber: '',
    });
    const initialState = {
      deviceId: '',
      customerId: '',
      customerLocationId: currentLocationAndFacility.customerLocationId,
      serialNumber: '',
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
      deviceId: yup.string(),
      customerId: yup.number().required('Required.'),
      customerLocationId: yup.number(),
      serialNumber: yup.string().required('Required'),
    });

    const handleCloseDeviceModal = () => {
      if (edit) {
        callBack();
      }
      setForm(initialState);
      onCloseDeviceModal();
    };

    const onSaveNewDevice = async () => {
      form.customerId = customer.customerId;
      schema
        .validate(form, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            if (edit) {
              await editDeviceData(customer.deviceId, form);
              snackActions.success(`Successfully edited device.`);
            } else {
              await registerDevice(form);
              snackActions.success(`Successfully created new device.`);
            }
            setShowErrors(false);
            setError('');
            setForm(initialState);
            handleCloseDeviceModal();
            handleUpdateData();
          } catch (err: any) {
            setError(err);
            setFormErrors({
              customerLocationId: '',
              serialNumber: '',
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
      if (edit) {
        setForm(customer);
      }
    }, [customer]);

    return (
      <Modal open={isDeviceModalOpen} onClose={() => handleCloseDeviceModal()}>
        <ModalBox>
          <ModalContent>
            {edit ? (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Edit Device
              </Typography>
            ) : (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Create Device
              </Typography>
            )}

            {!showErrors ? (
              <Input
                sx={{ width: '100%' }}
                placeholder="Serial Number"
                value={form.serialNumber}
                onChange={(value) => inputHandler('serialNumber', value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onSaveNewDevice();
                  }
                }}
              />
            ) : (
              <Input
                sx={{ width: '100%' }}
                placeholder="Serial Number"
                value={form.serialNumber}
                error={formErrors.serialNumber}
                onChange={(value) => inputHandler('serialNumber', value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onSaveNewDevice();
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
                onClick={() => handleCloseDeviceModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="contained"
                size="large"
                onClick={() => onSaveNewDevice()}
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
