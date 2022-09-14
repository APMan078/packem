import React, { useEffect, useState, useContext, ChangeEvent } from 'react';
import { useNavigate, createSearchParams } from 'react-router-dom';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid } from 'components/styles';
import TabPanel, { a11yProps } from 'components/tabpanel';
import { snackActions } from 'config/snackbar.js';
import BinCategory from 'helpers/bincategoryhelper';
import FileInputModal from 'pages/shared/addfilemodal';
import ConfirmationDialog from 'pages/shared/confirmdeletemodal';
import CreateBinModal from 'pages/shared/createbinmodal';
import CustomerFacilityZoneModal from 'pages/shared/customerfacilityzonemodal';
import {
  lookupCustomerZonesByFacilityId,
  getStorageManagementDetails,
  addImportedZonesAndBins,
} from 'services/api/customerfacilities/customerfacilities.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Search } from '@mui/icons-material';
import { Typography, Box, Tabs, Tab } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import {
  GridToolbar,
  GridActionsCellItem,
  GridColumns,
} from '@mui/x-data-grid';

import { MainContainer, ContentContainer } from '../styles';

type ImportedZonesBins = {
  customerId: number;
  customerLocationId: number;
  customerFacilityId: number;
  zoneName: string;
  binName: string;
  binZone: number;
};

