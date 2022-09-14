import React, { useEffect, useState, useContext, ChangeEvent } from 'react';
import {
  useSearchParams,
  createSearchParams,
  useNavigate,
} from 'react-router-dom';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid } from 'components/styles';
import CustomerBinModal from 'pages/shared/customerbinmodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import {
  getBinDetails,
  getCustomerBinByBinId,
} from 'services/api/customerfacilities/customerfacilities.api';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Search } from '@mui/icons-material';
import { Typography, Box } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import {
  GridToolbar,
  GridActionsCellItem,
  GridColumns,
} from '@mui/x-data-grid';

function BinView() {
  const theme = useTheme();
  const navigate = useNavigate();
  const [itemCount, setItemCount] = useState(0);
  const [uniqueSKUs, setUniqueSKUs] = useState(0);
  const [binItemDetails, setBinItemDetails] = useState([]);
  const [filteredBinItems, setFilteredBinItems] = useState([]);
  const [binDetails, setBinDetails] = useState([]);
  const [urlSearchParams] = useSearchParams();

  const { loading, updateData, onOpenTransferModal, onOpenAdjustModal } =
    useContext(GlobalContext);
  const [dataSelected, setDataSelected] = useState({
    binId: '',
    customerLocationId: '',
    zoneId: '',
    name: '',
  });
  const [searchParams, setSearchParams] = useState({
    searchSKU: '',
    searchQty: '',
    searchDesc: '',
    searchUOM: '',
    searchVendor: '',
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

  const onLoadBinDetails = async () => {
    try {
      const binFromApi = new Array(1);
      binFromApi[0] = await getCustomerBinByBinId(urlSearchParams.get('binId'));
      const binDetailsFromApi = await getBinDetails(
        urlSearchParams.get(`binId`),
      );

      setItemCount(binDetailsFromApi.items);
      setUniqueSKUs(binDetailsFromApi.uniqueSKUs);
      setBinItemDetails(binDetailsFromApi.itemDetails);
      setFilteredBinItems(binDetailsFromApi.itemDetails);
      setBinDetails(binFromApi);

      return true;
    } catch (error) {
      return error;
    }
  };

  useEffect(() => {
    setItemCount(0);
    setUniqueSKUs(0);
    setBinItemDetails([]);
    setFilteredBinItems([]);
    onLoadBinDetails();
  }, [loading, updateData]);

  const filterItems = () => {
    const searchSKUResult = binItemDetails.filter((c) =>
      c.itemSKU.toString().includes(searchParams.searchSKU),
    );
    const searchQtyResult = searchSKUResult.filter((i) =>
      i.qtyOnHand.toString().includes(searchParams.searchQty),
    );
    const searchDescResult = searchQtyResult.filter((c) =>
      c.description.toLowerCase().includes(searchParams.searchDesc),
    );
    const finalResult = searchDescResult.filter((c) =>
      c.uom.toString().toLowerCase().includes(searchParams.searchUOM),
    );

    setFilteredBinItems(finalResult);
  };

  const handleResetDataSelected = () => {
    setDataSelected({
      binId: '',
      customerLocationId: '',
      zoneId: '',
      name: '',
    });
  };

  type Row = typeof binDetails[number];

  const binColumns: GridColumns<Row> = [
    {
      field: 'name',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Bin</Typography>
        </Box>
      ),
      renderCell: (params) => (
        <Typography
          sx={{ cursor: 'pointer', color: [theme.palette.primary.main] }}
        >
          {params.row.name}
        </Typography>
      ),
    },
    {
      field: 'zoneName',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Zone</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.zoneName}</Typography>,
    },
  ];

  const handleViewItemDetails = (rowData) => {
    const querySearchParams = {
      itemId: rowData.itemId,
    };
    navigate({
      pathname: `/inventory/item/${rowData.itemSKU}`,
      search: `?${createSearchParams(querySearchParams)}`,
    });
  };

  type ItemRow = typeof binItemDetails[number];

  const binItemDetailColumns: GridColumns<ItemRow> = [
    {
      field: 'actions',
      type: 'actions',
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="View Item Details"
          onClick={() => handleViewItemDetails(params.row)}
          showInMenu
        />,
      ],
    },
    {
      field: 'itemSKU',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Item SKU</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchSKU}
            onChange={(value) => inputHandler('searchSKU', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterItems();
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
      field: 'desc',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Description</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchDesc}
            onChange={(value) => inputHandler('searchDesc', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterItems();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.description}</Typography>,
    },
    {
      field: 'uom',
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
                filterItems();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.uom}</Typography>,
    },
    {
      field: 'qtyOnHand',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Qty on Hand</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchQty}
            onChange={(value) => inputHandler('searchQty', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterItems();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.qty}</Typography>,
    },
  ];

  return (
    <MainContainer>
      <Header />
      <CustomerBinModal itemDetails={urlSearchParams.get(`itemId`)} />
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
              {uniqueSKUs}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Unique SKUs
            </Typography>
          </Card>
        </Box>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
          <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={binDetails}
              columns={binColumns}
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              pageSize={6}
              density="compact"
              rowsPerPageOptions={[6]}
              getRowId={(row) => row.binId}
              checkboxSelection
              onSelectionModelChange={(binId) => {
                const selectedId = binId[0];
                const selectedRowData = binDetails.filter(
                  (f) => f.binId === selectedId,
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
              componentsProps={{
                toolbar: {
                  title: 'Details',
                },
              }}
            />
          </Card>
          <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredBinItems}
              columns={binItemDetailColumns}
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
                const selectedRowData = binItemDetails.filter(
                  (c) => c.itemId === selectedId,
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
              componentsProps={{
                toolbar: {
                  title: 'Items',
                },
              }}
            />
          </Card>
        </Box>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(BinView);
