import React, { ReactNode } from 'react';

import { SxProps, Theme } from '@mui/system';

import { WMSCard } from '../styles';

interface CardProps {
  children: ReactNode;
  sx?: SxProps<Theme>;
}

function Card({ children, sx }: CardProps) {
  return <WMSCard sx={sx}>{children}</WMSCard>;
}

export default React.memo(Card);
