import React, { useContext, useState, ChangeEvent, useEffect } from 'react';

import Button from 'components/button';
import Input from 'components/input';
import { ModalBox, ModalContent } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import { updateReceiveQty } from 'services/api/receive/receive.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface EditReceiveProps {
  purchaseOrderItem: any;
}

export default React.memo(({ purchaseOrderItem }: EditReceiveProps) => {
  const theme = useTheme();
  const {
    isAdjustPurchaseOrderModalOpen,
    onClosePurchaseOrderItemAdjustModal,
    handleUpdateData,
  } = useContext(GlobalContext);
  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  const [showErrors, setShowErrors] = useState(false);
  const [error, setError] = useState('');

  const initialFormErrorsState: any = {
    qty: '',
  };
  const initialState: any = {
    receiveId: '',
    qty: '',
  };
  const [formErrors, setFormErrors] = useState<any>(initialFormErrorsState);
  const [form, setForm] = useState<any>(initialState);

  const onForm = (key, text) => {
    setForm(() => ({
      ...form,
      [key]: text,
    }));
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) =>
    onForm(key, event.target.value);

  const schema = yup.object().shape({
    qty: yup.number().typeError('Required'),
  });

  const handleClosePurchaseOrderItemAdjustModal = () => {
    setForm(initialState);
    setFormErrors(initialFormErrorsState);
    setError('');
    onClosePurchaseOrderItemAdjustModal();
  };

  const onEditPOItem = async () => {
    form.receiveId = purchaseOrderItem.receiveId;
    schema
      .validate(form, {
        abortEarly: false,
      })
      .then(async () => {
        try {
          await updateReceiveQty(form);
          snackActions.success(`Successfully updated purchase order item.`);

          setShowErrors(false);
          setError('');
          handleClosePurchaseOrderItemAdjustModal();
          handleUpdateData();
        } catch (err: any) {
          setError(err);
          setFormErrors(initialFormErrorsState);
          setShowErrors(true);
          snackActions.error('Unable to update purchase order item.');
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

  useEffect(() => {
    setForm(initialState);
  }, [currentLocationAndFacility]);

  if (!isAdjustPurchaseOrderModalOpen) return null;

  return (
    <Modal
      open={isAdjustPurchaseOrderModalOpen}
      onClose={() => handleClosePurchaseOrderItemAdjustModal()}
    >
      <ModalBox>
        <ModalContent>
          {!showErrors ? (
            <>
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Adjust PO Item
              </Typography>
              <Input
                sx={{ width: '100%' }}
                placeholder="Order Qty"
                value={form.qty}
                type="number"
                onChange={(value) => inputHandler('qty', value)}
              />
            </>
          ) : (
            <>
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                Adjust PO Item
              </Typography>
              <Input
                sx={{ width: '100%' }}
                placeholder="Order Qty"
                value={form.qty}
                error={formErrors.qty}
                type="number"
                onChange={(value) => inputHandler('qty', value)}
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
              onClick={() => handleClosePurchaseOrderItemAdjustModal()}
            >
              Cancel
            </Button>
            <Button
              sx={{ minHeight: '48px', maxWidth: '150px' }}
              variant="contained"
              size="large"
              onClick={() => onEditPOItem()}
            >
              Update
            </Button>
          </Box>
        </ModalContent>
      </ModalBox>
    </Modal>
  );
});
