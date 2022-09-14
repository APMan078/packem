import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const createSalesOrder = async (salesOrderForm) => {
  try {
    const response = await httpClient.post(
      `SaleOrder/CreateSaleOrder`,
      salesOrderForm,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const addImportedSaleOrders = async (customerLocationId, data) => {
  try {
    const response = await httpClient.post(
      `/SaleOrder/AddImportedSaleOrders/${customerLocationId}`,
      data,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getAllSalesOrdersByLocationAndFacility = async (
  customerLocationId,
  customerFacilityId,
) => {
  try {
    const response = await httpClient.get(
      `/SaleOrder/All/${customerLocationId}/${customerFacilityId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getSaleOrderById = async (saleOrderId) => {
  try {
    const response = await httpClient.get(`/SaleOrder/Detail/${saleOrderId}`);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const updateSalesOrderPrintedStatus = async (saleOrderId) => {
  try {
    const response = await httpClient.post(
      `/SaleOrder/Update/Status/Printed/${saleOrderId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const printSalesOrderById = async (saleOrderId) => {
  try {
    const response = await httpClient.get(`/SaleOrder/Print/${saleOrderId}`);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const printMultipleSalesOrderByIds = async (saleOrderIds) => {
  try {
    const response = await httpClient.post(
      `/SaleOrder/PrintMultiple`,
      saleOrderIds,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const addOrderLineItem = async (orderLineForm) => {
  try {
    const response = await httpClient.post(
      `/OrderLine/CreateOrderLine`,
      orderLineForm,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const setSaleOrderStatusPrinted = async (saleOrderId) => {
  try {
    const response = await httpClient.post(
      `/SaleOrder/Update/Status/Printed/${saleOrderId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const removeOrderLineFromOrder = async (deleteForm) => {
  try {
    const response = await httpClient.post(`/OrderLine/Delete`, deleteForm);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const editOrderLineItem = async (editForm) => {
  try {
    const response = await httpClient.post(`/OrderLine/Update`, editForm);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getSaleOrderPickQueue = async (
  customerLocationId,
  customerFacilityId,
) => {
  try {
    const response = await httpClient.get(
      `SaleOrder/Queue/${customerLocationId}/${customerFacilityId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
