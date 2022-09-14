import React, {
  useEffect,
  useState,
  useContext,
  useRef,
  ChangeEvent,
} from 'react';
import { useLocation } from 'react-router-dom';

import GridButton from 'components/button/gridbutton';
import GridMenu from 'components/button/gridbutton/gridmenu';
import Card from 'components/card';
import Header from 'components/header';
import Input from 'components/input';
import { Grid } from 'components/styles';
import TabPanel, { a11yProps } from 'components/tabpanel';
import UserRoles from 'helpers/userrolehelper';
import ConfirmationDialog from 'pages/shared/confirmdeletemodal';
import CustomerFacilityModal from 'pages/shared/customerfacilitymodal';
import CustomerLocationModal from 'pages/shared/customerlocationmodal';
import UserCreationModal from 'pages/shared/customerusermodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import { getCustomerById } from 'services/api/customer/customer.api';
import { getCustomerFacilitiesByCustomerId } from 'services/api/customerfacilities/customerfacilities.api';
import { getCustomerLocationsById } from 'services/api/customerlocations/customerlocations.api';
import { getUsersByCustomerId } from 'services/api/user/users.api';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Typography, Tabs, Tab } from '@mui/material';
import Box from '@mui/material/Box';
import { useTheme } from '@mui/material/styles';
import {
  GridToolbar,
  GridColumns,
  GridActionsCellItem,
} from '@mui/x-data-grid';

