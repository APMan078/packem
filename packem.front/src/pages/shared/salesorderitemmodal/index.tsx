import React, {
  useContext,
  useState,
  ChangeEvent,
  useEffect,
  useRef,
} from 'react';
import { useReactToPrint } from 'react-to-print';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import { getItemLookup, createItem } from 'services/api/item/item.api';
import {
  addOrderLineItem,
  editOrderLineItem,
} from 'services/api/salesorders/salesorders.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import Autocomplete from '@mui/material/Autocomplete';
import { useTheme } from '@mui/material/styles';
import TextField from '@mui/material/TextField';

interface CustomerUserModalProps {
  company?: any;
  superAdmin?: boolean;
  salesOrder?: any;
  orderLineItem?: any;
  callBack?: () => void;
  edit?: boolean;
}

interface AutoCompleteOptionType {
  id: number;
  label: string;
}

interface PurchaseOrderItemModalProps extends CustomerUserModalProps {
  salesOrder: any;
}

export default React.memo(
  ({
    company,
    superAdmin,
    salesOrder,
    orderLineItem,
    callBack,
    edit,
  }: PurchaseOrderItemModalProps) => {
    const theme = useTheme();
    const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
    const labelRef = useRef(null);
    const handlePrint = useReactToPrint({
      content: () => labelRef.current,
      onBeforeGetContent: () => console.log('before getting content to print'),
    });
    const {
      isAddOrderLineItemModalOpen,
      onCloseAddOrderLineItemModal,
      handleUpdateData,
    } = useContext(GlobalContext);
    const [items, setItems] = useState([]);
    const defaultAutocompleteOption: AutoCompleteOptionType | null = {
      id: -1,
      label: '',
    };
    const [searchText, setSearchText] = useState([]);
    const [showErrors, setShowErrors] = useState(false);
    const [error, setError] = useState('');
    const initialFormErrorsState: any = {
      customerLocationId: '',
      itemId: '',
      qty: '',
      perUnitItemPrice: '',
    };
    const initialState: any = {
      customerId: '',
      customerLocationId: currentLocationAndFacility.customerLocationId,
      itemId: {
        id: defaultAutocompleteOption.id,
        label: defaultAutocompleteOption.label,
      },
      qty: '',
      perUnitItemPrice: '',
    };
    const [formErrors, setFormErrors] = useState<any>(initialFormErrorsState);
    const [form, setForm] = useState<any>(initialState);

    const itemSelectionOptions = Array.isArray(items)
      ? items.map((i, index) => ({
          id: i.itemId,
          label: i.itemSKU,
        }))
      : [{ id: 0, label: 'New Item' }];

    const schema = yup.object().shape({
      customerId: yup.number().required('Required.'),
      itemId: yup
        .object()
        .shape({ id: yup.string().nullable(), label: yup.string() })
        .test(
          'empty-check',
          'An item SKU must be selected',
          (item) => !!item.label,
        )
        .typeError('Required.'),
      qty: yup.number().typeError('Required.'),
      perUnitItemPrice: yup.number().required('Required'),
    });

    const editSchema = yup.object().shape({
      orderLineId: yup.number().required(),
      qty: yup.number().typeError('Required.'),
      perUnitItemPrice: yup.number().required('Required'),
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

    const handleCloseSaleOrderLineItemModal = () => {
      callBack();
      setForm(initialState);
      setFormErrors(initialFormErrorsState);
      setError('');
      onCloseAddOrderLineItemModal();
    };

    const onEditOrderLineItem = async () => {
      if (superAdmin) {
        form.customerId = company.customerId;
        form.customerLocationId = company.customerLocationId;
      } else {
        form.customerId = currentUser.Claim_CustomerId;
      }
      editSchema
        .validate(form, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            if (edit) {
              await editOrderLineItem({
                orderLineId: orderLineItem.orderLineId,
                qty: form.qty,
                perUnitPrice: form.perUnitItemPrice,
              });
              snackActions.success(`Successfully edited item.`);
            } else {
              const newForm: any = form;
              newForm.itemId = form.itemId.id.toString();
              newForm.saleOrderId = salesOrder.saleOrderId;
              const newOrderLineItem = await addOrderLineItem(newForm);
              snackActions.success(`Successfully added new item to PO.`);
            }

            handleCloseSaleOrderLineItemModal();
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

    const onSaveNewSaleOrderLineItem = async () => {
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
            if (edit) {
              await editOrderLineItem({
                orderLineId: orderLineItem.orderLineId,
                qty: form.qty,
                perUnitPrice: form.perUnitItemPrice,
              });
              snackActions.success(`Successfully edited item.`);
            } else {
              const newForm: any = form;
              newForm.itemId = form.itemId.id.toString();
              newForm.saleOrderId = salesOrder.saleOrderId;
              const newOrderLineItem = await addOrderLineItem(newForm);
              snackActions.success(`Successfully added new item to PO.`);
            }

            handleCloseSaleOrderLineItemModal();
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

    const onLoadItemookup = async () => {
      try {
        const itemLookupFromApi = await getItemLookup(
          currentUser.Claim_CustomerId,
          searchText,
        );
        setItems(itemLookupFromApi);

        return true;
      } catch (err) {
        return err;
      }
    };

    useEffect(() => {
      setForm(initialState);
      onLoadItemookup();
      if (edit) {
        setForm({
          orderLineId: orderLineItem.orderLineId,
          qty: orderLineItem.orderQty,
          perUnitItemPrice: orderLineItem.perUnitPrice,
        });
      }
    }, [currentLocationAndFacility, edit, orderLineItem, salesOrder]);

    if (!isAddOrderLineItemModalOpen) return null;

    return (
      <Modal
        open={isAddOrderLineItemModalOpen}
        onClose={() => handleCloseSaleOrderLineItemModal()}
      >
        <ModalBox>
          <ModalContent>
            {edit ? (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Edit Item
              </Typography>
            ) : (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Add Item
              </Typography>
            )}

            {!showErrors ? (
              <>
                {!edit && (
                  <Autocomplete
                    options={itemSelectionOptions}
                    getOptionLabel={(option: AutoCompleteOptionType) =>
                      `${option.label}` || ''
                    }
                    size="small"
                    value={form.itemId}
                    sx={{ width: '100%' }}
                    onChange={(
                      event: any,
                      value: AutoCompleteOptionType | null,
                    ) => {
                      autoCompleteInputHandler('itemId', value);
                    }}
                    renderInput={(params) => (
                      <TextField {...params} label="Item SKU" />
                    )}
                  />
                )}
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Quantity"
                  value={form.qty}
                  type="number"
                  onChange={(value) => inputHandler('qty', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Per Unit Item Price"
                  type="number"
                  value={form.perUnitItemPrice}
                  onChange={(value) => inputHandler('perUnitItemPrice', value)}
                />
              </>
            ) : (
              <>
                {!edit && (
                  <Autocomplete
                    options={itemSelectionOptions}
                    getOptionLabel={(option: AutoCompleteOptionType) =>
                      `${option.label}` || ''
                    }
                    size="small"
                    value={form.itemId}
                    sx={{ width: '100%' }}
                    onChange={(
                      event: any,
                      value: AutoCompleteOptionType | null,
                    ) => {
                      autoCompleteInputHandler('itemId', value);
                    }}
                    renderInput={(params) =>
                      formErrors.itemId ? (
                        <TextField
                          {...params}
                          label="Item SKU"
                          error
                          helperText={formErrors.itemId}
                        />
                      ) : (
                        <TextField {...params} label="Item SKU" />
                      )
                    }
                  />
                )}
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Quantity"
                  value={form.qty}
                  error={formErrors.qty}
                  type="number"
                  onChange={(value) => inputHandler('qty', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Per Unit Item Price"
                  type="number"
                  value={form.perUnitItemPrice}
                  error={formErrors.perUnitItemPrice}
                  onChange={(value) => inputHandler('perUnitItemPrice', value)}
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
                onClick={() => handleCloseSaleOrderLineItemModal()}
              >
                Cancel
              </Button>
              {edit ? (
                <Button
                  sx={{ minHeight: '48px', maxWidth: '150px' }}
                  variant="contained"
                  size="large"
                  onClick={() => onEditOrderLineItem()}
                >
                  Edit Item
                </Button>
              ) : (
                <Button
                  sx={{ minHeight: '48px', maxWidth: '150px' }}
                  variant="contained"
                  size="large"
                  onClick={() => onSaveNewSaleOrderLineItem()}
                >
                  Add Item
                </Button>
              )}
            </Box>
          </ModalContent>
        </ModalBox>
      </Modal>
    );
  },
);
