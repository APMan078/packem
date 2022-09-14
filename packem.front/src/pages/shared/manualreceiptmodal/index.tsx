/* eslint-disable no-nested-ternary */
import React, {
  useContext,
  useState,
  ChangeEvent,
  useEffect,
  useRef,
} from 'react';
import Barcode from 'react-jsbarcode';
import { useReactToPrint } from 'react-to-print';

import Button from 'components/button';
import DatePickerInput from 'components/datepicker';
import Input from 'components/input/Input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import { getItemLookup, createItem } from 'services/api/item/item.api';
import { createLot, getLotLookup } from 'services/api/lot/lot.api.';
import { createReceipt } from 'services/api/receipts/receipts.api';
import { getCustomerUnitOfMeasures } from 'services/api/uoms/uoms.api';
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

type NewItem = {
  customerId: string;
  sku: string;
  description: string;
  uomId: number;
};

type NewCreatedItem = {
  customerId: string;
  sku: string;
  description: string;
  uomId: number;
  itemId: number;
};

type NewCreatedLotNumber = {
  lotId: number;
  customerLocationId: number;
  itemId: number;
  lotNo: string;
  expirationDate: string;
};

type NewLotNumber = {
  customerId: number;
  customerLocationId: number;
  itemId: number;
  lotNo: string;
  expirationDate: string;
};

interface PurchaseOrderItemModalProps extends CustomerUserModalProps {
  purchaseOrderItem?: any;
}

