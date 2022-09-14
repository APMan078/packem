import React, { useEffect, useState, useContext, ChangeEvent } from 'react';
import { useNavigate, createSearchParams } from 'react-router-dom';

import Card from 'components/card';
import Header from 'components/header';
import Input from 'components/input';
import { Grid } from 'components/styles';
import CustomerItemModal from 'pages/shared/customeritemmodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import { getTransferHistory } from 'services/api/transfers/transfers.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Typography, Box } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import {
  GridColDef,
  GridToolbar,
  GridColumns,
  GridActionsCellItem,
} from '@mui/x-data-grid';

function TransferView() {
  const theme = useTheme();
  const navigate = useNavigate();
  const { loading, updateLoading, updateData } = useContext(GlobalContext);
  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  const [transferHistory, setTransferHistory] = useState([]);
  const [transfersPending, setTransfersPending] = useState(0);
  const [transfersCompleted, setTransfersCompleted] = useState(0);
  const [unitsToday, setUnitsToday] = useState(0);
  const [filteredTransfers, setFilteredTransfers] = useState([]);

  const [dataSelected, setDataSelected] = useState({
    customerId: '',
    sku: '',
    description: '',
    uom: '',
  });
  const [searchParams, setSearchParams] = useState({
    searchStatus: '',
    searchSKU: '',
    searchDesc: '',
    searchUOM: '',
    searchFromZone: '',
    searchFromBin: '',
    searchToZone: '',
    searchToBin: '',
    searchQtyTransfer: '',
  });

  const filterTransferHistory = () => {
    const statusSearchResult = transferHistory.filter((t) =>
      t.status.toLowerCase().includes(searchParams.searchStatus.toLowerCase()),
    );
    const searchSKUResults = statusSearchResult.filter((t) =>
      t.itemSKU.toLowerCase().includes(searchParams.searchSKU.toLowerCase()),
    );
    const searchUOMResult = searchSKUResults.filter((t) =>
      t.itemUOM.toLowerCase().includes(searchParams.searchUOM.toLowerCase()),
    );
    const searchFromZoneResult = searchUOMResult.filter((t) =>
      t.fromZone
        .toLowerCase()
        .includes(searchParams.searchFromZone.toLowerCase()),
    );
    const searchFromBinResult = searchFromZoneResult.filter((t) =>
      t.fromBin
        .toLowerCase()
        .includes(searchParams.searchFromBin.toLowerCase()),
    );
    const searchToZoneResult = searchFromBinResult.filter((t) =>
      t.toZone.toLowerCase().includes(searchParams.searchToZone.toLowerCase()),
    );
    const searchToBinResult = searchToZoneResult.filter((t) =>
      t.toBin.toLowerCase().includes(searchParams.searchToBin.toLowerCase()),
    );
    const result = searchToBinResult.filter((t) =>
      t.qtyTransfer
        .toString()
        .toLowerCase()
        .includes(searchParams.searchQtyTransfer.toString().toLowerCase()),
    );

    setFilteredTransfers(result);
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

  const onLoadCustomerTransferQueue = async () => {
    try {
      const transfersFromApi = await getTransferHistory(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
      );

      setUnitsToday(transfersFromApi.unitToday);
      setTransfersPending(transfersFromApi.pending);
      setTransfersCompleted(transfersFromApi.completed);
      setTransferHistory(transfersFromApi.transfers);
      setFilteredTransfers(transfersFromApi.transfers);

      return true;
    } catch (error) {
      return error;
    }
  };

  useEffect(() => {
    setTransferHistory(null);
    setFilteredTransfers([]);
    onLoadCustomerTransferQueue();
    updateLoading(true);
  }, [loading, updateData]);

  type TransferHistoryRows = typeof transferHistory[number];

  const transferColumns: GridColumns<TransferHistoryRows> = [
    {
      field: 'status',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Status</Typography>
          <Input
            sx={{ maxWidth: '120px' }}
            placeholder="Search"
            value={searchParams.searchStatus}
            onChange={(value) => inputHandler('searchStatus', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterTransferHistory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.status}</Typography>,
    },
    {
      field: 'itemSKU',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Item SKU</Typography>
          <Input
            sx={{ maxWidth: '120px' }}
            placeholder="Search"
            value={searchParams.searchSKU}
            onChange={(value) => inputHandler('searchSKU', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterTransferHistory();
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
              itemId: params.row.itemId,
            };
            navigate({
              pathname: `/inventory/item/${params.row.itemSKU}`,
              search: `?${createSearchParams(querySearchParams)}`,
            });
          }}
        >
          {params.row.itemSKU}
        </Typography>
      ),
    },
    {
      field: 'itemDescription',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Description</Typography>
          <Input
            sx={{ maxWidth: '120px' }}
            placeholder="Search"
            value={searchParams.searchDesc}
            onChange={(value) => inputHandler('searchDesc', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterTransferHistory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{params.row.itemDescription}</Typography>
      ),
    },
    {
      field: 'uom',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>UoM</Typography>
          <Input
            sx={{ maxWidth: '120px' }}
            placeholder="Search"
            value={searchParams.searchUOM}
            onChange={(value) => inputHandler('searchUOM', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterTransferHistory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.itemUOM}</Typography>,
    },
    {
      field: 'fromZone',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>From Zone</Typography>
          <Input
            sx={{ maxWidth: '120px' }}
            placeholder="Search"
            value={searchParams.searchFromZone}
            onChange={(value) => inputHandler('searchFromZone', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterTransferHistory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.fromZone}</Typography>,
    },
    {
      field: 'fromBin',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>From Bin</Typography>
          <Input
            sx={{ maxWidth: '120px' }}
            placeholder="Search"
            value={searchParams.searchFromBin}
            onChange={(value) => inputHandler('searchFromBin', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterTransferHistory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.fromBin}
        </Typography>
      ),
    },
    {
      field: 'toZone',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>To Zone</Typography>
          <Input
            sx={{ maxWidth: '120px' }}
            placeholder="Search"
            value={searchParams.searchToZone}
            onChange={(value) => inputHandler('searchToZone', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterTransferHistory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.toZone}</Typography>,
    },
    {
      field: 'toBin',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>To Bin</Typography>
          <Input
            sx={{ maxWidth: '120px' }}
            placeholder="Search"
            value={searchParams.searchToBin}
            onChange={(value) => inputHandler('searchToBin', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterTransferHistory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.toBin}
        </Typography>
      ),
    },
    {
      field: 'qtyTransfer',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Qty Transfer</Typography>
          <Input
            sx={{ maxWidth: '120px' }}
            placeholder="Search"
            value={searchParams.searchQtyTransfer}
            onChange={(value) => inputHandler('searchQtyTransfer', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterTransferHistory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.qtyTransfer}
        </Typography>
      ),
    },
  ];

  return (
    <MainContainer>
      <Header />
      <CustomerItemModal item={dataSelected} />
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
              {transfersPending}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Pending
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
              {transfersCompleted}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Completed
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
              {unitsToday}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Units Today
            </Typography>
          </Card>
        </Box>
        <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography variant="h6" fontWeight="bold">
            Transfers ({currentLocationAndFacility.locationName} •{' '}
            {currentLocationAndFacility.facilityName})
          </Typography>
          <Grid
            autoHeight
            headerHeight={120}
            rows={filteredTransfers}
            columns={transferColumns}
            pageSize={15}
            density="compact"
            disableColumnFilter
            disableColumnSelector
            disableDensitySelector
            disableColumnMenu
            rowsPerPageOptions={[15]}
            getRowId={(row) => row.itemId}
            checkboxSelection
            onSelectionModelChange={(itemId) => {
              const selectedId = itemId[0];
              const selectedRowData = transferHistory.filter(
                (c) => c.itemId === selectedId,
              );
              if (selectedId === undefined) {
                setDataSelected({
                  customerId: '',
                  sku: '',
                  description: '',
                  uom: '',
                });
              } else {
                setDataSelected(selectedRowData[0]);
              }
            }}
            components={{ Toolbar: GridToolbar }}
            componentsProps={{
              toolbar: {
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

export default React.memo(TransferView);
