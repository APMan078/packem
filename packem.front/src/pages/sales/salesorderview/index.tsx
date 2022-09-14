import React, { useEffect, useState, useContext, ChangeEvent } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import moment from 'moment';
import ConfirmationModal from 'pages/shared/confirmationmodal';
import SaleOrderItemModal from 'pages/shared/salesorderitemmodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import {
  getSaleOrderById,
  removeOrderLineFromOrder,
  editOrderLineItem,
} from 'services/api/salesorders/salesorders.api';
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

function SalesOrderView() {
  const theme = useTheme();
  const navigate = useNavigate();
  const {
    loading,
    updateLoading,
    updateData,
    onOpenConfirmationDialog,
    onOpenConfirmationModal,
    onOpenAddOrderLineItemModal,
    handleUpdateData,
  } = useContext(GlobalContext);
  const [urlSearchParams] = useSearchParams();
  const [saleOrderDetails, setSaleOrderDetails] = useState([]);
  const [saleOrderLines, setSaleOrderLines] = useState([]);
  const [filteredLines, setFilteredLines] = useState([]);
  const [editOrderItem, setEditOrderItem] = useState(false);
  const [salePrice, setSalePrice] = useState(0);
  const [shippingAndBillingData, setShippingAndBillingData] = useState<any>({});
  const [dataSelected, setDataSelected] = useState<any>({});
  const [searchParams, setSearchParams] = useState<any>({});

  const formatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',

    // These options are needed to round to whole numbers if that's what you want.
    // minimumFractionDigits: 0, // (this suffices for whole numbers, but will print 2500.10 as $2,500.1)
    // maximumFractionDigits: 0, // (causes 2500.99 to be printed as $2,501)
  });

  const handleRemoveOrderLineItem = async () => {
    try {
      const removeResponse = await removeOrderLineFromOrder({
        orderLineId: dataSelected.orderLineId,
      });
      snackActions.success('Successfully removed line item.');
      handleUpdateData();
    } catch (err: any) {
      snackActions.error('Unable to remove line item, please try again.');
    }
  };

  const handleResetDataSelected = () => {
    setDataSelected({});
    setEditOrderItem(false);
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

  const filterSalesOrders = () => true;

  type OrderDetailsRows = typeof saleOrderDetails[number];

  const salesOrderColumns: GridColumns<OrderDetailsRows> = [
    {
      field: 'orderNo',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Order No.</Typography>
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.orderNo}
        </Typography>
      ),
    },
    {
      field: 'orderDate',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Order Date</Typography>
        </Box>
      ),
      renderCell: (params) => (
        <Typography>
          {moment(params.row.orderDate).format('MM/DD/YY')}
        </Typography>
      ),
    },
    {
      field: 'promisedDate',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Promised Date</Typography>
        </Box>
      ),
      renderCell: (params) => (
        <Typography>
          {moment(params.row.promiseDate).format('MM/DD/YY')}
        </Typography>
      ),
    },
    {
      field: 'customerName',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Customer</Typography>
        </Box>
      ),
      renderCell: (params) => (
        <Typography
          sx={{ cursor: 'pointer', color: [theme.palette.primary.main] }}
          onClick={() => navigate(`/customers/${params.row.orderCustomerId}`)}
        >
          {params.row.customerName}
        </Typography>
      ),
    },
    {
      field: 'qtyOrdered',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Qty Ordered</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.qtyOrdered}</Typography>,
    },
    {
      field: 'shipToCity',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Ship to City</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.shipToCity}</Typography>,
    },
  ];

  type OrderLines = typeof saleOrderLines[number];

  const handleEditOrderLineItem = (rowData) => {
    setDataSelected(rowData);
    setEditOrderItem(rowData);
    onOpenAddOrderLineItemModal();
    return true;
  };

  const handleClickRemoveOrderLineItem = (rowData) => {
    setDataSelected(rowData);
    onOpenConfirmationModal();
    return true;
  };

  const orderLineColumn: GridColumns<OrderLines> = [
    {
      field: 'actions',
      type: 'actions',
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="Change Qty"
          onClick={() => handleEditOrderLineItem(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Remove From Order"
          onClick={() => handleClickRemoveOrderLineItem(params.row)}
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
            value={searchParams.searchVendor}
            onChange={(value) => inputHandler('searchVendor', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterSalesOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.itemSKU}
        </Typography>
      ),
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
            value={searchParams.searchVendor}
            onChange={(value) => inputHandler('searchVendor', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterSalesOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.description}</Typography>,
    },
    {
      field: 'UoM',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">UoM</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchVendor}
            onChange={(value) => inputHandler('searchVendor', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterSalesOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.uom}</Typography>,
    },
    {
      field: 'orderQty',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Order Qty</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchVendor}
            onChange={(value) => inputHandler('searchVendor', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterSalesOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.orderQty}</Typography>,
    },
    {
      field: 'perUnitPrice',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Per Unit Price</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchVendor}
            onChange={(value) => inputHandler('searchVendor', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterSalesOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {formatter.format(params.row.perUnitPrice)}
        </Typography>
      ),
    },
    {
      field: 'zone',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Zone</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchVendor}
            onChange={(value) => inputHandler('searchVendor', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterSalesOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.zone}</Typography>,
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
            value={searchParams.searchVendor}
            onChange={(value) => inputHandler('searchVendor', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterSalesOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.binLocations}
        </Typography>
      ),
    },
  ];

  const onLoadSaleOrderDetails = async () => {
    try {
      const saleOrderFromApi = new Array<any>(1);
      const saleOrderDetailsForState = new Array<any>(1);

      saleOrderFromApi[0] = await getSaleOrderById(
        urlSearchParams.get(`salesOrderId`),
      );

      const { orderDetails } = saleOrderFromApi[0];
      const { orderLines } = saleOrderFromApi[0];
      saleOrderDetailsForState[0] = orderDetails;

      setSaleOrderDetails(saleOrderDetailsForState);
      setSalePrice(orderDetails.totalSalePrice);
      setSaleOrderLines(orderLines);
      setFilteredLines(orderLines);

      setShippingAndBillingData({
        shipToAddress1: orderDetails.shipToAddress1,
        shipToCity: orderDetails.shipToCity,
        shipToStateProvince: orderDetails.shipToStateProvince,
        shipToZipPostalCode: orderDetails.shipToZipPostalCode,
        shipToCountry: orderDetails.shipToCountry,
        shipToPhoneNumber: orderDetails.shipToPhoneNumber,
        billToAddress1: orderDetails.billToAddress1,
        billToCity: orderDetails.billToCity,
        billToStateProvince: orderDetails.billToStateProvince,
        billToZipPostalCode: orderDetails.billToZipPostalCode,
        billToCountry: orderDetails.billToCountry,
        billToPhoneNumber: orderDetails.billToPhoneNumber,
      });

      return true;
    } catch (error) {
      return error;
    }
  };

  useEffect(() => {
    setFilteredLines([]);
    setSaleOrderLines([]);
    setSaleOrderDetails([]);
    onLoadSaleOrderDetails();
  }, [updateData]);

  return (
    <MainContainer>
      <Header />
      {editOrderItem ? (
        <SaleOrderItemModal
          edit
          orderLineItem={dataSelected}
          salesOrder={{ saleOrderId: urlSearchParams.get(`salesOrderId`) }}
          callBack={handleResetDataSelected}
        />
      ) : (
        <SaleOrderItemModal
          edit={false}
          salesOrder={{ saleOrderId: urlSearchParams.get(`salesOrderId`) }}
          callBack={handleResetDataSelected}
        />
      )}
      <ConfirmationModal
        dialogText="Are you sure you want to remove this order item?"
        callBack={handleRemoveOrderLineItem}
      />
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
              {salePrice > 0
                ? formatter.format(salePrice)
                : formatter.format(0)}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Sale Total
            </Typography>
          </Card>
          <Card
            sx={{
              display: 'flex',
              flexDirection: 'column',
              minWidth: '192px',
            }}
          >
            <Typography variant="subtitle1" fontWeight="bold">
              Ship To:
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              {shippingAndBillingData.shipToAddress1},{' '}
              {shippingAndBillingData.shipToCity}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              {shippingAndBillingData.shipToStateProvince},{' '}
              {shippingAndBillingData.shipToCountry}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              {shippingAndBillingData.shipToZipPostalCode}
            </Typography>
          </Card>
          <Card
            sx={{
              display: 'flex',
              flexDirection: 'column',
              minWidth: '192px',
            }}
          >
            <Typography variant="subtitle1" fontWeight="bold">
              Bill To:
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              {shippingAndBillingData.billToAddress1},{' '}
              {shippingAndBillingData.billToCity}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              {shippingAndBillingData.billToStateProvince},{' '}
              {shippingAndBillingData.billToCountry}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              {shippingAndBillingData.billToZipPostalCode}
            </Typography>
          </Card>
        </Box>
        <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Grid
            autoHeight
            rows={saleOrderDetails}
            columns={salesOrderColumns}
            pageSize={1}
            density="compact"
            disableColumnFilter
            disableColumnSelector
            disableDensitySelector
            disableColumnMenu
            rowsPerPageOptions={[1]}
            getRowId={(row) => row.saleOrderId}
            components={{ Toolbar: GridToolbar }}
            componentsProps={{
              toolbar: {
                title: 'Order Details',
              },
            }}
          />
        </Card>
        <Card
          sx={{
            display: 'flex',
            flexDirection: 'column',
            gap: '8px',
            marginTop: '16px',
          }}
        >
          <Grid
            autoHeight
            headerHeight={120}
            rows={filteredLines}
            columns={orderLineColumn}
            pageSize={5}
            density="compact"
            disableColumnFilter
            disableColumnSelector
            disableDensitySelector
            disableColumnMenu
            rowsPerPageOptions={[5]}
            getRowId={(row) => row.orderLineId}
            checkboxSelection
            onSelectionModelChange={(orderLineId) => {
              const selectedId = orderLineId[0];
              const selectedRowData = saleOrderLines.filter(
                (c) => c.orderLineId === selectedId,
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
                title: 'Order Lines',
              },
            }}
          />
        </Card>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(SalesOrderView);
