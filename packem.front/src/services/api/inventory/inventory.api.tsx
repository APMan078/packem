import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const getCustomerInventory = async (customerId) => {
  try {
    const response = await httpClient.get(
      `/Inventory/CreateInventory/Get/${customerId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const createNewInventoryItem = async (inventoryData) => {
  try {
    const response = await httpClient.post(
      `/Inventory/CreateInventory`,
      inventoryData,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
