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
import { Grid } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import ConfirmDeleteModal from 'pages/shared/confirmdeletemodal';
import OrderCustomerModal from 'pages/shared/ordercustomermodal';
import { getOrderCustomerManagementData } from 'services/api/ordercustomers';
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

type ImportedSO = {
  customerId: '';
  customerLocationId: '';
  customerFacilityId: '';
  saleOrderNo: '';
  orderDate: '';
  promiseDate: '';
  customerNo: '';
  shipToAddress1: '';
  shipToAddress2: '';
  shipToCity: '';
  shipToStateProvince: '';
  shipToZipPostalCode: '';
  shipToPhoneNumber: '';
};

function OrderCustomers() {
  const theme = useTheme();
  const navigate = useNavigate();
  const {
    updateData,
    handleUpdateData,
    onOpenOrderCustomerModal,
    onOpenConfirmDeleteDialog,
  } = useContext(GlobalContext);
  const [initiatePrint, setInitiatePrint] = useState(false);
  const [editOrderCustomer, setEditOrderCustomer] = useState(false);
  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  const [dataSelected, setDataSelected] = useState({
    customerId: '',
    name: '',
    itemSKU: '',
    qtyOnHand: '',
    qtyOrdered: '',
  });
  const [searchParams, setSearchParams] = useState({
    searchOrderCustomerId: '',
    searchCustomerName: '',
    searchNoShippingAddresses: '',
    searchNoBillingAddresses: '',
  });
  const [error, setError] = useState('');
  const [filteredOrderCustomers, setFilteredOrderCustomers] = useState([]);
  const [orderCustomers, setOrderCustomers] = useState<any>([]);
  const ticketRef = useRef(null);
  const print = useReactToPrint({
    content: () => ticketRef.current,
  });

  const handleResetDataSelected = () => {
    setDataSelected({
      customerId: '',
      name: '',
      itemSKU: '',
      qtyOnHand: '',
      qtyOrdered: '',
    });
    setEditOrderCustomer(false);
  };

  const filterOrderCustomers = () => {
    const searchCustomerNameResult = orderCustomers.filter((c) =>
      c.customerName
        .toLowerCase()
        .includes(searchParams.searchCustomerName.toLowerCase()),
    );

    setFilteredOrderCustomers(searchCustomerNameResult);

    return true;
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

  type OrderCustomerRows = typeof orderCustomers[number];

  const handleViewOrderCustomerDetailsClick = (rowData) => {
    navigate(`/customers/${rowData.orderCustomerId}`);
    return true;
  };

  const handleEditCustomerInformation = (rowData) => {
    setEditOrderCustomer(true);
    setDataSelected(rowData);
    onOpenOrderCustomerModal();
  };

  const handleArchiveCustomer = (rowData) => {
    setDataSelected(rowData);
    onOpenConfirmDeleteDialog();
  };

  const onLoadOrderCustomers = async () => {
    try {
      const orderCustomersFromApi = await getOrderCustomerManagementData(
        currentUser.Claim_CustomerId,
      );

      setOrderCustomers(orderCustomersFromApi);
      setFilteredOrderCustomers(orderCustomersFromApi);

      return true;
    } catch (err: any) {
      return err;
    }
  };

  useEffect(() => {
    setOrderCustomers([]);
    setFilteredOrderCustomers([]);
    onLoadOrderCustomers();

    if (initiatePrint) {
      print();
      setInitiatePrint(false);
    }
  }, [updateData, initiatePrint]);

  const orderCustomerColumns: GridColumns<OrderCustomerRows> = [
    {
      field: 'actions',
      type: 'actions',
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="View Customer Details"
          onClick={() => handleViewOrderCustomerDetailsClick(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Edit Customer"
          onClick={() => handleEditCustomerInformation(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Archive"
          onClick={() => handleArchiveCustomer(params.row)}
          showInMenu
        />,
      ],
    },
    {
      field: 'customerName',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Name</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchCustomerName}
            onChange={(value) => inputHandler('searchCustomerName', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterOrderCustomers();
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
  ];

  return (
    <MainContainer>
      <Header />
      {editOrderCustomer ? (
        <OrderCustomerModal
          edit
          orderCustomer={dataSelected}
          callBack={handleResetDataSelected}
        />
      ) : (
        <OrderCustomerModal />
      )}
      <ConfirmDeleteModal
        orderCustomerData={dataSelected}
        orderCustomerDelete
        callBack={handleResetDataSelected}
      />
      <ContentContainer>
        <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Grid
            autoHeight
            headerHeight={120}
            rows={filteredOrderCustomers}
            columns={orderCustomerColumns}
            pageSize={15}
            density="compact"
            disableColumnFilter
            disableColumnSelector
            disableDensitySelector
            disableColumnMenu
            rowsPerPageOptions={[15]}
            getRowId={(row) => row.orderCustomerId}
            checkboxSelection
            onSelectionModelChange={(orderCustomerId) => {
              const selectedId = orderCustomerId[0];
              const selectedRowData = orderCustomers.filter(
                (c) => c.orderCustomerId === selectedId,
              );
              if (selectedId === undefined) {
                setDataSelected({
                  customerId: '',
                  name: '',
                  itemSKU: '',
                  qtyOnHand: '',
                  qtyOrdered: '',
                });
              } else {
                setDataSelected(selectedRowData[0]);
              }
            }}
            components={{ Toolbar: CustomGridToolbar }}
            componentsProps={{
              toolbar: {
                title: 'Customers',
              },
            }}
          />
        </Card>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(OrderCustomers);