export default React.memo(
  ({
    company,
    admin,
    superAdmin,
    purchaseOrderItem,
  }: PurchaseOrderItemModalProps) => {
    console.log(purchaseOrderItem);
    const theme = useTheme();
    const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
    const labelRef = useRef(null);
    const handlePrint = useReactToPrint({
      content: () => labelRef.current,
    });
    const {
      isManualReceiptModalOpen,
      onCloseManualReceiptModal,
      handleUpdateData,
    } = useContext(GlobalContext);
    const [newLotNumber, setNewLotNumber] = useState(false);
    const [newLotNumberValue, setNewLotNumberValue] = useState('');
    const [itemLotNumbers, setItemLotNumbers] = useState([]);
    const [newItem, setNewItem] = useState(false);
    const [newItemSKU, setNewItemSKU] = useState('');
    const [items, setItems] = useState([]);
    const [customerUoms, setCustomerUoms] = useState<any>([]);
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
      description: '',
      uomId: '',
      lotId: '',
      itemDescription: '',
      expirationDate: '',
    };
    const initialState: any = {
      customerId: '',
      customerLocationId: currentLocationAndFacility.customerLocationId,
      itemId: {
        id: defaultAutocompleteOption.id,
        label: defaultAutocompleteOption.label,
      },
      qty: '',
      description: '',
      uomId: {
        id: defaultAutocompleteOption.id,
        label: defaultAutocompleteOption.label,
      },
      lotId: {
        id: defaultAutocompleteOption.id,
        label: defaultAutocompleteOption.label,
      },
      itemDescription: '',
      expirationDate: '',
    };
    const [formErrors, setFormErrors] = useState<any>(initialFormErrorsState);
    const [form, setForm] = useState<any>(initialState);

    const itemSelectionOptions = Array.isArray(items)
      ? items.map((i, index) => ({
          id: i.itemId,
          label: i.itemSKU,
        }))
      : [{ id: 0, label: 'New Item' }];

    const lotNumberSelectionOptions = Array.isArray(itemLotNumbers)
      ? itemLotNumbers.map((i, index) => ({
          id: i.lotId,
          label: i.lotNo,
        }))
      : [];

    const uomSelectionOptions = Array.isArray(customerUoms)
      ? customerUoms.map((uom, index) => ({
          id: uom.unitOfMeasureId,
          label: `${uom.code} - ${uom.description}`,
        }))
      : [{ id: 0, label: 'Uom' }];

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
      description: yup.string().required('Required.'),
      uomId: newItem
        ? yup
            .object()
            .shape({ id: yup.string().nullable(), label: yup.string() })
            .test(
              'empty-check',
              'An item UOM must be selected',
              (uom) => !!uom.label,
            )
            .typeError('Required.')
        : null,
      itemDescription: newItem
        ? yup.string().required('Required')
        : yup.string(),
      expirationDate:
        newLotNumber && newLotNumberValue.length > 0
          ? yup.string().required('Required')
          : yup.string(),
    });

    const onForm = (key, text) => {
      setForm(() => ({
        ...form,
        [key]: text,
      }));
    };

    const onLoadLotNumbers = async (itemId) => {
      try {
        const lotNumbersFromApi = await getLotLookup(itemId);

        setItemLotNumbers(lotNumbersFromApi);
        return true;
      } catch (err) {
        return err;
      }
    };

    const inputHandler = (
      key: string,
      event: ChangeEvent<HTMLInputElement>,
    ) => {
      onForm(key, event.target.value);
    };

    const dateInputHandler = (newValue: Date | null) => {
      onForm('expirationDate', newValue);
    };

    const autoCompleteInputHandler = (
      key: string,
      value: AutoCompleteOptionType,
    ) => {
      let resetLotNumber = false;
      let lotNumberValue = form.lotId;
      if (key === 'itemId') {
        if (value) {
          if (newItem) {
            setNewItem(false);
          } else {
            onLoadLotNumbers(value.id);
          }
        }

        if (!value) {
          if (newItem) {
            setNewItemSKU('');
          } else {
            setItemLotNumbers([]);
            resetLotNumber = true;
          }
        }
      }

      if (key === 'lotId') {
        if (value) {
          lotNumberValue = value;
          setNewLotNumber(false);
        } else {
          lotNumberValue = {
            id: defaultAutocompleteOption.id,
            label: defaultAutocompleteOption.label,
          };
        }
      }

      setForm(() => ({
        ...form,
        [key]:
          value === null
            ? {
                id: defaultAutocompleteOption.id,
                label: defaultAutocompleteOption.label,
              }
            : value,
        lotId:
          resetLotNumber && !newLotNumber
            ? {
                id: defaultAutocompleteOption.id,
                label: defaultAutocompleteOption.label,
              }
            : lotNumberValue,
      }));
    };

    const handleCloseManualReceiptModal = () => {
      setForm(initialState);
      setFormErrors(initialFormErrorsState);
      setError('');
      setNewItem(false);
      setNewItemSKU('');
      setNewLotNumber(false);
      setNewLotNumberValue('');
      onCloseManualReceiptModal();
    };

    const onSaveNewManualReceipt = async () => {
      if (superAdmin) {
        form.customerId = company.customerId;
        form.customerLocationId = company.customerLocationId;
      } else {
        form.customerId = currentUser.Claim_CustomerId;
      }

      if (newItem) {
        form.itemId.label = newItemSKU;
      }

      if (newLotNumber) {
        form.lotId.label = newLotNumberValue;
      }

      if (purchaseOrderItem) {
        form.itemId.label = purchaseOrderItem.itemSKU;
      }
      schema
        .validate(form, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            let newItemProfile = {} as NewCreatedItem;
            let newLotNumberProfile = {} as NewCreatedLotNumber;
            const newForm: any = form;

            if (newItem) {
              const newItemForm = {} as NewItem;
              newItemForm.customerId = form.customerId;
              newItemForm.sku = newItemSKU;
              newItemForm.description = form.itemDescription;
              newItemForm.uomId = form.uomId.id;

              newItemProfile = await createItem(newItemForm);

              newForm.itemId = newItemProfile.itemId.toString();

              snackActions.success(`Successfully created new item.`);
            } else if (purchaseOrderItem && purchaseOrderItem.itemId) {
              newForm.itemId = purchaseOrderItem.itemId.toString();
              newForm.lotId = purchaseOrderItem.lotId
                ? purchaseOrderItem.lotId.toString()
                : null;
            } else {
              newForm.itemId = form.itemId.id.toString();
              newForm.lotId =
                form.lotId.id === -1 ? null : form.lotId.id.toString();
            }

            if (newLotNumber) {
              const newLotNumberForm = {} as NewLotNumber;
              newLotNumberForm.customerId = currentUser.Claim_CustomerId;
              newLotNumberForm.customerLocationId =
                currentLocationAndFacility.customerLocationId;
              newLotNumberForm.itemId = newForm.itemId;
              newLotNumberForm.lotNo = newLotNumberValue;
              newLotNumberForm.expirationDate = form.expirationDate;

              newLotNumberProfile = await createLot(newLotNumberForm);

              newForm.lotId = newLotNumberProfile.lotId;

              snackActions.success(`Successfully created new lot number.`);
            }

            newForm.customerLocationId =
              currentLocationAndFacility.customerLocationId;
            const newManualReceipt = await createReceipt(newForm);

            snackActions.success(`Successfully created new receipt.`);
            handleCloseManualReceiptModal();
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

    const onLoadItemLookup = async () => {
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

    const onLoadUomData = async () => {
      try {
        const customerSetUoms = await getCustomerUnitOfMeasures(
          currentUser.Claim_CustomerId,
        );

        setCustomerUoms(customerSetUoms);

        return true;
      } catch (err: any) {
        return err;
      }
    };

    useEffect(() => {
      setForm(initialState);
      onLoadItemLookup();
      onLoadUomData();
    }, [currentLocationAndFacility]);

    if (!isManualReceiptModalOpen) return null;

    return (
      <Modal
        open={isManualReceiptModalOpen}
        onClose={() => handleCloseManualReceiptModal()}
      >
        <ModalBox>
          <ModalContent>
            <Box
              sx={{
                display: 'none',
              }}
            >
              {newItem && newItemSKU ? (
                <Box
                  sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    justifyContent: 'center',
                    alignItems: 'center',
                  }}
                  ref={labelRef}
                >
                  <Barcode value={newItemSKU} />
                </Box>
              ) : (
                form.itemId && (
                  <Box
                    sx={{
                      display: 'flex',
                      flexDirection: 'column',
                      justifyContent: 'center',
                      alignItems: 'center',
                    }}
                    ref={labelRef}
                  >
                    {form.itemId.label !== '' && (
                      <Barcode value={form.itemId.label} />
                    )}
                  </Box>
                )
              )}
            </Box>
            <Typography
              sx={{ marginBottom: '16px' }}
              variant="h6"
              fontWeight="bold"
            >
              Manual Receipt
            </Typography>

            {newItem ? (
              <>
                <Autocomplete
                  sx={{ width: '100%' }}
                  freeSolo
                  options={itemSelectionOptions}
                  getOptionLabel={(option: AutoCompleteOptionType) =>
                    `${option.label}` || ''
                  }
                  size="small"
                  value={form.itemId}
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
                        onChange={(event) => {
                          setNewItemSKU(event.target.value);
                        }}
                      />
                    ) : (
                      <TextField
                        {...params}
                        label="Item SKU"
                        onChange={(event) => {
                          setNewItemSKU(event.target.value);
                        }}
                      />
                    )
                  }
                />
                <Autocomplete
                  options={uomSelectionOptions}
                  getOptionLabel={(option: AutoCompleteOptionType) =>
                    `${option.label}` || ''
                  }
                  size="small"
                  value={form.uomId}
                  sx={{ width: '100%' }}
                  onChange={(
                    event: any,
                    value: AutoCompleteOptionType | null,
                  ) => {
                    autoCompleteInputHandler('uomId', value);
                  }}
                  renderInput={(params) =>
                    formErrors.uomId ? (
                      <TextField
                        {...params}
                        label="Item UoM"
                        error
                        helperText={formErrors.uomId}
                      />
                    ) : (
                      <TextField {...params} label="Item UoM" />
                    )
                  }
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Item Description"
                  value={form.itemDescription}
                  error={formErrors.itemDescription}
                  onChange={(value) => inputHandler('itemDescription', value)}
                />
              </>
            ) : purchaseOrderItem ? (
              <Input
                sx={{ width: '100%' }}
                placeholder="Item SKU"
                disabled
                value={purchaseOrderItem.itemSKU}
              />
            ) : (
              <Autocomplete
                sx={{ width: '100%' }}
                options={itemSelectionOptions}
                getOptionLabel={(option: AutoCompleteOptionType) =>
                  `${option.label}` || ''
                }
                size="small"
                value={form.itemId}
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
                noOptionsText={
                  <Button
                    variant="text"
                    size="small"
                    onMouseDown={() => {
                      setNewItem(true);
                    }}
                  >
                    + Add New Item
                  </Button>
                }
              />
            )}
            <Input
              sx={{ width: '100%' }}
              placeholder="Receipt Description (150 limit)"
              value={form.description}
              error={formErrors.description}
              onChange={(value) => inputHandler('description', value)}
            />
            <Input
              sx={{ width: '100%' }}
              placeholder="Quantity"
              value={form.qty}
              error={formErrors.qty}
              type="number"
              onChange={(value) => inputHandler('qty', value)}
            />
            {purchaseOrderItem ? (
              <Input
                sx={{ width: '100%' }}
                placeholder="Lot Number"
                disabled
                value={purchaseOrderItem.lotNo}
              />
            ) : newLotNumber ? (
              <Autocomplete
                freeSolo
                options={lotNumberSelectionOptions}
                getOptionLabel={(option: AutoCompleteOptionType) =>
                  `${option.label}` || ''
                }
                size="small"
                value={form.lotId}
                sx={{ width: '100%' }}
                onChange={(
                  event: any,
                  value: AutoCompleteOptionType | null,
                ) => {
                  autoCompleteInputHandler('lotId', value);
                }}
                renderInput={(params) =>
                  formErrors.lotId ? (
                    <TextField
                      {...params}
                      label="Lot Number"
                      error
                      helperText={formErrors.lotId}
                      onChange={(event) => {
                        setNewLotNumberValue(event.target.value);
                      }}
                    />
                  ) : (
                    <TextField
                      {...params}
                      label="Lot Number"
                      onChange={(event) => {
                        setNewLotNumberValue(event.target.value);
                      }}
                    />
                  )
                }
              />
            ) : (
              <Autocomplete
                options={lotNumberSelectionOptions}
                getOptionLabel={(option: AutoCompleteOptionType) =>
                  `${option.label}` || ''
                }
                size="small"
                value={form.lotId}
                sx={{ width: '100%' }}
                onChange={(
                  event: any,
                  value: AutoCompleteOptionType | null,
                ) => {
                  autoCompleteInputHandler('lotId', value);
                }}
                renderInput={(params) =>
                  formErrors.lotId ? (
                    <TextField
                      {...params}
                      label="Lot Number"
                      error
                      helperText={formErrors.lotId}
                    />
                  ) : (
                    <TextField {...params} label="Lot Number" />
                  )
                }
                noOptionsText={
                  <Button
                    variant="text"
                    size="small"
                    onMouseDown={() => {
                      setForm({
                        ...form,
                        lotId: {
                          id: defaultAutocompleteOption.id,
                          label: defaultAutocompleteOption.label,
                        },
                      });
                      setNewLotNumber(true);
                    }}
                  >
                    + Add Lot Number
                  </Button>
                }
              />
            )}

            {newLotNumber && (
              <DatePickerInput
                label="Expiration Date"
                inputFormat="MM/dd/yyyy"
                value={form.expirationDate}
                onChange={dateInputHandler}
                renderInput={(params) =>
                  formErrors.expirationDate ? (
                    <TextField
                      {...params}
                      size="small"
                      error
                      helperText={formErrors.expirationDate}
                    />
                  ) : (
                    <TextField {...params} error={false} size="small" />
                  )
                }
              />
            )}
            <Typography
              sx={{
                cursor: 'pointer',
                color: [theme.palette.primary.main],
                width: '30%',
              }}
              onClick={handlePrint}
            >
              Print Label
            </Typography>

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
                onClick={() => handleCloseManualReceiptModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '150px' }}
                variant="contained"
                size="large"
                onClick={() => onSaveNewManualReceipt()}
              >
                Add Items
              </Button>
            </Box>
          </ModalContent>
        </ModalBox>
      </Modal>
    );
  },
);
