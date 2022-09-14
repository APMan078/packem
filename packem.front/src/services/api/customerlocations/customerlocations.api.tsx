import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const getAllCustomerLocations = async () => {
  try {
    const response = await httpClient.get(
      '/CustomerLocation/CustomerLocations',
    );

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const getCustomerLocationsById = async (customerId) => {
  try {
    const response = await httpClient.get(
      `/CustomerLocation/CustomerLocations/${customerId}`,
    );

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const createCustomerLocation = async (data) => {
  try {
    const response = await httpClient.post(
      `/CustomerLocation/CreateCustomerLocation`,
      data,
    );
    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const editCustomerLocation = async (customerLocationId, editForm) => {
  try {
    const response = await httpClient.put(
      `/CustomerLocation/${customerLocationId}`,
      editForm,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const deleteCustomerLocation = async (deleteForm) => {
  try {
    const response = await httpClient.post(
      `/CustomerLocation/Delete`,
      deleteForm,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
