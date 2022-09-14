/* eslint-disable jsx-a11y/anchor-is-valid */
import React, { useState, useContext, useEffect } from 'react';

import Button from 'components/button';
import { ModalBox, ModalContent } from 'components/styles';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface ConfirmationModalProps {
  dialogText: string;
  callBack?: () => void;
}

export default React.memo(
  ({ dialogText, callBack }: ConfirmationModalProps) => {
    const theme = useTheme();

    const { isConfirmationModalOpen, onCloseConfirmationModal } =
      useContext(GlobalContext);
    const [disabled, setDisabled] = useState(false);

    const handleModalClose = () => {
      onCloseConfirmationModal();
      setDisabled(false);
    };

    const handleConfirmClick = async () => {
      setDisabled(true);
      await callBack();
      onCloseConfirmationModal();
      setDisabled(false);
    };

    if (!isConfirmationModalOpen) return null;

    return (
      <Modal open={isConfirmationModalOpen} onClose={handleModalClose}>
        <ModalBox>
          <ModalContent>
            <Box
              sx={{
                width: '100%',
                display: 'flex',
                flexDirection: 'column',
                gap: '16px',
              }}
            >
              <Typography
                sx={{ marginBottom: '16px' }}
                variant="h6"
                fontWeight="bold"
              >
                {dialogText}
              </Typography>
            </Box>
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
                onClick={() => handleModalClose()}
              >
                Cancel
              </Button>
              <Button
                sx={{ minHeight: '48px', maxWidth: '91px' }}
                variant="contained"
                size="large"
                onClick={() => handleConfirmClick()}
              >
                Confirm
              </Button>
            </Box>
          </ModalContent>
        </ModalBox>
      </Modal>
    );
  },
);
