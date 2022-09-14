import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const createReceipt = async (data) => {
  try {
    const response = await httpClient.post(`/Receipt/CreateReceipt`, data);

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const getReceiptQueue = async (
  customerLocationId,
  customerFacilityId,
) => {
  try {
    const response = await httpClient.get(
      `/Receipt/Queue/${customerLocationId}/${customerFacilityId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
