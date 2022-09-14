import { AESEncrypt } from 'helpers/encryptdecrypt';
import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';
import { setValue } from 'store/localStorage/useLocalStorage';

import { getCustomerFacilitiesByCustomerId } from '../customerfacilities/customerfacilities.api';
import { getCustomerLocationsById } from '../customerlocations/customerlocations.api';
import { getUser } from '../user/users.api';

export const login = async (data) => {
  try {
    const response = await httpClient.post('Auth/RequestUserToken', data);
    setValue('token', response.data.token);

    const user = await getUser(response.data.userId);

    return { token: response.data.token, userData: user.customerId };
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getDefaultLocationAndFacility = async (customerId) => {
  try {
    const customerLocationsFromApi: any[] = await getCustomerLocationsById(
      customerId,
    );

    const customerFacilitiesFromApi: any[] =
      await getCustomerFacilitiesByCustomerId(customerId);

    const defaultFacility: any = customerFacilitiesFromApi.filter(
      (f) =>
        f.customerLocationId === customerLocationsFromApi[0].customerLocationId,
    );

    const locationAndFacilityData = defaultFacility.map((facility) => ({
      customerFacilityId: facility.customerFacilityId,
      customerLocationId: facility.customerLocationId,
      locationName: customerLocationsFromApi[0].name,
      facilityName: facility.name,
      facilityAddress: facility.address,
      facilityAddress2: facility.address2,
      facilityCity: facility.city,
      facilityZip: facility.zipPostalCode,
      facilityStateProvince: facility.stateProvince,
      facilityPhoneNumber: facility.phoneNumber,
    }));

    const encryptData: string = AESEncrypt(locationAndFacilityData[0]);

    setValue('locationAndFacility', encryptData);

    return {
      locationFacility: locationAndFacilityData[0],
      encryptedLocation: encryptData,
    };
  } catch (error) {
    throw errorCatch(error);
  }
};

export const requestResetPasswordToken = async (requestForm) => {
  try {
    const response = await httpClient.post(
      `/Auth/RequestPasswordResetTokenEmail`,
      requestForm,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const resetPassword = async (resetForm) => {
  try {
    const response = await httpClient.post(`/Auth/ResetPassword`, resetForm);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
