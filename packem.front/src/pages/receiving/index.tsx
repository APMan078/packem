import React, {
  useState,
  useEffect,
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
import { Grid, PrintDivider } from 'components/styles';
import Ticket from 'components/ticket';
import { snackActions } from 'config/snackbar.js';
import PurchaseOrderStatus from 'helpers/purchaseorderstatushelper';
import moment from 'moment';
import FileInputModal from 'pages/shared/addfilemodal';
import ManualReceiptModal from 'pages/shared/manualreceiptmodal';
import PurchaseOrderModal from 'pages/shared/purchaseordermodal';
import {
  getPurchaseOrdersByCustomerId,
  removePurchaseOrder,
  addImportedPurchaseOrders,
  getPurchaseOrder,
} from 'services/api/purchaseOrders/purchaseOrders.api';
import { getReceiptQueue } from 'services/api/receipts/receipts.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Search } from '@mui/icons-material';
import { Typography } from '@mui/material';
import Box from '@mui/material/Box';
import { useTheme } from '@mui/material/styles';
import {
  GridToolbar,
  GridActionsCellItem,
  GridColumns,
} from '@mui/x-data-grid';

import { MainContainer, ContentContainer } from '../styles';

type ImportedPO = {
  customerId: number;
  customerLocationId: number;
  customerFacilityId: number;
  purchaseOrderNo: string;
  shipVia: string;
  vendorName: string;
  orderDate: Date;
};

function Receipts() {
  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  const { updateData, handleUpdateData } = useContext(GlobalContext);
  const [purchaseOrders, setPurchaseOrders] = useState([]);
  const [receiptsQueue, setReceiptsQueue] = useState([]);
  const [totalPO, setTotalPO] = useState(0);
  const [initiatePrint, setInitiatePrint] = useState(false);
  const [poToPrint, setPOToPrint] = useState({});
  const [expectedUnits, setExpectedUnits] = useState(0);
  const [receiptsToday, setReceiptsToday] = useState(0);
  const [filteredPurchaseOrders, setFilteredPurchaseOrders] = useState([]);
  const [filteredReceiptQueue, setFilteredReceiptQueue] = useState([]);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const theme = useTheme();
  const ticketRef = useRef(null);
  const print = useReactToPrint({
    content: () => ticketRef.current,
  });
  const initialPurchaseOrdersSearchState = {
    searchStatus: '',
    searchPoNo: '',
    searchOrderDate: '',
    searchOrderQty: '',
    searchRemaining: '',
    searchLastUpdated: '',
    searchShipVia: '',
    searchVendorName: '',
    searchVendorCity: '',
    searchVendorNo: '',
  };

  const initialReceiptQueueSearchState = {
    searchItemDescription: '',
    searchItemSKU: '',
    searchItemUOM: '',
    searchItemQty: '',
    searchReceiptDate: '',
    searchRemaining: '',
  };
  const [searchPurchaseOrderParams, setPurchaseOrderSearchParams] = useState(
    initialPurchaseOrdersSearchState,
  );
  const [searchReceiptQueueParams, setReceiptQueueSearchParams] = useState(
    initialReceiptQueueSearchState,
  );

  type Row = typeof purchaseOrders[number];

  const onPOForm = (key, text) => {
    setPurchaseOrderSearchParams(() => ({
      ...searchPurchaseOrderParams,
      [key]: text,
    }));
  };

  const onReceiptQueueForm = (key, text) => {
    setReceiptQueueSearchParams(() => ({
      ...searchReceiptQueueParams,
      [key]: text,
    }));
  };

  const poInputHandler = (
    key: string,
    event: ChangeEvent<HTMLInputElement>,
  ) => {
    onPOForm(key, event.target.value);
  };

  const receiptQueueInputHandler = (
    key: string,
    event: ChangeEvent<HTMLInputElement>,
  ) => {
    onReceiptQueueForm(key, event.target.value);
  };

  const filterPurchaseOrders = () => {
    const statusSearchResults = purchaseOrders.filter((u) =>
      PurchaseOrderStatus[u.status]
        .toString()
        .toLowerCase()
        .includes(searchPurchaseOrderParams.searchStatus.toLowerCase()),
    );

    const poNoSearch = statusSearchResults.filter((u) =>
      u.purchaseOrderNo
        .toString()
        .toLowerCase()
        .includes(searchPurchaseOrderParams.searchPoNo.toLowerCase()),
    );

    const orderDateSearch = poNoSearch.filter((u) =>
      moment(u.orderDate)
        .format('MM/DD/YY')
        .includes(searchPurchaseOrderParams.searchOrderDate.toLowerCase()),
    );

    const orderQtySearch = orderDateSearch.filter((u) =>
      u.orderQty
        .toString()
        .toLowerCase()
        .includes(searchPurchaseOrderParams.searchOrderQty.toLowerCase()),
    );

    const remainingSearch = orderQtySearch.filter((u) =>
      u.remaining
        .toString()
        .toLowerCase()
        .includes(searchPurchaseOrderParams.searchRemaining.toLowerCase()),
    );

    const lastUpdatedSearch = remainingSearch.filter((u) =>
      moment(u.lastUpdated)
        .format('MM/DD/YY')
        .includes(searchPurchaseOrderParams.searchLastUpdated),
    );

    const vendorNameSearch = lastUpdatedSearch.filter((u) =>
      u.vendorName
        .toString()
        .toLowerCase()
        .includes(searchPurchaseOrderParams.searchVendorName.toLowerCase()),
    );

    const vendorCitySearch = vendorNameSearch.filter((u) =>
      u.vendorCity
        .toString()
        .toLowerCase()
        .includes(searchPurchaseOrderParams.searchVendorCity.toLowerCase()),
    );

    const vendorNoSearch = vendorCitySearch.filter((u) =>
      u.vendorNo
        .toString()
        .toLowerCase()
        .includes(searchPurchaseOrderParams.searchVendorNo.toLowerCase()),
    );

    const shipViaSearch = vendorNoSearch.filter((u) =>
      u.shipVia
        .toString()
        .toLowerCase()
        .includes(searchPurchaseOrderParams.searchShipVia.toLowerCase()),
    );

    const finalResult = shipViaSearch;

    setFilteredPurchaseOrders(finalResult);
  };

  const filterReceiptsQueue = () => {
    const finalResult = receiptsQueue
      .filter((u) =>
        u.itemDescription
          .toString()
          .toLowerCase()
          .includes(
            searchReceiptQueueParams.searchItemDescription.toLowerCase(),
          ),
      )
      .filter((u) =>
        u.itemSKU
          .toString()
          .toLowerCase()
          .includes(searchReceiptQueueParams.searchItemSKU.toLowerCase()),
      )
      .filter((u) =>
        u.itemUOM
          .toString()
          .toLowerCase()
          .includes(searchReceiptQueueParams.searchItemUOM.toLowerCase()),
      )
      .filter((u) =>
        u.qty
          .toString()
          .toLowerCase()
          .includes(searchReceiptQueueParams.searchItemQty.toLowerCase()),
      )
      .filter((u) =>
        moment(u.receiptDate)
          .format('MM/DD/YY')
          .includes(searchReceiptQueueParams.searchReceiptDate.toLowerCase()),
      )
      .filter((u) =>
        u.remaining
          .toString()
          .toLowerCase()
          .includes(searchReceiptQueueParams.searchRemaining.toLowerCase()),
      );

    setFilteredReceiptQueue(finalResult);
  };

  const onLoadPurchaseOrdersData = async () => {
    try {
      const purchaseOrdersFromApi = await getPurchaseOrdersByCustomerId();
      const total = purchaseOrdersFromApi.length;

      const receipts = purchaseOrdersFromApi.filter((x) => {
        const today = moment();
        return (
          x.statusUpdatedDateTime &&
          moment(x.statusUpdatedDateTime).isSame(today, 'day') &&
          PurchaseOrderStatus[x.status] === 'Receiving'
        );
      }).length;

      const totalRemaining = purchaseOrdersFromApi.reduce(
        (accumulator, currPO) => accumulator + currPO.remaining,
        0,
      );

      setPurchaseOrders(purchaseOrdersFromApi);
      setFilteredPurchaseOrders(purchaseOrdersFromApi);
      setTotalPO(total);
      setReceiptsToday(receipts);
      setExpectedUnits(totalRemaining);

      return true;
    } catch (err) {
      return err;
    }
  };

  const onLoadReceiptQueueData = async () => {
    try {
      const receiptQueueFromApi = await getReceiptQueue(
        currentLocationAndFacility.customerLocationId,
        currentLocationAndFacility.customerFacilityId,
      );

      setReceiptsQueue(receiptQueueFromApi.receipts);
      setFilteredReceiptQueue(receiptQueueFromApi.receipts);

      return true;
    } catch (err) {
      return err;
    }
  };

  const deletePO = async (purchaseOrderId, customerFacilityId) => {
    try {
      const data = {
        purchaseOrderId,
        customerFacilityId,
      };
      const newPurchaeOrder = await removePurchaseOrder(data);
      snackActions.success(`Successfully removed purchase order.`);
      handleUpdateData();
    } catch (err: any) {
      snackActions.error(`${err}`);
    }
  };

  const handlePrint = async (po) => {
    const purchaseOrderFromApi = await getPurchaseOrder(po.purchaseOrderId);
    setPOToPrint(purchaseOrderFromApi);
    setInitiatePrint(true);
  };

  const navigateToPODetails = (po) => {
    const querysearchParams = {
      purchaseOrderId: po.purchaseOrderId,
    };
    navigate({
      pathname: `/receiving/item/${po.purchaseOrderNo}`,
      search: `?${createSearchParams(querysearchParams)}`,
    });
  };

  useEffect(() => {
    setPurchaseOrders([]);
    onLoadPurchaseOrdersData();
    onLoadReceiptQueueData();
    if (initiatePrint) {
      print();
      setInitiatePrint(false);
    }
  }, [currentLocationAndFacility, updateData, initiatePrint]);

  const mapAndAddPOs = async (data) => {
    let importedPOs: ImportedPO[] = [];

    if (Array.isArray(data)) {
      importedPOs = data
        .filter((po) => po.PONumber !== '')
        .map((po) => ({
          customerId: currentUser.Claim_CustomerId,
          customerLocationId: currentLocationAndFacility.customerLocationId,
          customerFacilityId: currentLocationAndFacility.customerFacilityId,
          purchaseOrderNo: po.PONumber,
          shipVia: po.ShipVia,
          orderQty: po.OrderQty,
          orderDate: po.OrderDate,
          vendorName: po.VendorName,
          vendorAccount: po.VendorAccount,
          pointOfCOntact: po.VendorPOC,
          vendorCity: po.VendorCity,
          vendorPhone: po.VendorPhone,
          vendorAddress: po.VendorAddress,
          vendorAddress2: po.VendorAddress2,
          vendorZip: po.VendorZIP,
          vendorStateOrProvince: po.VendorStateorProvince,
          itemSku: po.ItemSKU,
          itemDescription: po.ItemDescription,
          itemUom: po.ItemUoM,
          itemOrderQty: po.ItemOrderQty,
          lotNumber: po.LotNumber,
          expirationDate: po.ExpirationDate,
        }));
    }

    try {
      await addImportedPurchaseOrders(
        currentLocationAndFacility.customerLocationId,
        importedPOs,
      );
      snackActions.success(`Successfully imported purchase orders.`);
      handleUpdateData();
    } catch (err: any) {
      setError(err);
      snackActions.error(`${err}`);
    }
  };

  const purchaseOrdercolumns: GridColumns<Row> = [
    {
      field: 'actions',
      type: 'actions',
      width: 80,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="Go to PO Details"
          onClick={() => navigateToPODetails(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Print Ticket"
          onClick={() => handlePrint(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Delete"
          onClick={() => deletePO(params.id, params.row.customerFacilityId)}
          showInMenu
        />,
      ],
    },
    {
      field: 'status',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Status</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchPurchaseOrderParams.searchStatus}
            onChange={(value) => poInputHandler('searchStatus', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPurchaseOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{PurchaseOrderStatus[params.row.status]}</Typography>
      ),
    },
    {
      field: 'purchaseOrderNo',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">PO No</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchPurchaseOrderParams.searchPoNo}
            onChange={(value) => poInputHandler('searchPoNo', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPurchaseOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography
          sx={{ cursor: 'pointer', color: [theme.palette.primary.main] }}
          onClick={() => navigateToPODetails(params.row)}
        >
          {params.row.purchaseOrderNo}
        </Typography>
      ),
    },
    {
      field: 'shipVia',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Ship Via</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchPurchaseOrderParams.searchShipVia}
            onChange={(value) => poInputHandler('searchShipVia', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPurchaseOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.shipVia}</Typography>,
    },
    {
      field: 'vendorName',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Vendor Name</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchPurchaseOrderParams.searchVendorName}
            onChange={(value) => poInputHandler('searchVendorName', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPurchaseOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.vendorName}</Typography>,
    },
    {
      field: 'vendorCity',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Vendor City</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchPurchaseOrderParams.searchVendorCity}
            onChange={(value) => poInputHandler('searchVendorCity', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPurchaseOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.vendorCity}</Typography>,
    },
    {
      field: 'vendorNo',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Vendor Acct.</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchPurchaseOrderParams.searchVendorNo}
            onChange={(value) => poInputHandler('searchVendorNo', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPurchaseOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.vendorNo}</Typography>,
    },
    {
      field: 'orderDate',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Order Date</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchPurchaseOrderParams.searchOrderDate}
            onChange={(value) => poInputHandler('searchOrderDate', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPurchaseOrders();
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
      field: 'orderQty',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Order Qty</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchPurchaseOrderParams.searchOrderQty}
            onChange={(value) => poInputHandler('searchOrderQty', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPurchaseOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.orderQty}</Typography>,
    },
    {
      field: 'remaining',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Remaining</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchPurchaseOrderParams.searchRemaining}
            onChange={(value) => poInputHandler('searchRemaining', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPurchaseOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.remaining}</Typography>,
    },
    {
      field: 'lastUpdated',
      sortable: true,
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Last Updated</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchPurchaseOrderParams.searchLastUpdated}
            onChange={(value) => poInputHandler('searchLastUpdated', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterPurchaseOrders();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>
          {moment(params.row.lastUpdated).format('MM/DD/YY')}
        </Typography>
      ),
    },
  ];

  const receiptQueuecolumns: GridColumns<Row> = [
    {
      field: 'itemDescription',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Item Description</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchReceiptQueueParams.searchItemDescription}
            onChange={(value) =>
              receiptQueueInputHandler('searchItemDescription', value)
            }
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterReceiptsQueue();
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
      field: 'itemSKU',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Item SKU</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchReceiptQueueParams.searchItemSKU}
            onChange={(value) =>
              receiptQueueInputHandler('searchItemSKU', value)
            }
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterReceiptsQueue();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.itemSKU}</Typography>,
    },
    {
      field: 'itemUOM',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Item UOM</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchReceiptQueueParams.searchItemUOM}
            onChange={(value) =>
              receiptQueueInputHandler('searchItemUOM', value)
            }
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterReceiptsQueue();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.itemUOM}</Typography>,
    },
    {
      field: 'qty',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Item Qty</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchReceiptQueueParams.searchItemQty}
            onChange={(value) =>
              receiptQueueInputHandler('searchItemQty', value)
            }
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterReceiptsQueue();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.qty}</Typography>,
    },
    {
      field: 'receiptDate',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Receipt Date</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchReceiptQueueParams.searchReceiptDate}
            onChange={(value) =>
              receiptQueueInputHandler('searchReceiptDate', value)
            }
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterReceiptsQueue();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>
          {moment(params.row.receiptDate).format('MM/DD/YY')}
        </Typography>
      ),
    },
    {
      field: 'remaining',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '5px' }}>
          <Typography fontWeight="bold">Remaining.</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchReceiptQueueParams.searchRemaining}
            onChange={(value) =>
              receiptQueueInputHandler('searchRemaining', value)
            }
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterReceiptsQueue();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.remaining}</Typography>,
    },
  ];

  return (
    <MainContainer>
      <PurchaseOrderModal admin />
      <ManualReceiptModal />
      <FileInputModal callBack={mapAndAddPOs} />
      <Box
        sx={{
          display: 'none',
        }}
      >
        <Ticket innerRef={ticketRef} purchaseOrder data={poToPrint} />
      </Box>
      <Header />
      <ContentContainer>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            maxWidth: '608px',
            gap: '16px',
            marginBottom: '16px',
          }}
        >
          <Card
            sx={{ display: 'flex', flexDirection: 'column', minWidth: '192px' }}
          >
            <Typography
              variant="h3"
              sx={{ color: [theme.palette.primary.main] }}
            >
              {totalPO}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Purchase Orders
            </Typography>
          </Card>
          <Card
            sx={{ display: 'flex', flexDirection: 'column', minWidth: '192px' }}
          >
            <Typography
              variant="h3"
              sx={{ color: [theme.palette.primary.main] }}
            >
              {expectedUnits}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Expected Units
            </Typography>
          </Card>
          <Card
            sx={{ display: 'flex', flexDirection: 'column', minWidth: '192px' }}
          >
            <Typography
              variant="h3"
              sx={{ color: [theme.palette.primary.main] }}
            >
              {receiptsToday}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Receipts Today
            </Typography>
          </Card>
        </Box>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
          <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredPurchaseOrders}
              density="compact"
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              columns={purchaseOrdercolumns}
              pageSize={6}
              rowsPerPageOptions={[6]}
              getRowId={(row) => row.purchaseOrderId}
              checkboxSelection
              components={{ Toolbar: CustomGridToolbar }}
              componentsProps={{
                toolbar: {
                  title: 'Purchase Orders',
                  printOptions: { disableToolbarButton: true },
                },
              }}
            />
          </Card>
          <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredReceiptQueue}
              density="compact"
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              columns={receiptQueuecolumns}
              pageSize={6}
              rowsPerPageOptions={[6]}
              getRowId={(row) => row.receiptId}
              checkboxSelection
              components={{ Toolbar: CustomGridToolbar }}
              componentsProps={{
                toolbar: {
                  title: 'Receipts Queue',
                  printOptions: { disableToolbarButton: true },
                },
              }}
            />
          </Card>
        </Box>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(Receipts);
