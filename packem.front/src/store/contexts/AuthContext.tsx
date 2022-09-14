import React, { createContext, useState, useMemo } from 'react';
import { decodeToken } from 'react-jwt';

import { AESDecrypt, AESEncrypt } from 'helpers/encryptdecrypt';
import useLocalStorage from 'store/localStorage/hooks/useLocalStorage';
import { getValue, clear } from 'store/localStorage/useLocalStorage';

export const AuthContext = createContext(null);

const AuthProvider = ({ children }) => {
  const [currentUser, setUserData] = useState(null);
  const [authorized, setAuthorized] = useState(false);
  const [useStorage, setUseStorage] = useLocalStorage('auth', null);
  const [storeLocationAndFacility, setStoreLocationAndFacility] =
    useLocalStorage('locationAndFacility', null);
  const [currentLocationAndFacility, setCurrentLocationAndFacility] =
    useState(null);

  const login = (userPayload) => {
    const decodedToken = decodeToken(userPayload);
    setUserData(decodedToken);
    setUseStorage(true);
  };

  const setFacilityAndLocation = (locationPayload) => {
    setStoreLocationAndFacility(locationPayload.encryptedLocation);
    setCurrentLocationAndFacility(locationPayload.locationFacility);
  };

  const updateLocationAndFacility = (locationPayload) => {
    setFacilityAndLocation(locationPayload);
  };

  const updateCurrentUser = (userPayload) => {
    login(userPayload);
  };

  const logout = () => {
    setUserData(null);
    setUseStorage(null);
    setStoreLocationAndFacility(null);
    setCurrentLocationAndFacility(null);
    setAuthorized(false);
    clear();
    window.location.reload();
  };

  const handleSetAuthorized = () => {
    setAuthorized(!authorized);
  };

  const isAuth = () => {
    if (currentUser !== null) return true;

    const authStateInStorage = useStorage && getValue('auth');
    const locationAndFacilityInStorage =
      storeLocationAndFacility && JSON.parse(getValue('locationAndFacility'));

    if (authStateInStorage && authStateInStorage !== 'false') {
      login(getValue('token'));
      if (
        locationAndFacilityInStorage &&
        locationAndFacilityInStorage !== null
      ) {
        const locationAndFacilityData = AESDecrypt(
          locationAndFacilityInStorage,
        );
        const locationAndFacilityPayload = {
          locationFacility: locationAndFacilityData,
          encryptedLocation: locationAndFacilityInStorage,
        };
        setFacilityAndLocation(locationAndFacilityPayload);
      }
      return true;
    }

    return false;
  };

  const isSuperAdmin = () => currentUser && currentUser.Claim_UserRole === '1';

  const isAdmin = () => currentUser && currentUser.Claim_UserRole === '2';

  const isOpManager = () => currentUser && currentUser.Claim_UserRole === '3';

  const isOperator = () => currentUser && currentUser.Claim_UserRole === '4';

  const isInventoryViewer = () =>
    currentUser && currentUser.Claim_UserRole === '5';

  const props = useMemo(
    () => ({
      login,
      logout,
      authorized,
      handleSetAuthorized,
      useStorage,
      setUseStorage,
      updateCurrentUser,
      setFacilityAndLocation,
      storeLocationAndFacility,
      currentLocationAndFacility,
      updateLocationAndFacility,
      currentUser,
      isOperator: Boolean(isOperator()),
      isOpManager: Boolean(isOpManager()),
      isSuperAdmin: Boolean(isSuperAdmin()),
      isAdmin: Boolean(isAdmin()),
      isInventoryViewer: Boolean(isInventoryViewer()),
      isAuth: Boolean(isAuth()),
    }),
    [
      currentUser,
      currentLocationAndFacility,
      authorized,
      storeLocationAndFacility,
    ],
  );

  return <AuthContext.Provider value={props}>{children}</AuthContext.Provider>;
};

export default React.memo(AuthProvider);
