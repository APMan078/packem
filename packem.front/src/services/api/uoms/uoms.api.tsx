import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const getCustomerUnitOfMeasures = async (customerId) => {
  try {
    const response = await httpClient.get(
      `/UnitOfMeasure/Lookup/${customerId}`,
    );

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const getDefaultUnitOfMeasures = async (searchText) => {
  try {
    const response = await httpClient.get(
      `/UnitOfMeasure/Lookup/${searchText}`,
    );

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const createUnitOfMeasureForCustomer = async (data) => {
  try {
    const response = await httpClient.post(
      `/UnitOfMeasure/CreatUnitOfMeasure`,
      data,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const creatCustomUnitOfMeasure = async (data) => {
  try {
    const response = await httpClient.post(
      `/UnitOfMeasure/CreatUnitOfMeasure/Custom`,
      data,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const deleteCustomerUnitOfMeasure = async (data) => {
  try {
    const response = await httpClient.post(`/UnitOfMeasure/Delete`, data);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
