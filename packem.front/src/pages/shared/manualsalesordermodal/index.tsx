/* eslint-disable no-nested-ternary */
import React, {
  useContext,
  useState,
  ChangeEvent,
  useEffect,
  useRef,
} from 'react';
import Barcode from 'react-jsbarcode';

import Button from 'components/button';
import DatePickerInput from 'components/datepicker';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import moment from 'moment';
import { getAllOrderCustomersByCustomerId } from 'services/api/ordercustomers';
import { createSalesOrder } from 'services/api/salesorders/salesorders.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import Autocomplete from '@mui/material/Autocomplete';
import { useTheme } from '@mui/material/styles';
import TextField from '@mui/material/TextField';

interface AutoCompleteOptionType {
  id: number;
  label: string;
}

interface ManualSaleOrderProps {
  customerDetails?: any;
  fromCustomerDetails?: boolean;
}

export default React.memo(
  ({ customerDetails, fromCustomerDetails }: ManualSaleOrderProps) => {
    const theme = useTheme();
    const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
    const labelRef = useRef(null);
    const {
      isManualSalesOrderOpen,
      onCloseManualSalesOrderModal,
      handleUpdateData,
    } = useContext(GlobalContext);
    const [showErrors, setShowErrors] = useState(false);
    const [error, setError] = useState('');
    const [orderCustomers, setOrderCustomers] = useState([]);
    const defaultAutoComplete: AutoCompleteOptionType = {
      id: -1,
      label: '',
    };

    const initialFormErrorsState: any = {
      customerId: '',
      customerLocationId: '',
      customerFacilityId: '',
      saleOrderNo: '',
      orderDate: '',
      promiseDate: '',
      orderCustomerId: '',
    };
    const initialState: any = {
      customerId: currentUser.Claim_CustomerId,
      customerLocationId: currentLocationAndFacility.customerLocationId,
      customerFacilityId: currentLocationAndFacility.customerFacilityId,
      saleOrderNo: '',
      orderDate: moment(),
      promiseDate: moment(),
      orderCustomerId: defaultAutoComplete,
    };
    const [formErrors, setFormErrors] = useState<any>(initialFormErrorsState);
    const [form, setForm] = useState<any>(initialState);

    const customerSelectionOptions = Array.isArray(orderCustomers)
      ? orderCustomers.map((o) => ({
          id: o.orderCustomerId,
          label: o.name,
        }))
      : [{ id: 0, label: '' }];

    const schema = yup.object().shape({
      customerId: yup.string().required('Required'),
      customerLocationId: yup.string().required('Required'),
      customerFacilityId: yup.string().required('Required'),
      orderDate: yup.string().required('Required'),
      promiseDate: yup.string().required('Required'),
      orderCustomerId: yup.number().required('Required'),
    });

    const onForm = (key, text) => {
      setForm(() => ({
        ...form,
        [key]: text,
      }));
    };

    const autoCompleteInputHandler = (
      key: string,
      value: AutoCompleteOptionType,
    ) => {
      setForm(() => ({
        ...form,
        [key]: value === null ? defaultAutoComplete : value,
      }));
    };

    const inputHandler = (
      key: string,
      event: ChangeEvent<HTMLInputElement>,
    ) => {
      onForm(key, event.target.value);
    };

    const orderDateInputHandler = (newValue: Date | null) => {
      onForm('orderDate', newValue);
    };

    const promiseDateInputHandler = (newValue: Date | null) => {
      onForm('promiseDate', newValue);
    };

    const handleCloseManualSaleOrderModal = () => {
      setForm(initialState);
      setFormErrors(initialFormErrorsState);
      setError('');
      onCloseManualSalesOrderModal();
    };

    const onSaveNewManualSaleOrder = async () => {
      const newForm = form;
      if (!fromCustomerDetails) {
        newForm.orderCustomerId = form.orderCustomerId.id;
      } else {
        newForm.orderCustomerId = customerDetails.orderCustomerId;
      }
      schema
        .validate(newForm, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            await createSalesOrder(newForm);
            snackActions.success(`Successfully created new item.`);
            handleCloseManualSaleOrderModal();
            handleUpdateData();
          } catch (err: any) {
            setError(err);
            setFormErrors(initialFormErrorsState);
            setShowErrors(true);
            snackActions.error(`${err}`);
          }
        })
        .catch((err) => {
          const errorsFound = err.inner.reduce((acc, curr) => {
            if (!acc[curr.path]) acc[curr.path] = curr.message;
            return acc;
          }, {});
          setFormErrors(errorsFound);
          setError('');
          setShowErrors(true);
        });
    };

    const onLoadOrderCustomers = async () => {
      try {
        const orderCustomersFromApi = await getAllOrderCustomersByCustomerId(
          currentUser.Claim_CustomerId,
        );

        setOrderCustomers(orderCustomersFromApi);

        return true;
      } catch (err: any) {
        return err;
      }
    };

    useEffect(() => {
      setForm(initialState);
      setOrderCustomers([]);
      onLoadOrderCustomers();
    }, [currentLocationAndFacility]);

    if (!isManualSalesOrderOpen) return null;

    return (
      <Modal
        open={isManualSalesOrderOpen}
        onClose={() => handleCloseManualSaleOrderModal()}
      >
        <ModalBox>
          <ModalContent>
            <Typography
              sx={{ marginBottom: '16px' }}
              variant="h6"
              fontWeight="bold"
            >
              Manual Sales Order
            </Typography>
            {!showErrors ? (
              <>
                <DatePickerInput
                  label="Order Date"
                  inputFormat="MM/dd/yyyy"
                  value={form.orderDate}
                  onChange={orderDateInputHandler}
                  renderInput={(params) => (
                    <TextField {...params} error={false} size="small" />
                  )}
                />
                <DatePickerInput
                  label="Promised Date"
                  inputFormat="MM/dd/yyyy"
                  value={form.promiseDate}
                  onChange={promiseDateInputHandler}
                  renderInput={(params) => (
                    <TextField {...params} error={false} size="small" />
                  )}
                />
                {!fromCustomerDetails && (
                  <Autocomplete
                    sx={{ width: '100%' }}
                    options={customerSelectionOptions}
                    getOptionLabel={(option: AutoCompleteOptionType) =>
                      `${option.label}` || ''
                    }
                    size="small"
                    value={form.orderCustomerId}
                    onChange={(
                      event: any,
                      value: AutoCompleteOptionType | null,
                    ) => {
                      autoCompleteInputHandler('orderCustomerId', value);
                    }}
                    renderInput={(params) => (
                      <TextField {...params} label="Customer" />
                    )}
                  />
                )}
              </>
            ) : (
              <>
                <DatePickerInput
                  label="Order Date"
                  inputFormat="MM/dd/yyyy"
                  value={form.orderDate}
                  onChange={orderDateInputHandler}
                  renderInput={(params) =>
                    formErrors.orderDate ? (
                      <TextField
                        {...params}
                        size="small"
                        error
                        helperText={formErrors.orderDate}
                      />
                    ) : (
                      <TextField {...params} error={false} size="small" />
                    )
                  }
                />
                <DatePickerInput
                  label="Promised Date"
                  inputFormat="MM/dd/yyyy"
                  value={form.promiseDate}
                  onChange={promiseDateInputHandler}
                  renderInput={(params) =>
                    formErrors.promiseDate ? (
                      <TextField
                        {...params}
                        size="small"
                        error
                        helperText={formErrors.promiseDate}
                      />
                    ) : (
                      <TextField {...params} error={false} size="small" />
                    )
                  }
                />
                {!fromCustomerDetails && (
                  <Autocomplete
                    sx={{ width: '100%' }}
                    options={customerSelectionOptions}
                    getOptionLabel={(option: AutoCompleteOptionType) =>
                      `${option.label}` || ''
                    }
                    size="small"
                    value={form.orderCustomerId}
                    onChange={(
                      event: any,
                      value: AutoCompleteOptionType | null,
                    ) => {
                      autoCompleteInputHandler('orderCustomerId', value);
                    }}
                    renderInput={(params) =>
                      formErrors.orderCustomerId ? (
                        <TextField
                          {...params}
                          label="Customer"
                          error
                          helperText={formErrors.orderCustomerId}
                        />
                      ) : (
                        <TextField {...params} label="Customer" />
                      )
                    }
                  />
                )}
              </>
            )}
            <Typography variant="subtitle2" color="#d32f2f">
              {error}
            </Typography>
            <Box
              sx={{
                display: 'flex',
                width: '100%',
                [theme.breakpoints.up('sm')]: {
                  justifyContent: 'flex-end',
                },
                [theme.breakpoints.down('sm')]: {
                  justifyContent: 'center',
                },
                gap: '8px',
              }}
            >
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="text"
                size="large"
                onClick={() => handleCloseManualSaleOrderModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '150px' }}
                variant="contained"
                size="large"
                onClick={() => onSaveNewManualSaleOrder()}
              >
                Submit
              </Button>
            </Box>
          </ModalContent>
        </ModalBox>
      </Modal>
    );
  },
);
