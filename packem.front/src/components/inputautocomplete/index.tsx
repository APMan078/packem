import React, { ReactNode, useState } from 'react';

import Input from 'components/input';

import { Box, Autocomplete, TextField } from '@mui/material';
import { SxProps, Theme } from '@mui/system';

interface AutocompleteProps {
  value?: any;
  onChange?: any;
  inputValue?: any;
  onInputChange?: any;
  options?: any;
  sx?: SxProps<Theme>;
  placeholder?: string;
  getOptionLabel?: any;
}

function InputAutocomplete({
  value,
  onChange,
  inputValue,
  onInputChange,
  options,
  getOptionLabel,
  sx,
  placeholder = 'Placeholder',
}: AutocompleteProps) {
  return (
    <Box>
      <Autocomplete
        size="small"
        sx={sx}
        freeSolo
        value={value}
        onChange={onChange}
        inputValue={inputValue}
        onInputChange={onInputChange}
        options={options}
        getOptionLabel={getOptionLabel}
        renderInput={(params) => <TextField {...params} label={placeholder} />}
      />
    </Box>
  );
}

export default React.memo(InputAutocomplete);
