import React, { useContext, useState, ChangeEvent, useRef } from 'react';
import Barcode from 'react-jsbarcode';
import { useReactToPrint } from 'react-to-print';

import Button from 'components/button';
import { ModalBox, ModalContent } from 'components/styles';
import { printSalesOrderById } from 'services/api/salesorders/salesorders.api';
import { errorCatch } from 'services/api/serivceBase.js';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import { Modal, Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';

interface PrintSalesOrderProps {
  salesOrderData?: any;
}

export default React.memo(({ salesOrderData }: PrintSalesOrderProps) => {
  const theme = useTheme();
  const {
    isSOPrintAndQueueModalOpen,
    onCloseSOPrintAndQueueModal,
    handleUpdateData,
  } = useContext(GlobalContext);
  const labelRef = useRef(null);
  const handlePrint = useReactToPrint({
    content: () => labelRef.current,
  });

  const handleCloseSOPrintAndQueueModal = () => {
    onCloseSOPrintAndQueueModal();
  };

  const onPrintSalesOrder = async () => {
    try {
      const printResponse = printSalesOrderById(salesOrderData.saleOrderId);

      return printResponse;
    } catch (error) {
      return errorCatch(error);
    }
  };

  if (!isSOPrintAndQueueModalOpen) return null;

  return (
    <Modal
      open={isSOPrintAndQueueModalOpen}
      onClose={() => handleCloseSOPrintAndQueueModal()}
    >
      <ModalBox>
        <ModalContent>
          <Typography
            sx={{ marginBottom: '16px' }}
            variant="h6"
            fontWeight="bold"
          >
            Print and Queue
          </Typography>
          <Typography sx={{ marginBottom: '32px' }} variant="subtitle1">
            Print Sale Order {salesOrderData.orderNo}
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
              onClick={() => handleCloseSOPrintAndQueueModal()}
            >
              Cancel
            </Button>
            <Button
              sx={{ minHeight: '48px', maxWidth: '180px' }}
              variant="contained"
              size="large"
              onClick={handlePrint}
            >
              Print & Queue
            </Button>
          </Box>
        </ModalContent>
      </ModalBox>
    </Modal>
  );
});
