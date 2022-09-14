import React, {
  useEffect,
  useState,
  useContext,
  ChangeEvent,
  useRef,
} from 'react';
import { useNavigate, createSearchParams } from 'react-router-dom';
import { useReactToPrint } from 'react-to-print';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid, PrintContainer } from 'components/styles';
import Ticket from 'components/ticket';
import { snackActions } from 'config/snackbar.js';
import moment from 'moment';
import FileInputModal from 'pages/shared/addfilemodal';
import ManualSalesOrderModal from 'pages/shared/manualsalesordermodal';
import SalesOrderItemModal from 'pages/shared/salesorderitemmodal';
import SalesOrderPrintAndQueueModal from 'pages/shared/salesordersprintandqueuemodal';
import {
  getAllSalesOrdersByLocationAndFacility,
  printSalesOrderById,
  printMultipleSalesOrderByIds,
  addImportedSaleOrders,
  setSaleOrderStatusPrinted,
} from 'services/api/salesorders/salesorders.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Search } from '@mui/icons-material';
import SearchIcon from '@mui/icons-material/Search';
import { Typography, Box } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import {
  GridToolbar,
  GridActionsCellItem,
  GridColumns,
} from '@mui/x-data-grid';

import { MainContainer, ContentContainer } from '../styles';

type ImportedSO = {
  customerId: number;
  customerLocationId: number;
  customerFacilityId: number;
  saleOrderNo?: string;
  orderDate: string;
  promiseDate: string; // not required
  fulfilledDate?: string; // not required
  customerName: string;
  shippingAddressId?: number;
  billingAddressId?: number;
  pickingStatus?: number;
  shippingAddress1?: string;
  shippingAddress2?: string;
  shippingCity?: string;
  shippingStateProvince?: string;
  shippingZipPostalCode?: string;
  shippingCountry?: string;
  shippingPhoneNumber?: string;
  billingAddress1?: string;
  billingAddress2?: string;
  billingCity?: string;
  billingStateProvince?: string;
  billingZipPostalCode?: string;
  billingCountry?: string;
  billingPhoneNumber?: string;
};

