import React, { useContext, useState, ChangeEvent, useEffect } from 'react';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import {
  getCustomerFacilityZones,
  createCustomerBin,
  editCustomerBinData,
} from 'services/api/customerfacilities/customerfacilities.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography, Autocomplete, TextField } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface AutoCompleteOptionType {
  id: number;
  label: string;
}

interface CreateBinModalProps {
  binData?: any;
  resetData?: () => void;
}

export default React.memo(({ binData, resetData }: CreateBinModalProps) => {
  const theme = useTheme();
  const { isCreateBinModalOpen, onCloseCreateBinModal, handleUpdateData } =
    useContext(GlobalContext);
  const { currentLocationAndFacility } = useContext(AuthContext);
  const [showErrors, setShowErrors] = useState(false);
  const [error, setError] = useState('');
  const [customerZones, setCustomerZones] = useState([]);
  const defaultAutocompleteOption: AutoCompleteOptionType | null = {
    id: -1,
    label: '',
  };
  const binCategoryOptions = [
    {
      id: 1,
      label: 'Unassigned',
    },
    {
      id: 2,
      label: 'Picking',
    },
    {
      id: 3,
      label: 'Bulk',
    },
  ];
  const initialState: any = {
    customerLocationId: currentLocationAndFacility.customerLocationId,
    zoneId: {
      id: defaultAutocompleteOption.id,
      label: defaultAutocompleteOption.label,
    },
    name: '',
    category: {
      id: defaultAutocompleteOption.id,
      label: defaultAutocompleteOption.label,
    },
  };
  const initialFormErrorsState: any = {
    customerLocationId: currentLocationAndFacility.customerLocationId,
    zoneId: '',
    name: '',
    category: {
      id: defaultAutocompleteOption.id,
      label: defaultAutocompleteOption.label,
    },
  };
  const [form, setForm] = useState<any>(initialState);
  const [formErrors, setFormErrors] = useState(initialFormErrorsState);

  const onForm = (key, text) => {
    setForm(() => ({
      ...form,
      [key]: text,
    }));
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) =>
    onForm(key, event.target.value);

  const autoCompleteInputHandler = (
    key: string,
    value: AutoCompleteOptionType,
  ) => {
    setForm(() => ({
      ...form,
      [key]: value,
    }));
  };

  const schema = yup.object().shape({
    customerLocationId: yup.number().required('Required.'),
    zoneId: yup
      .object()
      .shape({ id: yup.string().nullable(), label: yup.string() })
      .test(
        'empty-check',
        'A zone must be selected',
        (vendor) => !!vendor.label,
      )
      .typeError('Required.'),
    name: yup.string().required('Required.'),
    category: yup
      .object()
      .shape({ id: yup.string().nullable(), label: yup.string() })
      .test(
        'empty-check',
        'A category must be selected',
        (category) => !!category.label,
      )
      .typeError('Required.'),
  });

  const handleCloseCreateBinModal = () => {
    setForm(initialState);
    setFormErrors(initialFormErrorsState);
    setError('');
    resetData();
    onCloseCreateBinModal();
  };

  const zoneOptions = Array.isArray(customerZones)
    ? customerZones
        .filter(
          (z) =>
            z.customerFacilityId ===
            currentLocationAndFacility.customerFacilityId,
        )
        .filter(
          (z) =>
            z.customerLocationId ===
            currentLocationAndFacility.customerLocationId,
        )
        .map((zone) => ({
          id: zone.zoneId,
          label: zone.name,
        }))
    : [{ id: 0, label: '' }];

  const onSaveNewBin = async () => {
    form.customerLocationId = currentLocationAndFacility.customerLocationId;
    schema
      .validate(form, {
        abortEarly: false,
      })
      .then(async () => {
        try {
          const newForm: any = form;
          newForm.zoneId = form.zoneId.id;
          newForm.category = form.category.id;
          if (binData.binId !== '') {
            const newBin = await editCustomerBinData(binData.binId, newForm);
            snackActions.success(
              `Successfully edited bin from ${currentLocationAndFacility.locationName} in ${currentLocationAndFacility.facilityName}.`,
            );
          } else {
            const newBin = await createCustomerBin(newForm);
            snackActions.success(
              `Successfully created new bin for ${currentLocationAndFacility.locationName} in ${currentLocationAndFacility.facilityName}.`,
            );
          }
          // setShowErrors(false);
          // setError('');
          // setForm(initialState);
          handleCloseCreateBinModal();
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

  const onLoadCustomerZones = async () => {
    try {
      const customerZonesFromApi = await getCustomerFacilityZones(
        currentLocationAndFacility.customerLocationId,
      );

      if (binData.binId !== '') {
        const customerZone: any = customerZonesFromApi.filter(
          (z) => z.zoneId === binData.zoneId,
        );
        setForm({
          customerLocationId: currentLocationAndFacility.customerLocationId,
          zoneId: {
            id: binData.zoneId,
            label: customerZone[0].name,
          },
        });
      }

      setCustomerZones(customerZonesFromApi);
      return true;
    } catch (err) {
      return err;
    }
  };

  useEffect(() => {
    setForm(initialState);
    setCustomerZones([]);
    onLoadCustomerZones();
  }, [currentLocationAndFacility, binData]);

  if (!isCreateBinModalOpen) return null;

  if (currentLocationAndFacility === null) return null;

  return (
    <Modal
      open={isCreateBinModalOpen}
      onClose={() => handleCloseCreateBinModal()}
    >
      <ModalBox>
        <ModalContent>
          {binData.binId === '' ? (
            <Typography
              sx={{ marginBottom: '16px' }}
              variant="h6"
              fontWeight="bold"
            >
              New Bin for {currentLocationAndFacility.facilityName}
            </Typography>
          ) : (
            <Typography
              sx={{ marginBottom: '16px' }}
              variant="h6"
              fontWeight="bold"
            >
              Edit Bin From {currentLocationAndFacility.facilityName}
            </Typography>
          )}

          {!showErrors ? (
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <Autocomplete
                sx={{ width: '100%' }}
                options={zoneOptions}
                // getOptionLabel={(option: AutoCompleteOptionType) =>
                //   `${option.label}` || ''
                // }
                size="small"
                value={form.zoneId}
                onChange={(
                  event: any,
                  value: AutoCompleteOptionType | null,
                ) => {
                  autoCompleteInputHandler('zoneId', value);
                }}
                renderInput={(params) => <TextField {...params} label="Zone" />}
              />
              <Input
                sx={{ width: '100%' }}
                placeholder="Name"
                value={form.name}
                onChange={(value) => inputHandler('name', value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onSaveNewBin();
                  }
                }}
              />
              <Autocomplete
                sx={{ width: '100%' }}
                options={binCategoryOptions}
                // getOptionLabel={(option: AutoCompleteOptionType) =>
                //   `${option.label}` || ''
                // }
                size="small"
                value={form.category}
                onChange={(
                  event: any,
                  value: AutoCompleteOptionType | null,
                ) => {
                  autoCompleteInputHandler('category', value);
                }}
                renderInput={(params) => (
                  <TextField {...params} label="Category" />
                )}
              />
            </Box>
          ) : (
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <Autocomplete
                sx={{ width: '100%' }}
                options={zoneOptions}
                // getOptionLabel={(option: AutoCompleteOptionType) =>
                //   `${option.label}` || ''
                // }
                size="small"
                value={form.zoneId}
                onChange={(
                  event: any,
                  value: AutoCompleteOptionType | null,
                ) => {
                  autoCompleteInputHandler('zoneId', value);
                }}
                renderInput={(params) =>
                  formErrors.zoneId ? (
                    <TextField
                      {...params}
                      error
                      helperText={formErrors.zoneId}
                      label="Zone"
                    />
                  ) : (
                    <TextField {...params} label="Zone" />
                  )
                }
              />
              <Input
                sx={{ width: '100%' }}
                placeholder="Name"
                value={form.name}
                error={formErrors.name}
                onChange={(value) => inputHandler('name', value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onSaveNewBin();
                  }
                }}
              />
              <Autocomplete
                sx={{ width: '100%' }}
                options={binCategoryOptions}
                // getOptionLabel={(option: AutoCompleteOptionType) =>
                //   `${option.label}` || ''
                // }
                size="small"
                value={form.category}
                onChange={(
                  event: any,
                  value: AutoCompleteOptionType | null,
                ) => {
                  autoCompleteInputHandler('category', value);
                }}
                renderInput={(params) =>
                  formErrors.category ? (
                    <TextField
                      {...params}
                      error
                      helperText={formErrors.category}
                      label="Category"
                    />
                  ) : (
                    <TextField {...params} label="Category" />
                  )
                }
              />
            </Box>
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
              onClick={() => handleCloseCreateBinModal()}
            >
              Cancel
            </Button>
            <Button
              sx={{ minHeight: '48px', maxWidth: '91px' }}
              variant="contained"
              size="large"
              onClick={() => onSaveNewBin()}
            >
              Submit
            </Button>
          </Box>
        </ModalContent>
      </ModalBox>
    </Modal>
  );
});
