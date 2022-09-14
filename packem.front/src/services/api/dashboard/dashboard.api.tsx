import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const getDashboardInventoryFlow = async (
  customerLocationId,
  customerFacilityId,
  dateFilter,
) => {
  try {
    const response = await httpClient.get(
      `/Dashboard/InventoryFlow/${customerLocationId}/${customerFacilityId}/${dateFilter}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getDashboardQueues = async (
  customerLocationId,
  customerFacilityId,
  days,
) => {
  try {
    const response = await httpClient.get(
      `/Dashboard/Queues/${customerLocationId}/${customerFacilityId}/${days}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getDashboardTopSalesOrders = async (
  customerLocationId,
  customerFacilityId,
  dateFilter,
) => {
  try {
    const response = await httpClient.get(
      `/Dashboard/TopSalesOrders/${customerLocationId}/${customerFacilityId}/${dateFilter}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getDashboardLowStock = async (customerId) => {
  try {
    const response = await httpClient.get(`/Dashboard/LowStock/${customerId}`);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getDashboardOperations = async (
  customerLocationId,
  customerFacilityId,
  days,
) => {
  try {
    const response = await httpClient.get(
      `/Dashboard/Operations/${customerLocationId}/${customerFacilityId}/${days}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
