import React from 'react';

import Button from 'components/button';
import Card from 'components/card';
import CardWithHeader from 'components/card/CardWithHeader';
import Input from 'components/input';

import { Box, Typography } from '@mui/material';

function AccountInformation() {
  return (
    <>
      {/* Account Information */}
      <CardWithHeader
        sx={{ display: 'flex', flexDirection: 'column' }}
        header="Point of Contact"
        subHeader="This will serve as your account’s primary..."
        footer="Need to replace the Administrator account? To change the primary admin, please contact customer support at (800) 123 1234."
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'start',
            alignItems: 'center',
            width: { sm: '100%', md: '50%' },
            paddingY: '18px',
            gap: '18px',
          }}
        >
          <Input sx={{ width: '100%' }} placeholder="Full Name" size="large" />
          <Input sx={{ width: '100%' }} placeholder="Email" size="large" />
        </Box>
      </CardWithHeader>
      <CardWithHeader
        sx={{ display: 'flex', flexDirection: 'column' }}
        header="Access"
        subHeader="Select which modules Operations Managers have access to."
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'start',
            alignItems: 'center',
            width: { sm: '100%', md: '100%' },
            paddingY: '18px',
            gap: '18px',
          }}
        >
          <Input sx={{ width: '100%' }} placeholder="UoM" size="large" />

          <Box sx={{ display: 'flex', width: '100%' }}>
            <Typography
              sx={{
                paddingY: '12px',
                pr: '50px',
                fontSize: '18px',
                letterSpacing: '0.14px',
                lineHeight: '24px',
              }}
              variant="subtitle1"
              fontWeight="bold"
            >
              My units of measurement
            </Typography>
          </Box>
        </Box>
      </CardWithHeader>
    </>
  );
}

export default React.memo(AccountInformation);
