import httpClient from 'infra/httpClient/index.js';
import { errorCatch } from 'services/api/serivceBase.js';

export const getTransferHistory = async (
  customerLocationId,
  customerFacilityId,
) => {
  try {
    const response = await httpClient.get(
      `/transfer/transferhistory/${customerLocationId}/${customerFacilityId}`,
    );

    return response.data;
  } catch (error) {
    throw errorCatch(error);
  }
};
