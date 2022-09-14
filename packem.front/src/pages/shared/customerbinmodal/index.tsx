/* eslint-disable no-nested-ternary */
import React, { useContext, useState, ChangeEvent, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

import Button from 'components/button';
import DatePickerInput from 'components/datepicker';
import Input from 'components/input/Input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import { createCustomerVendor } from 'services/api/customer/customer.api';
import {
  lookupCustomerBinByZoneId,
  createCustomerBin,
} from 'services/api/customerfacilities/customerfacilities.api';
import {
  getCustomerInventory,
  createNewInventoryItem,
} from 'services/api/inventory/inventory.api';
import { createLot, getLotLookup } from 'services/api/lot/lot.api.';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography, Autocomplete, TextField } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface BinItemDetailProps {
  itemDetails: any;
}

interface AutoCompleteOptionType {
  id: number;
  label: string;
}

type NewVendor = {
  customerId: string;
  account: string;
  name: string;
  contact: string;
  phone: string;
  address: string;
};

type NewBin = {
  customerLocationId: string;
  zoneId: string;
  name: string;
};

type NewCreatedVendor = {
  customerId: string;
  vendorNo: string;
  name: string;
  contact: string;
  address: string;
  address2: string;
  city: string;
  stateProvince: string;
  zipPostalCode: string;
  phone: string;
  vendorId: number;
};

type NewCreatedBin = {
  customerLocationId: string;
  zoneId: string;
  name: string;
  binId: number;
};

type NewLotNumber = {
  customerId: number;
  customerLocationId: number;
  itemId: number;
  lotNo: string;
  expirationDate: string;
};

type NewCreatedLotNumber = {
  lotId: number;
  customerLocationId: number;
  itemId: number;
  lotNo: string;
  expirationDate: string;
};

