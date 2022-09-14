import React, { useContext, useState, useEffect } from 'react';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import {
  updateItemExpirationDate,
  updateItemThreshold,
} from 'services/api/item/item.api';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface ItemModalProps {
  item?: any;
}

export default React.memo(({ item }: ItemModalProps) => {
  const theme = useTheme();
  const {
    isEditThresholdModalOpen,
    onCloseEditThresholdModal,
    handleUpdateData,
  } = useContext(GlobalContext);

  const initialFormErrorsState: any = {
    threshold: '',
  };

  const initialState: any = {
    itemId: '',
    threshold: '',
  };

  const [formErrors, setFormErrors] = useState<any>(initialFormErrorsState);
  const [form, setForm] = useState(initialState);

  const [showErrors, setShowErrors] = useState(false);
  const [error, setError] = useState('');

  const handleCloseModal = () => {
    setForm(initialState);
    setError('');
    onCloseEditThresholdModal();
  };

  const onUpdateItemThreshold = async () => {
    try {
      // itemForAPI.threshold = itemThreshold;
      await updateItemThreshold(form);
      snackActions.success(`Successfully edited item threshold.`);
      handleUpdateData();
      handleCloseModal();
    } catch (err: any) {
      setError(err);
      setShowErrors(true);
      snackActions.error(`${err}`);
    }
  };

  useEffect(() => {
    setForm(item);
  }, [item]);

  if (!isEditThresholdModalOpen) return null;

  return (
    <Modal open={isEditThresholdModalOpen} onClose={() => handleCloseModal()}>
      <ModalBox>
        <ModalContent>
          <Typography
            sx={{ marginBotton: '16px' }}
            variant="h6"
            fontWeight="bold"
          >
            Update Item Threshold
          </Typography>
          <Input
            sx={{ width: '100%' }}
            placeholder="Threshold"
            type="number"
            min={0}
            value={form.threshold}
            size="small"
            onChange={(event) =>
              setForm({ ...form, threshold: event.target.value })
            }
          />
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
              onClick={() => onUpdateItemThreshold()}
            >
              Save
            </Button>
          </Box>
        </ModalContent>
      </ModalBox>
    </Modal>
  );
});
