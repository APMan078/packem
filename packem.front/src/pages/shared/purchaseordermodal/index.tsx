import React, { useContext, useState, ChangeEvent, useEffect } from 'react';

import Button from 'components/button';
import DatePickerInput from 'components/datepicker';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import moment from 'moment';
import { getCustomerInventory } from 'services/api/inventory/inventory.api';
import { createPurchaseOrder } from 'services/api/purchaseOrders/purchaseOrders.api';
import { getVendorLookup } from 'services/api/vendors/vendors.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import Autocomplete from '@mui/material/Autocomplete';
import { useTheme } from '@mui/material/styles';
import TextField from '@mui/material/TextField';

interface CustomerUserModalProps {
  company?: any;
  admin?: boolean;
  superAdmin?: boolean;
}

interface AutoCompleteOptionType {
  id: number;
  label: string;
}

export default React.memo(
  ({ company, admin, superAdmin }: CustomerUserModalProps) => {
    const theme = useTheme();
    const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
    const {
      isPurchaseOrderModalOpen,
      onClosePurchaseOrderModal,
      handleUpdateData,
    } = useContext(GlobalContext);
    const [vendors, setVendors] = useState([]);
    const defaultAutocompleteOption: AutoCompleteOptionType | null = {
      id: -1,
      label: '',
    };
    const [searchText, setSearchText] = useState([]);
    const [showErrors, setShowErrors] = useState(false);
    const [error, setError] = useState('');
    const initialFormErrorsState: any = {
      customerLocationId: '',
      customerFacilityId: '',
      purchaseOrderNo: '',
      shipVia: '',
      vendorId: '',
      orderDate: '',
    };
    const initialState: any = {
      customerId: '',
      customerLocationId: currentLocationAndFacility.customerLocationId,
      customerfacilityId: currentLocationAndFacility.customerFacilityId,
      purchaseOrderNo: '',
      shipVia: '',
      vendorId: {
        id: defaultAutocompleteOption.id,
        label: defaultAutocompleteOption.label,
      },
      orderDate: moment(),
    };
    const [formErrors, setFormErrors] = useState<any>(initialFormErrorsState);
    const [form, setForm] = useState<any>(initialState);

    const vendorSelectionOptions = Array.isArray(vendors)
      ? vendors
          .filter((v) => v.name !== 'New Vendor')
          .map((v, index) => ({
            id: v.vendorId,
            label: v.name,
          }))
      : [{ id: 0, label: 'New Vendor' }];

    const schema = yup.object().shape({
      customerId: yup.number().required('Required.'),
      purchaseOrderNo: yup.string().required('Required.'),
      shipVia: yup.string().required('Required.'),
      vendorId: yup
        .object()
        .shape({ id: yup.string().nullable(), label: yup.string() })
        .test(
          'empty-check',
          'A vendor must be selected',
          (vendor) => !!vendor.label,
        )
        .typeError('Required.'),
      orderDate: yup.string().required('Required').nullable(),
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

    const dateInputHandler = (newValue: Date | null) => {
      onForm('orderDate', newValue);
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

    const handleClosePurchaseOrderModal = () => {
      setForm(initialState);
      setFormErrors(initialFormErrorsState);
      setError('');
      onClosePurchaseOrderModal();
    };

    const onSaveNewPurchaseOrder = async () => {
      if (superAdmin) {
        form.customerId = company.customerId;
        form.customerLocationId = company.customerLocationId;
      } else {
        form.customerId = currentUser.Claim_CustomerId;
      }
      schema
        .validate(form, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            const newForm: any = form;
            newForm.vendorId = form.vendorId.id.toString();
            const newPurchaeOrder = await createPurchaseOrder(newForm);
            snackActions.success(`Successfully created new purchase order.`);
            handleClosePurchaseOrderModal();
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

    const onLoadVendorLookup = async () => {
      try {
        const vendorLookupFromApi = await getVendorLookup(
          currentUser.Claim_CustomerId,
          searchText,
        );
        setVendors(vendorLookupFromApi);

        return true;
      } catch (err) {
        return err;
      }
    };

    useEffect(() => {
      setForm(initialState);
      onLoadVendorLookup();
    }, [currentLocationAndFacility]);

    if (!isPurchaseOrderModalOpen) return null;

    return (
      <Modal
        open={isPurchaseOrderModalOpen}
        onClose={() => handleClosePurchaseOrderModal()}
      >
        <ModalBox>
          <ModalContent>
            <Typography
              sx={{ marginBottom: '16px' }}
              variant="h6"
              fontWeight="bold"
            >
              Add PO
            </Typography>
            {!showErrors ? (
              <>
                <Input
                  sx={{ width: '100%' }}
                  placeholder="PO No"
                  value={form.purchaseOrderNo}
                  onChange={(value) => inputHandler('purchaseOrderNo', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Ship Via"
                  value={form.shipVia}
                  onChange={(value) => inputHandler('shipVia', value)}
                />
                <Autocomplete
                  options={vendorSelectionOptions}
                  size="small"
                  value={form.vendorId}
                  sx={{ width: '100%' }}
                  onChange={(
                    event: any,
                    value: AutoCompleteOptionType | null,
                  ) => {
                    autoCompleteInputHandler('vendorId', value);
                  }}
                  renderInput={(params) => (
                    <TextField {...params} label="Vendor" />
                  )}
                />
                <DatePickerInput
                  label="Order Date"
                  inputFormat="MM/dd/yyyy"
                  value={form.orderDate}
                  onChange={dateInputHandler}
                  renderInput={(params) => (
                    <TextField {...params} error={false} size="small" />
                  )}
                />
              </>
            ) : (
              <>
                <Input
                  sx={{ width: '100%' }}
                  placeholder="PO No"
                  value={form.purchaseOrderNo}
                  error={formErrors.purchaseOrderNo}
                  onChange={(value) => inputHandler('purchaseOrderNo', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Ship Via"
                  value={form.shipVia}
                  error={formErrors.shipVia}
                  onChange={(value) => inputHandler('shipVia', value)}
                />
                <Autocomplete
                  options={vendorSelectionOptions}
                  getOptionLabel={(option: AutoCompleteOptionType) =>
                    `${option.label}` || ''
                  }
                  size="small"
                  value={form.vendorId}
                  sx={{ width: '100%' }}
                  onChange={(
                    event: any,
                    value: AutoCompleteOptionType | null,
                  ) => {
                    autoCompleteInputHandler('vendorId', value);
                  }}
                  renderInput={(params) =>
                    formErrors.vendorId ? (
                      <TextField
                        {...params}
                        label="Vendor"
                        error
                        helperText={formErrors.vendorId}
                      />
                    ) : (
                      <TextField {...params} label="Vendor" />
                    )
                  }
                />

                <DatePickerInput
                  label="Order Date"
                  inputFormat="MM/dd/yyyy"
                  value={form.orderDate}
                  onChange={dateInputHandler}
                  renderInput={(params) =>
                    formErrors.orderDate ? (
                      <TextField
                        {...params}
                        size="small"
                        error
                        helperText={formErrors.orderDate}
                      />
                    ) : (
                      <TextField {...params} size="small" />
                    )
                  }
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
                onClick={() => handleClosePurchaseOrderModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '150px' }}
                variant="contained"
                size="large"
                onClick={() => onSaveNewPurchaseOrder()}
              >
                Add PO
              </Button>
            </Box>
          </ModalContent>
        </ModalBox>
      </Modal>
    );
  },
);
