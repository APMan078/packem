import React, { ReactNode } from 'react';

import { Box, Typography } from '@mui/material';
import { SxProps, Theme } from '@mui/system';

import { WMSCard } from '../styles';

interface CardWithHeaderProps {
  header: string;
  subHeader?: string;
  footer?: string;
  children: ReactNode;
  sx?: SxProps<Theme>;
}

function CardWithHeader({
  children,
  sx,
  header,
  subHeader,
  footer,
}: CardWithHeaderProps) {
  return (
    <WMSCard sx={sx}>
      <Typography sx={{ paddingY: '12px' }} variant="h6" fontWeight="bold">
        {header}
      </Typography>
      {subHeader && (
        <Typography
          sx={{
            paddingY: '12px',
            pr: '50px',
            fontSize: '18px',
            letterSpacing: '0.14px',
            lineHeight: '24px',
          }}
          variant="subtitle1"
          fontWeight="medium"
        >
          {subHeader}
        </Typography>
      )}
      {children}
      {footer && (
        <Box
          sx={{
            display: 'flex',
            width: '100%',
            borderTop: 1,
            borderColor: 'grey.300',
          }}
        >
          <Typography
            sx={{
              paddingY: '18px',
              pr: '50px',
              fontSize: '18px',
              letterSpacing: '0.14px',
              lineHeight: '24px',
            }}
            variant="subtitle1"
            fontWeight="medium"
          >
            {footer}
          </Typography>
        </Box>
      )}
    </WMSCard>
  );
}

export default React.memo(CardWithHeader);
