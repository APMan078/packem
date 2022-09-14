import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const createReceive = async (data) => {
  try {
    const response = await httpClient.post(`/Receive/CreateReceive`, data);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const updateReceiveQty = async (data) => {
  try {
    const response = await httpClient.post(`/Receive/Update/Qty`, data);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const cancelReceive = async (data) => {
  try {
    const response = await httpClient.post(`/Receive/Delete`, data);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
