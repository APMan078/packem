import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const getPutAwayQueue = async (
  customerLocationId,
  customerFacilityId,
) => {
  try {
    const response = await httpClient.get(
      `/PutAway/Queue/${customerLocationId}/${customerFacilityId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};

export const createPutAwayBin = async (form) => {
  try {
    const response = await httpClient.post(
      `/PutAway/Queue/CreatePutAwayBin`,
      form,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
