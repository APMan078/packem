import React, { useEffect, useState, useContext, ChangeEvent } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid } from 'components/styles';
import moment from 'moment';
import DeviceTokenCreationModal from 'pages/shared/devicetokencreationmodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import { getCustomerDeviceTokensById } from 'services/api/devices/devices.api';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Search } from '@mui/icons-material';
import { Typography, Box } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { GridColDef, GridToolbar } from '@mui/x-data-grid';

function DeviceView() {
  const theme = useTheme();
  const { loading, updateLoading, updateData } = useContext(GlobalContext);
  const [currentView, setCurrentView] = useState(0);
  const [deviceTokens, setDeviceTokens] = useState([]);
  const [filteredTokens, setFilteredTokens] = useState([]);
  const navigate = useNavigate();
  const location = useLocation();
  const getDeviceId = location.pathname.split('/')[3];
  const [dataSelected, setDataSelected] = useState({
    customerDeviceId: '',
  });
  const [searchParams, setSearchParams] = useState({
    searchDeviceTokenId: '',
    searchCustomerDeviceId: '',
    searchDeviceToken: '',
    searchDateAdded: '',
    searchIsValidated: '',
    searchStatus: '',
  });

  const onForm = (key, text) => {
    setSearchParams(() => ({
      ...searchParams,
      [key]: text,
    }));
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) => {
    onForm(key, event.target.value);
  };

  function getStatus(params) {
    let status = '';
    if (!params.row.isActive) {
      status = 'Inactive';
    } else {
      status = 'Active';
    }
    return status;
  }

  function getValidationStatus(params) {
    let status = '';
    if (!params.row.isValidated) {
      status = 'False';
    } else {
      status = 'True';
    }
    return status;
  }

  const onLoadDeviceTokens = async () => {
    try {
      const deviceTokensFromApi = await getCustomerDeviceTokensById(
        getDeviceId,
      );
      setDeviceTokens(deviceTokensFromApi);
      setFilteredTokens(deviceTokensFromApi);

      return deviceTokensFromApi;
    } catch (error) {
      return error;
    }
  };

  useEffect(() => {
    updateLoading(true);
    setDeviceTokens([]);
    setFilteredTokens([]);
    onLoadDeviceTokens();
  }, [loading, updateData]);

  const filter = () => {
    const customerDeviceTokenResult = deviceTokens.filter((d) =>
      d.customerDeviceTokenId
        .toString()
        .includes(searchParams.searchDeviceTokenId),
    );
    const customerDeviceIdResult = customerDeviceTokenResult.filter((d) =>
      d.customerDeviceId
        .toString()
        .includes(searchParams.searchCustomerDeviceId),
    );
    const tokenResult = customerDeviceIdResult.filter((d) =>
      d.deviceToken.includes(searchParams.searchDeviceToken),
    );
    const dateResult = tokenResult.filter((d) =>
      moment(d.dateAdded)
        .format('MM/DD/YY')
        .includes(searchParams.searchDateAdded),
    );
    const dateResults = dateResult.filter((d) =>
      d.isValidated.toString().includes(searchParams.searchIsValidated),
    );
    const result = dateResults.filter((d) =>
      d.isActive.toString().includes(searchParams.searchStatus),
    );

    setFilteredTokens(result);
  };

  const deviceTokenColumns: GridColDef[] = [
    {
      field: 'customerDeviceTokenId',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Token Id</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchDeviceTokenId}
            onChange={(value) => inputHandler('searchDeviceTokenId', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filter();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.customerDeviceTokenId}
        </Typography>
      ),
    },
    {
      field: 'customerDeviceId',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Device Id</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchCustomerDeviceId}
            onChange={(value) => inputHandler('searchCustomerDeviceId', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filter();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{params.row.customerDeviceId}</Typography>
      ),
    },
    {
      field: 'deviceToken',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Token</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchDeviceToken}
            onChange={(value) => inputHandler('searchDeviceToken', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filter();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.deviceToken}</Typography>,
    },
    {
      field: 'addedDateTime',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Date Created</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchDateAdded}
            onChange={(value) => inputHandler('searchDateAdded', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filter();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>
          {moment(params.row.dateAdded).format('MM/DD/YY')}
        </Typography>
      ),
    },
    {
      field: 'isValidated',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Validated</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchIsValidated}
            onChange={(value) => inputHandler('searchIsValidated', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filter();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{getValidationStatus(params)}</Typography>
      ),
    },
    {
      field: 'isActive',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Status</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchStatus}
            onChange={(value) => inputHandler('searchStatus', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filter();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{getStatus(params)}</Typography>,
    },
  ];

  return (
    <MainContainer>
      <Header />
      <DeviceTokenCreationModal device={getDeviceId} />
      <ContentContainer>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            gap: '16px',
            marginBottom: '16px',
          }}
        >
          <Card
            sx={{ display: 'flex', flexDirection: 'column', minWidth: '192px' }}
          >
            <Typography
              sx={{ color: [theme.palette.primary.main], cursor: 'pointer' }}
              variant="h3"
              onClick={() => setCurrentView(0)}
            >
              {deviceTokens.length}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Device Tokens
            </Typography>
          </Card>
        </Box>
        {currentView === 0 && (
          <Card>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredTokens}
              columns={deviceTokenColumns}
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              pageSize={15}
              density="compact"
              rowsPerPageOptions={[15]}
              getRowId={(row) => row.customerDeviceTokenId}
              checkboxSelection
              components={{ Toolbar: CustomGridToolbar }}
              componentsProps={{
                toolbar: {
                  title: 'Device Details',
                },
              }}
            />
          </Card>
        )}
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(DeviceView);
