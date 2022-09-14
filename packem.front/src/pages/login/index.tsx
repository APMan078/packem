import React, { useState, useContext, ChangeEvent, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

import Button from 'components/button';
import Input from 'components/input';
import ResetPasswordModal from 'pages/shared/resetpasswordmodal';
import {
  login as apiLogin,
  getDefaultLocationAndFacility,
} from 'services/api/auth/auth.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Box, Typography, Link } from '@mui/material';

import {
  LoginContainer,
  LoginBackground,
  FormContainer,
  PackemLogo,
  PackTextLogo,
  LoginRedBar,
} from '../styles';

function Login() {
  const navigate = useNavigate();
  const { onOpenResetPasswordModal, isResetPasswordModalOpen } =
    useContext(GlobalContext);
  const {
    login,
    setFacilityAndLocation,
    currentUser,
    isAuth,
    useStorage,
    authorized,
    handleSetAuthorized,
  } = useContext(AuthContext);
  const [showErrors, setShowErrors] = useState(false);
  const [error, setError] = useState('');
  const [formErrors, setFormErrors] = useState({
    username: '',
    password: '',
  });
  const [form, setForm] = useState({
    username: '',
    password: '',
  });

  const schema = yup.object().shape({
    username: yup.string().required('Required.'),
    password: yup.string().required('Required.'),
  });

  const onForm = (key, text) => {
    setForm(() => ({
      ...form,
      [key]: text,
    }));
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) =>
    onForm(key, event.target.value);

  const onSignIn = async () => {
    schema
      .validate(form, {
        abortEarly: false,
      })
      .then(async () => {
        setShowErrors(false);
        setFormErrors({
          username: '',
          password: '',
        });
        try {
          const user = await apiLogin(form);
          login(user.token);
          const locationAndFacility = await getDefaultLocationAndFacility(
            user.userData,
          );
          setFacilityAndLocation(locationAndFacility);
          handleSetAuthorized();
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
        case '5':
          navigate(`/customer-inventory`);
          break;
        default:
          navigate(`/dashboard`);
      }
    }
  }, [authorized, currentUser, useStorage]);

  return (
    <LoginContainer>
      <ResetPasswordModal />
      <LoginRedBar />
      <LoginBackground>
        <PackemLogo />
      </LoginBackground>
      <FormContainer>
        <Box
          sx={{
            display: 'flex',
            maxWidth: '328px',
            width: '100%',
            alignItems: 'flex-start',
          }}
        >
          <PackTextLogo />
        </Box>
        <Box
          sx={{
            display: 'flex',
            maxWidth: '328px',
            width: '100%',
            alignItems: 'flex-start',
          }}
        >
          <Typography
            sx={{ marginBottom: '24px', fontWeight: 'bold' }}
            variant="h6"
          >
            Administration Portal
          </Typography>
        </Box>
        <Box
          component="form"
          autoComplete="off"
          sx={{
            display: 'flex',
            flexDirection: 'column',
            maxWidth: '328px',
            width: '100%',
            gap: '16px',
          }}
        >
          {!showErrors ? (
            <>
              <Input
                sx={{ width: '100%' }}
                placeholder="User"
                value={form.username}
                onChange={(value) => inputHandler('username', value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onSignIn();
                  }
                }}
              />
              <Input
                sx={{ width: '100%' }}
                placeholder="Password"
                value={form.password}
                type="password"
                onChange={(value) => inputHandler('password', value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onSignIn();
                  }
                }}
              />
            </>
          ) : (
            <>
              <Input
                sx={{ width: '100%' }}
                placeholder="User"
                value={form.username}
                error={formErrors.username}
                onChange={(value) => inputHandler('username', value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onSignIn();
                  }
                }}
              />
              <Input
                sx={{ width: '100%' }}
                placeholder="Password"
                value={form.password}
                error={formErrors.password}
                type="password"
                onChange={(value) => inputHandler('password', value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onSignIn();
                  }
                }}
              />
            </>
          )}

          <Typography
            sx={{ marginTop: '4px' }}
            variant="subtitle2"
            color="#d32f2f"
          >
            {error}
          </Typography>

          <Button variant="contained" size="large" onClick={() => onSignIn()}>
            Login
          </Button>
          <Typography sx={{ marginTop: '24px' }} variant="subtitle1">
            Welcome to your PackemWMS
            <br /> Administrator Portal where you can
            <br /> access inventory, sales, and warehouse
            <br /> operation data.
            <br />
            <br />
            First time user? See our{' '}
            <Link href="/" underline="none">
              Tutorial.
            </Link>
          </Typography>
          <Typography
            sx={{ cursor: 'pointer', color: '#0087DB' }}
            onClick={() => onOpenResetPasswordModal()}
          >
            Password Help
          </Typography>
        </Box>
      </FormContainer>
    </LoginContainer>
  );
}

export default React.memo(Login);
