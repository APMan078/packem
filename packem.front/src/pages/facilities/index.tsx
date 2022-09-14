import React, { useEffect, useState, useContext, ChangeEvent } from 'react';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid } from 'components/styles';
import TabPanel, { a11yProps } from 'components/tabpanel';
import ConfirmationDialog from 'pages/shared/confirmdeletemodal';
import CustomerFacilityModal from 'pages/shared/customerfacilitymodal';
import CustomerLocationModal from 'pages/shared/customerlocationmodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import {
  getCustomerFacilitiesByCustomerId,
  lookupCustomerZonesByFacilityId,
} from 'services/api/customerfacilities/customerfacilities.api';
import { getCustomerLocationsById } from 'services/api/customerlocations/customerlocations.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Search } from '@mui/icons-material';
import { Typography, Box, Tab, Tabs } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import {
  GridToolbar,
  GridColumns,
  GridActionsCellItem,
} from '@mui/x-data-grid';

function FacilityManagement() {
  const theme = useTheme();
  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  const {
    loading,
    updateLoading,
    updateData,
    onOpenConfirmDeleteDialog,
    onOpenCustomerLocationModal,
    onOpenFacilityModal,
    onOpenFacilityZoneModal,
  } = useContext(GlobalContext);
  const [facilities, setFacilities] = useState([]);
  const [locations, setLocations] = useState([]);
  const [filteredLocations, setFilteredLocations] = useState([]);
  const [filteredFacilities, setFilteredFacilities] = useState([]);
  const [locationAction, setLocationAction] = useState(false);
  const [facilityAction, setFacilityAction] = useState(false);
  const [editLocation, setEditLocation] = useState(false);
  const [editFacility, setEditFacility] = useState(false);
  const [dataSelected, setDataSelected] = useState<any>({
    customerId: '',
    customerFacilityId: '',
    customerLocationId: '',
    name: '',
  });
  const [currentView, setCurrentView] = useState(0);
  const [searchParams, setSearchParams] = useState({
    searchLocationId: '',
    searchFacilityId: '',
    searchName: '',
    zoneId: '',
  });

  const handleResetDataSelected = () => {
    setDataSelected({
      customerId: currentUser.Claim_CustomerId,
      customerLocationId: '',
      name: '',
    });
    setLocationAction(false);
    setFacilityAction(false);
    setEditLocation(false);
    setEditFacility(false);
  };

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
  };

  const filterLocations = () => {
    const customerLocationIdResult = locations.filter((f) =>
      f.customerLocationId.toString().includes(searchParams.searchLocationId),
    );
    const result = customerLocationIdResult.filter((f) =>
      f.name.toLowerCase().includes(searchParams.searchName.toLowerCase()),
    );

    setFilteredLocations(result);
  };

  const filterFacilities = () => {
    const customerFacilityIdResult = facilities.filter((f) =>
      f.customerFacilityId.toString().includes(searchParams.searchFacilityId),
    );
    const customerLocResult = customerFacilityIdResult.filter((f) =>
      f.customerLocationId.toString().includes(searchParams.searchLocationId),
    );
    const result = customerLocResult.filter((f) =>
      f.name.toLowerCase().includes(searchParams.searchName.toLowerCase()),
    );

    setFilteredFacilities(result);
  };

  const onLoadCustomerFacilitiesAndLocations = async () => {
    try {
      const facilitiesFromApi = await getCustomerFacilitiesByCustomerId(
        currentUser.Claim_CustomerId,
      );
      const locationsFromApi = await getCustomerLocationsById(
        currentUser.Claim_CustomerId,
      );

      setLocations(locationsFromApi);
      setFilteredLocations(locationsFromApi);
      setFacilities(facilitiesFromApi);
      setFilteredFacilities(facilitiesFromApi);

      return true;
    } catch (error) {
      return error;
    }
  };

  type FacilityRows = typeof facilities[number];

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

  const facilityColumns: GridColumns<FacilityRows> = [
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
          <Typography fontWeight="bold">Facility Id</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchFacilityId}
            onChange={(value) => inputHandler('searchFacilityId', value)}
            rightIcon={<Search />}
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
          <Typography fontWeight="bold">Location Id</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchLocationId}
            onChange={(value) => inputHandler('searchLocationId', value)}
            rightIcon={<Search />}
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
          <Typography fontWeight="bold">Facility Name</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchName}
            onChange={(value) => inputHandler('searchName', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterFacilities();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.name}
        </Typography>
      ),
    },
  ];

  type LocationRows = typeof locations[number];

  const handleClickAddFacility = (rowData) => {
    setDataSelected(rowData);
    onOpenFacilityModal();
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

  const locationColumns: GridColumns<LocationRows> = [
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
          <Typography fontWeight="bold">Location Id</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchLocationId}
            onChange={(value) => inputHandler('searchLocationId', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterLocations();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.customerLocationId}
        </Typography>
      ),
    },
    {
      field: 'name',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Location Name</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchName}
            onChange={(value) => inputHandler('searchName', value)}
            rightIcon={<Search />}
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

  useEffect(() => {
    updateLoading(true);
    setFacilities([]);
    setFilteredFacilities([]);
    setLocations([]);
    setFilteredLocations([]);
    onLoadCustomerFacilitiesAndLocations();
  }, [loading, updateData, currentLocationAndFacility]);

  return (
    <MainContainer>
      <Header />
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
      {editLocation ? (
        <CustomerLocationModal
          edit
          customer={dataSelected}
          customerManagement={false}
          callBack={handleResetDataSelected}
        />
      ) : (
        <CustomerLocationModal
          customer={dataSelected}
          customerManagement={false}
          callBack={handleResetDataSelected}
        />
      )}
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
              sx={{ color: [theme.palette.primary.main] }}
              variant="h3"
            >
              {locations.length}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Locations
            </Typography>
          </Card>
          <Card
            sx={{ display: 'flex', flexDirection: 'column', minWidth: '192px' }}
          >
            <Typography
              sx={{ color: [theme.palette.primary.main] }}
              variant="h3"
            >
              {facilities.length}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Facilities
            </Typography>
          </Card>
        </Box>
        <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Tabs value={currentView} onChange={handleChange}>
            <Tab label="Locations" {...a11yProps(0)} />
            <Tab label="Facilities" {...a11yProps(1)} />
          </Tabs>
          <TabPanel value={currentView} index={0}>
            <Box>
              <Grid
                autoHeight
                headerHeight={120}
                rows={filteredLocations}
                columns={locationColumns}
                disableColumnFilter
                disableColumnSelector
                disableDensitySelector
                disableColumnMenu
                pageSize={15}
                density="compact"
                rowsPerPageOptions={[15]}
                getRowId={(row) => row.customerLocationId}
                checkboxSelection
                onSelectionModelChange={(customerLocationId) => {
                  const selectedId = customerLocationId[0];
                  const selectedRowData = locations.filter(
                    (f) => f.customerLocationId === selectedId,
                  );
                  setDataSelected(selectedRowData[0]);
                }}
                components={{ Toolbar: CustomGridToolbar }}
              />
            </Box>
          </TabPanel>
          <TabPanel value={currentView} index={1}>
            <Box>
              <Grid
                autoHeight
                headerHeight={120}
                rows={filteredFacilities}
                columns={facilityColumns}
                disableColumnFilter
                disableColumnSelector
                disableDensitySelector
                disableColumnMenu
                pageSize={15}
                density="compact"
                rowsPerPageOptions={[15]}
                getRowId={(row) => row.customerFacilityId}
                checkboxSelection
                onSelectionModelChange={(customerId) => {
                  const selectedId = customerId[0];
                  const selectedRowData = facilities.filter(
                    (f) => f.customerId === selectedId,
                  );
                  setDataSelected(selectedRowData[0]);
                }}
                components={{ Toolbar: CustomGridToolbar }}
              />
            </Box>
          </TabPanel>
        </Card>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(FacilityManagement);
