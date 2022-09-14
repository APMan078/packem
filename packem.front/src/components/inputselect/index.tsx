import React, { ReactNode } from 'react';

import {
  Box,
  FormControl,
  InputLabel,
  MenuItem,
  FormHelperText,
} from '@mui/material';
import { SxProps, Theme } from '@mui/system';

import { SelectInput } from '../styles';

type InputProps = {
  placeholder?: string;
  disabled?: boolean;
  variant?: 'outlined';
  label?: string;
  error?: any;
  value: any;
  onChange?: any;
  options?: any;
  leftIcon?: ReactNode;
  rightIcon?: ReactNode;
  sx?: SxProps<Theme>;
  size?: 'small' | 'medium';
};

function InputSelect({
  placeholder = 'Placeholder',
  variant = 'outlined',
  disabled,
  error,
  label,
  onChange,
  leftIcon,
  rightIcon,
  value,
  options,
  size = 'small',
  sx,
  ...props
}: InputProps) {
  return (
    <Box sx={sx}>
      {leftIcon}
      <FormControl fullWidth>
        <InputLabel
          sx={{
            color: error && '#d32f2f',
          }}
        >
          {label || placeholder}
        </InputLabel>
        <SelectInput
          disabled={disabled}
          error={error != null}
          label={placeholder}
          variant={variant}
          value={value}
          onChange={onChange}
          {...props}
          size={size}
        >
          {options &&
            options.map((option, index) => (
              <MenuItem
                // eslint-disable-next-line react/no-array-index-key
                key={`menu-item-${index}-${option.value}-${option.label}`}
                value={option.value}
              >
                {option.label}
              </MenuItem>
            ))}
        </SelectInput>
        {error && (
          <FormHelperText
            sx={{
              color: '#d32f2f',
            }}
          >
            {error}
          </FormHelperText>
        )}
      </FormControl>
      {rightIcon}
    </Box>
  );
}

export default React.memo(InputSelect);
