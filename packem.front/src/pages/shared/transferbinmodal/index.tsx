import React, { useContext, useState, ChangeEvent, useEffect } from 'react';
import { useLocation, useSearchParams } from 'react-router-dom';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import {
  getCustomerBinsByLocationId,
  getCustomerFacilityZones,
  lookupCustomerBinByZoneId,
} from 'services/api/customerfacilities/customerfacilities.api';
import { createItemTransfer } from 'services/api/item/item.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography, Autocomplete, TextField } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface TransferBinModalProps {
  ItemDetails?: any;
  callBack?: () => void;
}

interface AutoCompleteOptionType {
  id: number;
  label: string;
}

export default React.memo(
  ({ ItemDetails, callBack }: TransferBinModalProps) => {
    const theme = useTheme();
    const getLocation = useLocation();
    const [urlSearchParams] = useSearchParams();

    const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
    const {
      isTransferModalOpen,
      onCloseTransferModal,
      handleUpdateData,
      updateData,
    } = useContext(GlobalContext);

    const [zones, setZones] = useState([]);
    const [bins, setBins] = useState([]);

    const defaultAutocompleteOption: AutoCompleteOptionType | null = {
      id: -1,
      label: '',
    };

    const [showErrors, setShowErrors] = useState(false);
    const [error, setError] = useState('');
    const [formErrors, setFormErrors] = useState({
      customerId: '',
      customerLocationId: '',
      itemId: '',
      itemFacilityId: '',
      itemBinId: '',
      newZoneId: '',
      newBinId: '',
      qtyToTransfer: '',
    });

    const initialState: any = {
      customerId: currentUser.Claim_CustomerId,
      customerLocationId: currentLocationAndFacility.customerLocationId,
      itemId: urlSearchParams.get('itemId'),
      itemFacilityId: '',
      itemBinId: '',
      newZoneId: {
        id: defaultAutocompleteOption.id,
        label: defaultAutocompleteOption.label,
      },
      newBinId: {
        id: defaultAutocompleteOption.id,
        label: defaultAutocompleteOption.label,
      },
      qtyToTransfer: '',
    };

    const [form, setForm] = useState<any>(initialState);

    const currentItem = getLocation.pathname.split('/')[3];

    const zoneSelectionOptions =
      Array.isArray(zones) && form.facilityId !== null
        ? zones
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

    const binSelectionOptions =
      Array.isArray(bins) && form.newZoneId !== null
        ? bins.map((bin) => ({
            id: bin.binId,
            label: bin.name,
          }))
        : [{ id: '', label: '' }];

    const onForm = (key, text) => {
      setForm(() => ({
        ...form,
        [key]: text,
      }));
    };

    const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) =>
      onForm(key, event.target.value);

    const schema = yup.object().shape({
      customerLocationId: yup.number().required('Required.'),
      customerId: yup.number().required('Required.'),
      itemId: yup.string().required('Required.'),
      itemFacilityId: yup.string().required('Required.'),
      itemBinId: yup.string().required('Required.'),
      newZoneId: yup.string().required('Required.'),
      newBinId: yup.string().required('Required'),
      qtyToTransfer: yup.number().typeError('A role must be selected'),
    });

    const handleCloseTransferModal = () => {
      setForm(initialState);
      if (callBack !== undefined) {
        callBack();
      }
      onCloseTransferModal();
    };

    const onCreateNewTransfer = async () => {
      form.itemBinId = ItemDetails.binId;
      form.itemFacilityId = ItemDetails.customerFacilityId;
      form.newBinId = form.newBinId.id;
      form.newZoneId = form.newZoneId.id;
      schema
        .validate(form, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            await createItemTransfer(form);
            snackActions.success(
              `Successfully added ${currentItem} to transfer queue.`,
            );
            setForm(initialState);
            handleCloseTransferModal();
            handleUpdateData();
          } catch (err: any) {
            setError(err);
            setFormErrors({
              customerId: '',
              customerLocationId: '',
              itemId: '',
              itemFacilityId: '',
              itemBinId: '',
              newZoneId: '',
              newBinId: '',
              qtyToTransfer: '',
            });
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

    const onLoadCustomerBins = async (value) => {
      form.newBinId = {
        id: defaultAutocompleteOption.id,
        label: defaultAutocompleteOption.label,
      };
      try {
        const binsFromApi = await lookupCustomerBinByZoneId(value.id, '');

        setBins(binsFromApi);
        return true;
      } catch (err) {
        return err;
      }
    };

    const autoCompleteInputHandler = (
      key: string,
      value: AutoCompleteOptionType,
    ) => {
      setForm(() => ({
        ...form,
        [key]: value,
      }));
      if (key === 'newZoneId') onLoadCustomerBins(value);
    };

    const onLoadLocationsZonesAndBins = async () => {
      try {
        const binsFromApi = await getCustomerBinsByLocationId(
          currentLocationAndFacility.customerLocationId,
        );
        const zonesFromApi = await getCustomerFacilityZones(
          currentLocationAndFacility.customerLocationId,
        );
        setZones(zonesFromApi);
        setBins(binsFromApi);
        return true;
      } catch (err) {
        return err;
      }
    };

    useEffect(() => {
      setBins([]);
      setZones([]);
      onLoadLocationsZonesAndBins();
    }, [ItemDetails, currentLocationAndFacility]);

    if (!isTransferModalOpen) return null;

    return (
      <Modal
        open={isTransferModalOpen}
        onClose={() => handleCloseTransferModal()}
      >
        <ModalBox>
          <ModalContent>
            <Typography
              sx={{ marginBotton: '16px' }}
              variant="h6"
              fontWeight="bold"
            >
              Transfer
            </Typography>
            <Input
              sx={{ width: '100%' }}
              placeholder="Item"
              disabled
              value={currentItem}
            />
            <Input
              sx={{ width: '100%' }}
              placeholder="Item"
              disabled
              value={ItemDetails.qtyOnHand}
            />
            <Typography variant="subtitle1" fontWeight="bold">
              From:
            </Typography>
            <Input
              sx={{ width: '100%' }}
              placeholder="Current Facility"
              disabled
              value={ItemDetails.facility}
            />
            <Input
              sx={{ width: '100%' }}
              placeholder="Current Zone"
              disabled
              value={ItemDetails.zone}
            />
            <Input
              sx={{ width: '100%' }}
              placeholder="Current Bin"
              disabled
              value={ItemDetails.bin}
            />
            <Typography variant="subtitle1" fontWeight="bold">
              To:
            </Typography>
            {!showErrors ? (
              <>
                <Autocomplete
                  sx={{ width: '100%' }}
                  options={zoneSelectionOptions}
                  getOptionLabel={(option: AutoCompleteOptionType) =>
                    `${option.label}` || ''
                  }
                  size="small"
                  value={form.newZoneId}
                  onChange={(
                    event: any,
                    value: AutoCompleteOptionType | null,
                  ) => {
                    autoCompleteInputHandler('newZoneId', value);
                  }}
                  renderInput={(params) => (
                    <TextField {...params} label="Zone" />
                  )}
                />
                {form.newZoneId === null || form.newZoneId.id === -1 ? (
                  <Autocomplete
                    sx={{ width: '100%' }}
                    disabled
                    options={binSelectionOptions}
                    getOptionLabel={(option: AutoCompleteOptionType) =>
                      `${option.label}` || ''
                    }
                    size="small"
                    value={form.newBinId}
                    onChange={(
                      event: any,
                      value: AutoCompleteOptionType | null,
                    ) => {
                      autoCompleteInputHandler('newBinId', value);
                    }}
                    renderInput={(params) => (
                      <TextField {...params} disabled label="Bin" />
                    )}
                  />
                ) : (
                  <Autocomplete
                    sx={{ width: '100%' }}
                    options={binSelectionOptions}
                    getOptionLabel={(option: AutoCompleteOptionType) =>
                      `${option.label}` || ''
                    }
                    size="small"
                    value={form.newBinId}
                    onChange={(
                      event: any,
                      value: AutoCompleteOptionType | null,
                    ) => {
                      autoCompleteInputHandler('newBinId', value);
                    }}
                    renderInput={(params) => (
                      <TextField {...params} label="Bin" />
                    )}
                  />
                )}
                <Input
                  type="number"
                  sx={{ width: '100% ' }}
                  placeholder="Quantity to Transfer"
                  value={form.qtyToTransfer}
                  onChange={(value) => inputHandler('qtyToTransfer', value)}
                />
              </>
            ) : (
              <>
                <Autocomplete
                  sx={{ width: '100%' }}
                  options={zoneSelectionOptions}
                  getOptionLabel={(option: AutoCompleteOptionType) =>
                    `${option.label}` || ''
                  }
                  size="small"
                  value={form.newZoneId}
                  onChange={(
                    event: any,
                    value: AutoCompleteOptionType | null,
                  ) => {
                    autoCompleteInputHandler('newZoneId', value);
                  }}
                  renderInput={(params) => (
                    <TextField
                      error
                      helperText={formErrors.newZoneId}
                      {...params}
                      label="Zone"
                    />
                  )}
                />
                <Autocomplete
                  sx={{ width: '100%' }}
                  options={binSelectionOptions}
                  getOptionLabel={(option: AutoCompleteOptionType) =>
                    `${option.label}` || ''
                  }
                  size="small"
                  value={form.newBinId}
                  onChange={(
                    event: any,
                    value: AutoCompleteOptionType | null,
                  ) => {
                    autoCompleteInputHandler('newBinId', value);
                  }}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      error
                      helperText={formErrors.newBinId}
                      label="Bin"
                    />
                  )}
                />
                <Input
                  type="number"
                  sx={{ width: '100% ' }}
                  placeholder="Quantity to Transfer"
                  error={formErrors.qtyToTransfer}
                  value={form.qtyToTransfer}
                  onChange={(value) => inputHandler('qtyToTransfer', value)}
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
                onClick={() => handleCloseTransferModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="contained"
                size="large"
                onClick={() => onCreateNewTransfer()}
              >
                Queue
              </Button>
            </Box>
          </ModalContent>
        </ModalBox>
      </Modal>
    );
  },
);
