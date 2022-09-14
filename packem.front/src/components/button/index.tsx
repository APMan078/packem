import React, { ReactNode } from 'react';

import GridMenu from 'components/button/gridbutton/gridmenu';

import { Button as MUIButton } from '@mui/material';
import { SxProps, Theme } from '@mui/system';

import { WMSButton } from '../styles';

interface ButtonProps {
  variant: 'text' | 'outlined' | 'contained';
  size: 'small' | 'medium' | 'large';
  component?: any;
  fileInput?: boolean;
  children: ReactNode;
  onClick?: any;
  onMouseDown?: any;
  sx?: SxProps<Theme>;
}

function Button({
  variant,
  size,
  component,
  fileInput,
  children,
  onClick,
  onMouseDown,
  sx,
  ...props
}: ButtonProps) {
  if (fileInput) {
    return (
      <MUIButton
        size={size}
        component={component}
        variant={variant}
        sx={sx}
        onClick={onClick}
        onMouseDown={onMouseDown}
        {...props}
      >
        {children}
      </MUIButton>
    );
  }
  return (
    <WMSButton
      size={size}
      variant={variant}
      sx={sx}
      onClick={onClick}
      onMouseDown={onMouseDown}
      {...props}
    >
      {children}
    </WMSButton>
  );
}

export default React.memo(Button);
