import React, { useContext, useState, ChangeEvent } from 'react';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import { createDeviceToken } from 'services/api/devices/devices.api';
import { GlobalContext } from 'store/contexts/GlobalContext';
import { v4 as uuid } from 'uuid';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface DeviceTokenCreationProps {
  device?: any;
}

export default React.memo(({ device }: DeviceTokenCreationProps) => {
  const theme = useTheme();
  const { isDeviceTokenModalOpen, onCloseDeviceTokenModal, handleUpdateData } =
    useContext(GlobalContext);

  const [showErrors, setShowErrors] = useState(false);
  const [error, setError] = useState('');
  const [formErrors, setFormErrors] = useState({
    deviceToken: '',
  });
  const initialState = {
    customerDeviceId: '',
    deviceToken: '',
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

  const handleGenerateToken = () => {
    setForm({
      customerDeviceId: '',
      deviceToken: uuid(),
    });
  };

  const schema = yup.object().shape({
    customerDeviceId: yup.number().required('Required.'),
    deviceToken: yup.string().required('Required.'),
  });

  const handleCloseDeviceTokenModal = () => {
    setForm(initialState);
    onCloseDeviceTokenModal();
  };

  const onSaveNewDeviceToken = async () => {
    form.customerDeviceId = device;
    schema
      .validate(form, {
        abortEarly: false,
      })
      .then(async () => {
        try {
          const newDeviceToken = await createDeviceToken(form);
          snackActions.success(`Successfully created new device.`);
          setShowErrors(false);
          setError('');
          setForm(initialState);
          handleCloseDeviceTokenModal();
          handleUpdateData();
        } catch (err: any) {
          setError(err);
          setFormErrors({
            deviceToken: '',
          });
          setShowErrors(true);
          snackActions.error(`${error}`);
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

  if (!isDeviceTokenModalOpen) return null;

  return (
    <Modal
      open={isDeviceTokenModalOpen}
      onClose={() => handleCloseDeviceTokenModal()}
    >
      <ModalBox>
        <ModalContent>
          <Typography
            sx={{ marginBottom: '16px' }}
            variant="h6"
            fontWeight="bold"
          >
            Enter Device Token
          </Typography>
          {!showErrors ? (
            <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
              <Input
                sx={{ width: '100%' }}
                placeholder="Token"
                value={form.deviceToken}
                onChange={(value) => inputHandler('deviceToken', value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onSaveNewDeviceToken();
                  }
                }}
              />
              <Button
                sx={{ minHeight: '40px', maxWidth: '91px' }}
                variant="text"
                size="large"
                onClick={() => handleGenerateToken()}
              >
                Generate
              </Button>
            </Box>
          ) : (
            <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
              <Input
                sx={{ width: '100%' }}
                placeholder="Token"
                value={form.deviceToken}
                error={formErrors.deviceToken}
                onChange={(value) => inputHandler('deviceToken', value)}
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
              onClick={() => handleCloseDeviceTokenModal()}
            >
              Cancel
            </Button>
            <Button
              sx={{ minHeight: '48px', maxWidth: '91px' }}
              variant="contained"
              size="large"
              onClick={() => onSaveNewDeviceToken()}
            >
              Submit
            </Button>
          </Box>
        </ModalContent>
      </ModalBox>
    </Modal>
  );
});
