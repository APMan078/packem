import React, {
  useEffect,
  useState,
  useContext,
  useRef,
  ChangeEvent,
} from 'react';

import Button from 'components/button';
import Card from 'components/card';
import CardWithHeader from 'components/card/CardWithHeader';
import Header from 'components/header';
import Input from 'components/input';
import { Grid } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import UserRoles from 'helpers/userrolehelper';
import UserCreationModal from 'pages/shared/customerusermodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import {
  getUsersByCustomerId,
  changeUserActiveStatus,
} from 'services/api/user/users.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Typography, Box, ButtonBase } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import {
  GridToolbar,
  GridActionsCellItem,
  GridColumns,
} from '@mui/x-data-grid';

import AccountInformation from './tabviews/AccountInformation';
import CompanyProfile from './tabviews/CompanyProfile';
import InventorySettings from './tabviews/InventorySettings';

function Settings() {
  const [view, setView] = useState('companyProfile');
  return (
    <MainContainer>
      <Header />
      <ContentContainer>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            width: '100%',
            gap: '12px',
          }}
        >
          <Box sx={{ display: 'flex', flexDirection: 'column', width: '25%' }}>
            <Card sx={{ display: 'flex', flexDirection: 'column' }}>
              <Typography variant="subtitle1" fontWeight="light">
                JUMP TO
              </Typography>
              <ButtonBase
                onClick={(event) => {
                  setView('companyProfile');
                }}
              >
                <Box
                  sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    justifyContent: 'start',
                    alignItems: 'center',
                    width: '100%',
                    borderBottom: 1,
                    borderColor: 'grey.300',
                    paddingY: '18px',
                  }}
                >
                  <Typography variant="h6" fontWeight="bold">
                    Company Details
                  </Typography>
                </Box>
              </ButtonBase>
              <ButtonBase
                onClick={(event) => {
                  setView('inventorySettings');
                }}
              >
                <Box
                  sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    justifyContent: 'start',
                    alignItems: 'center',
                    width: '100%',
                    borderBottom: 1,
                    borderColor: 'grey.300',
                    paddingY: '18px',
                  }}
                >
                  <Typography variant="h6" fontWeight="bold">
                    Inventory Settings
                  </Typography>
                </Box>
              </ButtonBase>
              {/* <ButtonBase
                onClick={(event) => {
                  setView('accountInformation');
                }}
              >
                <Box
                  sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    justifyContent: 'start',
                    alignItems: 'center',
                    width: '100%',
                    borderBottom: 1,
                    borderColor: 'grey.300',
                    paddingY: '18px',
                  }}
                >
                  <Typography variant="h6" fontWeight="bold">
                    Account Information
                  </Typography>
                </Box>
              </ButtonBase>
              <ButtonBase
                onClick={(event) => {
                  setView('accountInformation');
                }}
              >
                <Box
                  sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    justifyContent: 'start',
                    alignItems: 'center',
                    width: '100%',
                    pt: '18px',
                  }}
                >
                  <Typography variant="h6" fontWeight="bold">
                    Notifications
                  </Typography>
                </Box>
              </ButtonBase> */}
            </Card>
          </Box>
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              width: '50%',
              gap: '12px',
            }}
          >
            {view === 'companyProfile' && <CompanyProfile />}
            {view === 'inventorySettings' && <InventorySettings />}
            {/* {view === 'accountInformation' && <AccountInformation />} */}
          </Box>
        </Box>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(Settings);
