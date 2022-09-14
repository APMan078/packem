import React, { useContext, useState, ChangeEvent, useEffect } from 'react';
import { useLocation } from 'react-router-dom';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import {
  createOrderCustomerAddress,
  editOrderCustomerAddress,
} from 'services/api/ordercustomers';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import Autocomplete from '@mui/material/Autocomplete';
import { useTheme } from '@mui/material/styles';
import TextField from '@mui/material/TextField';

interface OrderCustomerAddressModalProps {
  callBack?: () => void;
  edit?: boolean;
  orderCustomerAddress?: any;
  orderCustomer?: any;
  shipping?: boolean;
  billing?: boolean;
}

export default React.memo(
  ({
    callBack,
    edit,
    orderCustomerAddress,
    orderCustomer,
    shipping,
    billing,
  }: OrderCustomerAddressModalProps) => {
    const theme = useTheme();
    const location = useLocation();
    const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
    const {
      isOrderCustomerAddressModalOpen,
      onCloseOrderCustomerAddressModal,
      handleUpdateData,
    } = useContext(GlobalContext);
    const [currentView, setCurrentView] = useState(0);
    const [showErrors, setShowErrors] = useState(false);
    const [error, setError] = useState('');
    const initialFormErrorsState: any = {
      orderCustomerId: '',
      addressType: '',
      address1: '',
      address2: '',
      city: '',
      stateProvince: '',
      zipPostalCode: '',
      country: '',
      phoneNumber: '',
    };
    const initialOrderCustomerAddress: any = {
      orderCustomerId: location.pathname.split('/')[2],
      addressType: '',
      address1: '',
      address2: '',
      city: '',
      stateProvince: '',
      zipPostalCode: '',
      country: '',
      phoneNumber: '',
    };
    const [formErrors, setFormErrors] = useState<any>(initialFormErrorsState);
    const [form, setForm] = useState<any>(initialOrderCustomerAddress);

    const schema = yup.object().shape({
      orderCustomerId: yup.number().required('Required.'),
      addressType: yup.number().required('Required'),
      address1: yup.string().required('Required.'),
      address2: yup.string(),
      city: yup.string().required('Required.'),
      stateProvince: yup.string().required('Required.'),
      zipPostalCode: yup.string().required('Required.'),
      country: yup.string().required('Required.'),
      phoneNumber: yup.string().required('Required.'),
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

    const handleCloseOrderCustomerAddressModal = () => {
      if (edit) {
        callBack();
      }
      setForm(initialOrderCustomerAddress);
      setFormErrors(initialFormErrorsState);
      setError('');
      onCloseOrderCustomerAddressModal();
      setCurrentView(0);
    };

    const onSaveNewOrderCustomerAddress = async () => {
      if (shipping && !edit) {
        form.addressType = 1;
      }
      if (billing && !edit) {
        form.addressType = 2;
      }

      schema
        .validate(form, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            if (edit) {
              await editOrderCustomerAddress(form);
              snackActions.success(
                `Successfully edited customer address for ${orderCustomer.name}`,
              );
            } else {
              await createOrderCustomerAddress(form);
              snackActions.success(
                `Successfully added new address for ${orderCustomer.name}`,
              );
            }
            handleCloseOrderCustomerAddressModal();
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
      setForm(initialOrderCustomerAddress);
      if (edit) {
        setForm(orderCustomerAddress);
      }
    }, [
      currentLocationAndFacility,
      orderCustomer,
      orderCustomerAddress,
      edit,
      shipping,
      billing,
    ]);

    if (!isOrderCustomerAddressModalOpen) return null;

    return (
      <Modal
        open={isOrderCustomerAddressModalOpen}
        onClose={() => handleCloseOrderCustomerAddressModal()}
      >
        <ModalBox>
          <ModalContent>
            {edit ? (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Edit Customer Address
              </Typography>
            ) : (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Add Customer Address
              </Typography>
            )}
            {!showErrors ? (
              <>
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Street"
                  value={form.address1}
                  onChange={(value) => inputHandler('address1', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Address Line 2"
                  value={form.address2}
                  onChange={(value) => inputHandler('address2', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="City"
                  value={form.city}
                  onChange={(value) => inputHandler('city', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="State / Province"
                  value={form.stateProvince}
                  onChange={(value) => inputHandler('stateProvince', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Zip/Postal Code"
                  value={form.zipPostalCode}
                  onChange={(value) => inputHandler('zipPostalCode', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Country"
                  value={form.country}
                  onChange={(value) => inputHandler('country', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Phone Number"
                  value={form.phoneNumber}
                  onChange={(value) => inputHandler('phoneNumber', value)}
                />
              </>
            ) : (
              <>
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Street"
                  error={formErrors.address1}
                  value={form.address1}
                  onChange={(value) => inputHandler('address1', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  error={formErrors.address2}
                  placeholder="Address Line 2"
                  value={form.address2}
                  onChange={(value) => inputHandler('address2', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="City"
                  error={formErrors.city}
                  value={form.city}
                  onChange={(value) => inputHandler('city', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="State / Province"
                  error={formErrors.stateProvince}
                  value={form.stateProvince}
                  onChange={(value) => inputHandler('stateProvince', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Zip/Postal Code"
                  error={formErrors.zipPostalCode}
                  value={form.zipPostalCode}
                  onChange={(value) => inputHandler('zipPostalCode', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Country"
                  error={formErrors.country}
                  value={form.country}
                  onChange={(value) => inputHandler('country', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Phone Number"
                  error={formErrors.phoneNumber}
                  value={form.phoneNumber}
                  onChange={(value) => inputHandler('phoneNumber', value)}
                />
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
                onClick={() => handleCloseOrderCustomerAddressModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '150px' }}
                variant="contained"
                size="large"
                onClick={() => onSaveNewOrderCustomerAddress()}
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
