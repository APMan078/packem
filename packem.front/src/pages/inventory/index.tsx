import React, { useEffect, useState, useContext, ChangeEvent } from 'react';
import { useNavigate, createSearchParams } from 'react-router-dom';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid } from 'components/styles';
import moment from 'moment';
import ConfirmationDialog from 'pages/shared/confirmdeletemodal';
import CustomerItemModal from 'pages/shared/customeritemmodal';
import EditExpirationDateModal from 'pages/shared/editexpirationdatemodal';
import EditItemThresholdModal from 'pages/shared/edititemthreshold';
import { getCustomerLocationsById } from 'services/api/customerlocations/customerlocations.api';
import { getCustomerInventoryByCustomerId } from 'services/api/item/item.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Search } from '@mui/icons-material';
import { Typography, Box } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import {
  GridToolbar,
  GridActionsCellItem,
  GridColumns,
} from '@mui/x-data-grid';

import { MainContainer, ContentContainer } from '../styles';

function Inventory() {
  const theme = useTheme();
  const navigate = useNavigate();
  const {
    loading,
    updateLoading,
    updateData,
    onOpenConfirmDeleteDialog,
    onOpenEditExpirationDateModal,
    onOpenEditThresholdModal,
  } = useContext(GlobalContext);
  const { currentUser, isInventoryViewer } = useContext(AuthContext);
  const [customerInventory, setCustomerInventory] = useState([]);
  const [filteredInventory, setFilteredInventory] = useState([]);
  const [customerLocations, setCustomerLocations] = useState([]);
  const [dataSelected, setDataSelected] = useState({
    customerId: '',
    sku: '',
    description: '',
    uom: '',
  });
  const [searchParams, setSearchParams] = useState({
    searchSKU: '',
    searchBinLocation: '',
    searchQty: '',
    searchDesc: '',
    searchUOM: '',
    searchVendor: '',
    searchExpirationDate: '',
    searchThreshold: '',
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

  const filterInventory = () => {
    const searchSKUResult = customerInventory.filter((c) =>
      c.itemSKU.toString().includes(searchParams.searchSKU),
    );
    const searchBinLocationsResult = searchSKUResult.filter((c) =>
      c.binLocations
        .toString()
        .toLowerCase()
        .includes(searchParams.searchBinLocation),
    );
    const searchQtyResult = searchBinLocationsResult.filter((i) =>
      i.qtyOnHand.toString().includes(searchParams.searchQty),
    );
    const searchDescResult = searchQtyResult.filter((c) =>
      c.description.toLowerCase().includes(searchParams.searchDesc),
    );
    const searchUOMResult = searchDescResult.filter((c) =>
      c.uom.toString().toLowerCase().includes(searchParams.searchUOM),
    );
    const searchExpirationDate = searchUOMResult.filter((c) =>
      moment(c.expirationDate)
        .format('MM/DD/YY')
        .includes(searchParams.searchExpirationDate.toLowerCase()),
    );
    const finalResult = searchExpirationDate.filter((c) =>
      c.vendors.toString().toLowerCase().includes(searchParams.searchVendor),
    );

    setFilteredInventory(finalResult);
  };

  const onLoadCustomerLocations = async () => {
    try {
      const customerLocationsFromApi = await getCustomerLocationsById(
        currentUser.Claim_CustomerId,
      );

      setCustomerLocations(customerLocationsFromApi);
      return true;
    } catch (error) {
      return error;
    }
  };

  const onLoadCustomerInventory = async () => {
    try {
      const customerInventoryFromApi = await getCustomerInventoryByCustomerId({
        customerId: currentUser.Claim_CustomerId,
      });

      setCustomerInventory(customerInventoryFromApi);
      setFilteredInventory(customerInventoryFromApi);

      return true;
    } catch (error) {
      return error;
    }
  };

  useEffect(() => {
    updateLoading(true);
    setDataSelected({
      customerId: '',
      sku: '',
      description: '',
      uom: '',
    });
    setCustomerInventory([]);
    setFilteredInventory([]);
    setCustomerLocations([]);
    onLoadCustomerLocations();
    onLoadCustomerInventory();
  }, [loading, updateData]);

  const handleDeleteItem = (rowData) => {
    setDataSelected(rowData);
    onOpenConfirmDeleteDialog();
  };

  const handleEditExpirationDate = (rowData) => {
    setDataSelected(rowData);
    onOpenEditExpirationDateModal();
  };

  const handleEditThreshold = (rowData) => {
    setDataSelected(rowData);
    onOpenEditThresholdModal();
  };

  type InventoryRow = typeof customerInventory[number];

  const inventoryColumns: GridColumns<InventoryRow> = [
    {
      field: 'actions',
      type: 'actions',
      hide: isInventoryViewer,
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="Delete Item"
          onClick={() => handleDeleteItem(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Edit Expiration Date"
          onClick={() => handleEditExpirationDate(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Edit Threshold"
          onClick={() => handleEditThreshold(params.row)}
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
                filterInventory();
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
              pathname: isInventoryViewer
                ? `/customer-inventory/item/${params.row.itemSKU}`
                : `/inventory/item/${params.row.itemSKU}`,
              search: `?${createSearchParams(querySearchParams)}`,
            });
          }}
        >
          {params.row.itemSKU}
        </Typography>
      ),
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
                filterInventory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.uom}</Typography>,
    },
    {
      field: 'binLocations',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Bin Locations</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            /*             value={searchParams.searchCustomerLocationId} */
            onChange={(value) => inputHandler('searchBinLocations', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterInventory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{params.row.binLocations}</Typography>
      ),
    },
    {
      field: 'qtyOnHand',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Qty On Hand</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchQty}
            onChange={(value) => inputHandler('searchQty', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterInventory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.qtyOnHand}</Typography>,
    },
    {
      field: 'description',
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
                filterInventory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.description}</Typography>,
    },
    {
      field: 'vendors',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Vendor</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchVendor}
            onChange={(value) => inputHandler('searchVendor', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterInventory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.vendors}
        </Typography>
      ),
    },
    {
      field: 'expiratioDate',
      width: 160,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Expiration Date</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchExpirationDate}
            onChange={(value) => inputHandler('searchExpirationDate', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterInventory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>
          {params.row.expirationDate
            ? moment(params.row.expirationDate).format('MM/DD/YY')
            : 'N/A'}
        </Typography>
      ),
    },
    {
      field: 'threshold',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Threshold</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchThreshold}
            onChange={(value) => inputHandler('searchThreshold', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterInventory();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.threshold}
        </Typography>
      ),
    },
  ];

  return (
    <MainContainer>
      <Header />
      <EditItemThresholdModal item={dataSelected} />
      <EditExpirationDateModal item={dataSelected} />
      <CustomerItemModal item={dataSelected} />
      <ConfirmationDialog itemData={dataSelected} deleteInventoryItem />
      <ContentContainer>
        <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Grid
            autoHeight
            headerHeight={120}
            rows={filteredInventory}
            columns={inventoryColumns}
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
              const selectedRowData = customerInventory.filter(
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
            components={{ Toolbar: CustomGridToolbar }}
            componentsProps={{
              toolbar: {
                title: 'Inventory',
              },
            }}
          />
        </Card>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(Inventory);
