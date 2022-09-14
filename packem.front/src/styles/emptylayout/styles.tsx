import { Box } from '@mui/material';
import { styled } from '@mui/material/styles';

export const MainContainer = styled(Box)`
  && {
    top: 0;
    padding: 0px;
    width: 100%;

    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
  }
`;

export const Container = styled(Box)`
  && {
    display: flex;
    padding: 0;

    width: 100%;

    ${(props) => props.theme.breakpoints.up('lg')} {
      height: 100vh;
      flex-direction: row;
    }
    ${(props) => props.theme.breakpoints.down('lg')} {
      height: 30vh;
      flex-direction: column;
    }
  }
`;
