/* eslint-disable react/no-array-index-key */
import React, { ChangeEvent, useContext, useEffect, useState } from 'react';

import Button from 'components/button';
import CardWithHeader from 'components/card/CardWithHeader';
import Input from 'components/input';
import { snackActions } from 'config/snackbar.js';
import {
  getCustomerDefaultThreshold,
  updateCustomerDefaultThreshold,
} from 'services/api/customer/customer.api';
import {
  getCustomerUnitOfMeasures,
  getDefaultUnitOfMeasures,
  createUnitOfMeasureForCustomer,
  creatCustomUnitOfMeasure,
  deleteCustomerUnitOfMeasure,
} from 'services/api/uoms/uoms.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Box, Typography, Autocomplete, TextField, Chip } from '@mui/material';

interface AutoCompleteOptionType {
  id: number;
  label: string;
}

function InventorySettings() {
  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  const { updateData, handleUpdateData } = useContext(GlobalContext);
  const defaultAutocompleteOption: AutoCompleteOptionType | null = {
    id: -1,
    label: '',
  };
  const initialState: any = {
    customerId: '',
    code: '',
    description: '',
  };
  const initialUOMFormErrorsState: any = {
    customerId: '',
    code: '',
    description: '',
  };
  const [uomForm, setUomForm] = useState<any>(initialState);
  const [uomFormErrors, setUomFormErrors] = useState<any>(
    initialUOMFormErrorsState,
  );
  const [defaultThreshold, setDefaultThreshold] = useState(0);
  const [defaultThresholdError, setDefailtThresholdError] = useState('');
  const [error, setError] = useState('');
  const [defaultUoms, setDefaultUoms] = useState<any>([]);
  const [customerUoms, setCustomerUoms] = useState<any>([]);
  const [isAddingCustomUOM, setIsAddingCustomUOM] = useState<boolean>(false);
  const [selectedUOM, setSelectedUOM] = useState<AutoCompleteOptionType | null>(
    defaultAutocompleteOption,
  );

  const schema = yup.object().shape({
    code: yup.string().required('Required'),
    description: yup.string().required('Required'),
  });
  const uomSelectionOptions = Array.isArray(defaultUoms)
    ? defaultUoms.map((uom, index) => ({
        id: uom.unitOfMeasureId,
        label: `${uom.code} - ${uom.description}`,
      }))
    : [{ id: 0, label: 'Uom' }];

  const onUomForm = (key, text) => {
    setUomForm(() => ({
      ...uomForm,
      [key]: text,
    }));
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) => {
    onUomForm(key, event.target.value);
  };

  const autoCompleteInputHandler = (
    key: string,
    value: AutoCompleteOptionType,
  ) => {
    if (value !== null) {
      setSelectedUOM(value);
      const exists = customerUoms.find(
        (uom) => uom.unitOfMeasureId.toString() === value.id.toString(),
      );

      if (!exists) {
        setCustomerUoms([
          ...customerUoms,
          defaultUoms.find((uom) => uom.unitOfMeasureId === value.id),
        ]);
      }
    }
  };

  const handleSetCustomUOM = () => {
    if (isAddingCustomUOM) {
      setUomForm(initialState);
      setUomFormErrors(initialUOMFormErrorsState);
    }
    setIsAddingCustomUOM(!isAddingCustomUOM);
  };

  const handleUomDelete = async (uomId) => {
    try {
      await deleteCustomerUnitOfMeasure({
        customerId: currentUser.Claim_CustomerId,
        unitOfMeasureId: uomId,
      });
      snackActions.success(`Successfully removed UoM from list.`);
      const newCustomerUoms = customerUoms.filter(
        (uom) => uom.unitOfMeasureId !== uomId,
      );
      setCustomerUoms(newCustomerUoms);
      return true;
    } catch (err: any) {
      snackActions.error(err);
      return err;
    }
  };

  const onLoadUomData = async () => {
    try {
      const globalUoms = await getDefaultUnitOfMeasures('');
      const customerSetUoms = await getCustomerUnitOfMeasures(
        currentUser.Claim_CustomerId,
      );

      setDefaultUoms(globalUoms);
      setCustomerUoms(customerSetUoms);

      return true;
    } catch (err: any) {
      return err;
    }
  };

  const onLoadCustomerDefaultThreshold = async () => {
    try {
      const thresholdFromApi = await getCustomerDefaultThreshold(
        currentUser.Claim_CustomerId,
      );

      setDefaultThreshold(thresholdFromApi);

      return true;
    } catch (err: any) {
      return err;
    }
  };

  const handleSaveCustomerDefaultThreshold = async () => {
    try {
      await updateCustomerDefaultThreshold(
        currentUser.Claim_CustomerId,
        defaultThreshold,
      );
      snackActions.success(`Successfully updated default threshold.`);
    } catch (err: any) {
      if (!defaultThreshold) {
        snackActions.error(`A threshold is required`);
      }
    }
  };

  const handleSaveCustomerUoms = async () => {
    try {
      await createUnitOfMeasureForCustomer({
        customerId: currentUser.Claim_CustomerId,
        unitOfMeasureIds: customerUoms.map((uom) => uom.unitOfMeasureId),
      });
      snackActions.success(`Successfully updated UoM list.`);
    } catch (err: any) {
      snackActions.error(`${err}`);
    }
  };

  const handleSaveCustomCustomerUom = async () => {
    uomForm.customerId = currentUser.Claim_CustomerId;
    schema
      .validate(uomForm, {
        abortEarly: false,
      })
      .then(async () => {
        try {
          const customUom = await creatCustomUnitOfMeasure(uomForm);
          setCustomerUoms([...customerUoms, customUom]);
          snackActions.success(`Successfully created new custom UoM.`);
        } catch (err: any) {
          setError(err);
          setUomFormErrors(initialUOMFormErrorsState);
          snackActions.error(`${error}`);
        }
      })
      .catch((err) => {
        const errorsFound = err.inner.reduce((acc, curr) => {
          if (!acc[curr.path]) acc[curr.path] = curr.message;
          return acc;
        }, {});
        setUomFormErrors(errorsFound);
        setError('');
      });
  };

  useEffect(() => {
    onLoadUomData();
    onLoadCustomerDefaultThreshold();
  }, [currentLocationAndFacility, updateData]);

  return (
    <>
      {/* Inventory Settings */}
      <CardWithHeader
        sx={{ display: 'flex', flexDirection: 'column' }}
        header="Unit of Measurement"
        subHeader="Manage which units of measure show in your account."
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'start',
            alignItems: 'center',
            width: { sm: '100%', md: '100%' },
            paddingY: '18px',
            gap: '18px',
          }}
        >
          <Box
            sx={{
              display: 'flex',
              width: '100%',
              justifyContent: 'end',
              alignItems: 'center',
              gap: '8px',
            }}
          >
            {isAddingCustomUOM && (
              <>
                <Box sx={{ display: 'flex', width: '25%' }}>
                  <Input
                    sx={{ width: '100%' }}
                    placeholder="Code"
                    value={uomForm.code}
                    error={uomFormErrors.code}
                    size="large"
                    onChange={(event) => inputHandler('code', event)}
                  />
                </Box>
                <Box sx={{ display: 'flex', width: '25%' }}>
                  <Input
                    sx={{ width: '100%' }}
                    placeholder="Description"
                    value={uomForm.description}
                    error={uomFormErrors.description}
                    size="large"
                    onChange={(event) => inputHandler('description', event)}
                  />
                </Box>
                <Box sx={{ display: 'flex', width: '25%' }}>
                  <Button
                    sx={{ display: 'flex', width: '100%' }}
                    variant="contained"
                    size="small"
                    onClick={handleSaveCustomCustomerUom}
                  >
                    ADD
                  </Button>
                </Box>
              </>
            )}
            <Box sx={{ display: 'flex', width: '25%' }}>
              <Button
                sx={{ display: 'flex' }}
                variant="text"
                size="large"
                onClick={handleSetCustomUOM}
              >
                {isAddingCustomUOM ? 'CANCEL' : '+ ADD CUSTOM'}
              </Button>
            </Box>
          </Box>
          <Box sx={{ display: 'flex', width: '100%' }}>
            <Autocomplete
              sx={{ width: '100%' }}
              options={uomSelectionOptions}
              getOptionLabel={(option: AutoCompleteOptionType) =>
                `${option.label}` || ''
              }
              size="small"
              value={selectedUOM}
              onChange={(event: any, value: AutoCompleteOptionType | null) => {
                autoCompleteInputHandler('uom', value);
              }}
              renderInput={(params) => (
                <TextField {...params} label="Select a UoM" />
              )}
            />
          </Box>
          <Box sx={{ display: 'flex', flexDirection: 'column', width: '100%' }}>
            <Typography
              sx={{
                paddingY: '12px',
                pr: '50px',
                fontSize: '18px',
                letterSpacing: '0.14px',
                lineHeight: '24px',
              }}
              variant="subtitle1"
              fontWeight="bold"
            >
              My units of measurement
            </Typography>
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                flexWrap: 'wrap',
                width: '100%',
                gap: '16px',
              }}
            >
              {customerUoms.map((uom) => (
                <Chip
                  key={uom.unitOfMeasureId}
                  label={`${uom.code} - ${uom.description}`}
                  variant="outlined"
                  onDelete={(event: any) =>
                    handleUomDelete(uom.unitOfMeasureId)
                  }
                />
              ))}
            </Box>
          </Box>
          <Box
            sx={{
              display: 'flex',
              width: '100%',
              justifyContent: 'end',
              gap: '8px',
            }}
          >
            <Box
              sx={{ display: 'flex', width: '25%', justifyContent: 'center' }}
            >
              <Button
                sx={{ display: 'flex', width: '100%' }}
                variant="contained"
                size="small"
                onClick={handleSaveCustomerUoms}
              >
                SAVE
              </Button>
            </Box>
          </Box>
        </Box>
      </CardWithHeader>
      <CardWithHeader
        sx={{ display: 'flex', flexDirection: 'column' }}
        header="Low Stock Threshold"
        subHeader="Manage what the threshold is for low stock items in inventory."
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'start',
            alignItems: 'center',
            width: { sm: '100%', md: '100%' },
            paddingY: '18px',
            gap: '18px',
          }}
        >
          <Box
            sx={{
              display: 'flex',
              width: '100%',
              justifyContent: 'space-between',
              alignItems: 'center',
              gap: '8px',
            }}
          >
            <Box
              sx={{ display: 'flex', width: '25%', justifyContent: 'center' }}
            >
              <Input
                sx={{ width: '100%' }}
                placeholder="Threshold"
                type="number"
                min={0}
                value={defaultThreshold}
                size="large"
                onChange={(event) => setDefaultThreshold(event.target.value)}
              />
            </Box>
            <Box
              sx={{ display: 'flex', width: '25%', justifyContent: 'center' }}
            >
              <Button
                sx={{ display: 'flex', width: '100%' }}
                variant="contained"
                size="small"
                onClick={handleSaveCustomerDefaultThreshold}
              >
                SAVE
              </Button>
            </Box>
          </Box>
        </Box>
      </CardWithHeader>
    </>
  );
}

export default React.memo(InventorySettings);
