import React, { useEffect, useState, useContext, ChangeEvent } from 'react';
import {
  useNavigate,
  useSearchParams,
  useLocation,
  createSearchParams,
} from 'react-router-dom';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid } from 'components/styles';
import moment from 'moment';
import ConfirmDeleteModal from 'pages/shared/confirmdeletemodal';
import ManualSalesOrderModal from 'pages/shared/manualsalesordermodal';
import OrderCustomerAddressModal from 'pages/shared/ordercustomeraddressmodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import {
  getOrderCustomerDetails,
  getAllOrderCustomerAddresses,
  getOrderCustomerByOrderCustomerId,
} from 'services/api/ordercustomers';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Search } from '@mui/icons-material';
import AddIcon from '@mui/icons-material/Add';
import { Typography, Box, IconButton } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import {
  GridToolbar,
  GridActionsCellItem,
  GridColumns,
} from '@mui/x-data-grid';

function OrderCustomerDetails() {
  const theme = useTheme();
  const navigate = useNavigate();
  const location = useLocation();
  const {
    loading,
    updateLoading,
    updateData,
    onOpenConfirmDeleteDialog,
    onOpenOrderCustomerAddressModal,
  } = useContext(GlobalContext);
  const [urlSearchParams] = useSearchParams();
  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  const [customerDetail, setCustomerDetails] = useState<any>([]);
  const [customerItemHistory, setCustomerItemHistory] = useState([]);
  const [filteredItemHistory, setFilteredItemHistory] = useState([]);
  const [customerSaleOrderHistory, setCustomerSaleOrderHistory] = useState([]);
  const [filteredSaleOrderHistory, setFilteredSaleOrderHistory] = useState([]);
  const [customerShippingAddresses, setCustomerShippingAddresses] = useState(
    [],
  );
  const [customerBillingAddresses, setCustomerBillingAddresses] = useState([]);
  const [addShippingAddress, setAddShippingAddress] = useState(false);
  const [addBillingAddress, setAddBillingAddress] = useState(false);
  const [filteredShippingAddresses, setFilteredShippingAddresses] = useState(
    [],
  );
  const [editCustomerAddress, setEditCustomerAddress] = useState(false);
  const [filteredBillingAddresses, setFilteredBillingAddresses] = useState([]);
  const [uniqueItems, setUniqueItems] = useState(0);
  const [totalOrders, setTotalOrders] = useState(0);
  const [dataSelected, setDataSelected] = useState<any>({});
  const [searchParams, setSearchParams] = useState<any>({});

  const formatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',

    // These options are needed to round to whole numbers if that's what you want.
    // minimumFractionDigits: 0, // (this suffices for whole numbers, but will print 2500.10 as $2,500.1)
    // maximumFractionDigits: 0, // (causes 2500.99 to be printed as $2,501)
  });

  function getTotalSalePrice() {
    let sum = 0;
    customerItemHistory.forEach((saleOrder) => {
      const orderSum = saleOrder.orderQty * saleOrder.perUnitPrice;
      sum += orderSum;
    });
    return formatter.format(sum);
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

  const handleResetDataSelected = () => {
    setDataSelected({});
    setEditCustomerAddress(false);
    setAddShippingAddress(false);
    setAddBillingAddress(false);
  };

  const filterSalesOrders = () => true;

  type CustomerItemHistoryRows = typeof customerItemHistory[number];

  const handleViewOrderClick = (rowData) => {
    setDataSelected(rowData);
    return true;
  };

  const handlePrintTicketClick = (rowData) => {
    setDataSelected(rowData);
    return true;
  };

  const handleAddShippingAddress = () => {
    setAddShippingAddress(true);
    onOpenOrderCustomerAddressModal();
  };

  const handleAddBillingAddress = () => {
    setAddBillingAddress(true);
    onOpenOrderCustomerAddressModal();
  };

  const customerItemHistoryColumns: GridColumns<CustomerItemHistoryRows> = [
    {
      field: 'actions',
      type: 'actions',
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="Remove From Order"
          onClick={() => handleViewOrderClick(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Change Qty"
          onClick={() => handlePrintTicketClick(params.row)}
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

  type SalesOrderRows = typeof customerSaleOrderHistory[number];

  const salesOrderHistoryColumns: GridColumns<SalesOrderRows> = [
    /* {
      field: 'actions',
      type: 'actions',
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="View Order Details"
          onClick={() => handleViewOrderClick(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Add Order Line Item"
          onClick={() => handleAddOrderLine(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Print Pick Ticket"
          onClick={() => handlePrint(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Archive"
          onClick={() => handleArchiveSalesOrderClick(params.row)}
          showInMenu
        />,
      ],
    }, */
    {
      field: 'status',
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
                filterSalesOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.status}</Typography>,
    },
    {
      field: 'orderNo',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Order No.</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchOrderNo}
            onChange={(value) => inputHandler('searchOrderNo', value)}
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
        <Typography
          sx={{ cursor: 'pointer', color: [theme.palette.primary.main] }}
          onClick={() => {
            const querySearchParams = {
              salesOrderId: params.row.saleOrderId,
            };
            navigate({
              pathname: `/sales/order/${params.row.orderNo}`,
              search: `?${createSearchParams(querySearchParams)}`,
            });
          }}
        >
          {params.row.orderNo}
        </Typography>
      ),
    },
    {
      field: 'orderDate',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Order Date</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchOrderDate}
            onChange={(value) => inputHandler('searchOrderDate', value)}
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
        <Typography>
          {moment(params.row.orderDate).format('MM/DD/YY')}
        </Typography>
      ),
    },
    {
      field: 'promisedDate',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Promised Date</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchPromiseDate}
            onChange={(value) => inputHandler('searchPromiseDate', value)}
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
        <Typography>
          {moment(params.row.promiseDate).format('MM/DD/YY')}
        </Typography>
      ),
    },
    {
      field: 'orderQty',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Qty Ordered</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchQtyOrdered}
            onChange={(value) => inputHandler('searchQtyOrdered', value)}
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
  ];

  type AddressRows = typeof customerShippingAddresses[number];

  const handleEditAddressClick = (rowData) => {
    setDataSelected(rowData);
    setEditCustomerAddress(true);
    onOpenOrderCustomerAddressModal();
    return true;
  };

  const handleDeleteAddressClick = (rowData) => {
    setDataSelected(rowData);
    onOpenConfirmDeleteDialog();
    return true;
  };

  const shippingBillingAddressColumns: GridColumns<AddressRows> = [
    {
      field: 'actions',
      type: 'actions',
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="Edit"
          onClick={() => handleEditAddressClick(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Delete"
          onClick={() => handleDeleteAddressClick(params.row)}
          showInMenu
        />,
      ],
    },
    {
      field: 'address1',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Primary Address</Typography>
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
          {params.row.address1}
        </Typography>
      ),
    },
    {
      field: 'city',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">City</Typography>
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
      renderCell: (params) => <Typography>{params.row.city}</Typography>,
    },
    {
      field: 'stateProvince',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">State / Province</Typography>
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
        <Typography>{params.row.stateProvince}</Typography>
      ),
    },
    {
      field: 'zipPostalCode',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Zip Code</Typography>
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
        <Typography>{params.row.zipPostalCode}</Typography>
      ),
    },
    {
      field: 'Country',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Country</Typography>
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
      renderCell: (params) => <Typography>{params.row.country}</Typography>,
    },
    {
      field: 'phoneNumber',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Phone Number</Typography>
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
      renderCell: (params) => <Typography>{params.row.phoneNumber}</Typography>,
    },
  ];

  const onLoadCustomerDetails = async () => {
    try {
      const customerFromApi = new Array<any>(1);
      const customerDetailsFromApi = new Array<any>(1);
      const orderCustomerId = location.pathname.split('/')[2];

      customerFromApi[0] = await getOrderCustomerDetails(orderCustomerId);

      const customerOrderInformation = await getOrderCustomerByOrderCustomerId(
        orderCustomerId,
      );

      const customerShippingBillingAddressesFromApi =
        await getAllOrderCustomerAddresses(orderCustomerId);

      const shippingAddresses = customerShippingBillingAddressesFromApi.filter(
        (a) => a.addressType === 1,
      );

      const billingAddresses = customerShippingBillingAddressesFromApi.filter(
        (a) => a.addressType === 2,
      );

      const { itemOrders } = customerFromApi[0];
      const { saleOrders } = customerFromApi[0];
      const { orders } = customerFromApi[0];
      const { uniqueItemsOrdered } = customerFromApi[0];
      customerDetailsFromApi[0] = customerDetail;

      setTotalOrders(orders);
      setUniqueItems(uniqueItemsOrdered);
      setCustomerDetails(customerOrderInformation);
      setCustomerSaleOrderHistory(saleOrders);
      setFilteredSaleOrderHistory(saleOrders);
      setCustomerItemHistory(itemOrders);
      setFilteredItemHistory(itemOrders);
      setCustomerBillingAddresses(billingAddresses);
      setFilteredBillingAddresses(billingAddresses);
      setCustomerShippingAddresses(shippingAddresses);
      setFilteredShippingAddresses(shippingAddresses);

      return true;
    } catch (error) {
      return error;
    }
  };

  useEffect(() => {
    setFilteredItemHistory([]);
    setCustomerItemHistory([]);
    setCustomerSaleOrderHistory([]);
    setFilteredSaleOrderHistory([]);
    setCustomerShippingAddresses([]);
    setCustomerBillingAddresses([]);
    setFilteredShippingAddresses([]);
    setFilteredBillingAddresses([]);
    setCustomerDetails([]);
    onLoadCustomerDetails();
  }, [updateData]);

  return (
    <MainContainer>
      <Header
        orderCustomerDetails={{
          customerName: customerDetail.name,
          lastDateOrdered: customerDetail.lastOrderDate,
        }}
      />
      <ConfirmDeleteModal
        callBack={handleResetDataSelected}
        orderCustomerAddressData={dataSelected}
        orderCustomerAddressDelete
      />
      <ManualSalesOrderModal
        customerDetails={{ orderCustomerId: location.pathname.split('/')[2] }}
        fromCustomerDetails
      />
      {editCustomerAddress && (
        <OrderCustomerAddressModal
          callBack={handleResetDataSelected}
          orderCustomer={customerDetail}
          orderCustomerAddress={dataSelected}
          edit
        />
      )}
      {addShippingAddress && (
        <OrderCustomerAddressModal
          callBack={handleResetDataSelected}
          orderCustomer={customerDetail}
          shipping
        />
      )}
      {addBillingAddress && (
        <OrderCustomerAddressModal
          callBack={handleResetDataSelected}
          orderCustomer={customerDetail}
          billing
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
              {totalOrders}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Orders
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
              {uniqueItems}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Unique Items Ordered
            </Typography>
          </Card>
        </Box>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            gap: '16px',
            width: '100%',
          }}
        >
          <Card
            sx={{
              display: 'flex',
              flexDirection: 'column',
              gap: '8px',
              marginTop: '16px',
              width: '100%',
            }}
          >
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                justifyContent: 'flex-end',
                alignItems: 'center',
              }}
            >
              <IconButton onClick={() => handleAddShippingAddress()}>
                <AddIcon sx={{ color: [theme.palette.primary.main] }} />
              </IconButton>
            </Box>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredShippingAddresses}
              columns={shippingBillingAddressColumns}
              pageSize={5}
              density="compact"
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              rowsPerPageOptions={[5]}
              getRowId={(row) => row.orderCustomerAddressId}
              components={{ Toolbar: CustomGridToolbar }}
              componentsProps={{
                toolbar: {
                  title: 'Shipping Addresses',
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
              width: '100%',
            }}
          >
            <Box
              sx={{
                display: 'flex',
                flexDirection: 'row',
                justifyContent: 'flex-end',
                alignItems: 'center',
              }}
            >
              <IconButton onClick={() => handleAddBillingAddress()}>
                <AddIcon sx={{ color: [theme.palette.primary.main] }} />
              </IconButton>
            </Box>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredBillingAddresses}
              columns={shippingBillingAddressColumns}
              pageSize={5}
              density="compact"
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              rowsPerPageOptions={[5]}
              getRowId={(row) => row.orderCustomerAddressId}
              components={{ Toolbar: CustomGridToolbar }}
              componentsProps={{
                toolbar: {
                  title: 'Billing Addresses',
                },
              }}
            />
          </Card>
        </Box>
        <Card
          sx={{
            display: 'flex',
            flexDirection: 'column',
            marginTop: '16px',
            gap: '8px',
          }}
        >
          <Grid
            autoHeight
            headerHeight={120}
            rows={filteredSaleOrderHistory}
            columns={salesOrderHistoryColumns}
            pageSize={5}
            density="compact"
            disableColumnFilter
            disableColumnSelector
            disableDensitySelector
            disableColumnMenu
            rowsPerPageOptions={[5]}
            getRowId={(row) => row.saleOrderId}
            checkboxSelection
            onSelectionModelChange={(saleOrderId) => {
              const selectedId = saleOrderId[0];
              const selectedRowData = customerDetail.filter(
                (c) => c.saleOrderId === selectedId,
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
                title: 'Order History',
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
            rows={filteredItemHistory}
            columns={customerItemHistoryColumns}
            pageSize={5}
            density="compact"
            disableColumnFilter
            disableColumnSelector
            disableDensitySelector
            rowsPerPageOptions={[5]}
            getRowId={(row) => row.itemId}
            checkboxSelection
            onSelectionModelChange={(itemId) => {
              const selectedId = itemId[0];
              const selectedRowData = customerItemHistory.filter(
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
                title: 'Items Ordered',
              },
            }}
          />
        </Card>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(OrderCustomerDetails);
