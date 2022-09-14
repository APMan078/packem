import React, { useContext } from 'react';

import Button from 'components/button';
import { ModalBox, ModalContent } from 'components/styles';
import Papa from 'papaparse';
import { GlobalContext } from 'store/contexts/GlobalContext';
import * as yup from 'yup';

import AddIcon from '@mui/icons-material/Add';
import { Modal, Box } from '@mui/material';

interface AddFileProps {
  callBack?: (x: any) => void;
}

export default React.memo(({ callBack }: AddFileProps) => {
  const { isFileInputModalOpen, onCloseInputFilenModal } =
    useContext(GlobalContext);

  const changeHandler = (event) => {
    // Passing file data (event.target.files[0]) to parse using Papa.parse
    // https://medium.com/how-to-react/how-to-parse-or-read-csv-files-in-reactjs-81e8ee4870b0
    Papa.parse(event.target.files[0], {
      header: true,
      skipEmptyLines: true,
      transformHeader(h) {
        return h.replace(/\s/g, '');
      },
      complete(results) {
        callBack(results.data);
      },
    });
  };

  const handleClosePurchaseOrderItemAdjustModal = () => {
    onCloseInputFilenModal();
  };

  if (!isFileInputModalOpen) return null;

  return (
    <Modal
      open={isFileInputModalOpen}
      onClose={() => handleClosePurchaseOrderItemAdjustModal()}
    >
      <ModalBox>
        <ModalContent>
          <Box>
            <Button
              sx={{ width: '100%' }}
              variant="contained"
              size="large"
              component="label"
              fileInput
            >
              {' '}
              <AddIcon /> Upload a file
              <input
                type="file"
                name="file"
                accept=".csv"
                onChange={changeHandler}
                hidden
              />
            </Button>
          </Box>
        </ModalContent>
      </ModalBox>
    </Modal>
  );
});
