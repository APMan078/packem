import React from 'react';

import { Search } from '@mui/icons-material';
import { Typography, Box, InputAdornment } from '@mui/material';
import { GridToolbarExport, GridToolbarQuickFilter } from '@mui/x-data-grid';

interface CustomGridToolbarProps {
  title?: string;
}

function CustomGridToolbar({ title }: CustomGridToolbarProps) {
  return (
    <Box
      sx={{
        width: '100%',
        display: 'flex',
        flexDirection: 'row',
        justifyContent: 'flex-end',
        gap: '8px',
      }}
    >
      <Box
        sx={{
          width: '100%',
          display: 'flex',
          flexDirection: 'row',
          justifyContent: 'space-between',
          gap: '8px',
        }}
      >
        <Typography variant="h6" fontWeight="bold">
          {title}
        </Typography>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            justifyContent: 'center',
            gap: '8px',
          }}
        >
          <GridToolbarExport />
          <GridToolbarQuickFilter variant="outlined" size="small" />
        </Box>
      </Box>
    </Box>
  );
}

export default React.memo(CustomGridToolbar);
