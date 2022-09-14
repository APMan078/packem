import React, { ReactNode } from 'react';

import Stack from '@mui/material/Stack';
import { SxProps, Theme } from '@mui/system';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { DesktopDatePicker } from '@mui/x-date-pickers/DesktopDatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';

type DatePickerInputProps = {
  value?: any;
  label?: string;
  inputFormat?: string;
  sx?: SxProps<Theme>;
  onChange?: any;
  renderInput?: any;
};

function DatePickerInput({
  value,
  label,
  inputFormat,
  onChange,
  renderInput,
  sx,
  ...props
}: DatePickerInputProps) {
  return (
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      <Stack spacing={3}>
        <DesktopDatePicker
          label={label}
          inputFormat={inputFormat}
          value={value}
          onChange={onChange}
          renderInput={renderInput}
        />
      </Stack>
    </LocalizationProvider>
  );
}

export default React.memo(DatePickerInput);
