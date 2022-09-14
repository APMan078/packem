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
};

function Input({
  placeholder = 'Placeholder',
  value,
  error,
  size,
  type,
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
  if (error !== undefined && error !== '') {
    return (
      <InputContainer>
        {leftIcon}
        <InputField
          sx={sx}
          error
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
            maxLength: 150,
            endAdornment: rightIcon ? (
              <InputAdornment position="end">{rightIcon}</InputAdornment>
            ) : null,
          }}
        />
      </InputContainer>
    );
  }
  return (
    <InputContainer>
      {leftIcon}
      <InputField
        sx={sx}
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
        {...props}
        InputProps={{
          ...InputProps,
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
