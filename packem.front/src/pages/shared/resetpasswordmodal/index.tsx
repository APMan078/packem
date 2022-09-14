import React, { useContext, useState, ChangeEvent } from 'react';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import { requestResetPasswordToken } from 'services/api/auth/auth.api';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

export default React.memo(() => {
  const theme = useTheme();
  const {
    isResetPasswordModalOpen,
    onCloseResetPasswordModal,
    handleUpdateData,
  } = useContext(GlobalContext);

  const [showErrors, setShowErrors] = useState(false);
  const [error, setError] = useState('');
  const [formErrors, setFormErrors] = useState({
    emailAddress: '',
  });
  const initialState = {
    emailAddress: '',
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
    emailAddress: yup.string().required('Required.'),
  });

  const handleCloseResetPasswordModal = () => {
    setForm(initialState);
    onCloseResetPasswordModal();
  };

  const onDispatchResetPasswordEmail = async () => {
    schema
      .validate(form, {
        abortEarly: false,
      })
      .then(async () => {
        try {
          await requestResetPasswordToken(form);
          setShowErrors(false);
          setError('');
          snackActions.success(`Email sent to ${form.emailAddress}.`);
          handleCloseResetPasswordModal();
          handleUpdateData();
        } catch (err: any) {
          setError(err);
          setFormErrors({
            emailAddress: '',
          });
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

  if (!isResetPasswordModalOpen) return null;

  return (
    <Modal
      open={isResetPasswordModalOpen}
      onClose={() => handleCloseResetPasswordModal()}
    >
      <ModalBox>
        <ModalContent>
          <Typography
            sx={{ marginBottom: '16px' }}
            variant="h6"
            fontWeight="bold"
          >
            Password Help
          </Typography>
          {!showErrors ? (
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <Input
                sx={{ width: '100%' }}
                placeholder="Email Associated to Account"
                value={form.emailAddress}
                onChange={(value) => inputHandler('emailAddress', value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onDispatchResetPasswordEmail();
                  }
                }}
              />
            </Box>
          ) : (
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <Input
                sx={{ width: '100%' }}
                placeholder="Email Associated to Account"
                error={formErrors.emailAddress}
                value={form.emailAddress}
                onChange={(value) => inputHandler('emailAddress', value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onDispatchResetPasswordEmail();
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
              onClick={() => handleCloseResetPasswordModal()}
            >
              Cancel
            </Button>
            <Button
              sx={{ minHeight: '48px', maxWidth: '91px' }}
              variant="contained"
              size="large"
              onClick={() => onDispatchResetPasswordEmail()}
            >
              Reset
            </Button>
          </Box>
        </ModalContent>
      </ModalBox>
    </Modal>
  );
});
