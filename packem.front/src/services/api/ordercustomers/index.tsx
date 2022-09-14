import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const createOrderCustomer = async (orderCustomerForm) => {
  try {
    const response = await httpClient.post(
      `/OrderCustomer/Create`,
      orderCustomerForm,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const editOrderCustomer = async (orderCustomerForm) => {
  try {
    const response = await httpClient.post(
      `/OrderCustomer/Edit`,
      orderCustomerForm,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const deleteOrderCustomer = async (orderCustomerDeleteForm) => {
  try {
    const response = await httpClient.post(
      `/OrderCustomer/Delete`,
      orderCustomerDeleteForm,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getAllOrderCustomersByCustomerId = async (customerId) => {
  try {
    const response = await httpClient.get(`/OrderCustomer/All/${customerId}`);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getOrderCustomerByOrderCustomerId = async (orderCustomerId) => {
  try {
    const response = await httpClient.get(`/OrderCustomer/${orderCustomerId}`);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getOrderCustomerManagementData = async (customerId) => {
  try {
    const response = await httpClient.get(
      `/OrderCustomer/Management/${customerId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getOrderCustomerDetails = async (orderCustomerId) => {
  try {
    const response = await httpClient.get(
      `/OrderCustomer/Detail/${orderCustomerId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const createOrderCustomerAddress = async (orderCustomerAddressForm) => {
  try {
    const response = await httpClient.post(
      `/OrderCustomerAddress/Create`,
      orderCustomerAddressForm,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const editOrderCustomerAddress = async (
  orderCustomerAddressEditForm,
) => {
  try {
    const response = await httpClient.post(
      `/OrderCustomerAddress/Edit`,
      orderCustomerAddressEditForm,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const deleteOrderCustomerAddress = async (orderCustomerDeleteForm) => {
  try {
    const response = await httpClient.post(
      `/OrderCustomerAddress/Delete`,
      orderCustomerDeleteForm,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getAllOrderCustomerAddresses = async (orderCustomerId) => {
  try {
    const response = await httpClient.get(
      `/OrderCustomerAddress/All/${orderCustomerId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getOrderCustomerAddressById = async (orderCustomerAddressId) => {
  try {
    const response = await httpClient.get(
      `/OrderCustomerAddress/${orderCustomerAddressId}`,
      orderCustomerAddressId,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
