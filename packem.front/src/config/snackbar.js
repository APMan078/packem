/* eslint-disable react/destructuring-assignment */
import React from 'react';

import { useSnackbar } from 'notistack';

const InnerSnackbarUtilsConfig = (props) => {
  props.setUseSnackbarRef(useSnackbar());
  return null;
};

let useSnackbarRef;
const setUseSnackbarRef = (useSnackbarRefProp) => {
  useSnackbarRef = useSnackbarRefProp;
};

export const SnackbarConfig = () => (
  <InnerSnackbarUtilsConfig setUseSnackbarRef={setUseSnackbarRef} />
);

export const snackActions = {
  success(msg) {
    this.toast(msg, 'success');
  },
  warning(msg) {
    this.toast(msg, 'warning');
  },
  info(msg) {
    this.toast(msg, 'info');
  },
  error(msg) {
    this.toast(msg, 'error');
  },
  toast(msg, variant = 'default') {
    useSnackbarRef.enqueueSnackbar(msg, { variant });
  },
};
