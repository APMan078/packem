import React, { useEffect, useState, useContext, ChangeEvent } from 'react';
import { useNavigate } from 'react-router-dom';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid } from 'components/styles';
import moment from 'moment';
import DeviceModal from 'pages/shared/deviceregistermodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import { getCustomerDevicesByCustomerId } from 'services/api/devices/devices.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Search } from '@mui/icons-material';
import { Typography, Box } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import {
  GridColDef,
  GridToolbar,
  GridColumns,
  GridActionsCellItem,
} from '@mui/x-data-grid';

function DeviceManagement() {
  const theme = useTheme();
  const navigate = useNavigate();
  const { loading, updateLoading, updateData, onOpenDeviceModal } =
    useContext(GlobalContext);
  const [currentView, setCurrentView] = useState(0);
  const [devices, setDevices] = useState([]);
  const [editDevice, setEditDevice] = useState(false);
  const [filteredDevices, setFilteredDevices] = useState([]);
  const { currentUser } = useContext(AuthContext);
  const [dataSelected, setDataSelected] = useState({
    customerId: currentUser.Claim_CustomerId,
  });
  const [searchParams, setSearchParams] = useState({
    searchDeviceId: '',
    searchCustomerLocationId: '',
    searchSerialNo: '',
    searchDateAdded: '',
    searchStatus: '',
  });

  const filter = () => {
    const customerDeviceResult = devices.filter((d) =>
      d.customerDeviceId.toString().includes(searchParams.searchDeviceId),
    );
    const customerLocResult = customerDeviceResult.filter((d) =>
      d.customerLocationId
        .toString()
        .includes(searchParams.searchCustomerLocationId),
    );
    const dateResult = customerLocResult.filter((d) =>
      moment(d.addedDateTime)
        .format('MM/DD/YY')
        .includes(searchParams.searchDateAdded),
    );
    const serialNoResult = dateResult.filter((d) =>
      d.serialNumber.toString().includes(searchParams.searchSerialNo),
    );
    const result = serialNoResult.filter((d) =>
      d.isActive.toString().includes(searchParams.searchStatus),
    );

    setFilteredDevices(result);
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

  const onForm = (key, text) => {
    setSearchParams(() => ({
      ...searchParams,
      [key]: text,
    }));
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) => {
    onForm(key, event.target.value);
  };

  const onLoadDeviceAndUserData = async () => {
    try {
      const customerDevicesFromApi = await getCustomerDevicesByCustomerId(
        currentUser.Claim_CustomerId,
      );
      setDevices(customerDevicesFromApi);
      setFilteredDevices(customerDevicesFromApi);

      return customerDevicesFromApi;
    } catch (error) {
      return error;
    }
  };

  useEffect(() => {
    updateLoading(true);
    setDevices([]);
    setFilteredDevices([]);
    onLoadDeviceAndUserData();
  }, [loading, updateData]);

  type DeviceRows = typeof devices[number];

  const handleEditDeviceClick = (rowData) => {
    setDataSelected(rowData);
    setEditDevice(true);
    onOpenDeviceModal();
  };

  const handleDeactiveReactiveDevice = (rowData) => {
    setDataSelected(rowData);
    return true;
  };

  const handleResetDataSelected = () => {
    setDataSelected({
      customerId: currentUser.Claim_CustomerId,
    });
    setEditDevice(false);
  };

  const deviceColumns: GridColumns<DeviceRows> = [
    {
      field: 'actions',
      type: 'actions',
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="Edit Device"
          onClick={() => handleEditDeviceClick(params.row)}
          showInMenu
        />,
      ],
    },
    {
      field: 'deviceId',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Device Id</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchDeviceId}
            rightIcon={<Search />}
            onChange={(value) => inputHandler('searchDeviceId', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filter();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography
          sx={{ cursor: 'pointer', color: [theme.palette.primary.main] }}
          onClick={() =>
            navigate(`/devices/device/${params.row.customerDeviceId}`)
          }
        >
          {params.row.customerDeviceId}
        </Typography>
      ),
    },
    {
      field: 'customerLocationId',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Location Id</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchCustomerLocationId}
            rightIcon={<Search />}
            onChange={(value) =>
              inputHandler('searchCustomerLocationId', value)
            }
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filter();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{params.row.customerLocationId}</Typography>
      ),
    },
    {
      field: 'serialNumber',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Serial No.</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchSerialNo}
            rightIcon={<Search />}
            onChange={(value) => inputHandler('searchSerialNo', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filter();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{params.row.serialNumber}</Typography>
      ),
    },
    {
      field: 'addedDateTime',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Date Registered</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchDateAdded}
            rightIcon={<Search />}
            onChange={(value) => inputHandler('searchDateAdded', value)}
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
          {moment(params.row.addedDateTime).format('MM/DD/YY')}
        </Typography>
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
            rightIcon={<Search />}
            onChange={(value) => inputHandler('searchStatus', value)}
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
      {editDevice ? (
        <DeviceModal
          edit
          customer={dataSelected}
          callBack={handleResetDataSelected}
        />
      ) : (
        <DeviceModal customer={dataSelected} />
      )}
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
              {devices.length}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Active Devices
            </Typography>
          </Card>
        </Box>
        <Card>
          {currentView === 0 && (
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredDevices}
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              columns={deviceColumns}
              pageSize={15}
              density="compact"
              rowsPerPageOptions={[15]}
              getRowId={(row) => row.customerDeviceId}
              checkboxSelection
              onSelectionModelChange={(customerId) => {
                const selectedId = customerId[0];
                const selectedRowData = devices.filter(
                  (c) => c.customerId === selectedId,
                );
                setDataSelected(selectedRowData[0]);
              }}
              components={{ Toolbar: CustomGridToolbar }}
              componentsProps={{
                toolbar: {
                  title: 'Devices',
                },
              }}
            />
          )}
        </Card>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(DeviceManagement);
