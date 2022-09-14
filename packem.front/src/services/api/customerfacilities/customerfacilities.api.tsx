import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const getAllCustomerFacilities = async () => {
  try {
    const response = await httpClient.get(`/CustomerFacility/all`);

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};
export const getCustomerFacilitiesByCustomerId = async (customerId) => {
  try {
    const response = await httpClient.get(
      `/CustomerFacility/CustomerFacilities/Customer/${customerId}`,
    );

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const getCustomerFacilityByFacilityId = async (customerFacilityId) => {
  try {
    const response = await httpClient.get(
      `/CustomerFacility/${customerFacilityId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const createCustomerFacility = async (data) => {
  try {
    const response = await httpClient.post(
      `/customerfacility/createcustomerfacility`,
      data,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const editCustomerFacility = async (
  customerFacilityId,
  editFormData,
) => {
  try {
    const response = await httpClient.put(
      `/CustomerFacility/${customerFacilityId}`,
      editFormData,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const deleteCustomerFacility = async (deleteForm) => {
  try {
    const response = await httpClient.post(
      `/CustomerFacility/Delete`,
      deleteForm,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getCustomerFacilityZones = async (customerLocationId) => {
  try {
    const response = await httpClient.get(`/Zone/Zones/${customerLocationId}`);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const lookupCustomerBinByZoneId = async (zoneId, form) => {
  try {
    const response = await httpClient.get(`/Bin/Lookup/${zoneId}`, form);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const lookupCustomerZonesByFacilityId = async (
  customerFacilityZone,
  form,
) => {
  try {
    const response = await httpClient.get(
      `/Zone/Lookup/${customerFacilityZone}`,
      form,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const createCustomerFacilityZone = async (zoneData) => {
  try {
    const response = await httpClient.post(`/Zone/CreateZone`, zoneData);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const editCustomerFacilityZone = async (zoneId, editForm) => {
  try {
    const response = await httpClient.put(`/Zone/${zoneId}`, editForm);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const deleteCustomerFacilityZone = async (deleteForm) => {
  try {
    const response = await httpClient.post(`/Zone/Delete`, deleteForm);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const createCustomerBin = async (binData) => {
  try {
    const response = await httpClient.post(`/Bin/CreateBin`, binData);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getCustomerBinsByLocationId = async (customerLocationId) => {
  try {
    const response = await httpClient.get(`/Bin/Bins/${customerLocationId}`);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getCustomerBinByBinId = async (binId) => {
  try {
    const response = await httpClient.get(`/Bin/${binId}`);

    return response.data;
  } catch (error) {
    return errorCatch(error);
  }
};

export const adjustCustomerBinQty = async (adjustData) => {
  try {
    const response = await httpClient.post(
      `/AdjustBinQuantity/CreateAdjustBinQuantity`,
      adjustData,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const editCustomerBinData = async (binId, editFormData) => {
  try {
    const response = await httpClient.put(`/Bin/${binId}`, editFormData);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getCustomerBinAdjustmentQueue = async (
  itemId,
  customerLocationId,
  binId,
) => {
  try {
    const response = await httpClient.get(
      `/AdjustBinQuantity/CreateAdjustBinQuantity/${itemId}/${customerLocationId}/${binId}'`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getStorageManagementDetails = async (
  customerLocationId,
  customerFacilityId,
) => {
  try {
    const response = await httpClient.get(
      `/Bin/StorageManagement/${customerLocationId}/${customerFacilityId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getBinDetails = async (binId) => {
  try {
    const response = await httpClient.get(`bin/storagemanagement/detail/${binId}
    `);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const addImportedZonesAndBins = async (customerLocationId, data) => {
  try {
    const response = await httpClient.post(
      `/Bin/AddImportedZonesBins/${customerLocationId}`,
      data,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
