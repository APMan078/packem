import React, {
  useEffect,
  useState,
  useContext,
  ChangeEvent,
  useRef,
} from 'react';
import { useSearchParams } from 'react-router-dom';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid } from 'components/styles';
import moment from 'moment';
import AdjustPOItemModal from 'pages/shared/adjustpoitemmodal';
import ConfirmationModal from 'pages/shared/confirmationmodal';
import ManualReceiptModal from 'pages/shared/manualreceiptmodal';
import PurchaseOrderItemModal from 'pages/shared/purchaseorderitemmodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import { getPurchaseOrder } from 'services/api/purchaseOrders/purchaseOrders.api';
import { cancelReceive } from 'services/api/receive/receive.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Search } from '@mui/icons-material';
import { Typography, Box } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { GridColDef, GridActionsCellItem, GridColumns } from '@mui/x-data-grid';

function ReceiptItemView() {
  const theme = useTheme();
  const [urlSearchParams] = useSearchParams();
  const { currentUser } = useContext(AuthContext);
  const [itemDetails, setItemDetails] = useState({
    itemId: '',
    sku: '',
  });

  const {
    loading,
    updateLoading,
    handleUpdateData,
    updateData,
    onOpenManualReceiptModal,
    onOpenConfirmationModal,
    onOpenPurchaseOrderItemAdjustModal,
  } = useContext(GlobalContext);
  const [dialogText, setDialogText] = useState('');
  const [purchaseOrder, setPurchaseOrder] = useState([]);
  const [poItemForModal, setPOItemForModal] = useState([]);
  const [purchaseOrderItems, setPurchaseOrderItems] = useState([]);
  const [filteredPurchaseOrderItems, setFilteredPurchaseOrderItems] = useState(
    [],
  );

  const [searchParams, setSearchParams] = useState({
    searchItemSKU: '',
    searchDescription: '',
    searchUOM: '',
    searchOrderQty: '',
    searchReceiveQty: '',
    searchLotNo: '',
    searchExpirationDate: '',
  });

  type Row = typeof purchaseOrderItems[number];

  const onForm = (key, text) => {
    setSearchParams(() => ({
      ...searchParams,
      [key]: text,
    }));
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) => {
    onForm(key, event.target.value);
  };

  const filterItems = () => {
    const itemSKUSearchResults = purchaseOrderItems.filter((u) =>
      u.itemSKU
        .toString()
        .toLowerCase()
        .includes(searchParams.searchItemSKU.toLowerCase()),
    );

    const descriptionSearch = itemSKUSearchResults.filter((u) =>
      u.description
        .toString()
        .toLowerCase()
        .includes(searchParams.searchDescription.toLowerCase()),
    );

    const uomSearch = descriptionSearch.filter((u) =>
      u.uom
        .toString()
        .toLowerCase()
        .includes(searchParams.searchUOM.toLowerCase()),
    );

    const orderQtySearch = uomSearch.filter((u) =>
      u.orderQty
        .toString()
        .toLowerCase()
        .includes(searchParams.searchOrderQty.toLowerCase()),
    );

    const receiveQtySearch = orderQtySearch.filter((u) =>
      u.receivedQty
        .toString()
        .toLowerCase()
        .includes(searchParams.searchReceiveQty.toLowerCase()),
    );

    const lotNoSearch = receiveQtySearch.filter((u) =>
      u.lotNo
        ? u.lotNo
            .toString()
            .toLowerCase()
            .includes(searchParams.searchLotNo.toLowerCase())
        : 'N/a'.toLowerCase().includes(searchParams.searchLotNo.toLowerCase()),
    );

    const expirationDateSearch = lotNoSearch.filter((u) =>
      u.expirationDate
        ? moment(u.expirationDate)
            .format('MM/DD/YY')
            .includes(searchParams.searchExpirationDate.toLowerCase())
        : 'N/a'.toLowerCase().includes(searchParams.searchLotNo.toLowerCase()),
    );

    const finalResult = expirationDateSearch;

    setFilteredPurchaseOrderItems(finalResult);
  };

  const onLoadPurchaseOrderDetails = async () => {
    try {
      const purchaseOrderFromApi = new Array<any>(1);
      const purchaseOrderForState = new Array<any>(1);
      purchaseOrderFromApi[0] = await getPurchaseOrder(
        urlSearchParams.get('purchaseOrderId'),
      );

      const { purchaseOrderDetail } = purchaseOrderFromApi[0];
      const { items } = purchaseOrderFromApi[0];
      purchaseOrderForState[0] = purchaseOrderDetail;

      setPurchaseOrder(purchaseOrderForState);
      setPurchaseOrderItems(items);
      setFilteredPurchaseOrderItems(items);

      return true;
    } catch (err) {
      return err;
    }
  };

  useEffect(() => {
    setItemDetails({
      itemId: '',
      sku: '',
    });
    onLoadPurchaseOrderDetails();
  }, [loading, updateData]);

  const cancelItemCallBack = async () => {
    await cancelReceive(poItemForModal);
    handleUpdateData();
  };

  const handleOpenConfirmationModal = (poItem) => {
    setDialogText('Are you sure you want to cancel this PO item?');
    setPOItemForModal(poItem);
    onOpenConfirmationModal();
  };

  const handleOpenManualReceiptModal = (poItem) => {
    setPOItemForModal(poItem);
    onOpenManualReceiptModal();
  };

  const handleOpenPurchaseOrderItemAdjustModal = (poItem) => {
    setPOItemForModal(poItem);
    onOpenPurchaseOrderItemAdjustModal();
  };

  const purchaseOrdercolumns: GridColDef[] = [
    {
      field: 'status',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography fontWeight="bold">Status</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.status}</Typography>,
    },
    {
      field: 'purchaseOrderNo',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography fontWeight="bold">PO No</Typography>
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{params.row.purchaseOrderNo}</Typography>
      ),
    },
    {
      field: 'shipVia',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography fontWeight="bold">Ship Via</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.shipVia}</Typography>,
    },
    {
      field: 'vendorName',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography fontWeight="bold">Vendor Name</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.vendorName}</Typography>,
    },
    {
      field: 'vendorCity',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography fontWeight="bold">Vendor City</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.vendorCity}</Typography>,
    },
    {
      field: 'vendorAccount',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography fontWeight="bold">Vendor Acct.</Typography>
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{params.row.vendorAccount}</Typography>
      ),
    },
    {
      field: 'orderDate',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
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
      field: 'orderQty',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography fontWeight="bold">Order Qty</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.orderQty}</Typography>,
    },
    {
      field: 'remaining',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography fontWeight="bold">Remaining</Typography>
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.remaining}</Typography>,
    },
    {
      field: 'lastUpdated',
      width: 200,
      sortable: false,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography fontWeight="bold">Last Updated</Typography>
        </Box>
      ),
      renderCell: (params) => (
        <Typography>
          {moment(params.row.lastUpdated).format('MM/DD/YY')}
        </Typography>
      ),
    },
  ];

  const purchaseOrderItemsColumns: GridColumns<Row> = [
    {
      field: 'actions',
      type: 'actions',
      width: 80,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="Receipt"
          onClick={() => handleOpenManualReceiptModal(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Adjustment"
          onClick={() => handleOpenPurchaseOrderItemAdjustModal(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Cancel"
          onClick={() => handleOpenConfirmationModal(params.row)}
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
            value={searchParams.searchItemSKU}
            onChange={(value) => inputHandler('searchItemSKU', value)}
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
            value={searchParams.searchDescription}
            onChange={(value) => inputHandler('searchDescription', value)}
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
      field: 'orderQty',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Order Qty</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchOrderQty}
            onChange={(value) => inputHandler('searchOrderQty', value)}
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
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.orderQty}
        </Typography>
      ),
    },
    {
      field: 'receivedQTy',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Received Qty</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchReceiveQty}
            onChange={(value) => inputHandler('searchReceiveQty', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterItems();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.receivedQty}</Typography>,
    },
    {
      field: 'lotNo',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography fontWeight="bold">Lot Number.</Typography>
          <Input
            placeholder="Search"
            value={searchParams.searchLotNo}
            onChange={(value) => inputHandler('searchLotNo', value)}
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
        <Typography>{params.row.lotNo ? params.row.lotNo : 'N/A'}</Typography>
      ),
    },
    {
      field: 'expirationDate',
      width: 200,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography fontWeight="bold">Expiration Date</Typography>
          <Input
            placeholder="Search"
            value={searchParams.searchExpirationDate}
            onChange={(value) => inputHandler('searchExpirationDate', value)}
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
        <Typography>
          {params.row.expirationDate
            ? moment(params.row.expirationDate).format('MM/DD/YY')
            : 'N/A'}
        </Typography>
      ),
    },
  ];

  return (
    <MainContainer>
      <Header />
      <ConfirmationModal
        dialogText={dialogText}
        callBack={cancelItemCallBack}
      />
      <AdjustPOItemModal purchaseOrderItem={poItemForModal} />
      <ManualReceiptModal purchaseOrderItem={poItemForModal} />
      <PurchaseOrderItemModal purchaseOrder={purchaseOrder[0]} />
      <ContentContainer>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
          <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={purchaseOrder}
              density="compact"
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              columns={purchaseOrdercolumns}
              pageSize={1}
              rowsPerPageOptions={[1]}
              getRowId={(row) => row.purchaseOrderId}
              components={{ Toolbar: CustomGridToolbar }}
              componentsProps={{
                toolbar: {
                  title: 'Purchase Order Details',
                  printOptions: { disableToolbarButton: true },
                },
              }}
            />
          </Card>

          <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredPurchaseOrderItems}
              density="compact"
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              columns={purchaseOrderItemsColumns}
              pageSize={6}
              rowsPerPageOptions={[6]}
              getRowId={(row) => row.itemId}
              components={{ Toolbar: CustomGridToolbar }}
              componentsProps={{
                toolbar: {
                  title: 'Items',
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

export default React.memo(ReceiptItemView);