export default React.memo(({ itemDetails }: BinItemDetailProps) => {
  const theme = useTheme();
  const navigate = useNavigate();
  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  const { isBinModalOpen, onCloseBinModal, handleUpdateData } =
    useContext(GlobalContext);
  const [inventoryVendors, setInventoryVendors] = useState([]);
  const [inventoryBins, setInventoryBins] = useState<any>([]);
  const [inventoryZones, setInventoryZones] = useState([]);
  const [newVendor, setNewVendor] = useState(false);
  const [newVendorName, setNewVendorName] = useState('');
  const [newBin, setNewBin] = useState(false);
  const [newBinName, setNewBinName] = useState('');
  const [newLotNumber, setNewLotNumber] = useState(false);
  const [newLotNumberValue, setNewLotNumberValue] = useState('');
  const [itemLotNumbers, setItemLotNumbers] = useState([]);
  const defaultAutocompleteOption: AutoCompleteOptionType | null = {
    id: -1,
    label: '',
  };
  const [showErrors, setShowErrors] = useState(false);
  const [error, setError] = useState('');
  const initialFormErrorsState: any = {
    itemId: '',
    zoneId: '',
    vendorId: '',
    vendorNo: '',
    vendorPointOfContact: '',
    vendorAddress: '',
    vendorPhoneNumber: '',
    binId: '',
    qtyAtBin: '',
    lotId: '',
    expirationDate: '',
  };
  const initialState: any = {
    itemId: '',
    customerId: '',
    customerLocationId: currentLocationAndFacility.customerLocationId,
    facilityId: currentLocationAndFacility.customerFacilityId,
    zoneId: {
      id: defaultAutocompleteOption.id,
      label: defaultAutocompleteOption.label,
    },
    vendorId: {
      id: defaultAutocompleteOption.id,
      label: defaultAutocompleteOption.label,
    },
    vendorNo: '',
    vendorPointOfContact: '',
    vendorAddress: '',
    vendorPhoneNumber: '',
    binId: {
      id: defaultAutocompleteOption.id,
      label: defaultAutocompleteOption.label,
    },
    qtyAtBin: '',
    lotId: {
      id: defaultAutocompleteOption.id,
      label: defaultAutocompleteOption.label,
    },
    expirationDate: '',
  };
  const [formErrors, setFormErrors] = useState<any>(initialFormErrorsState);
  const [form, setForm] = useState<any>(initialState);

  const zoneSelectionOptions =
    Array.isArray(inventoryZones) && form.facilityId !== null
      ? inventoryZones
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

  const vendorSelectionOptions = Array.isArray(inventoryVendors)
    ? inventoryVendors
        .filter((v) => v.name !== 'New Vendor')
        .map((vendor) => ({
          id: vendor.vendorId,
          label: vendor.name,
        }))
    : [{ id: 0, label: 'New Vendor' }];

  const binSelectionOptions =
    Array.isArray(inventoryBins) && form.zoneId !== null
      ? inventoryBins.map((bin) => ({
          id: bin.binId,
          label: bin.name,
        }))
      : [{ id: 0, label: '' }];

  const lotNumberSelectionOptions = Array.isArray(itemLotNumbers)
    ? itemLotNumbers.map((i, index) => ({
        id: i.lotId,
        label: i.lotNo,
      }))
    : [];

  const schema = yup.object().shape({
    zoneId: yup
      .object()
      .shape({ id: yup.string().nullable(), label: yup.string() })
      .test('empty-check', 'A zone must be selected', (zone) => !!zone.label)
      .typeError('Required.'),
    vendorId: yup
      .object()
      .shape({ id: yup.string().nullable(), label: yup.string() })
      .test(
        'empty-check',
        'A vendor must be selected',
        (vendor) => !!vendor.label,
      )
      .typeError('Required.'),
    vendorNo: newVendor ? yup.string().required('Required') : yup.string(),
    vendorPointOfContact: newVendor ? yup.string() : yup.string(),
    vendorAddress: newVendor ? yup.string() : yup.string(),
    vendorPhoneNumber: newVendor ? yup.string() : yup.string(),
    binId:
      form.zoneId === null || form.zoneId.id === -1
        ? null
        : yup
            .object()
            .shape({ id: yup.string().nullable(), label: yup.string() })
            .test(
              'empty-check',
              'An bin must be selected',
              (bin) => !!bin.label,
            )
            .typeError('Required.'),
    qtyAtBin:
      form.zoneId === null || form.zoneId.id === -1
        ? null
        : yup.string().required('Required.'),
    expirationDate: newLotNumber
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

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) => {
    onForm(key, event.target.value);
  };

  const dateInputHandler = (newValue: Date | null) => {
    onForm('expirationDate', newValue);
  };

  const onLoadCustomerBins = async (value) => {
    form.binId = {
      id: defaultAutocompleteOption.id,
      label: defaultAutocompleteOption.label,
    };
    try {
      const binsFromApi = await lookupCustomerBinByZoneId(value.id, '');

      setInventoryBins(binsFromApi);
      return true;
    } catch (err) {
      return err;
    }
  };

  const autoCompleteInputHandler = (
    key: string,
    value: AutoCompleteOptionType,
  ) => {
    const lotNumberValue = form.lotId;

    if (key === 'binId') {
      if (value) {
        if (newBin) {
          setNewBin(false);
        } else {
          setFormErrors({ ...formErrors, binId: '' });
        }
      } else if (newBin) {
        setNewBinName('');
      }
    }

    if (key === 'vendorId') {
      if (value) {
        if (newVendor) {
          setNewVendor(false);
        } else {
          setFormErrors({ ...formErrors, vendorId: '' });
        }
      } else if (newBin) {
        setNewVendorName('');
      }
    }

    if (key === 'lotId') {
      if (value) {
        if (newLotNumber) {
          setNewLotNumber(false);
        }
      } else if (newLotNumber) {
        setNewLotNumberValue('');
      }
    }

    setForm(() => ({
      ...form,
      [key]: !value
        ? {
            id: defaultAutocompleteOption.id,
            label: defaultAutocompleteOption.label,
          }
        : value,
    }));

    if (key === 'zoneId') {
      if (!value) {
        setFormErrors({ ...formErrors, binId: '' });
      }
      setFormErrors({ ...formErrors, zoneId: '' });
      onLoadCustomerBins(value);
    }
  };

  const handleCloseBinModal = () => {
    setForm(initialState);
    setFormErrors(initialFormErrorsState);
    setError('');
    setNewVendor(false);
    setNewVendorName('');
    setNewBinName('');
    setNewBin(false);
    setNewLotNumber(false);
    setNewLotNumberValue('');
    onCloseBinModal();
  };

  const onSaveNewInventory = async () => {
    form.customerId = currentUser.Claim_CustomerId;
    form.itemId = itemDetails;

    if (newVendor) {
      form.vendorId.label = newVendorName;
    }

    if (newBin) {
      form.binId.label = newBinName;
    }

    if (newLotNumber) {
      form.lotId.label = newLotNumberValue;
    }

    schema
      .validate(form, {
        abortEarly: false,
      })
      .then(async () => {
        try {
          let newVendorProfile = {} as NewCreatedVendor;
          let newBinProfile = {} as NewCreatedBin;
          let newLotNumberProfile = {} as NewCreatedLotNumber;
          const newForm: any = form;

          if (newVendor) {
            const newVendorForm = {} as NewVendor;
            newVendorForm.customerId = form.customerId;
            newVendorForm.name = newVendorName;
            newVendorForm.account = form.vendorNo;
            newVendorForm.contact = form.vendorPointOfContact;
            newVendorForm.phone = form.vendorPhoneNumber;
            newVendorForm.address = form.vendorAddress;

            newVendorProfile = await createCustomerVendor(newVendorForm);

            newForm.vendorId = newVendorProfile.vendorId.toString();

            snackActions.success(`Successfully created new vendor.`);
          } else {
            newForm.vendorId = form.vendorId.id.toString();
          }

          if (newBin) {
            const newBinForm = {} as NewBin;

            newBinForm.customerLocationId =
              currentLocationAndFacility.customerLocationId;
            newBinForm.name = newBinName;
            newBinForm.zoneId = form.zoneId.id.toString();

            newBinProfile = await createCustomerBin(newBinForm);

            newForm.binId = newBinProfile.binId.toString();

            snackActions.success(`Successfully created new bin.`);
          } else {
            newForm.binId = form.binId.id.toString();
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
          } else {
            newForm.lotId =
              form.lotId.id === -1 ? null : form.lotId.id.toString();
          }

          newForm.zoneId = form.zoneId.id;
          await createNewInventoryItem(newForm);
          snackActions.success(`Successfully added new inventory.`);
          handleCloseBinModal();
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

  const onLoadCustomerInventory = async () => {
    try {
      const inventoryFromApi = await getCustomerInventory(
        currentUser.Claim_CustomerId,
      );

      setInventoryVendors(inventoryFromApi.vendors);
      setInventoryZones(inventoryFromApi.zones);

      return true;
    } catch (err) {
      return err;
    }
  };

  useEffect(() => {
    setForm(initialState);
    setInventoryVendors([]);
    setInventoryBins([]);
    setInventoryZones([]);
    onLoadCustomerInventory();
    onLoadLotNumbers(itemDetails);
  }, [itemDetails, currentLocationAndFacility]);

  if (!isBinModalOpen) return null;

  return (
    <Modal open={isBinModalOpen} onClose={() => handleCloseBinModal()}>
      <ModalBox>
        <ModalContent>
          <Typography
            sx={{ marginBottom: '16px' }}
            variant="h6"
            fontWeight="bold"
          >
            Add Bin & Qty
          </Typography>
          <Box
            sx={{
              display: 'flex',
              width: '100%',
              [theme.breakpoints.up('sm')]: {
                flexDirection: 'row',
              },
              [theme.breakpoints.down('sm')]: {
                flexDirection: 'column',
              },
              gap: '16px',
            }}
          >
            <Box
              sx={{
                width: '100%',
                display: 'flex',
                flexDirection: 'column',
                gap: '16px',
              }}
            >
              <Autocomplete
                sx={{ width: '100%' }}
                options={zoneSelectionOptions}
                getOptionLabel={(option: AutoCompleteOptionType) =>
                  `${option.label}` || ''
                }
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
                      label="Zone"
                      error
                      helperText={formErrors.zoneId}
                    />
                  ) : (
                    <TextField {...params} label="Zone" />
                  )
                }
              />
              {form.zoneId === null || form.zoneId.id === -1 ? (
                <Autocomplete
                  sx={{ width: '100%' }}
                  disabled
                  options={binSelectionOptions}
                  getOptionLabel={(option: AutoCompleteOptionType) =>
                    `${option.label}` || ''
                  }
                  size="small"
                  value={form.binId}
                  onChange={(
                    event: any,
                    value: AutoCompleteOptionType | null,
                  ) => {
                    autoCompleteInputHandler('binId', value);
                  }}
                  renderInput={(params) => (
                    <TextField {...params} disabled label="Bin" />
                  )}
                />
              ) : newBin ? (
                <>
                  <Autocomplete
                    sx={{ width: '100%' }}
                    freeSolo
                    options={binSelectionOptions}
                    getOptionLabel={(option: AutoCompleteOptionType) =>
                      `${option.label}` || ''
                    }
                    size="small"
                    value={form.binId}
                    onChange={(
                      event: any,
                      value: AutoCompleteOptionType | null,
                    ) => {
                      autoCompleteInputHandler('binId', value);
                    }}
                    renderInput={(params) =>
                      formErrors.binId ? (
                        <TextField
                          {...params}
                          label="Bin"
                          error
                          helperText={formErrors.binId}
                          onChange={(event) => {
                            setNewBinName(event.target.value);
                          }}
                        />
                      ) : (
                        <TextField
                          {...params}
                          label="Bin"
                          onChange={(event) => {
                            setNewBinName(event.target.value);
                          }}
                        />
                      )
                    }
                  />
                  <Box
                    sx={{
                      width: '100%',
                      display: 'flex',
                      flexDirection: 'column',
                      gap: '16px',
                    }}
                  >
                    <Input
                      sx={{ width: '100%' }}
                      placeholder="Qty at Bin"
                      value={form.qtyAtBin}
                      error={formErrors.qtyAtBin}
                      onChange={(value) => inputHandler('qtyAtBin', value)}
                    />
                  </Box>
                </>
              ) : (
                <>
                  <Autocomplete
                    sx={{ width: '100%' }}
                    options={binSelectionOptions}
                    getOptionLabel={(option: AutoCompleteOptionType) =>
                      `${option.label}` || ''
                    }
                    size="small"
                    value={form.binId}
                    onChange={(
                      event: any,
                      value: AutoCompleteOptionType | null,
                    ) => {
                      setNewBinName(value.label);
                      autoCompleteInputHandler('binId', value);
                    }}
                    renderInput={(params) =>
                      formErrors.binId ? (
                        <TextField
                          {...params}
                          label="Bin"
                          error
                          helperText={formErrors.binId}
                        />
                      ) : (
                        <TextField {...params} label="Bin" />
                      )
                    }
                    noOptionsText={
                      <Button
                        variant="text"
                        size="small"
                        onMouseDown={() => {
                          setForm({
                            ...form,
                            binId: {
                              id: defaultAutocompleteOption.id,
                              label: defaultAutocompleteOption.label,
                            },
                          });
                          setNewBin(true);
                        }}
                      >
                        + Add New Bin
                      </Button>
                    }
                  />
                  <Box
                    sx={{
                      width: '100%',
                      display: 'flex',
                      flexDirection: 'column',
                      gap: '16px',
                    }}
                  >
                    <Input
                      sx={{ width: '100%' }}
                      placeholder="Qty at Bin"
                      value={form.qtyAtBin}
                      error={formErrors.qtyAtBin}
                      onChange={(value) => inputHandler('qtyAtBin', value)}
                    />
                  </Box>
                </>
              )}
              {newLotNumber ? (
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
                    formErrors.itemId ? (
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
                    formErrors.itemId ? (
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
                  label="Exp. Date"
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
            </Box>
            <Box
              sx={{
                width: '100%',
                display: 'flex',
                flexDirection: 'column',
                gap: '16px',
              }}
            >
              {newVendor ? (
                <>
                  <Autocomplete
                    sx={{ width: '100%' }}
                    freeSolo
                    options={vendorSelectionOptions}
                    getOptionLabel={(option: AutoCompleteOptionType) =>
                      `${option.label}` || ''
                    }
                    size="small"
                    value={form.vendorId}
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
                          onChange={(event) => {
                            setNewVendorName(event.target.value);
                          }}
                        />
                      ) : (
                        <TextField
                          {...params}
                          label="Vendor"
                          onChange={(event) => {
                            setNewVendorName(event.target.value);
                          }}
                        />
                      )
                    }
                  />
                  <Box
                    sx={{
                      width: '100%',
                      display: 'flex',
                      flexDirection: 'column',
                      gap: '16px',
                    }}
                  >
                    <Input
                      sx={{ width: '100%' }}
                      placeholder="Vendor Account."
                      value={form.vendorNo}
                      error={formErrors.vendorNo}
                      onChange={(value) => inputHandler('vendorNo', value)}
                    />
                    <Input
                      sx={{ width: '100%' }}
                      placeholder="Vendor Point of Contact"
                      value={form.vendorPointOfContact}
                      error={formErrors.vendorPointOfContact}
                      onChange={(value) =>
                        inputHandler('vendorPointOfContact', value)
                      }
                    />
                    <Input
                      sx={{ width: '100%' }}
                      placeholder="Vendor Address"
                      value={form.vendorAddress}
                      error={formErrors.vendorAddress}
                      onChange={(value) => inputHandler('vendorAddress', value)}
                    />
                    <Input
                      sx={{ width: '100%' }}
                      placeholder="Vendor Phone Number"
                      value={form.vendorPhoneNumber}
                      error={formErrors.vendorPhoneNumber}
                      onChange={(value) =>
                        inputHandler('vendorPhoneNumber', value)
                      }
                    />
                  </Box>
                </>
              ) : (
                <Autocomplete
                  sx={{ width: '100%' }}
                  options={vendorSelectionOptions}
                  getOptionLabel={(option: AutoCompleteOptionType) =>
                    `${option.label}` || ''
                  }
                  size="small"
                  value={form.vendorId}
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
                  noOptionsText={
                    <Button
                      variant="text"
                      size="small"
                      onMouseDown={() => {
                        setForm({
                          ...form,
                          vendorId: {
                            id: defaultAutocompleteOption.id,
                            label: defaultAutocompleteOption.label,
                          },
                        });
                        setNewVendor(true);
                      }}
                    >
                      + Add New Vendor
                    </Button>
                  }
                />
              )}
            </Box>
          </Box>
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
              onClick={() => handleCloseBinModal()}
            >
              Cancel
            </Button>
            <Button
              sx={{ minHeight: '48px', maxWidth: '164px' }}
              variant="contained"
              size="large"
              onClick={() => onSaveNewInventory()}
            >
              Add Bin & Qty
            </Button>
          </Box>
        </ModalContent>
      </ModalBox>
    </Modal>
  );
});
