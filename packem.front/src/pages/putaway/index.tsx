import React, { useEffect, useState, useContext, ChangeEvent } from 'react';
import { useNavigate, createSearchParams } from 'react-router-dom';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid } from 'components/styles';
import moment from 'moment';
import { getPutAwayQueue } from 'services/api/putaway/putaway.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Search } from '@mui/icons-material';
import { Typography, Box, InputAdornment, IconButton } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { GridColumns } from '@mui/x-data-grid';

import { MainContainer, ContentContainer } from '../styles';

function PutAway() {
  const theme = useTheme();
  const navigate = useNavigate();
  const { loading, updateLoading, updateData } = useContext(GlobalContext);
  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  const [error, setError] = useState('');
  const [putAwayQueue, setPutAwayQueue] = useState([]);
  const [filteredPutAwayQueue, setFilteredPutAwayQueue] = useState([]);
  const [itemCount, setItemCount] = useState(0);
  const [dataSelected, setDataSelected] = useState({
    putAwayId: '',
    sku: '',
    uom: '',
    description: '',
    qty: '',
    remaining: '',
    receivedTime: '',
  });
  const [searchParams, setSearchParams] = useState({
    searchSku: '',
    searchUOM: '',
    searchDescription: '',
    searchQty: '',
    searchRemaining: '',
    searchReceivedTime: '',
  });

  const onForm = (key, text) => {
    setSearchParams(() => ({
      ...searchParams,
      [key]: text,
    }));
  };

  const handleResetDataSelected = () => {
    setDataSelected({
      putAwayId: '',
      sku: '',
      uom: '',
      description: '',
      qty: '',
      remaining: '',
      receivedTime: '',
    });
  };

  const filterPutAwayQueue = () => {
    const putAwaySkuResult = putAwayQueue.filter((p) =>
      p.sku.toString().toLowerCase().includes(searchParams.searchSku),
    );
    const descriptionResult = putAwaySkuResult.filter((p) =>
      p.description.toLowerCase().includes(searchParams.searchDescription),
    );
    const uomResult = descriptionResult.filter((p) =>
      p.uom.toString().toLowerCase().includes(searchParams.searchDescription),
    );
    const searchQtyResult = uomResult.filter((p) =>
      p.qty.includes(searchParams.searchQty),
    );
    const searchRemainingResult = searchQtyResult.filter((p) =>
      p.remaining.includes(searchParams.searchRemaining),
    );
    const result = searchRemainingResult.filter((p) =>
      moment(p.receivedTime)
        .toString()
        .includes(searchParams.searchReceivedTime),
    );

    setFilteredPutAwayQueue(result);
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) => {
    onForm(key, event.target.value);
  };

  const onLoadCustomerPutAwayQueue = async () => {
    try {
      const customerPutAwayQueueFromApi = await getPutAwayQueue(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
      );
      setItemCount(customerPutAwayQueueFromApi.items);
      setPutAwayQueue(customerPutAwayQueueFromApi.putAways);

      return true;
    } catch (err: any) {
      return err;
    }
  };

  useEffect(() => {
    setDataSelected({
      putAwayId: '',
      sku: '',
      uom: '',
      description: '',
      qty: '',
      remaining: '',
      receivedTime: '',
    });
    updateLoading(true);
    setPutAwayQueue([]);
    onLoadCustomerPutAwayQueue();
  }, [loading, updateData, currentLocationAndFacility]);

  type Row = typeof putAwayQueue[number];

  const putAwayColumns: GridColumns<Row> = [
    {
      field: 'itemSKU',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '5px' }}>
          <Typography fontWeight="bold">Item SKU.</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchSku}
            onChange={(value) => inputHandler('searchSku', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPutAwayQueue();
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
              pathname: `/inventory/item/${params.row.sku}`,
              search: `?${createSearchParams(querySearchParams)}`,
            });
          }}
        >
          {params.row.sku}
        </Typography>
      ),
    },
    {
      field: 'uom',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '5px' }}>
          <Typography fontWeight="bold">UoM</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchUOM}
            onChange={(value) => inputHandler('searchUoM', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPutAwayQueue();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.uom}</Typography>,
    },
    {
      field: 'itemQty',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '5px' }}>
          <Typography fontWeight="bold">Qty</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchQty}
            onChange={(value) => inputHandler('searchQty', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPutAwayQueue();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.qty}</Typography>,
    },
    {
      field: 'description',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '5px' }}>
          <Typography fontWeight="bold">Description</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchDescription}
            onChange={(value) => inputHandler('searchDescription', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPutAwayQueue();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.description}</Typography>,
    },
    {
      field: 'dateReceived',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '5px' }}>
          <Typography fontWeight="bold">Date Received</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchReceivedTime}
            onChange={(value) => inputHandler('searchReceivedTime', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPutAwayQueue();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>
          {moment(params.row.receivedTime).format('DD/MM/YYYY')}
        </Typography>
      ),
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
        </Box>
        <Card sx={{ display: 'flex', flexDirection: 'column', gap: '5px' }}>
          <Grid
            autoHeight
            headerHeight={120}
            rows={putAwayQueue}
            columns={putAwayColumns}
            pageSize={15}
            density="compact"
            disableColumnFilter
            disableColumnSelector
            disableDensitySelector
            disableColumnMenu
            rowsPerPageOptions={[15]}
            getRowId={(row) => row.putAwayId}
            onSelectionModelChange={(putAwayId) => {
              const selectedId = putAwayId[0];
              const selectedRowData = putAwayQueue.filter(
                (c) => c.putAwayId === selectedId,
              );
              if (selectedId === undefined) {
                setDataSelected({
                  putAwayId: '',
                  sku: '',
                  uom: '',
                  description: '',
                  qty: '',
                  remaining: '',
                  receivedTime: '',
                });
              } else {
                setDataSelected(selectedRowData[0]);
              }
            }}
            components={{ Toolbar: CustomGridToolbar }}
            componentsProps={{
              toolbar: {
                title: 'Items',
              },
            }}
          />
        </Card>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(PutAway);
