import React, { useContext, useState, ChangeEvent, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import { createItem, searchItemBySKU } from 'services/api/item/item.api';
import { getCustomerUnitOfMeasures } from 'services/api/uoms/uoms.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography, Autocomplete, TextField } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface ItemModalProps {
  item?: any;
}

interface AutoCompleteOptionType {
  id: number;
  label: string;
}

export default React.memo(({ item }: ItemModalProps) => {
  const theme = useTheme();
  const navigate = useNavigate();
  const { currentUser } = useContext(AuthContext);
  const { isItemModalOpen, onCloseItemModal, handleUpdateData } =
    useContext(GlobalContext);
  const [currentStage, setCurrentStage] = useState(0);
  const [customerUoms, setCustomerUoms] = useState<any>([]);

  const [itemExists, setItemExists] = useState(false);
  const [itemFromApi, setItemFromApi] = useState({
    itemId: '',
    sku: '',
  });
  const defaultAutocompleteOption: AutoCompleteOptionType | null = {
    id: -1,
    label: '',
  };
  const [showErrors, setShowErrors] = useState(false);
  const [error, setError] = useState('');

  const initialFormErrorsState: any = {
    sku: '',
    description: '',
    uomId: '',
  };

  const initialState: any = {
    customerId: '',
    sku: '',
    description: '',
    uomId: {
      id: defaultAutocompleteOption.id,
      label: defaultAutocompleteOption.label,
    },
  };
  const [formErrors, setFormErrors] = useState<any>(initialFormErrorsState);
  const [form, setForm] = useState(initialState);

  const uomSelectionOptions = Array.isArray(customerUoms)
    ? customerUoms.map((uom, index) => ({
        id: uom.unitOfMeasureId,
        label: `${uom.code} - ${uom.description}`,
      }))
    : [{ id: 0, label: 'Uom' }];

  const onForm = (key, text) => {
    setForm(() => ({
      ...form,
      [key]: text,
    }));
  };

  const handleNextStage = () => {
    setCurrentStage(currentStage + 1);
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) =>
    onForm(key, event.target.value);

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

  const querySchema = yup.object().shape({
    sku: yup.string().required('Required.'),
  });

  const schema = yup.object().shape({
    customerId: yup.number().required('Required.'),
    sku: yup.string().required('Required.'),
    description: yup.string().required('Required.'),
    uomId: yup
      .object()
      .shape({ id: yup.string().nullable(), label: yup.string() })
      .test('empty-check', 'An item UOM must be selected', (uom) => !!uom.label)
      .typeError('Required.'),
  });

  const handleCloseItemModal = () => {
    setForm(initialState);
    setFormErrors(initialFormErrorsState);
    setError('');
    setCurrentStage(0);
    onCloseItemModal();
  };

  const onQueryNewItem = async () => {
    querySchema
      .validate(form, {
        abortEarly: false,
      })
      .then(async () => {
        try {
          const newItem = await searchItemBySKU(
            currentUser.Claim_CustomerId.toString(),
            form.sku,
          );
          snackActions.error(`Item already exists.`);
          setItemFromApi(newItem);
          setItemExists(true);
        } catch (err: any) {
          setFormErrors(initialFormErrorsState);
          setError(err);
          snackActions.success(`No item found.`);
          handleNextStage();
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

  const onSaveNewItem = async () => {
    form.customerId = currentUser.Claim_CustomerId;
    schema
      .validate(form, {
        abortEarly: false,
      })
      .then(async () => {
        try {
          form.uomId = form.uomId.id;
          const newItem = await createItem(form);
          snackActions.success(`Successfully added new item.`);
          handleUpdateData();
          handleCloseItemModal();
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

  const onLoadEditItem = () => {
    setForm(item);
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
    onLoadEditItem();
    onLoadUomData();
  }, [item, itemExists]);

  if (!isItemModalOpen) return null;

  return (
    <Modal open={isItemModalOpen} onClose={() => handleCloseItemModal()}>
      <ModalBox>
        {currentStage === 0 && (
          <ModalContent>
            {item.customerId === '' ? (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Add New Item
              </Typography>
            ) : (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Edit Item
              </Typography>
            )}
            {!showErrors ? (
              <Input
                sx={{ width: '100%' }}
                placeholder="Item SKU"
                value={form.sku}
                onChange={(value) => inputHandler('sku', value)}
              />
            ) : (
              <Input
                sx={{ width: '100%' }}
                placeholder="Item SKU"
                value={form.sku}
                error={formErrors.sku}
                onChange={(value) => inputHandler('sku', value)}
              />
            )}

            {itemExists && (
              <>
                <Typography fontWeight="bold" variant="subtitle1">
                  Item already exists in inventory.
                </Typography>
                <Typography
                  sx={{
                    cursor: 'pointer',
                    color: [theme.palette.primary.main],
                  }}
                  onClick={() => navigate(`/inventory/item/${itemFromApi.sku}`)}
                  fontWeight="bold"
                  variant="subtitle1"
                >
                  Go to Item {itemFromApi.sku}
                </Typography>
              </>
            )}
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
                onClick={() => handleCloseItemModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="contained"
                size="large"
                onClick={() => onQueryNewItem()}
              >
                Search
              </Button>
            </Box>
          </ModalContent>
        )}
        {currentStage === 1 && (
          <ModalContent>
            {item.customerId === '' ? (
              <Typography variant="h6" fontWeight="bold">
                Add New Item
              </Typography>
            ) : (
              <Typography variant="h6" fontWeight="bold">
                Edit Item
              </Typography>
            )}
            {!showErrors ? (
              <>
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Item SKU"
                  value={form.sku}
                  onChange={(value) => inputHandler('sku', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Description (150 Chars)"
                  value={form.description}
                  onChange={(value) => inputHandler('description', value)}
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
                  renderInput={(params) => (
                    <TextField {...params} label="Item UoM" />
                  )}
                />
              </>
            ) : (
              <>
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Item SKU"
                  value={form.sku}
                  error={formErrors.sku}
                  onChange={(value) => inputHandler('sku', value)}
                />
                <Input
                  sx={{ width: '100%' }}
                  placeholder="Description (150 Chars)"
                  value={form.description}
                  error={formErrors.description}
                  onChange={(value) => inputHandler('description', value)}
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
              </>
            )}

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
                onClick={() => handleCloseItemModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '240px' }}
                variant="contained"
                size="large"
                onClick={() => onSaveNewItem()}
              >
                Add to Inventory
              </Button>
            </Box>
          </ModalContent>
        )}
      </ModalBox>
    </Modal>
  );
});
