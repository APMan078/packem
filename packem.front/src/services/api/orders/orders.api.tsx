import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const getCustomerOrders = async (customerId) => {
  try {
    const response = await httpClient.get(``);

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};
