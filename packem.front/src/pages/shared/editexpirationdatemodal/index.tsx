import React, { useContext, useState, ChangeEvent, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

import Button from 'components/button';
import DatePickerInput from 'components/datepicker';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import { updateItemExpirationDate } from 'services/api/item/item.api';
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
  const {
    isEditExpirationDateModalOpen,
    onCloseExpirationDateModal,
    handleUpdateData,
  } = useContext(GlobalContext);

  const [showErrors, setShowErrors] = useState(false);
  const [error, setError] = useState('');

  const initialFormErrorsState: any = {
    expirationDate: '',
  };

  const initialState: any = {
    itemId: '',
    expirationDate: '',
  };
  const [formErrors, setFormErrors] = useState<any>(initialFormErrorsState);
  const [form, setForm] = useState(initialState);

  const onForm = (key, text) => {
    setForm(() => ({
      ...form,
      [key]: text,
    }));
  };

  const dateInputHandler = (newValue: Date | null) => {
    onForm('expirationDate', newValue);
  };

  const schema = yup.object().shape({});

  const handleCloseModal = () => {
    setForm(initialState);
    setFormErrors(initialFormErrorsState);
    setError('');
    onCloseExpirationDateModal();
  };

  const onUpdateItemExpirationDate = async () => {
    schema
      .validate(form, {
        abortEarly: false,
      })
      .then(async () => {
        try {
          await updateItemExpirationDate(form);
          snackActions.success(`Successfully edited item expiration date.`);
          handleUpdateData();
          handleCloseModal();
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
    if (item.expirationDate) {
      setForm(item);
    } else {
      setForm({ expirationDate: '', itemId: item.itemId });
    }
  };

  useEffect(() => {
    setForm(initialState);
    onLoadEditItem();
  }, [item]);

  if (!isEditExpirationDateModalOpen) return null;

  return (
    <Modal
      open={isEditExpirationDateModalOpen}
      onClose={() => handleCloseModal()}
    >
      <ModalBox>
        <ModalContent>
          <Typography
            sx={{ marginBotton: '16px' }}
            variant="h6"
            fontWeight="bold"
          >
            Update Expiration Date
          </Typography>
          {!showErrors ? (
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <DatePickerInput
                label="Expiration Date"
                inputFormat="MM/dd/yyyy"
                value={form.expirationDate}
                onChange={dateInputHandler}
                renderInput={(params) => (
                  <TextField {...params} error={false} size="small" />
                )}
              />
            </Box>
          ) : (
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <DatePickerInput
                label="Expiration Date"
                inputFormat="MM/dd/yyyy"
                value={form.expiratioDate}
                onChange={dateInputHandler}
                renderInput={(params) => (
                  <TextField {...params} error={false} size="small" />
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
              onClick={() => handleCloseModal()}
            >
              Cancel
            </Button>
            <Button
              sx={{ minHeight: '48px', maxWidth: '91px' }}
              variant="contained"
              size="large"
              onClick={() => onUpdateItemExpirationDate()}
            >
              Save
            </Button>
          </Box>
        </ModalContent>
      </ModalBox>
    </Modal>
  );
});
