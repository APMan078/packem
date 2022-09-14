import React from 'react';

import MoreVertIcon from '@mui/icons-material/MoreVert';
import { IconButton, Box } from '@mui/material';

interface GridMenuButtonProps {
  ref?: React.Ref<HTMLButtonElement>;
  onClick?: React.MouseEventHandler<HTMLButtonElement>;
}
function GridMenuButton({ ref, onClick }: GridMenuButtonProps) {
  return (
    <Box>
      <IconButton ref={ref} onClick={onClick}>
        <MoreVertIcon />
      </IconButton>
    </Box>
  );
}

export default React.memo(GridMenuButton);
