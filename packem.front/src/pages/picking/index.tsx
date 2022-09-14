import React, { useEffect, useState, useContext, ChangeEvent } from 'react';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid } from 'components/styles';
import UserRoles from 'helpers/userrolehelper';
import { MainContainer, ContentContainer } from 'pages/styles';
import { getSaleOrderPickQueue } from 'services/api/salesorders/salesorders.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Search } from '@mui/icons-material';
import { Typography, Box } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { GridColumns } from '@mui/x-data-grid';

function Picking() {
  const theme = useTheme();
  const { currentLocationAndFacility } = useContext(AuthContext);
  const { updateData } = useContext(GlobalContext);
  const [pickingQueue, setPickingQueue] = useState([]);
  const [filteredPickingQueue, setFilteredPickingQueue] = useState([]);
  const [completedToday, setCompletedToday] = useState(0);
  const [unitsPending, setUnitsPending] = useState(0);
  const [itemCount, setItemCount] = useState(0);
  const initialSearchState = {
    searchSaleOrderNo: '',
    searchStatus: '',
    searchUnits: '',
    searchLocations: '',
  };
  const [searchParams, setSearchParams] = useState(initialSearchState);
  const onForm = (key, text) => {
    setSearchParams(() => ({
      ...searchParams,
      [key]: text,
    }));
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) => {
    onForm(key, event.target.value);
  };

  const filterUsers = () => {
    const saleOrderNoSearchResults = pickingQueue.filter((u) =>
      u.saleOrderNo.toString().includes(searchParams.searchSaleOrderNo),
    );
    const pickingStatusSearchResults = saleOrderNoSearchResults.filter((u) =>
      u.pickingStatus
        .toString()
        .toLowerCase()
        .includes(searchParams.searchStatus),
    );
    const unitsSearchResults = pickingStatusSearchResults.filter((u) =>
      u.units.toString().toLowerCase().includes(searchParams.searchUnits),
    );
    const finalResult = unitsSearchResults.filter((u) =>
      u.locations
        .toString()
        .toLowerCase()
        .includes(searchParams.searchLocations),
    );

    setFilteredPickingQueue(finalResult);
  };

  const onLoadPickingQueue = async () => {
    try {
      const pickingQueueFromApi = await getSaleOrderPickQueue(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
      );

      setPickingQueue(pickingQueueFromApi.items);
      setFilteredPickingQueue(pickingQueueFromApi.items);
      setItemCount(pickingQueueFromApi.itemCount);
      setUnitsPending(pickingQueueFromApi.unitsPending);
      setCompletedToday(pickingQueueFromApi.completedToday);

      return true;
    } catch (error) {
      return error;
    }
  };

  useEffect(() => {
    onLoadPickingQueue();
  }, [updateData]);

  type PickingRow = typeof pickingQueue[number];

  const pickingColumns: GridColumns<PickingRow> = [
    {
      field: 'saleOrderNo',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Order</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchSaleOrderNo}
            onChange={(value) => inputHandler('searchSaleOrderNo', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.saleOrderNo}</Typography>,
    },
    {
      field: 'pickingStatus',
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
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{params.row.pickingStatus}</Typography>
      ),
    },
    {
      field: 'units',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Units</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchUnits}
            onChange={(value) => inputHandler('searchUnits', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.units}</Typography>,
    },
    {
      field: 'locations',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Locations</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchLocations}
            onChange={(value) => inputHandler('searchLocations', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.locations}</Typography>,
    },
  ];

  return (
    <MainContainer>
      <Header />
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
              {itemCount}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Items
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
              {completedToday}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Completed Today
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
              {unitsPending}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Units Pending
            </Typography>
          </Card>
        </Box>
        <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Grid
            autoHeight
            headerHeight={120}
            rows={filteredPickingQueue}
            density="compact"
            disableColumnFilter
            disableColumnSelector
            disableDensitySelector
            disableColumnMenu
            columns={pickingColumns}
            pageSize={15}
            rowsPerPageOptions={[15]}
            getRowId={(row) => row.saleOrderId}
            components={{ Toolbar: CustomGridToolbar }}
            componentsProps={{
              toolbar: {
                title: 'Items',
                printOptions: { disableToolbarButton: true },
                showQuickFilter: true,
                quickFilterProps: { debounceMs: 500 },
              },
            }}
          />
        </Card>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(Picking);