function BinManagement() {
  const theme = useTheme();
  const navigate = useNavigate();
  const {
    loading,
    updateLoading,
    updateData,
    onOpenCreateBinModal,
    onOpenFacilityZoneModal,
    onOpenConfirmDeleteDialog,
    handleUpdateData,
  } = useContext(GlobalContext);
  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  const [currentView, setCurrentView] = useState(0);
  const [binCount, setBinCount] = useState(0);
  const [zoneCount, setZoneCount] = useState(0);
  const [error, setError] = useState('');
  const [customerBins, setCustomerBins] = useState([]);
  const [filteredBins, setFilteredBins] = useState([]);
  const [facilityZones, setFacilityZones] = useState([]);
  const [filteredZones, setFilteredZones] = useState([]);
  const [zoneAction, setZoneAction] = useState(false);
  const [editZone, setEditZone] = useState(false);
  const [dataSelected, setDataSelected] = useState({
    binId: '',
    customerLocationId: '',
    zoneId: '',
    name: '',
  });
  const [searchParams, setSearchParams] = useState({
    searchZone: '',
    searchName: '',
    searchUOM: '',
    searchItemQty: '',
    searchCategory: '',
  });

  const mapAndAddZonesBins = async (data) => {
    let importedZonesBins: ImportedZonesBins[] = [];

    if (Array.isArray(data)) {
      importedZonesBins = data
        .filter((zb) => zb.BinName !== '')
        .map((zb) => ({
          customerId: currentUser.Claim_CustomerId,
          customerLocationId: currentLocationAndFacility.customerLocationId,
          customerFacilityId: currentLocationAndFacility.customerFacilityId,
          zoneName: zb.ZoneName,
          binName: zb.BinName,
          binZone: zb.BinZone,
        }));
    }

    try {
      await addImportedZonesAndBins(
        currentLocationAndFacility.customerLocationId,
        importedZonesBins,
      );
      snackActions.success(
        `Successfully imported areas and locations for ${currentLocationAndFacility.locationName}, ${currentLocationAndFacility.facilityName}.`,
      );
      handleUpdateData();
    } catch (err: any) {
      setError(err);
      snackActions.error(`${err}`);
    }
  };

  const onForm = (key, text) => {
    setSearchParams(() => ({
      ...searchParams,
      [key]: text,
    }));
  };

  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setCurrentView(newValue);
  };

  const handleResetDataSelected = () => {
    setDataSelected({
      binId: '',
      customerLocationId: '',
      zoneId: '',
      name: '',
    });
    setEditZone(false);
    setZoneAction(false);
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) => {
    onForm(key, event.target.value);
  };

  const filterBins = () => {
    const nameSearchResult = customerBins.filter((b) =>
      b.name.toLowerCase().includes(searchParams.searchName.toLowerCase()),
    );
    const zoneSearchResult = nameSearchResult.filter((b) =>
      b.zone.toLowerCase().includes(searchParams.searchZone.toLowerCase()),
    );
    const uomSearchResult = zoneSearchResult.filter((b) =>
      b.uom.toLowerCase().includes(searchParams.searchUOM.toLowerCase()),
    );
    const categorySearchResult = uomSearchResult.filter((b) =>
      BinCategory[b.category]
        .toLowerCase()
        .includes(searchParams.searchCategory.toLowerCase()),
    );
    const result = categorySearchResult.filter((b) =>
      b.qty
        .toString()
        .toLowerCase()
        .includes(searchParams.searchItemQty.toString().toLowerCase()),
    );

    setFilteredBins(result);
  };

  const filterZones = () => {
    const customerZoneIdResult = facilityZones.filter((f) =>
      f.zoneId.toString().includes(searchParams.searchZone),
    );
    const result = customerZoneIdResult.filter((f) =>
      f.name.toLowerCase().includes(searchParams.searchName.toLowerCase()),
    );

    setFilteredZones(result);
  };

  const onLoadCustomerBinsAndZones = async () => {
    try {
      const customerBinsFromApi = await getStorageManagementDetails(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
      );

      const zonesFromApi = await lookupCustomerZonesByFacilityId(
        currentLocationAndFacility.customerFacilityId,
        '',
      );

      setCustomerBins(customerBinsFromApi.binDetails);
      setFilteredBins(customerBinsFromApi.binDetails);
      setBinCount(customerBinsFromApi.bins);
      setZoneCount(customerBinsFromApi.zones);
      setFacilityZones(zonesFromApi);
      setFilteredZones(zonesFromApi);

      return true;
    } catch (err: any) {
      return err;
    }
  };

  useEffect(() => {
    setDataSelected({
      binId: '',
      customerLocationId: '',
      zoneId: '',
      name: '',
    });
    setBinCount(0);
    setZoneCount(0);
    updateLoading(true);
    setCustomerBins([]);
    setFilteredBins([]);
    setFacilityZones([]);
    setFilteredZones([]);
    onLoadCustomerBinsAndZones();
  }, [loading, updateData, currentLocationAndFacility]);

  const handleEditBin = (rowData) => {
    setDataSelected(rowData);
    onOpenCreateBinModal();
  };

  const handleClickViewBinDetails = (rowData) => {
    setDataSelected(rowData);
    const querySearchParams = {
      binId: rowData.binId,
    };
    navigate({
      pathname: `/bins/bin/${rowData.name}`,
      search: `?${createSearchParams(querySearchParams)}`,
    });
  };

  type Row = typeof customerBins[number];

  const binColumns: GridColumns<Row> = [
    {
      field: 'actions',
      type: 'actions',
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="View Bin Details"
          onClick={() => handleClickViewBinDetails(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Edit Bin"
          onClick={() => handleEditBin(params.row)}
          showInMenu
        />,
      ],
    },
    {
      field: 'name',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Bin</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchName}
            onChange={(value) => inputHandler('searchName', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterBins();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography
          sx={{ cursor: 'pointer', color: [theme.palette.primary.main] }}
          onClick={() => {
            const querySearchParams = {
              binId: params.row.binId,
            };
            navigate({
              pathname: `/bins/bin/${params.row.name}`,
              search: `?${createSearchParams(querySearchParams)}`,
            });
          }}
        >
          {params.row.name}
        </Typography>
      ),
    },
    {
      field: 'zoneName',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Zone</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchZone}
            onChange={(value) => inputHandler('searchZone', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterBins();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.zone}</Typography>,
    },
    {
      field: 'UOM',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">UoM</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchUOM}
            onChange={(value) => inputHandler('searchUOM', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterBins();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.uom}</Typography>,
    },
    {
      field: 'uniqueSKUs',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Unique SKUs</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchItemQty}
            onChange={(value) => inputHandler('searchItemQty', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterBins();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.uniqueSKU}</Typography>,
    },
    {
      field: 'qtyOnHand',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Item Qty</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchName}
            onChange={(value) => inputHandler('searchName', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterBins();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.qty}</Typography>,
    },
    {
      field: 'category',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Category</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchCategory}
            onChange={(value) => inputHandler('searchCategory', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterBins();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{BinCategory[params.row.category]}</Typography>
      ),
    },
  ];

  type ZoneRows = typeof facilityZones[number];

  const handleClickEditZone = (rowData) => {
    setEditZone(true);
    setDataSelected(rowData);
    onOpenFacilityZoneModal();
  };

  const handleClickDeleteZone = (rowData) => {
    setZoneAction(true);
    setDataSelected(rowData);
    onOpenConfirmDeleteDialog();
  };

  const zoneColumns: GridColumns<ZoneRows> = [
    {
      field: 'actions',
      type: 'actions',
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="Edit"
          onClick={() => handleClickEditZone(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Delete"
          onClick={() => handleClickDeleteZone(params.row)}
          showInMenu
        />,
      ],
    },
    {
      field: 'zoneId',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Zone Id</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchZone}
            onChange={(value) => inputHandler('searchZone', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterZones();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.zoneId}
        </Typography>
      ),
    },
    {
      field: 'name',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Zone Name</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchName}
            onChange={(value) => inputHandler('searchName', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterZones();
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
      <Header />
      <FileInputModal callBack={mapAndAddZonesBins} />
      <CreateBinModal
        binData={dataSelected}
        resetData={handleResetDataSelected}
      />
      {zoneAction && (
        <ConfirmationDialog
          deleteZone
          zoneData={dataSelected}
          callBack={handleResetDataSelected}
        />
      )}
      {editZone ? (
        <CustomerFacilityZoneModal
          edit
          zoneData={dataSelected}
          callBack={handleResetDataSelected}
        />
      ) : (
        <CustomerFacilityZoneModal />
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
              {binCount}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Locations
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
              {zoneCount}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Areas
            </Typography>
          </Card>
        </Box>
        <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Tabs value={currentView} onChange={handleChange}>
            <Tab label="Locations" {...a11yProps(0)} />
            <Tab label="Areas" {...a11yProps(1)} />
          </Tabs>
          <TabPanel value={currentView} index={0}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredBins}
              columns={binColumns}
              pageSize={15}
              density="compact"
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              rowsPerPageOptions={[15]}
              getRowId={(row) => row.binId}
              checkboxSelection
              onSelectionModelChange={(binId) => {
                const selectedId = binId[0];
                const selectedRowData = customerBins.filter(
                  (c) => c.binId === selectedId,
                );
                if (selectedId === undefined) {
                  setDataSelected({
                    binId: '',
                    customerLocationId: '',
                    zoneId: '',
                    name: '',
                  });
                } else {
                  setDataSelected(selectedRowData[0]);
                }
              }}
              components={{ Toolbar: CustomGridToolbar }}
            />
          </TabPanel>
          <TabPanel value={currentView} index={1}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredZones}
              columns={zoneColumns}
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              pageSize={15}
              density="compact"
              rowsPerPageOptions={[15]}
              getRowId={(row) => row.zoneId}
              checkboxSelection
              onSelectionModelChange={(customerId) => {
                const selectedId = customerId[0];
                const selectedRowData = facilityZones.filter(
                  (f) => f.zoneId === selectedId,
                );
                setDataSelected(selectedRowData[0]);
              }}
              components={{ Toolbar: CustomGridToolbar }}
            />
          </TabPanel>
        </Card>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(BinManagement);
