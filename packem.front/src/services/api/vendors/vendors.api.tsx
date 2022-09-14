import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const getVendorLookup = async (customerId, searchText) => {
  try {
    const response = await httpClient.get(
      `/Vendor/Lookup/${customerId}?searchText=${searchText}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const editVendor = async (vendorId, editForm) => {
  try {
    const response = await httpClient.put(`/Vendor/${vendorId}`, editForm);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
