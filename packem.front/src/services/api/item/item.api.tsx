import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

interface CustomerInventoryGetProps {
  customerId: string;
  searchString?: string;
}

export const getCustomerInventoryByCustomerId = async ({
  customerId,
  searchString,
}: CustomerInventoryGetProps) => {
  try {
    const response = await httpClient.get(`/Item/Lookup/1${customerId}`);

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const createItem = async (itemData) => {
  try {
    const response = await httpClient.post(`/Item/CreateItem`, itemData);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const updateItemExpirationDate = async (itemData) => {
  try {
    const response = await httpClient.post(
      `/Item/Update/ExpirationDate`,
      itemData,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const updateItemThreshold = async (itemData) => {
  try {
    const response = await httpClient.post(`/Item/Update/Threshold`, itemData);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const deleteItem = async (deleteForm) => {
  try {
    const response = await httpClient.post(`/Item/Delete`, deleteForm);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getItemById = async (itemId) => {
  try {
    const response = await httpClient.get(`/Item/${itemId}`);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const searchItemBySKU = async (customerId, itemSKU) => {
  try {
    const response = await httpClient.get(
      `/Item/Lookup/SKU/${customerId}?sku=${itemSKU}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getItemDetails = async (customerId, itemId) => {
  try {
    const response = await httpClient.get(
      `/Item/Detail/${customerId}/${itemId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getItemLookup = async (customerId, searchText) => {
  try {
    const response = await httpClient.get(
      `/Item/Lookup/${customerId}?searchText=${searchText}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const createItemTransfer = async (transferForm) => {
  try {
    const response = await httpClient.post(
      `/Transfer/CreateTransfer`,
      transferForm,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
