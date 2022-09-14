import React, { useContext, useEffect, useState } from 'react';
import { useNavigate, createSearchParams } from 'react-router-dom';

import Card from 'components/card';
import LineChart from 'components/charts/LineChart';
import Header from 'components/header';
import { DashboardDivider, Grid } from 'components/styles';
import { MainContainer, ContentContainer } from 'pages/styles';
import {
  getDashboardInventoryFlow,
  getDashboardTopSalesOrders,
  getDashboardLowStock,
  getDashboardOperations,
  getDashboardQueues,
} from 'services/api/dashboard/dashboard.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import {
  Typography,
  Box,
  Tabs,
  Tab,
  Select,
  MenuItem,
  SelectChangeEvent,
  InputLabel,
  FormControl,
  Divider,
} from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { borderBottomColor } from '@mui/system';
import { GridColumns } from '@mui/x-data-grid';

function Dashboard() {
  const theme = useTheme();
  const navigate = useNavigate();
  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  const { updateData } = useContext(GlobalContext);

  const operationsDefault = {
    opsManagers: '',
    activeDevices: '',
    operators: '',
    registeredBins: '',
    binsInUse: '',
    utilization: '',
    salesOrders: '',
    salesOrdersUnits: '',
  };

  const queuesDefault = {
    expected: '',
    purchaseOrders: '',
    putAway: '',
    pick: '',
    transfer: '',
  };

  // Data
  const [topSalesData, setTopSalesData] = React.useState([]);
  const [lowStockData, setLowStockData] = React.useState([]);
  const [operationsData, setOperationsData] = React.useState(operationsDefault);
  const [queuesData, setQueuesData] = React.useState(queuesDefault);
  const [chartData, setChartData] = React.useState({});

  // Day ranges
  const [chartRange, setChartRange] = React.useState(1);
  const [topSalesRange, setTopSalesRange] = React.useState(1);
  const [queuesRange, setQueuesRange] = React.useState(1);
  const [operationsRange, setOperationsRange] = React.useState(1);

  const handleTopSalesRangeChange = async (
    event: React.SyntheticEvent,
    newValue: number,
  ) => {
    setTopSalesRange(newValue);
    try {
      const topSalesFromApi = await getDashboardTopSalesOrders(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
        newValue,
      );
      setTopSalesData(topSalesFromApi);
      return true;
    } catch (err: any) {
      return err;
    }
  };
  const handleQueuesRangeChange = async (
    event: React.SyntheticEvent,
    newValue: number,
  ) => {
    setQueuesRange(newValue);
    try {
      const queuesFromApi = await getDashboardQueues(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
        newValue,
      );
      setQueuesData(queuesFromApi);
      return true;
    } catch (err: any) {
      return err;
    }
  };
  const handleOperationsRangeChange = async (
    event: React.SyntheticEvent,
    newValue: number,
  ) => {
    setOperationsRange(newValue);
    try {
      const operationsFromApi = await getDashboardOperations(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
        newValue,
      );
      setOperationsData(operationsFromApi);
      return true;
    } catch (err: any) {
      return err;
    }
  };

  const handleChartRangeChange = async (event: SelectChangeEvent) => {
    setChartRange(Number(event.target.value));
    try {
      const inventoryFlowFromApi = await getDashboardInventoryFlow(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
        Number(event.target.value),
      );
      setChartData(inventoryFlowFromApi);
      return true;
    } catch (err: any) {
      return err;
    }
  };

  const onLoadDashboardData = async () => {
    try {
      const inventoryFlowFromApi = await getDashboardInventoryFlow(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
        chartRange,
      );
      const topSalesFromApi = await getDashboardTopSalesOrders(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
        topSalesRange,
      );
      const lowStockFromApi = await getDashboardLowStock(
        currentUser.Claim_CustomerId,
      );
      const operationsFromApi = await getDashboardOperations(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
        operationsRange,
      );
      const queuesFromApi = await getDashboardQueues(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
        queuesRange,
      );

      setChartData(inventoryFlowFromApi);
      setTopSalesData(topSalesFromApi);
      setLowStockData(lowStockFromApi);
      setOperationsData(operationsFromApi);
      setQueuesData(queuesFromApi);
      return true;
    } catch (err: any) {
      return err;
    }
  };

  useEffect(() => {
    onLoadDashboardData();
  }, [currentLocationAndFacility, updateData]);

  type LowStockRow = typeof lowStockData[number];
  type ToSellersRow = typeof topSalesData[number];
  const topSellersColumns: GridColumns<ToSellersRow> = [
    {
      field: 'saleOderNo',
      width: 125,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Order No.</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.saleOrderNo}</Typography>,
    },
    {
      field: 'customer',
      width: 150,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Customer</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.customer}</Typography>,
    },
    {
      field: 'total',
      width: 100,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Total</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.total}</Typography>,
    },
    {
      field: 'units',
      width: 75,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Units</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.units}</Typography>,
    },
    {
      field: 'skUs',
      width: 100,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>SKUs</Typography>
        </Box>
      ),
      renderCell: (params) => (
        <Typography
          sx={{ cursor: 'pointer', color: [theme.palette.primary.main] }}
        >
          {params.row.skUs}
        </Typography>
      ),
    },
  ];
  const lowStockColumns: GridColumns<LowStockRow> = [
    {
      field: 'sku',
      width: 150,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>SKU</Typography>
        </Box>
      ),
      renderCell: (params) => (
        <Typography
          sx={{ cursor: 'pointer', color: [theme.palette.primary.main] }}
        >
          {params.row.sku}
        </Typography>
      ),
    },
    {
      field: 'description',
      width: 150,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Description</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.description}</Typography>,
    },
    {
      field: 'expect',
      width: 85,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Expect.</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.expect}</Typography>,
    },
    {
      field: 'onHand',
      width: 100,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography>On Hand</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.onHand}</Typography>,
    },
    {
      field: 'sold',
      width: 100,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography>Sold</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.sold}</Typography>,
    },
    {
      field: 'backorder',
      width: 100,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography>Back Order</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.backorder}</Typography>,
    },
  ];

  return (
    <MainContainer>
      <Header />
      <ContentContainer>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
          <Box
            sx={{
              display: 'flex',
              flexDirection: { xs: 'column', lg: 'row' },
              width: '100%',
              gap: '12px',
            }}
          >
            <Card sx={{ width: { xs: '100%', lg: '66%' } }}>
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'column',
                  width: '100%',
                  height: '100%',
                }}
              >
                <Box
                  sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                    width: '100%',
                  }}
                >
                  <Typography variant="h6" fontWeight="bold">
                    Inventory Flow (Units)
                  </Typography>
                  <Box
                    sx={{
                      display: 'flex',
                      flexDirection: 'column',
                      width: '30%',
                    }}
                  >
                    <FormControl fullWidth>
                      <InputLabel id="chart-range-label">History</InputLabel>
                      <Select
                        labelId="chart-range-label"
                        sx={{ width: '100%' }}
                        value={chartRange.toString()}
                        defaultValue="MTD"
                        label="History"
                        onChange={(event) => handleChartRangeChange(event)}
                      >
                        <MenuItem value={1}>YTD</MenuItem>
                        <MenuItem value={2}>MTD</MenuItem>
                        <MenuItem value={3}>WTD</MenuItem>
                      </Select>
                    </FormControl>
                  </Box>
                </Box>

                <Box sx={{ display: 'flex', width: '100%', height: '100%' }}>
                  <LineChart data={chartData} />
                </Box>
              </Box>
            </Card>
            <Card
              sx={{
                display: 'flex',
                flexDirection: 'column',
                width: { xs: '100%', lg: '33%' },
                gap: '8px',
              }}
            >
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'column',
                  width: '100%',
                  minHeight: '400px',
                  gap: '24px',
                }}
              >
                <Typography variant="h6" fontWeight="bold">
                  Queues (Units)
                </Typography>
                <Box
                  sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    justifyContent: 'space-between',
                    height: '100%',
                  }}
                >
                  <Box
                    sx={{
                      display: 'flex',
                      flexDirection: 'row',
                      alignItems: 'center',
                      justifyContent: 'space-between',
                      width: '100%',
                    }}
                  >
                    <Typography variant="subtitle1" fontWeight="bold">
                      Expected
                    </Typography>
                    <Typography
                      sx={{
                        color: [theme.palette.primary.main],
                      }}
                      variant="h4"
                      fontWeight="regular"
                    >
                      {queuesData.expected}
                    </Typography>
                  </Box>
                  <Box
                    sx={{
                      display: 'flex',
                      flexDirection: 'row',
                      alignItems: 'center',
                      justifyContent: 'space-between',
                      width: '100%',
                    }}
                  >
                    <Box
                      sx={{
                        display: 'flex',
                        width: '70%',
                        height: '50%',
                        alignItems: 'center',
                      }}
                    >
                      <Tabs
                        value={queuesRange}
                        onChange={handleQueuesRangeChange}
                        centered
                      >
                        <Tab
                          sx={{ minWidth: '50px' }}
                          value={10}
                          label="10 Days"
                        />
                        <Divider orientation="vertical" flexItem />
                        <Tab
                          sx={{ minWidth: '50px' }}
                          value={30}
                          label="30 Days"
                        />
                        <Divider orientation="vertical" flexItem />
                        <Tab
                          sx={{ minWidth: '50px' }}
                          value={60}
                          label="60 Days"
                        />
                      </Tabs>
                    </Box>
                    <Typography variant="subtitle2" fontWeight="light">
                      {queuesData.purchaseOrders} Purchase Orders
                    </Typography>
                  </Box>
                  <DashboardDivider />
                  <Box
                    sx={{
                      display: 'flex',
                      flexDirection: 'row',
                      alignItems: 'center',
                      justifyContent: 'space-between',
                      width: '100%',
                    }}
                  >
                    <Typography variant="subtitle1" fontWeight="bold">
                      Put Away
                    </Typography>
                    <Typography
                      sx={{
                        color: [theme.palette.primary.main],
                      }}
                      variant="h4"
                      fontWeight="regular"
                    >
                      {queuesData.putAway}
                    </Typography>
                  </Box>
                  <DashboardDivider />
                  <Box
                    sx={{
                      display: 'flex',
                      flexDirection: 'row',
                      alignItems: 'center',
                      justifyContent: 'space-between',
                      width: '100%',
                    }}
                  >
                    <Typography variant="subtitle1" fontWeight="bold">
                      Pick
                    </Typography>
                    <Typography
                      sx={{
                        color: [theme.palette.primary.main],
                      }}
                      variant="h4"
                      fontWeight="regular"
                    >
                      {queuesData.pick}
                    </Typography>
                  </Box>
                  <DashboardDivider />
                  <Box
                    sx={{
                      display: 'flex',
                      flexDirection: 'row',
                      alignItems: 'center',
                      justifyContent: 'space-between',
                      width: '100%',
                    }}
                  >
                    <Typography variant="subtitle1" fontWeight="bold">
                      Transfer
                    </Typography>
                    <Typography
                      sx={{
                        color: [theme.palette.primary.main],
                      }}
                      variant="h4"
                      fontWeight="regular"
                    >
                      {queuesData.transfer}
                    </Typography>
                  </Box>
                </Box>
              </Box>
            </Card>
          </Box>
          <Box
            sx={{
              display: 'flex',
              flexDirection: { xs: 'column', lg: 'row' },
              width: '100%',
              minHeight: '50%',
              gap: '12px',
            }}
          >
            <Card
              sx={{
                display: 'flex',
                flexDirection: 'column',
                width: { xs: '100%', lg: '33%' },
                gap: '8px',
              }}
            >
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'column',
                  width: '100%',
                  minHeight: '450px',
                }}
              >
                <Box
                  sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                    gap: '12px',
                  }}
                >
                  <Typography
                    sx={{ width: '40%' }}
                    variant="h6"
                    fontWeight="bold"
                  >
                    Top Sales Orders
                  </Typography>
                  <Box
                    sx={{
                      display: 'flex',
                      width: '40%',
                      alignItems: 'center',
                    }}
                  >
                    <Tabs
                      value={topSalesRange}
                      onChange={handleTopSalesRangeChange}
                      centered
                    >
                      <Tab sx={{ minWidth: '50px' }} value={1} label="YTD" />
                      <Tab sx={{ minWidth: '50px' }} value={2} label="MTD" />
                      <Tab sx={{ minWidth: '50px' }} value={3} label="WTD" />
                    </Tabs>
                  </Box>
                  <Box
                    sx={{
                      display: 'flex',
                      width: '20%',
                      justifyContent: 'end',
                    }}
                  >
                    <Typography
                      sx={{
                        cursor: 'pointer',
                        color: [theme.palette.primary.main],
                      }}
                      fontWeight="bold"
                      onClick={() => {
                        navigate('/sales');
                      }}
                    >
                      View All
                    </Typography>
                  </Box>
                </Box>
                <Grid
                  headerHeight={120}
                  sx={{
                    '& .MuiDataGrid-cell': {
                      borderBottomColor: '#F0F0F0',
                    },
                    '& .MuiDataGrid-columnHeaders': {
                      borderBottom: 'none',
                    },
                    '& .MuiTypography-body1': {
                      fontWeight: 'bold',
                    },
                  }}
                  rows={topSalesData}
                  columns={topSellersColumns}
                  pageSize={10}
                  density="compact"
                  disableColumnFilter
                  disableColumnSelector
                  disableDensitySelector
                  disableColumnMenu
                  rowsPerPageOptions={[10]}
                  getRowId={(row) => row.saleOrderId}
                  componentsProps={{
                    toolbar: {
                      printOptions: { disableToolbarButton: true },
                      showQuickFilter: true,
                      quickFilterProps: { debounceMs: 500 },
                    },
                  }}
                />
              </Box>
            </Card>
            <Card
              sx={{
                display: 'flex',
                flexDirection: 'column',
                width: { xs: '100%', lg: '33%' },
                gap: '8px',
              }}
            >
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'column',
                  width: '100%',
                  minHeight: '450px',
                }}
              >
                <Box
                  sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                  }}
                >
                  <Typography
                    sx={{ fontSize: '20px' }}
                    variant="h5"
                    fontWeight="bold"
                  >
                    Low Stock
                  </Typography>
                  <Typography
                    sx={{
                      cursor: 'pointer',
                      color: [theme.palette.primary.main],
                      fontWeight: 'bold',
                    }}
                    onClick={() => {
                      navigate('/inventory');
                    }}
                  >
                    Inventory
                  </Typography>
                </Box>
                <Grid
                  headerHeight={120}
                  sx={{
                    '& .MuiDataGrid-cell': {
                      borderBottomColor: '#F0F0F0',
                    },
                    '& .MuiDataGrid-columnHeaders': {
                      borderBottom: 'none',
                    },
                    '& .MuiTypography-body1': {
                      fontWeight: 'bold',
                    },
                  }}
                  rows={lowStockData}
                  columns={lowStockColumns}
                  pageSize={10}
                  density="compact"
                  disableColumnFilter
                  disableColumnSelector
                  disableDensitySelector
                  disableColumnMenu
                  rowsPerPageOptions={[10]}
                  getRowId={(row) => row.itemId}
                  componentsProps={{
                    toolbar: {
                      printOptions: { disableToolbarButton: true },
                      showQuickFilter: true,
                      quickFilterProps: { debounceMs: 500 },
                    },
                  }}
                />
              </Box>
            </Card>
            <Card sx={{ width: { xs: '100%', lg: '33%' } }}>
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'column',
                  width: '100%',
                  minHeight: '400px',
                }}
              >
                <Box
                  sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                  }}
                >
                  <Typography variant="h6" fontWeight="bold">
                    Operations
                  </Typography>
                  <Box
                    sx={{
                      display: 'flex',
                      flexDirection: 'row',
                      alignItems: 'center',
                      gap: '24px',
                    }}
                  >
                    <Typography
                      sx={{
                        cursor: 'pointer',
                        color: [theme.palette.primary.main],
                      }}
                      fontWeight="bold"
                      onClick={() => {
                        navigate('/users');
                      }}
                    >
                      Users
                    </Typography>
                    <Typography
                      sx={{
                        cursor: 'pointer',
                        color: [theme.palette.primary.main],
                      }}
                      fontWeight="bold"
                      onClick={() => {
                        navigate('/bins');
                      }}
                    >
                      Storage
                    </Typography>
                  </Box>
                </Box>
                <Box
                  sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    width: '100%',
                    padding: '24px',
                    height: '100%',
                  }}
                >
                  <Box
                    sx={{
                      display: 'flex',
                      flexDirection: 'row',
                      width: '100%',
                      height: '40%',
                      gap: '24px',
                    }}
                  >
                    <Box
                      sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'start',
                        justifyContent: 'center',
                        width: '33%',
                        gap: '12px',
                      }}
                    >
                      <Typography variant="h3" fontWeight="regular">
                        {Object.keys(operationsData).length > 0 &&
                          operationsData.opsManagers}
                      </Typography>
                      <Typography variant="subtitle1" fontWeight="bold">
                        Ops Managers
                      </Typography>
                    </Box>
                    <Box
                      sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'start',
                        justifyContent: 'center',
                        width: '33%',
                        gap: '12px',
                      }}
                    >
                      <Typography variant="h3" fontWeight="regular">
                        {Object.keys(operationsData).length > 0 &&
                          operationsData.activeDevices}
                      </Typography>
                      <Typography variant="subtitle1" fontWeight="bold">
                        Active Devices
                      </Typography>
                    </Box>
                    <Box
                      sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'start',
                        justifyContent: 'center',
                        width: '33%',
                        gap: '12px',
                      }}
                    >
                      <Typography variant="h3" fontWeight="regular">
                        {Object.keys(operationsData).length > 0 &&
                          operationsData.operators}
                      </Typography>
                      <Typography variant="subtitle1" fontWeight="bold">
                        Operators
                      </Typography>
                    </Box>
                  </Box>
                  <DashboardDivider />
                  <Box
                    sx={{
                      display: 'flex',
                      flexDirection: 'row',
                      width: '100%',
                      height: '40%',
                      gap: '24px',
                    }}
                  >
                    <Box
                      sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'start',
                        justifyContent: 'center',
                        width: '33%',
                        gap: '12px',
                      }}
                    >
                      <Typography variant="h3" fontWeight="regular">
                        {Object.keys(operationsData).length > 0 &&
                          operationsData.registeredBins}
                      </Typography>
                      <Typography variant="subtitle1" fontWeight="bold">
                        Registered Bins
                      </Typography>
                    </Box>
                    <Box
                      sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'start',
                        justifyContent: 'center',
                        width: '33%',
                        gap: '12px',
                      }}
                    >
                      <Typography variant="h3" fontWeight="regular">
                        {Object.keys(operationsData).length > 0 &&
                          operationsData.binsInUse}
                      </Typography>
                      <Typography variant="subtitle1" fontWeight="bold">
                        Bins in Use
                      </Typography>
                    </Box>
                    <Box
                      sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'start',
                        justifyContent: 'center',
                        width: '33%',
                        gap: '12px',
                      }}
                    >
                      <Typography variant="h3" fontWeight="regular">
                        {Object.keys(operationsData).length > 0 &&
                          `${operationsData.utilization}`}
                      </Typography>
                      <Typography variant="subtitle1" fontWeight="bold">
                        Utilization
                      </Typography>
                    </Box>
                  </Box>
                  <DashboardDivider />
                  <Box
                    sx={{
                      display: 'flex',
                      flexDirection: 'row',
                      width: '100%',
                      height: '20%',
                      gap: '24px',
                    }}
                  >
                    <Box
                      sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'start',
                        width: '100%',
                        pt: '12px',
                        gap: '8px',
                      }}
                    >
                      <Box
                        sx={{
                          display: 'flex',
                          flexDirection: 'row',
                          alignItems: 'center',
                          justifyContent: 'start',
                          width: '100%',
                        }}
                      >
                        <Typography variant="h4" fontWeight="regular">
                          {Object.keys(operationsData).length > 0 &&
                            `${operationsData.salesOrders} (${operationsData.salesOrdersUnits} Units)`}
                        </Typography>
                      </Box>
                      <Box
                        sx={{
                          display: 'flex',
                          flexDirection: 'row',
                          alignItems: 'center',
                          justifyContent: 'space-between',
                          width: '100%',
                        }}
                      >
                        <Typography variant="subtitle1" fontWeight="bold">
                          Daily Sales Orders (Avg)
                        </Typography>
                        <Box
                          sx={{
                            display: 'flex',
                            width: '60%',
                            height: '20%',
                            alignItems: 'center',
                            justifyContent: 'end',
                          }}
                        >
                          <Tabs
                            value={operationsRange}
                            onChange={handleOperationsRangeChange}
                            centered
                          >
                            <Tab
                              sx={{ minWidth: '30px', padding: '8px' }}
                              value={5}
                              label="5 Days"
                            />
                            <Divider orientation="vertical" flexItem />
                            <Tab
                              sx={{ minWidth: '30px', padding: '8px' }}
                              value={15}
                              label="15 Days"
                            />
                            <Divider orientation="vertical" flexItem />
                            <Tab
                              sx={{ minWidth: '30px', padding: '8px' }}
                              value={30}
                              label="30 Days"
                            />
                          </Tabs>
                        </Box>
                      </Box>
                    </Box>
                  </Box>
                </Box>
              </Box>
            </Card>
          </Box>
        </Box>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(Dashboard);
