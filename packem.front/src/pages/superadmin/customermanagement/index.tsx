import React, {
  useEffect,
  useState,
  useContext,
  ChangeEvent,
  useRef,
} from 'react';
import { useNavigate } from 'react-router-dom';

import GridMenu from 'components/button/gridbutton/gridmenu';
import Card from 'components/card';
import Header from 'components/header';
import Input from 'components/input';
import { Grid } from 'components/styles';
import CustomerLocationModal from 'pages/shared/customerlocationmodal';
import CustomerModal from 'pages/shared/customermodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import { getCustomers } from 'services/api/customer/customer.api';
import { GlobalContext } from 'store/contexts/GlobalContext';

import MoreVertIcon from '@mui/icons-material/MoreVert';
import { Typography, IconButton, Box } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import {
  GridToolbar,
  GridColumns,
  GridActionsCellItem,
} from '@mui/x-data-grid';

function ClientManagement() {
  const theme = useTheme();
  const gridMenu = useRef();
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = useState(null);
  const [openMenu, setOpenMenu] = useState(false);
  const {
    loading,
    updateLoading,
    updateData,
    onOpenCustomerLocationModal,
    onOpenCustomerModal,
  } = useContext(GlobalContext);
  const [customers, setCustomers] = useState([]);
  const [editCustomer, setEditCustomer] = useState(false);
  const [filteredCustomers, setFilteredCustomers] = useState([]);
  const [dataSelected, setDataSelected] = useState({
    userId: '',
    customerLocationId: '',
    customerId: '',
    name: '',
  });
  const [searchParams, setSearchParams] = useState({
    searchCustomerId: '',
    searchName: '',
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

  const handleClickMenu = () => {
    if (!anchorEl) setAnchorEl(gridMenu.current);
    setOpenMenu(!openMenu);
  };

  const onMenuClose = () => {
    setAnchorEl(null);
    setOpenMenu(!openMenu);
  };

  const onLoadInformation = async () => {
    try {
      const customersFromApi = await getCustomers();
      setCustomers(customersFromApi);
      setFilteredCustomers(customersFromApi);
      updateLoading(false);
      return true;
    } catch (error) {
      return error;
    }
  };

  useEffect(() => {
    updateLoading(true);
    setCustomers([]);
    onLoadInformation();
  }, [loading, updateData]);

  const filterCustomers = () => {
    const searchCustomerIdResult = customers.filter((c) =>
      c.customerId.toString().includes(searchParams.searchCustomerId),
    );
    const finalResult = searchCustomerIdResult.filter((c) =>
      c.name.toLowerCase().includes(searchParams.searchName),
    );

    setFilteredCustomers(finalResult);
  };

  type CustomerRows = typeof customers[number];

  const handleResetDataSelected = () => {
    setDataSelected({
      userId: '',
      customerLocationId: '',
      customerId: '',
      name: '',
    });
    setEditCustomer(false);
  };

  const handleAddNewLocationClick = (rowData) => {
    setDataSelected(rowData);
    onOpenCustomerLocationModal();
  };

  const handleEditCustomerClick = (rowData) => {
    setEditCustomer(true);
    setDataSelected(rowData);
    onOpenCustomerModal();
  };

  const customerColumns: GridColumns<CustomerRows> = [
    {
      field: 'actions',
      type: 'actions',
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="Add Location"
          onClick={() => handleAddNewLocationClick(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Edit Customer"
          onClick={() => handleEditCustomerClick(params.row)}
          showInMenu
        />,
      ],
    },
    {
      field: 'customerId',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Id</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchCustomerId}
            onChange={(value) => inputHandler('searchCustomerId', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterCustomers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.customerId}</Typography>,
    },
    {
      field: 'name',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Name</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchName}
            onChange={(value) => inputHandler('searchName', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterCustomers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography
          sx={{ cursor: 'pointer', color: [theme.palette.primary.main] }}
          onClick={() =>
            navigate(`/client-management/client/${params.row.customerId}`)
          }
        >
          {params.row.name}
        </Typography>
      ),
    },
  ];

  return (
    <MainContainer>
      <CustomerLocationModal
        customer={dataSelected}
        customerManagement
        callBack={handleResetDataSelected}
      />
      {editCustomer ? (
        <CustomerModal
          edit
          customerData={dataSelected}
          callBack={handleResetDataSelected}
        />
      ) : (
        <CustomerModal />
      )}
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
            sx={{ display: 'flex', flexDirection: 'column', minWidth: '192px' }}
          >
            <Typography
              sx={{ color: [theme.palette.primary.main] }}
              variant="h3"
            >
              {customers.length}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Customers
            </Typography>
          </Card>
        </Box>
        <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography variant="h6" fontWeight="bold">
            Customers
          </Typography>
          <Grid
            autoHeight
            headerHeight={120}
            rows={filteredCustomers}
            columns={customerColumns}
            pageSize={15}
            density="compact"
            disableColumnFilter
            disableColumnSelector
            disableDensitySelector
            rowsPerPageOptions={[15]}
            getRowId={(row) => row.customerId}
            checkboxSelection
            onSelectionModelChange={(customerId) => {
              const selectedId = customerId[0];
              const selectedRowData = customers.filter(
                (c) => c.customerId === selectedId,
              );
              setDataSelected(selectedRowData[0]);
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

export default React.memo(ClientManagement);
