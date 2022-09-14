import React, {
  useEffect,
  useState,
  useContext,
  ChangeEvent,
  useRef,
} from 'react';

import Card from 'components/card';
import Header from 'components/header';
import Input from 'components/input';
import { Grid } from 'components/styles';
import CustomerVendorModal from 'pages/shared/customervendormodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import { getCustomerVendors } from 'services/api/customer/customer.api';
import { getItemById } from 'services/api/item/item.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import { Typography, IconButton, Box } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { GridColDef, GridToolbar } from '@mui/x-data-grid';

function Vendors() {
  const theme = useTheme();
  const { onOpenVendorModal, loading, updateLoading, updateData } =
    useContext(GlobalContext);
  const { currentUser } = useContext(AuthContext);
  const [customerVendors, setCustomerVendors] = useState([]);
  const [filteredVendors, setFilteredVendors] = useState([]);
  const [dataSelected, setDataSelected] = useState({
    vendorId: '',
    customerId: '',
    account: '',
    name: '',
    contact: '',
    address: '',
    city: '',
    stateProvince: '',
    zipPostalCode: '',
    phone: '',
  });
  const [searchParams, setSearchParams] = useState({
    searchVendorNo: '',
    searchName: '',
    searchPointOfContact: '',
    searchAddress: '',
    searchPhoneNumber: '',
  });

  const filterVendors = () => {
    const vendorNoResult = customerVendors.filter((v) =>
      v.account.toString().toLowerCase().includes(searchParams.searchVendorNo),
    );
    const searchNameResult = vendorNoResult.filter((v) =>
      v.name.toLowerCase().includes(searchParams.searchName),
    );
    const searchPointOfContactResult = searchNameResult.filter((v) =>
      v.contact.toLowerCase().includes(searchParams.searchPointOfContact),
    );
    const searchAddressResult = searchPointOfContactResult.filter((v) =>
      v.address.toLowerCase().includes(searchParams.searchAddress),
    );
    const finalResult = searchAddressResult.filter((v) =>
      v.phone.toString().toLowerCase().includes(searchParams.searchPhoneNumber),
    );

    setFilteredVendors(finalResult);
  };

  const onForm = (key, text) => {
    setSearchParams(() => ({
      ...searchParams,
      [key]: text,
    }));
  };

  const onLoadCustomerVendors = async () => {
    try {
      const vendorsFromApi = await getCustomerVendors(
        currentUser.Claim_CustomerId,
      );

      setCustomerVendors(vendorsFromApi);
      setFilteredVendors(vendorsFromApi);

      return true;
    } catch (error) {
      return error;
    }
  };

  const inputHandler = (key: string, event: ChangeEvent<HTMLInputElement>) => {
    onForm(key, event.target.value);
  };

  const handleClickEdit = () => {
    onOpenVendorModal();
  };

  useEffect(() => {
    updateLoading(true);
    setCustomerVendors([]);
    setFilteredVendors([]);
    onLoadCustomerVendors();
  }, [loading, updateData]);

  const vendorColumns: GridColDef[] = [
    {
      field: 'menu',
      align: 'center',
      width: 48,
      renderHeader: () => <Box />,
      renderCell: () => (
        <Box>
          <IconButton onClick={() => handleClickEdit()}>
            <EditOutlinedIcon />
          </IconButton>
        </Box>
      ),
    },
    {
      field: 'account',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Vendor No.</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchVendorNo}
            onChange={(value) => inputHandler('searchVendorNo', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterVendors();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.account}
        </Typography>
      ),
    },
    {
      field: 'items',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Items</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchVendorNo}
            onChange={(value) => inputHandler('searchVendorNo', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterVendors();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.items}
        </Typography>
      ),
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
                filterVendors();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.name}</Typography>,
    },
    {
      field: 'contact',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Contact</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchPointOfContact}
            onChange={(value) => inputHandler('searchPointOfContact', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterVendors();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.contact}</Typography>,
    },
    {
      field: 'address',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Address</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchAddress}
            onChange={(value) => inputHandler('searchAddress', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterVendors();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.address}
        </Typography>
      ),
    },
    {
      field: 'phone',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography>Phone Number</Typography>
          <Input
            sx={{ maxWidth: '96px' }}
            variant="standard"
            placeholder="Filter"
            value={searchParams.searchPhoneNumber}
            onChange={(value) => inputHandler('searchPhoneNumber', value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterVendors();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.phone}</Typography>,
    },
  ];

  return (
    <MainContainer>
      <Header />
      <CustomerVendorModal vendor={dataSelected} />
      <ContentContainer>
        <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography variant="h6" fontWeight="bold">
            Vendors
          </Typography>
          <Grid
            autoHeight
            headerHeight={120}
            rows={filteredVendors}
            columns={vendorColumns}
            pageSize={15}
            density="compact"
            disableColumnFilter
            disableColumnSelector
            disableDensitySelector
            rowsPerPageOptions={[15]}
            getRowId={(row) => row.vendorId}
            checkboxSelection
            onSelectionModelChange={(vendorId) => {
              const selectedId = vendorId[0];
              const selectedRowData = customerVendors.filter(
                (c) => c.vendorId === selectedId,
              );
              if (selectedId === undefined) {
                setDataSelected({
                  vendorId: '',
                  customerId: '',
                  account: '',
                  name: '',
                  contact: '',
                  address: '',
                  city: '',
                  stateProvince: '',
                  zipPostalCode: '',
                  phone: '',
                });
              } else {
                setDataSelected(selectedRowData[0]);
              }
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

export default React.memo(Vendors);
