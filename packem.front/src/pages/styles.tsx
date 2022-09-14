import PackemTextLogo from 'assets/icons/PackemTextLogo.svg';
import PackemWhite from 'assets/icons/PackemWhite.svg';
import LoginBackgroundImage from 'assets/images/LoginBackground.png';

import { Box, Card } from '@mui/material';
import { styled as MuiStyled } from '@mui/material/styles';

// Styles page for the 'pages' region of the repo. Think of this as a global
// style sheet for every area of the site, but not specific to our set
// components.

// Login Region Start

export const LoginContainer = MuiStyled(Box)`
  && {
    display: flex;
    width: 100%;
    height: 100vh;

    ${(props) => props.theme.breakpoints.up('lg')} {
      flex-direction: row;
    }
    ${(props) => props.theme.breakpoints.down('lg')} {
      flex-direction: column;
      align-items: center;
    }
  }
`;

export const LoginBackground = MuiStyled(Box)`
  && {
    display: flex;
    position: relative;

    justify-content: center;
    align-items: center;

    background-image: url(${LoginBackgroundImage});
    background-position: center;
    background-size: cover;
    background-repeat: no-repeat;

    width: 100%;
    min-height: 100%;

    ${(props) => props.theme.breakpoints.up('lg')} {
    }
    ${(props) => props.theme.breakpoints.down('lg')} {
      display: none;
    }
  }
`;

export const PackemLogo = MuiStyled(Box)`
  && {
    z-index: 1;
    display: flex;
    position: absolute;
    bottom: 96px;
    left: 96px;

    background-image: url(${PackemWhite});
    background-position: center;
    background-size: cover;
    background-repeat: no-repeat;

    min-width: 224px;
    min-height: 56px;
  }
`;

export const PackTextLogo = MuiStyled(Box)`
  && {
    z-index: 1;
    display: flex;
    
    background-image: url(${PackemTextLogo});
    background-position: center;
    background-size: cover;
    background-repeat: no-repeat;

    min-width: 176px;
    min-height: 32px;

    margin-top: 120px;
    margin-bottom: 64px;
  }
`;

export const FormContainer = MuiStyled(Box)`
  && {
    display: flex;
    flex-direction: column;
    align-items: center;
    max-width: 456px;

    ${(props) => props.theme.breakpoints.up('lg')} {
      width: 100%;
    }
    ${(props) => props.theme.breakpoints.down('lg')} {
    }
  }
`;

export const LoginRedBar = MuiStyled(Box)`
  && {
    background: #DE4A50;
    width: 100%;
    min-height: 16px;

    ${(props) => props.theme.breakpoints.up('lg')} {
      display: none;
    }
    ${(props) => props.theme.breakpoints.down('lg')} {
      display: flex;
    }
  }
`;
// End Login Region

// Reset password region Begin

export const ResetPasswordTextLogo = MuiStyled(Box)`
&& {
    z-index: 1;
    display: flex;
    margin-top: 35px;
    background-image: url(${PackemTextLogo});
    background-position: center;
    background-size: cover;
    background-repeat: no-repeat;

    width: 187px;
    height: 34px;
}
`;

export const ResetPasswordContainer = MuiStyled(Box)`
&& {
    display: flex;
    position: relative;
    background: #EFF2F7;
    width: 100%;
    height: 100vh;
    flex-direction: column;
    align-items: center;
  }
`;

export const ResetPasswordRedBar = MuiStyled(Box)`
&& {
  display: flex;
  background: #DE4A50;
  width: 100%;
  min-height: 16px;
  }
`;

export const ResetPasswordHeader = MuiStyled(Box)`
  && {
    display: flex;
    position: relative;
    align-items: flex-start;
    justify-content: center;
    background: white;

    width: 100%;
    min-height: 304px;

    margin-bottom: 98px;
  }
`;

export const ResetPasswordForm = MuiStyled(Box)`
  && {
    position: absolute;
    display: flex;
    flex-direction: column;
    gap: 16px;
    top: 176px;

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

export const ResetPasswordFormContainer = MuiStyled(Box)`
&& {
    display: flex;
    flex-direction: column;
    border-radius: 4px;

    padding: 24px;
    gap: 16px;
  }
`;
// Reset password region end

// Main Pages Region Begin

export const MainContainer = MuiStyled(Box)`
&& {
  display: flex;
  flex-direction: column;
  height: 100vh;
}
`;

export const ContentContainer = MuiStyled(Box)`
  && {
    display: flex; 
    flex-direction: column;
    
    ${(props) => props.theme.breakpoints.up('lg')} {
      padding: 24px;
    }
    ${(props) => props.theme.breakpoints.down('lg')} {
      padding: 16px;
    }
    ${(props) => props.theme.breakpoints.down('md')} {
      padding: 8px;
    }
  }
`;
