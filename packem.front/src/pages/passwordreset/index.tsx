import React, { useState, useContext, ChangeEvent, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';

import Button from 'components/button';
import Input from 'components/input';
import { snackActions } from 'config/snackbar.js';
import { resetPassword } from 'services/api/auth/auth.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Box, Typography, Link } from '@mui/material';
import { useTheme } from '@mui/material/styles';

import {
  ResetPasswordContainer,
  ResetPasswordRedBar,
  ResetPasswordHeader,
  ResetPasswordForm,
  ResetPasswordFormContainer,
  ResetPasswordTextLogo,
} from '../styles';

function useQuery() {
  const { search } = useLocation();

  return React.useMemo(() => new URLSearchParams(search), [search]);
}

function ResetPassword() {
  const token = useQuery();
  const theme = useTheme();
  const navigate = useNavigate();
  const { currentUser, isAuth } = useContext(AuthContext);
  const [showSuccessMessage, setShowSuccessMessage] = useState(false);
  const [showErrors, setShowErrors] = useState(false);
  const [error, setError] = useState('');
  const [formErrors, setFormErrors] = useState({
    token: '',
    password: '',
    confirmPassword: '',
  });
  const [form, setForm] = useState({
    token: token.get('token'),
    password: '',
    confirmPassword: '',
  });

  const schema = yup.object().shape({
    token: yup.string().required('Invalid token.'),
    password: yup.string().required('Required.'),
    confirmPassword: yup
      .string()
      .oneOf([yup.ref('password'), null], 'Passwords must match'),
  });

  const onForm = (key, text) => {
    setForm(() => ({
      ...form,
      [key]: text,
    }));
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) =>
    onForm(key, event.target.value);

  const onResetPassword = async () => {
    schema
      .validate(form, {
        abortEarly: false,
      })
      .then(async () => {
        setShowErrors(false);
        setFormErrors({
          token: '',
          password: '',
          confirmPassword: '',
        });
        try {
          const resetForm = {
            token: form.token,
            password: form.password,
          };
          await resetPassword(resetForm);
          navigate('/login');
          snackActions.success('Successfully reset password.');
          setShowSuccessMessage(true);
        } catch (err: any) {
          setError(err);
          setShowErrors(true);
        }
      })
      .catch((err) => {
        const errorsFound = err.inner.reduce((acc, curr) => {
          if (!acc[curr.path]) acc[curr.path] = curr.message;
          return acc;
        }, {});
        setFormErrors(errorsFound);
        setShowErrors(true);
      });
  };

  useEffect(() => {
    if (isAuth) {
      switch (currentUser.Claim_UserRole) {
        case '1':
          navigate(`/customer-management`);
          break;
        default:
          snackActions.error('Cannot access this page without a valid token.');
          navigate(`/inventory`);
      }
    }
  }, [currentUser, showSuccessMessage]);

  return (
    <ResetPasswordContainer>
      <ResetPasswordRedBar />
      <ResetPasswordHeader>
        <ResetPasswordTextLogo />
      </ResetPasswordHeader>
      <ResetPasswordForm>
        <ResetPasswordFormContainer>
          <Typography variant="h6" fontWeight="bold">
            New Password
          </Typography>
          <Typography variant="subtitle1" fontWeight="normal">
            Must be at least 8 characters long and include a special character.
          </Typography>
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              gap: '16px',
              marginTop: '24px',
              marginBottom: '32px',
            }}
          >
            <Input
              sx={{ width: '100%' }}
              type="password"
              placeholder="New Password"
              value={form.password}
              onChange={(value) => inputHandler('password', value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter') {
                  onResetPassword();
                }
              }}
            />
            <Input
              sx={{ width: '100%' }}
              type="password"
              placeholder="Confirm Password"
              value={form.confirmPassword}
              onChange={(value) => inputHandler('confirmPassword', value)}
              onKeyDown={(e) => {
                if (e.key === 'Enter') {
                  onResetPassword();
                }
              }}
            />
          </Box>
          <Typography
            sx={{ marginTop: '4px' }}
            variant="subtitle2"
            color="#d32f2f"
          >
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
              onClick={() => navigate('/login')}
            >
              Cancel
            </Button>
            <Button
              sx={{ minHeight: '48px', maxWidth: '91px' }}
              variant="contained"
              size="large"
              onClick={() => onResetPassword()}
            >
              Reset
            </Button>
          </Box>
        </ResetPasswordFormContainer>
      </ResetPasswordForm>
    </ResetPasswordContainer>
  );
}

export default ResetPassword;
