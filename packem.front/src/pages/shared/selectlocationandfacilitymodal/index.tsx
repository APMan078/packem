import React, { useContext, useState, useEffect } from 'react';

import Button from 'components/button';
import { ModalBox, ModalContent } from 'components/styles';
import { AESEncrypt } from 'helpers/encryptdecrypt';
import { getCustomerFacilitiesByCustomerId } from 'services/api/customerfacilities/customerfacilities.api';
import { getCustomerLocationsById } from 'services/api/customerlocations/customerlocations.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Modal, Box, Typography, Autocomplete, TextField } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface AutoCompleteOptionType {
  id: number;
  label: string;
}

export default React.memo(() => {
  const theme = useTheme();
  const { currentUser, updateLocationAndFacility, currentLocationAndFacility } =
    useContext(AuthContext);
  const {
    isLocationAndFacilityModalOpen,
    updateData,
    onCloseLocationAndFacilityModal,
    handleUpdateData,
  } = useContext(GlobalContext);
  const [locations, setLocations] = useState([]);
  const [facilities, setFacilities] = useState([]);
  const locationDefault: AutoCompleteOptionType | null = {
    id: -1,
    label: '',
  };
  const facilityDefault: AutoCompleteOptionType | null = {
    id: -1,
    label: '',
  };

  const [showErrors, setShowErrors] = useState(false);
  const [error, setError] = useState('');
  const [formErrors, setFormErrors] = useState({
    customerLocationId: '',
    customerFacilityId: '',
  });
  const initialState: any = {
    customerLocationId: locationDefault,
    customerFacilityId: facilityDefault,
  };
  const [form, setForm] = useState<any>(initialState);

  const selectOptions = Array.isArray(locations)
    ? locations.map((location) => ({
        id: location.customerLocationId,
        label: location.name,
      }))
    : [{ id: 0, label: '' }];

  const facilityOptions =
    Array.isArray(facilities) && form.customerLocationId !== null
      ? facilities
          .filter((f) => f.customerLocationId === form.customerLocationId.id)
          .map((facility) => ({
            id: facility.customerFacilityId,
            label: facility.name,
          }))
      : [{ id: 0, label: '' }];

  const autoCompleteInputHandler = (
    key: string,
    value: AutoCompleteOptionType,
  ) => {
    if (key === 'customerLocationId') {
      form.customerFacilityId = facilityDefault;
    }
    setForm(() => ({
      ...form,
      [key]: value,
    }));
  };

  const handleCloseLocationAndFacilityModal = () => {
    onCloseLocationAndFacilityModal();
  };

  const onSaveLocationAndFacility = () => {
    form.customerLocationId = form.customerLocationId.id;
    form.customerFacilityId = form.customerFacilityId.id;

    const locationData: any = locations.filter(
      (l) => l.customerLocationId === form.customerLocationId,
    );
    const facilityData: any = facilities.filter(
      (f) =>
        f.customerFacilityId === form.customerFacilityId &&
        f.customerLocationId === form.customerLocationId,
    );
    const locationAndFacilityData = facilityData.map((facility) => ({
      customerFacilityId: facility.customerFacilityId,
      customerLocationId: facility.customerLocationId,
      locationName: locationData[0].name,
      facilityName: facility.name,
      facilityAddress: facility.address,
      facilityAddress2: facility.address2,
      facilityCity: facility.city,
      facilityZip: facility.zipPostalCode,
      facilityStateProvince: facility.stateProvince,
      facilityPhoneNumber: facility.phoneNumber,
    }));

    const encryptedLocationAndFacility = AESEncrypt(locationAndFacilityData[0]);

    updateLocationAndFacility({
      locationFacility: locationAndFacilityData[0],
      encryptedLocation: encryptedLocationAndFacility,
    });

    setForm(initialState);
    handleCloseLocationAndFacilityModal();
    handleUpdateData();
  };

  const onLoadLocationsAndFacilities = async () => {
    try {
      const locationsFromApi = await getCustomerLocationsById(
        currentUser.Claim_CustomerId,
      );
      const facilitiesFromApi = await getCustomerFacilitiesByCustomerId(
        currentUser.Claim_CustomerId,
      );

      setLocations(locationsFromApi);
      setFacilities(facilitiesFromApi);

      return true;
    } catch (err) {
      return err;
    }
  };

  useEffect(() => {
    if (currentLocationAndFacility) {
      setForm({
        customerLocationId: {
          id: currentLocationAndFacility.customerLocationId,
          label: currentLocationAndFacility.locationName,
        },
        customerFacilityId: {
          id: currentLocationAndFacility.customerFacilityId,
          label: currentLocationAndFacility.facilityName,
        },
      });
    }
    setLocations([]);
    onLoadLocationsAndFacilities();
  }, [updateData, currentLocationAndFacility]);

  if (!isLocationAndFacilityModalOpen) return null;

  return (
    <Modal
      open={isLocationAndFacilityModalOpen}
      onClose={() => handleCloseLocationAndFacilityModal()}
    >
      <ModalBox>
        <ModalContent>
          <Typography
            sx={{ marginBotton: '16px' }}
            variant="h6"
            fontWeight="bold"
          >
            Choose Facility & Location
          </Typography>
          {!showErrors ? (
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
                freeSolo
                options={selectOptions}
                getOptionLabel={(option: AutoCompleteOptionType) =>
                  `${option.label}` || ''
                }
                size="small"
                value={form.customerLocationId}
                onChange={(
                  event: any,
                  value: AutoCompleteOptionType | null,
                ) => {
                  autoCompleteInputHandler('customerLocationId', value);
                }}
                renderInput={(params) => (
                  <TextField {...params} label="Location" />
                )}
              />
              {form.customerLocationId !== locationDefault ? (
                <Autocomplete
                  sx={{ width: '100%' }}
                  options={facilityOptions}
                  getOptionLabel={(option: AutoCompleteOptionType) =>
                    `${option.label}` || ''
                  }
                  size="small"
                  value={form.customerFacilityId}
                  onChange={(
                    event: any,
                    value: AutoCompleteOptionType | null,
                  ) => {
                    autoCompleteInputHandler('customerFacilityId', value);
                  }}
                  renderInput={(params) => (
                    <TextField {...params} label="Facility" />
                  )}
                />
              ) : (
                <Autocomplete
                  sx={{ width: '100%' }}
                  disabled
                  options={facilityOptions}
                  getOptionLabel={(option: AutoCompleteOptionType) =>
                    `${option.label}` || ''
                  }
                  size="small"
                  value={form.customerFacilityId}
                  renderInput={(params) => (
                    <TextField {...params} disabled label="Facility" />
                  )}
                />
              )}
            </Box>
          ) : (
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
                options={selectOptions}
                getOptionLabel={(option: AutoCompleteOptionType) =>
                  `${option.label}` || ''
                }
                size="small"
                value={form.customerLocationId}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    error
                    helperText={formErrors.customerLocationId}
                    label="Location"
                  />
                )}
              />
              <Autocomplete
                sx={{ width: '100%' }}
                options={facilityOptions}
                getOptionLabel={(option: AutoCompleteOptionType) =>
                  `${option.label}` || ''
                }
                size="small"
                value={form.customerFacilityId}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    error
                    helperText={formErrors.customerFacilityId}
                    label="Facility"
                  />
                )}
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
              onClick={() => handleCloseLocationAndFacilityModal()}
            >
              Cancel
            </Button>
            <Button
              sx={{ minHeight: '48px', maxWidth: '91px' }}
              variant="contained"
              size="large"
              onClick={() => onSaveLocationAndFacility()}
            >
              Submit
            </Button>
          </Box>
        </ModalContent>
      </ModalBox>
    </Modal>
  );
});
