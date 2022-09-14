import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const createPurchaseOrder = async (data) => {
  try {
    const response = await httpClient.post(
      `/PurchaseOrder/CreatePurchaseOrder`,
      data,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const addImportedPurchaseOrders = async (customerLocationId, data) => {
  try {
    const response = await httpClient.post(
      `/PurchaseOrder/AddImportedPurchaseOrders/${customerLocationId}`,
      data,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const deletePurchaseOrder = async (deleteForm) => {
  try {
    const response = await httpClient.post(`/PurchaseOrder/Delete`, deleteForm);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getPurchaseOrder = async (purchaseOrderId) => {
  try {
    const response = await httpClient.get(
      `/PurchaseOrder/Detail/${purchaseOrderId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getPurchaseOrdersByCustomerLocationId = async (
  customerLocationId,
) => {
  try {
    const response = await httpClient.get(
      `/PurchaseOrder/PurchaseOrders/${customerLocationId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getPurchaseOrdersByCustomerId = async () => {
  try {
    const response = await httpClient.get(
      `/PurchaseOrder/PurchaseOrders/GetPurchaseOrdersByCustomerId`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const removePurchaseOrder = async (data) => {
  try {
    const response = await httpClient.post(`/PurchaseOrder/Delete`, data);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
