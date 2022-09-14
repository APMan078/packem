import React, {
  useContext,
  useState,
  ChangeEvent,
  useEffect,
  useRef,
} from 'react';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import {
  createOrderCustomer,
  editOrderCustomer,
  createOrderCustomerAddress,
} from 'services/api/ordercustomers';
import { createReceive } from 'services/api/receive/receive.api';
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

interface OrderCustomerModalProps {
  callBack?: () => void;
  edit?: boolean;
  orderCustomer?: string;
}

export default React.memo(
  ({ callBack, edit, orderCustomer }: OrderCustomerModalProps) => {
    const theme = useTheme();
    const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
    const {
      isOrderCustomerModalOpen,
      onCloseOrderCustomerModal,
      handleUpdateData,
    } = useContext(GlobalContext);
    const [currentView, setCurrentView] = useState(0);
    const defaultAutocompleteOption: AutoCompleteOptionType | null = {
      id: -1,
      label: '',
    };
    const [showErrors, setShowErrors] = useState(false);
    const [error, setError] = useState('');
    const initialFormErrorsState: any = {
      customerLocationId: '',
      itemId: '',
      qty: '',
      uom: '',
      itemDescription: '',
    };
    const initialOrderCustomerState: any = {
      customerId: currentUser.Claim_CustomerId,
      name: '',
      paymentType: {
        id: defaultAutocompleteOption.id,
        label: defaultAutocompleteOption.label,
      },
    };
    const initialOrderCustomerAddress: any = {
      orderCustomerId: '',
      addressType: '',
      address1: '',
      address2: '',
      stateProvince: '',
      zipPostalCode: '',
      country: '',
      phoneNumber: '',
    };
    const [formErrors, setFormErrors] = useState<any>(initialFormErrorsState);
    const [form, setForm] = useState<any>(initialOrderCustomerState);
    const [addressForm, setAddressForm] = useState<any>(
      initialOrderCustomerAddress,
    );

    const schema = yup.object().shape({
      customerId: yup.number().required('Required.'),
      name: yup.string().required(),
      paymentType: yup.number().typeError('Required.'),
    });

    const onForm = (key, text) => {
      setForm(() => ({
        ...form,
        [key]: text,
      }));
    };

    const inputHandler = (
      key: string,
      event: ChangeEvent<HTMLInputElement>,
    ) => {
      onForm(key, event.target.value);
    };

    const autoCompleteInputHandler = (
      key: string,
      value: AutoCompleteOptionType,
    ) => {
      setForm(() => ({
        ...form,
        [key]:
          value === null
            ? {
                id: defaultAutocompleteOption.id,
                label: defaultAutocompleteOption.label,
              }
            : value,
      }));
    };

    const handleNextView = () => {
      setCurrentView(1);
    };

    const handleCloserOrderCustomerModal = () => {
      if (edit) {
        callBack();
      }
      setForm(initialOrderCustomerState);
      setFormErrors(initialFormErrorsState);
      setError('');
      onCloseOrderCustomerModal();
      setCurrentView(0);
    };

    const onSaveNewOrderCustomer = async () => {
      form.paymentType = 2;
      schema
        .validate(form, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            const customer = createOrderCustomer(form);
            snackActions.success(
              `Successfully added new customer, ${form.name}`,
            );
            handleCloserOrderCustomerModal();
            handleUpdateData();
          } catch (err: any) {
            setError(err);
            setFormErrors(initialFormErrorsState);
            setShowErrors(true);
            snackActions.error(`${error}`);
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

    useEffect(() => {
      setForm(initialOrderCustomerState);
    }, [currentLocationAndFacility]);

    if (!isOrderCustomerModalOpen) return null;

    return (
      <Modal
        open={isOrderCustomerModalOpen}
        onClose={() => handleCloserOrderCustomerModal()}
      >
        <ModalBox>
          <ModalContent>
            <Typography
              sx={{ marginBottom: '16px' }}
              variant="h6"
              fontWeight="bold"
            >
              Add Customer
            </Typography>
            {!showErrors ? (
              <Input
                sx={{ width: '100%' }}
                placeholder="Name"
                value={form.name}
                onChange={(value) => inputHandler('name', value)}
              />
            ) : (
              <Input
                sx={{ width: '100%' }}
                placeholder="Name"
                value={form.name}
                error={formErrors.name}
                onChange={(value) => inputHandler('name', value)}
              />
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
                onClick={() => handleCloserOrderCustomerModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '150px' }}
                variant="contained"
                size="large"
                onClick={() => onSaveNewOrderCustomer()}
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
