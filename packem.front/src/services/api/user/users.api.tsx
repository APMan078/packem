import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const getUser = async (userId) => {
  try {
    const response = await httpClient.get(`/User/${userId}`);

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const getUsers = async () => {
  try {
    const response = await httpClient.get(`/User/Users`);

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const getUserVendor = async (userId) => {
  try {
    const response = await httpClient.get(`/User/Vendor/${userId}`);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getUsersByCustomerId = async (customerId) => {
  try {
    const response = await httpClient.get(`/User/customer/${customerId}`);

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const createUser = async (data) => {
  try {
    const response = await httpClient.post(`/User/CreateUser`, data);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const editUser = async (userId, editForm) => {
  try {
    const response = await httpClient.put(`/User/${userId}`, editForm);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const changeUserActiveStatus = async (statusForm) => {
  try {
    const response = await httpClient.post(`/User/Update/isActive`, statusForm);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
