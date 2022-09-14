import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

import { responsiveFontSizes } from '@mui/material';

export const getCustomers = async () => {
  try {
    const response = await httpClient.get('/Customer/Customers');

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const getCustomerById = async (customerId) => {
  try {
    const response = await httpClient.get(`/Customer/${customerId}`);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getCustomerDefaultThreshold = async (customerId) => {
  try {
    const response = await httpClient.get(
      `/Customer/GetDefaultThreshold/${customerId}`,
    );
    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const updateCustomerDefaultThreshold = async (customerId, threshold) => {
  try {
    const response = await httpClient.put(
      `/Customer/UpdateDefaultThreshold/${customerId}/${threshold}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const createCustomer = async (data) => {
  try {
    const response = await httpClient.post(`/Customer/CreateCustomer`, data);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const editCustomer = async (customerId, editForm) => {
  try {
    const response = await httpClient.put(`/Customer/${customerId}`, editForm);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const updateCustomerIsActive = async (isActiveData) => {
  try {
    const response = await httpClient.post(
      `/Customer/Update/IsActive`,
      isActiveData,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getCustomerVendors = async (customerId) => {
  try {
    const response = await httpClient.get(`/Vendor/VendorItems/${customerId}`);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const createCustomerVendor = async (data) => {
  try {
    const response = await httpClient.post(`/Vendor/CreateVendor`, data);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
