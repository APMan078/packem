import Box from '@mui/material/Box';
import { styled } from '@mui/material/styles';

export const MainContainer = styled(Box)`
  && {
    display: flex;
    width: 100%;

    ${(props) => props.theme.breakpoints.up('lg')} {
      flex-direction: row;
    }
    ${(props) => props.theme.breakpoints.down('lg')} {
      flex-direction: column;
    }
  }
`;

export const Content = styled(Box)`
  && {
    width: 100%;
    display: flex;
    flex-direction: column;
  }
`;
