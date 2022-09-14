import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const getLotLookup = async (itemId) => {
  try {
    const response = await httpClient.get(`/Lot/Lookup/${itemId}`);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const createLot = async (lot) => {
  try {
    const response = await httpClient.post(`/Lot/Create`, lot);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
