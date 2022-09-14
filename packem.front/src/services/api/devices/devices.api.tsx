import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const getCustomerDevicesByCustomerId = async (customerId) => {
  try {
    const response = await httpClient.get(
      `/CustomerDevice/CustomerDevices/all/${customerId}`,
    );

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const getCustomerDevicesByLocationId = async (locationId) => {
  try {
    const response = await httpClient.get(
      `/CustomerDevice/CustomerDevices/${locationId}`,
    );

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const registerDevice = async (data) => {
  try {
    const response = await httpClient.post(
      `/CustomerDevice/CreateCustomerDevice`,
      data,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const editDeviceData = async (deviceId, editFormData) => {
  try {
    const response = await httpClient.put(
      `/CustomerDevice/${deviceId}`,
      editFormData,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getCustomerDeviceTokensById = async (customerDeviceId) => {
  try {
    const response = await httpClient.get(
      `/CustomerDeviceToken/CustomerDeviceTokens/${customerDeviceId}`,
    );

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const createDeviceToken = async (data) => {
  try {
    const response = await httpClient.post(
      `/CustomerDeviceToken/CreateCustomerDeviceToken`,
      data,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
