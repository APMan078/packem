import React, { useContext, useState, ChangeEvent, useEffect } from 'react';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import {
  createCustomerFacilityZone,
  editCustomerFacilityZone,
} from 'services/api/customerfacilities/customerfacilities.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface FacilityZoneModalProps {
  zoneData?: any;
  edit?: boolean;
  callBack?: () => void;
}

export default React.memo(
  ({ zoneData, edit, callBack }: FacilityZoneModalProps) => {
    const theme = useTheme();
    const {
      isFacilityZoneModalOpen,
      onCloseFacilityZoneModal,
      handleUpdateData,
    } = useContext(GlobalContext);
    const { currentLocationAndFacility } = useContext(AuthContext);
    const [errors, setErrors] = useState({});
    const initialState = {
      customerLocationId: currentLocationAndFacility.customerLocationId,
      customerFacilityId: currentLocationAndFacility.customerFacilityId,
      name: '',
      zoneId: '',
    };
    const [form, setForm] = useState(initialState);

    const onForm = (key, text) => {
      setForm(() => ({
        ...form,
        [key]: text,
      }));
    };

    const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) =>
      onForm(key, event.target.value);

    const schema = yup.object().shape({
      customerLocationId: yup.string().required('Required.'),
      customerFacilityId: yup.string().required('Required'),
      name: yup.string().required('Required.'),
    });

    const handleCloseFacilityZoneModal = () => {
      setForm(initialState);
      onCloseFacilityZoneModal();
      if (callBack !== undefined) {
        callBack();
      }
    };

    const onSaveNewFacilityZone = async () => {
      schema
        .validate(form, {
          abortEarly: false,
        })
        .then(async () => {
          try {
            if (edit) {
              form.zoneId = zoneData.zoneId;
              await editCustomerFacilityZone(zoneData.zoneId, form);
              snackActions.success(`Successfully edited ${zoneData.name}.`);
            } else {
              const newZone = await createCustomerFacilityZone(form);
              snackActions.success(`Successfully created new zone.`);
            }
            setForm(initialState);
            handleCloseFacilityZoneModal();
            handleUpdateData();
          } catch (error) {
            setErrors(error);
            snackActions.error(`${error}`);
          }
        })
        .catch((err) => {
          const errorsFound = err.inner.reduce((acc, curr) => {
            if (!acc[curr.path]) acc[curr.path] = curr.message;
            return acc;
          }, {});
          setErrors(errorsFound);
        });
    };

    useEffect(() => {
      if (edit) {
        setForm(zoneData);
      }
    }, [edit]);

    if (!isFacilityZoneModalOpen) return null;

    return (
      <Modal
        open={isFacilityZoneModalOpen}
        onClose={() => handleCloseFacilityZoneModal()}
      >
        <ModalBox>
          <ModalContent>
            {edit ? (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Edit Zone ({zoneData.name})
              </Typography>
            ) : (
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Add Zone
                <br /> ({currentLocationAndFacility.locationName} •{' '}
                {currentLocationAndFacility.facilityName})
              </Typography>
            )}

            <Input
              sx={{ width: '100%' }}
              placeholder="Name"
              value={form.name}
              onChange={(value) => inputHandler('name', value)}
            />
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
                onClick={() => handleCloseFacilityZoneModal()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="contained"
                size="large"
                onClick={() => onSaveNewFacilityZone()}
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
