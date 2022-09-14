import React, { ReactNode } from 'react';

import InputAdornment from '@mui/material/InputAdornment';
import { SxProps, Theme } from '@mui/system';

import { InputContainer, InputField } from '../styles';

type InputProps = {
  placeholder?: string;
  value?: any;
  error?: any;
  size?: any;
  type?: string;
  variant?: 'outlined' | 'standard';
  disabled?: boolean;
  leftIcon?: ReactNode;
  rightIcon?: ReactNode;
  InputProps?: any;
  sx?: SxProps<Theme>;
  onChange?: any;
  onKeyDown?: any;
  min?: number;
};

function Input({
  placeholder = 'Placeholder',
  value,
  error,
  size,
  type,
  min,
  variant,
  disabled,
  leftIcon,
  rightIcon,
  InputProps,
  onChange,
  onKeyDown,
  sx,
  ...props
}: InputProps) {
  return (
    <InputContainer>
      {leftIcon}
      <InputField
        sx={sx}
        error={error !== undefined && error !== ''}
        label={placeholder}
        type={type}
        size={size || 'small'}
        variant={variant}
        disabled={disabled}
        value={value}
        onChange={onChange}
        onKeyDown={onKeyDown}
        onClick={(e) => {
          e.stopPropagation();
        }}
        helperText={error}
        {...props}
        inputProps={{
          ...InputProps,
          min,
          maxLength: 150,
          endAdornment: rightIcon ? (
            <InputAdornment position="end">{rightIcon}</InputAdornment>
          ) : null,
        }}
      />
    </InputContainer>
  );
}

export default React.memo(Input);
