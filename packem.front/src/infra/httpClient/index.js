import axios from 'axios';
import { API_URL } from 'config/constants';
import { environmentVariable } from 'store/environment';
import { getValue } from 'store/localStorage/useLocalStorage';

const httpClient = axios.create({
  baseURL: environmentVariable(API_URL),
  timeout: 300000,
});

httpClient.interceptors.request.use((request) => {
  const token = getValue('token');
  if (token) {
    request.headers.common.Authorization = `Bearer ${token}`;
  }
  return request;
});

httpClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response.status === 401) {
      localStorage.clear();
      window.location.href = '/feed';
    }
    return Promise.reject(error);
  },
);

export default httpClient;
