/* eslint-disable no-param-reassign */
export const errorCatch = (error) => {
  const errorString = error.response.data;
  return errorString;
};

export const transformToFormData = (object) =>
  Object.keys(object).reduce((formData, key) => {
    formData.append(key, object[key]);
    return formData;
  }, new FormData());