function ClientView() {
  const theme = useTheme();
  const location = useLocation();
  const [currentView, setCurrentView] = useState(0);
  const {
    loading,
    updateLoading,
    updateData,
    onOpenFacilityModal,
    onOpenUserModal,
    onOpenCustomerLocationModal,
    onOpenConfirmDeleteDialog,
  } = useContext(GlobalContext);

  const gridMenu = useRef();
  const [anchorEl, setAnchorEl] = useState(null);
  const [openMenu, setOpenMenu] = useState(false);

  const [clientUsers, setClientUsers] = useState([]);
  const [filteredUsers, setFilteredUsers] = useState([]);
  const [editUser, setEditUser] = useState(false);
  const [userAction, setUserAction] = useState(false);

  const [clientFacilities, setClientFacilities] = useState([]);
  const [filteredFacilities, setFilteredFacilities] = useState([]);
  const [editFacility, setEditFacility] = useState(false);
  const [facilityAction, setFacilityAction] = useState(false);

  const [clientLocations, setClientLocations] = useState([]);
  const [filteredLocations, setFilteredLocations] = useState([]);
  const [editLocation, setEditLocation] = useState(false);
  const [locationAction, setLocationAction] = useState(false);

  const [client, setClient] = useState({
    customerId: '',
    name: '',
  });

  const [dataSelected, setDataSelected] = useState({
    userId: '',
    customerLocationId: '',
    customerId: '',
    name: '',
  });

  const initialSearchState = {
    searchName: '',
    searchUserId: '',
    searchUsername: '',
    searchEmail: '',
    searchRoleId: '',
    searchIsActive: '',
    searchCustomerLocationId: '',
    searchCustomerFacilityId: '',
  };

  const [searchParams, setSearchParams] = useState(initialSearchState);

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

  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setCurrentView(newValue);
    setSearchParams(initialSearchState);
  };

  const handleResetDataSelected = () => {
    setDataSelected({
      userId: '',
      customerLocationId: '',
      customerId: '',
      name: '',
    });
    setUserAction(false);
    setEditUser(false);
    setLocationAction(false);
    setFacilityAction(false);
    setEditLocation(false);
    setEditFacility(false);
  };

  const handleClickMenu = () => {
    if (!anchorEl) setAnchorEl(gridMenu.current);
    setOpenMenu(!openMenu);
  };

  const onMenuClose = () => {
    setAnchorEl(null);
    setOpenMenu(!openMenu);
  };

  const filterUsers = () => {
    const userIdSearchResults = clientUsers.filter((u) =>
      u.userId.toString().includes(searchParams.searchUserId),
    );
    const userNameSearch = userIdSearchResults.filter((u) =>
      u.name.toString().toLowerCase().includes(searchParams.searchName),
    );
    const usernameSearch = userNameSearch.filter((u) =>
      u.username.toString().toLowerCase().includes(searchParams.searchUsername),
    );
    const emailSearchResults = usernameSearch.filter((u) =>
      u.email.toString().toLowerCase().includes(searchParams.searchEmail),
    );
    const roleSearchResults = emailSearchResults.filter((u) =>
      UserRoles[u.roleId].toLowerCase().includes(searchParams.searchRoleId),
    );
    const finalResult = roleSearchResults.filter((u) =>
      u.isActive.toString().includes(searchParams.searchIsActive),
    );

    setFilteredUsers(finalResult);
  };

  const filterLocations = () => {
    // customerLocationId, Name,
    const searchCustomerLocationIdResult = clientLocations.filter((l) =>
      l.customerLocationId
        .toString()
        .includes(searchParams.searchCustomerLocationId),
    );
    const finalResult = searchCustomerLocationIdResult.filter((l) =>
      l.name.toLowerCase().includes(searchParams.searchName),
    );

    setFilteredLocations(finalResult);
  };

  const filterFacilities = () => {
    const searchCustomerFacilityIdResults = clientFacilities.filter((f) =>
      f.customerFacilityId
        .toString()
        .includes(searchParams.searchCustomerFacilityId),
    );
    const searchFacilityLocationIdResults =
      searchCustomerFacilityIdResults.filter((f) =>
        f.customerLocationId
          .toString()
          .includes(searchParams.searchCustomerLocationId),
      );
    const finalResult = searchFacilityLocationIdResults.filter((f) =>
      f.name.toLowerCase().includes(searchParams.searchName),
    );

    setFilteredFacilities(finalResult);
  };

  const onLoadClientData = async () => {
    try {
      const customerIdFromRoute = location.pathname.split('/')[3];
      const clientFromApi = await getCustomerById(customerIdFromRoute);
      const clientUsersFromApi = await getUsersByCustomerId(
        customerIdFromRoute,
      );
      const clientLocationsFromApi = await getCustomerLocationsById(
        customerIdFromRoute,
      );
      const clientFacilitiesFromApi = await getCustomerFacilitiesByCustomerId(
        customerIdFromRoute,
      );

      setClient(clientFromApi);
      setClientUsers(clientUsersFromApi);
      setFilteredUsers(clientUsersFromApi);
      setClientFacilities(clientFacilitiesFromApi);
      setFilteredFacilities(clientFacilitiesFromApi);
      setClientLocations(clientLocationsFromApi);
      setFilteredLocations(clientLocationsFromApi);

      return true;
    } catch (error) {
      return error;
    }
  };

  useEffect(() => {
    setClientUsers([]);
    setClientFacilities([]);
    setClientLocations([]);
    onLoadClientData();
  }, [updateData]);

  type UserRows = typeof clientUsers[number];

  const userColumns: GridColumns<UserRows> = [
    {
      field: 'userId',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>User Id</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchUserId}
            onChange={(value) => inputHandler('searchUserId', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.userId}</Typography>,
    },
    {
      field: 'name',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Name</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchName}
            onChange={(value) => inputHandler('searchName', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.name}</Typography>,
    },
    {
      field: 'username',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Username</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchUsername}
            onChange={(value) => inputHandler('searchUsername', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.username}</Typography>,
    },
    {
      field: 'email',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Email</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchEmail}
            onChange={(value) => inputHandler('searchEmail', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.email}</Typography>,
    },
    {
      field: 'roleId',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Role</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchRoleId}
            onChange={(value) => inputHandler('searchRoleId', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{UserRoles[params.row.roleId]}</Typography>
      ),
    },
    {
      field: 'isActive',
      sortable: true,
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Status</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchIsActive}
            onChange={(value) => inputHandler('searchIsActive', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{getStatus(params)}</Typography>,
    },
  ];

  type LocationRows = typeof clientLocations[number];

  const handleClickAddFacility = (rowData) => {
    setDataSelected(rowData);
    onOpenFacilityModal();
  };

  const handleAddUserClick = (rowData) => {
    setDataSelected(rowData);
    onOpenUserModal();
  };

  const handleClickEditLocation = (rowData) => {
    setEditLocation(true);
    setDataSelected(rowData);
    onOpenCustomerLocationModal();
  };

  const handleClickDeleteLocation = (rowData) => {
    setLocationAction(true);
    setDataSelected(rowData);
    onOpenConfirmDeleteDialog();
  };

  const customerLocationColumns: GridColumns<LocationRows> = [
    {
      field: 'actions',
      type: 'actions',
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="Add Facility"
          onClick={() => handleClickAddFacility(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Add User"
          onClick={() => handleAddUserClick(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Edit"
          onClick={() => handleClickEditLocation(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Delete"
          onClick={() => handleClickDeleteLocation(params.row)}
          showInMenu
        />,
      ],
    },
    {
      field: 'customerLocationId',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Location Id</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchCustomerLocationId}
            onChange={(value) =>
              inputHandler('searchCustomerLocationId', value)
            }
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterLocations();
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
      field: 'name',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Location Name</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchName}
            onChange={(value) => inputHandler('searchName', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterLocations();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.name}</Typography>,
    },
  ];

  type FacilityRows = typeof clientFacilities[number];

  const handleClickEditFacility = (rowData) => {
    setDataSelected(rowData);
    setEditFacility(true);
    onOpenFacilityModal();
  };

  const handleClickDeleteFacility = (rowData) => {
    setFacilityAction(true);
    setDataSelected(rowData);
    onOpenConfirmDeleteDialog();
  };

  const customerFacilityColumns: GridColumns<FacilityRows> = [
    {
      field: 'actions',
      type: 'actions',
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="Edit"
          onClick={() => handleClickEditFacility(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Delete"
          onClick={() => handleClickDeleteFacility(params.row)}
          showInMenu
        />,
      ],
    },
    {
      field: 'customerFacilityId',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Id</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchCustomerFacilityId}
            onChange={(value) =>
              inputHandler('searchCustomerFacilityId', value)
            }
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterFacilities();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{params.row.customerFacilityId}</Typography>
      ),
    },
    {
      field: 'customerLocationId',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Location Id</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchCustomerLocationId}
            onChange={(value) =>
              inputHandler('searchCustomerLocationId', value)
            }
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterFacilities();
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
      field: 'name',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Name</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchName}
            onChange={(value) => inputHandler('searchName', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterFacilities();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.name}</Typography>,
    },
  ];

  return (
    <MainContainer>
      {facilityAction && (
        <ConfirmationDialog
          deleteFacility
          facilityData={dataSelected}
          callBack={handleResetDataSelected}
        />
      )}
      {locationAction && (
        <ConfirmationDialog
          deleteLocation
          locationData={dataSelected}
          callBack={handleResetDataSelected}
        />
      )}
      {editLocation && (
        <CustomerLocationModal
          edit
          customer={dataSelected}
          customerManagement={false}
          callBack={handleResetDataSelected}
        />
      )}
      <UserCreationModal userData={dataSelected} superAdmin />
      {editFacility ? (
        <CustomerFacilityModal
          edit
          facilityData={dataSelected}
          callBack={handleResetDataSelected}
        />
      ) : (
        <CustomerFacilityModal
          facilityData={dataSelected}
          callBack={handleResetDataSelected}
        />
      )}
      <Header />
      <ContentContainer>
        {currentView === 0 && (
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'row',
              gap: '16px',
              marginBottom: '16px',
            }}
          >
            <Card
              sx={{
                display: 'flex',
                flexDirection: 'column',
                minWidth: '192px',
              }}
            >
              <Typography
                sx={{ color: [theme.palette.primary.main] }}
                variant="h3"
              >
                {clientUsers.length}
              </Typography>
              <Typography variant="caption" fontWeight="bold">
                Customer Users
              </Typography>
            </Card>

            <Card
              sx={{
                display: 'flex',
                flexDirection: 'column',
                minWidth: '192px',
              }}
            >
              <Typography
                sx={{ color: [theme.palette.primary.main] }}
                variant="h3"
              >
                {clientUsers.filter((u) => u.roleId === 2).length}
              </Typography>
              <Typography variant="caption" fontWeight="bold">
                Admins
              </Typography>
            </Card>
            <Card
              sx={{
                display: 'flex',
                flexDirection: 'column',
                minWidth: '192px',
              }}
            >
              <Typography
                sx={{ color: [theme.palette.primary.main] }}
                variant="h3"
              >
                {clientUsers.filter((u) => u.roleId === 3).length}
              </Typography>
              <Typography variant="caption" fontWeight="bold">
                Ops Manager
              </Typography>
            </Card>
            <Card
              sx={{
                display: 'flex',
                flexDirection: 'column',
                minWidth: '192px',
              }}
            >
              <Typography
                sx={{ color: [theme.palette.primary.main] }}
                variant="h3"
              >
                {clientUsers.filter((u) => u.roleId === 4).length}
              </Typography>
              <Typography variant="caption" fontWeight="bold">
                Operator
              </Typography>
            </Card>
          </Box>
        )}
        {currentView === 1 && (
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'row',
              gap: '16px',
              marginBottom: '16px',
            }}
          >
            <Card
              sx={{
                display: 'flex',
                flexDirection: 'column',
                minWidth: '192px',
              }}
            >
              <Typography
                sx={{ color: [theme.palette.primary.main] }}
                variant="h3"
              >
                {clientLocations.length}
              </Typography>
              <Typography variant="caption" fontWeight="bold">
                Customer Locations
              </Typography>
            </Card>
          </Box>
        )}
        {currentView === 2 && (
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'row',
              gap: '16px',
              marginBottom: '16px',
            }}
          >
            <Card
              sx={{
                display: 'flex',
                flexDirection: 'column',
                minWidth: '192px',
              }}
            >
              <Typography
                sx={{ color: [theme.palette.primary.main] }}
                variant="h3"
              >
                {clientFacilities.length}
              </Typography>
              <Typography variant="caption" fontWeight="bold">
                Customer Facilities
              </Typography>
            </Card>
          </Box>
        )}
        <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Tabs value={currentView} onChange={handleChange}>
            <Tab label="Users" {...a11yProps(0)} />
            <Tab label="Locations" {...a11yProps(1)} />
            <Tab label="Facilities" {...a11yProps(2)} />
          </Tabs>
          <TabPanel value={currentView} index={0}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredUsers}
              onSelectionModelChange={(id) => {
                const selectedId = new Set(id);
                const selectedRowData = clientUsers.filter((ca) =>
                  selectedId.has(ca.userId),
                );
                /* setDataSelected(selectedRowData); */
              }}
              density="compact"
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              columns={userColumns}
              pageSize={15}
              rowsPerPageOptions={[15]}
              getRowId={(row) => row.userId}
              checkboxSelection
              components={{ Toolbar: GridToolbar }}
              componentsProps={{
                toolbar: {
                  showQuickFilter: true,
                  quickFilterProps: { debounceMs: 500 },
                },
              }}
            />
          </TabPanel>
          <TabPanel value={currentView} index={1}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredLocations}
              onSelectionModelChange={(id) => {
                const selectedId = id[0];
                const selectedRowData = filteredLocations.filter(
                  (c) => c.customerLocationId === selectedId,
                );
                setDataSelected(selectedRowData[0]);
              }}
              density="compact"
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              columns={customerLocationColumns}
              pageSize={15}
              rowsPerPageOptions={[15]}
              getRowId={(row) => row.customerLocationId}
              checkboxSelection
              components={{ Toolbar: GridToolbar }}
              componentsProps={{
                toolbar: {
                  showQuickFilter: true,
                  quickFilterProps: { debounceMs: 500 },
                },
              }}
            />
          </TabPanel>
          <TabPanel value={currentView} index={2}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredFacilities}
              onSelectionModelChange={(facilityId) => {
                const selectedId = facilityId[0];
                const selectedRowData = clientLocations.filter(
                  (c) => c.customerLocationId === selectedId,
                );
                /* setDataSelected(selectedRowData[0]); */
              }}
              density="compact"
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              columns={customerFacilityColumns}
              pageSize={15}
              rowsPerPageOptions={[15]}
              getRowId={(row) => row.customerFacilityId}
              checkboxSelection
              components={{ Toolbar: GridToolbar }}
              componentsProps={{
                toolbar: {
                  showQuickFilter: true,
                  quickFilterProps: { debounceMs: 500 },
                },
              }}
            />
          </TabPanel>
        </Card>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(ClientView);