function Sales() {
  const theme = useTheme();
  const navigate = useNavigate();
  const {
    updateData,
    onOpenSOPrintAndQueueModal,
    handleUpdateData,
    onOpenAddOrderLineItemModal,
  } = useContext(GlobalContext);
  const [initiatePrint, setInitiatePrint] = useState(false);
  const [salesOrdersToPrint, setSalesOrdersToPrint] = useState([]);

  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  const [dataSelected, setDataSelected] = useState({
    customerId: '',
    sku: '',
    description: '',
    uom: '',
  });
  const [saleOrderIdsSelected, setSaleOrderIdsSelected] = useState([]);
  const [searchParams, setSearchParams] = useState({
    searchStatus: '',
    searchOrderNo: '',
    searchOrderDate: '',
    searchPromiseDate: '',
    searchCustomerName: '',
    searchItemSKU: '',
    searchQtyOrdered: '',
    searchQtyOnHand: '',
  });
  const [pendingOrders, setPendingOrders] = useState(0);
  const [inPicking, setInPicking] = useState(0);
  const [error, setError] = useState('');
  const [filteredSalesOrders, setFilteredSalesOrders] = useState([]);
  const [salesOrders, setSalesOrders] = useState<any>([]);
  const ticketRef = useRef(null);
  const print = useReactToPrint({
    content: () => ticketRef.current,
    onAfterPrint() {
      salesOrdersToPrint.forEach(async (so) => {
        try {
          await setSaleOrderStatusPrinted(so.orderDetail.saleOrderId);
          handleUpdateData();
          return true;
        } catch (err) {
          snackActions.error(`${err}`);
        }
        return true;
      });
    },
  });
  const handlePrintTickets = async (saleOrderIds) => {
    const salesOrdersFromApi = await printMultipleSalesOrderByIds(saleOrderIds);
    setSalesOrdersToPrint(salesOrdersFromApi);
    setInitiatePrint(true);
  };

  const filterSalesOrders = () => {
    const searchStatusResult = salesOrders.filter((so) =>
      so.status.toLowerCase().includes(searchParams.searchStatus),
    );
    const orderNoSearchResult = searchStatusResult.filter((so) =>
      so.orderNo.toLowerCase().includes(searchParams.searchOrderNo),
    );
    const orderDateSearchResult = orderNoSearchResult.filter((so) =>
      moment(so.orderDate)
        .format('MM/DD/YY')
        .includes(searchParams.searchOrderDate.toLowerCase()),
    );
    const promiseDateSearchResult = orderDateSearchResult.filter((so) =>
      moment(so.promiseDate)
        .format('MM/DD/YY')
        .includes(searchParams.searchPromiseDate.toLowerCase()),
    );
    const customerNameSearchResult = promiseDateSearchResult.filter((so) =>
      so.customerName.includes(searchParams.searchCustomerName.toLowerCase()),
    );
    const qtyOrderedSearchResult = customerNameSearchResult.filter((so) =>
      so.qtyOrdered.toString().includes(searchParams.searchQtyOrdered),
    );

    setFilteredSalesOrders(qtyOrderedSearchResult);

    return true;
  };

  const mapAndAddSOs = async (data) => {
    let importedSOs: ImportedSO[] = [];

    if (Array.isArray(data)) {
      importedSOs = data
        .filter((so) => so.SONumber !== '')
        .map((so) => ({
          customerId: currentUser.Claim_CustomerId,
          customerLocationId: currentLocationAndFacility.customerLocationId,
          customerFacilityId: currentLocationAndFacility.customerFacilityId,
          saleOrderNo: so.SONumber,
          promiseDate: so.PromiseDate,
          orderDate: so.OrderDate,
          customerName: so.CustomerName,
          pickingStatus: so.PickingStatus,
          shippingAddress1: so.ShippingAddress1,
          shippingAddress2: so.ShippingAddress2,
          shippingCity: so.ShippingCity,
          shippingStateProvince: so.ShippingStateProvince,
          shippingZipPostalCode: so.ShippingZipPostalCode,
          shippingCountry: so.ShippingCountry,
          shippingPhoneNumber: so.ShippingPhoneNumber,
          billingAddress1: so.BillingAddress1,
          billingAddress2: so.BillingAddress2,
          billingCity: so.BillingCity,
          billingStateProvince: so.BillingStateProvince,
          billingZipPostalCode: so.BillingZipPostalCode,
          billingCountry: so.BillingCountry,
          billingPhoneNumber: so.BillingPhoneNumber,
        }));
    }

    try {
      await addImportedSaleOrders(
        currentLocationAndFacility.customerLocationId,
        importedSOs,
      );
      snackActions.success(`Successfully imported sale orders.`);
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

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) => {
    onForm(key, event.target.value);
  };

  type SalesOrderRows = typeof salesOrders[number];

  const handleViewOrderClick = (rowData) => {
    const querySearchParams = {
      salesOrderId: rowData.saleOrderId,
    };
    navigate({
      pathname: `/sales/order/${rowData.orderNo}`,
      search: `?${createSearchParams(querySearchParams)}`,
    });
    return true;
  };

  const handleResetDataSelected = () => {
    setDataSelected({
      customerId: '',
      sku: '',
      description: '',
      uom: '',
    });
  };

  const handleArchiveSalesOrderClick = (rowData) => {
    setDataSelected(rowData);
    onOpenSOPrintAndQueueModal();
    return true;
  };

  const handleAddOrderLine = async (rowData) => {
    setDataSelected(rowData);
    onOpenAddOrderLineItemModal();
  };

  const onLoadCustomerSalesOrders = async () => {
    try {
      const salesOrdersFromApi = await getAllSalesOrdersByLocationAndFacility(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
      );

      setPendingOrders(salesOrdersFromApi.pendingOrders);
      setInPicking(salesOrdersFromApi.inPicking);
      setSalesOrders(salesOrdersFromApi.saleOrders);
      setFilteredSalesOrders(salesOrdersFromApi.saleOrders);

      return true;
    } catch (err: any) {
      return err;
    }
  };

  useEffect(() => {
    setSalesOrders([]);
    setFilteredSalesOrders([]);
    onLoadCustomerSalesOrders();

    if (initiatePrint) {
      print();
      setInitiatePrint(false);
    }
  }, [updateData, initiatePrint]);

  const salesOrderColumns: GridColumns<SalesOrderRows> = [
    {
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
          onClick={() => handlePrintTickets([params.row.saleOrderId])}
          showInMenu
        />,
        <GridActionsCellItem
          label="Archive"
          onClick={() => handleArchiveSalesOrderClick(params.row)}
          showInMenu
        />,
      ],
    },
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
      field: 'customerName',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Customer</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchCustomerName}
            onChange={(value) => inputHandler('searchCustomerName', value)}
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
            navigate(`/customers/${params.row.orderCustomerId}`);
          }}
        >
          {params.row.customerName}
        </Typography>
      ),
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
            value={searchParams.searchItemSKU}
            onChange={(value) => inputHandler('searchItemSKU', value)}
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
      field: 'qtyOrdered',
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
      renderCell: (params) => <Typography>{params.row.qtyOrdered}</Typography>,
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
            value={searchParams.searchQtyOnHand}
            onChange={(value) => inputHandler('searchQtyOnHand', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterSalesOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.qtyOnHand}</Typography>,
    },
  ];

  return (
    <MainContainer>
      <Box
        sx={{
          display: 'none',
        }}
      >
        <PrintContainer ref={ticketRef} sx={{ display: 'block' }}>
          <>
            {salesOrdersToPrint.map((so) => (
              <>
                <Ticket key={so.saleOrderId} salesOrder data={so} />
                <div className="page-break" />
              </>
            ))}
          </>
        </PrintContainer>
      </Box>
      <Header
        printSOTicketsCallBack={() => handlePrintTickets(saleOrderIdsSelected)}
      />
      <ManualSalesOrderModal />
      <SalesOrderItemModal
        salesOrder={dataSelected}
        callBack={handleResetDataSelected}
      />
      <FileInputModal callBack={mapAndAddSOs} />
      <SalesOrderPrintAndQueueModal salesOrderData={dataSelected} />
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
              {pendingOrders}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Pending Orders
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
              {inPicking}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              In Picking
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
              {0}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Items Low on Stock
            </Typography>
          </Card>
        </Box>
        <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Grid
            autoHeight
            headerHeight={120}
            rows={filteredSalesOrders}
            columns={salesOrderColumns}
            pageSize={15}
            density="compact"
            disableColumnFilter
            disableColumnSelector
            disableDensitySelector
            disableColumnMenu
            rowsPerPageOptions={[15]}
            getRowId={(row) => row.saleOrderId}
            checkboxSelection
            onSelectionModelChange={(saleOrderId) => {
              const selectedId = saleOrderId[0];
              const selectedRowData = salesOrders.filter(
                (c) => c.salesOrderId === selectedId,
              );
              if (selectedId === undefined) {
                setDataSelected({
                  customerId: '',
                  sku: '',
                  description: '',
                  uom: '',
                });
              } else {
                // setDataSelected(saleOrderId);
                setSaleOrderIdsSelected(saleOrderId);
              }
            }}
            components={{ Toolbar: CustomGridToolbar }}
            componentsProps={{
              toolbar: {
                title: 'Sales Orders',
              },
            }}
          />
        </Card>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(Sales);
