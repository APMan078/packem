import PackemLogo from 'assets/icons/PackemLogo.svg';
import styled from 'styled-components';

import MenuIcon from '@mui/icons-material/Menu';
import {
  Box,
  Card,
  Button,
  TextField,
  Menu,
  Modal,
  Select,
} from '@mui/material';
import { styled as MuiStyled } from '@mui/material/styles';
import { DataGrid } from '@mui/x-data-grid';

export const HeaderBox = MuiStyled(Box)`
  && {
    display: flex;
    flex-direction: row;
    max-height: 48px;
    align-items: center;

    box-shadow: 0px 3px 6px #0000000d;
    padding: 0px 24px 0px 24px;

    ${(props) => props.theme.breakpoints.down('lg')} {
      padding: 0px 16px 0px 16px;
    }
    ${(props) => props.theme.breakpoints.down('md')} {
      padding: 0px 8px 0px 8px;
    }
    background: ${(props) =>
      props.theme.palette.mode === 'dark'
        ? props.theme.palette.background.default
        : 'white'};
  }
`;

export const LowHeaderBox = MuiStyled(Box)`
  && {
    display: flex;
    min-height: 64px;
    align-items: center;
    box-shadow: 0px 3px 6px #0000000d;
    padding: 0px 24px 0px 24px;

    ${(props) => props.theme.breakpoints.up('lg')} {
      flex-direction: row;
    }
    ${(props) => props.theme.breakpoints.down('sm')} {
      flex-direction: column;
      padding: 0px 16px 0px 16px;
    }
    background: ${(props) =>
      props.theme.palette.mode === 'dark'
        ? props.theme.palette.background.default
        : 'white'};
  }
`;

export const WMSCard = MuiStyled(Card)`
  && {
    display: flex;
    min-height: ${(props) => (props.style ? props.style.minHeight : '80px')};
    padding: 24px;

    background: ${(props) => props.theme.palette.mode === 'light' && '#ffffff'};

    box-shadow: 0px 3px 6px #0000000d;
    border-radius: 4px;
  }
`;

export const WMSButton = MuiStyled(Button)`
  && {
    border-color: ${(props) =>
      props.variant === 'outlined'
        ? props.theme.palette.primary.main
        : 'primary'};
    color: ${(props) => {
      switch (props.variant) {
        case 'contained':
          return 'white';
        case 'outlined':
          return props.theme.palette.primary.main;
        case 'text':
          return props.theme.palette.primary.main;
        default:
          return props.theme.palette.primary.main;
      }
    }};
    background: ${(props) => {
      switch (props.variant) {
        case 'contained':
          return props.theme.palette.secondary.main;
        case 'outlined':
          return props.theme.palette.mode === 'dark'
            ? props.theme.palette.background.default
            : '#ffffff';
        case 'text':
          return props.theme.palette.mode === 'dark'
            ? props.theme.palette.background.default
            : '#ffffff';
        default:
          return 'white';
      }
    }};
    &:hover {
      background-color: ${(props) => {
        switch (props.variant) {
          case 'contained':
            return props.theme.palette.secondary.light;
          case 'outlined':
            return props.theme.palette.primary.light;
          case 'text':
            return props.theme.palette.primary.light;
          default:
            return props.theme.palette.primary.light;
        }
      }};
    }
    padding: 16px;
    text-transform: uppercase;
    box-shadow: ${(props) =>
      props.variant === 'text'
        ? '0px 0px 0px #00000000'
        : '0px 5px 10px #00000029'};
    border-radius: 4px;
    opacity: 1;
  }
`;

export const Grid = MuiStyled(DataGrid)`
  && {
    display: flex;
    border: none;
    .MuiDataGrid-columnSeparator {
      display: none;
    }
    .MuiDataGrid-columnHeaderTitleContainer {
      align-items: flex-end;
    }
    .MuiDataGrid-iconButtonContainer {
      margin: 0px 0px 5px 5px;
    }
    background: ${(props) => props.theme.palette.mode === 'light' && '#ffffff'};
  }
`;

export const InputContainer = styled(Box)`
  && {
    width: 100%;
    display: flex;
    align-items: flex-end;
  }
`;

export const InputField = styled(TextField)`
  && {
    opacity: 1;
    .MuiOutlinedInput-root {
      border-radius: 4px;
    }
  }
`;

export const SelectInput = styled(Select)`
  && {
    .MuiOutlinedInput-notchedOutline {
      border-radius: 4px;
    }
  }
`;

export const ErrorLabel = styled.p`
  && {
    color: red;
    padding-left: 3px;
    font-size: 10px;
  }
`;

export const PrintDivider = styled.hr`
  && {
    display: block;
    height: 1px;
    background: transparent;
    width: 100%;
    border: none;
    border-top: solid 1px #aaa;
  }
`;

export const DashboardDivider = styled.hr`
  && {
    display: block;
    height: 0.5px;
    background: transparent;
    width: 100%;
    border: none;
    border-top: solid 0.5px lightgray;
  }
`;

export const NavBarLogo = styled(Box)`
  && {
    display: flex;
    width: 35px;
    height: 40px;

    margin: 16px 0px 48px 15px;

    background-image: url(${PackemLogo});
    background-position: center;
    background-size: cover;
    background-repeat: no-repeat;
  }
`;

export const WMSRedContainer = MuiStyled(Box)`
  && {
    padding: 4px 12px 4px 12px;
    border-radius: 6px;
    background: ${(props) => props.theme.palette.secondary.light};
`;

export const MenuButton = MuiStyled(MenuIcon)`
  && {
    ${(props) => props.theme.breakpoints.up('lg')} {
      display: none;
    }
    ${(props) => props.theme.breakpoints.down('lg')} {
      display: block;
    }
  }
`;

export const MenuBase = MuiStyled(Menu)`
    && {
      margin-top: 32px;
      .MuiList-root {
        width: 100vw;
        display: flex;
        border-radius: 0;
        flex-direction: column;
        align-items: center;
        overflow: hidden;
        box-shadow: none;
        background: ${(props) =>
          props.theme.palette.mode === 'dark'
            ? props.theme.palette.background.default
            : 'white'};
      }
    }
`;

export const TopHeader = MuiStyled(Box)`
  && {
    display: flex;
    flex-direction: row;
    align-items: center;
    width: 100%;
    padding: 16px 0px 16px 0px;

    ${(props) => props.theme.breakpoints.up('lg')} {
      justify-content: space-between;
    }
    ${(props) => props.theme.breakpoints.down('lg')} {
      justify-content: space-between;
    }
  }
`;
// Main Pages Region End.

// Begin Modal components
export const ModalBox = MuiStyled(Box)`
  && {
    position: absolute;
    display: flex;
    flex-direction: column;
    gap: 16px;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);

    background: ${(props) =>
      props.theme.palette.mode === 'dark'
        ? props.theme.palette.background.default
        : 'white'};
    box-shadow: 0px 24px 38px #00000024;

    width: 100%;
    max-width: 376px;
    border-radius: 4px;
  }
`;

export const ModalContent = styled(Box)`
  && {
    display: flex;
    flex-direction: column;
    border-radius: 4px;

    padding: 24px;
    gap: 16px;
  }
`;

export const PrintContainer = styled(Box)`
  && {
    @media all {
      .page-break {
        display: none;
      }
    }

    @media print {
      html,
      body {
        height: initial !important;
        overflow: initial !important;
        -webkit-print-color-adjust: exact;
      }
    }

    @media print {
      .page-break {
        margin-top: 1rem;
        display: block;
        page-break-after: always;
      }
    }

    @page {
      size: auto;
    }
  }
`;
