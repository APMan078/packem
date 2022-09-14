import React, { useEffect, useState, useContext, ChangeEvent } from 'react';
import { useSearchParams } from 'react-router-dom';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid } from 'components/styles';
import moment from 'moment';
import AdjustQtyModal from 'pages/shared/adjustqtymodal';
import CustomerBinModal from 'pages/shared/customerbinmodal';
import TransferBinModal from 'pages/shared/transferbinmodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import { getItemDetails } from 'services/api/item/item.api';
import { AuthContext } from 'store/contexts/AuthContext';
import { GlobalContext } from 'store/contexts/GlobalContext';

import { Search } from '@mui/icons-material';
import { Typography, Box } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import {
  GridColDef,
  GridToolbar,
  GridActionsCellItem,
  GridColumns,
} from '@mui/x-data-grid';

function ItemView() {
  const theme = useTheme();
  const { currentUser, isInventoryViewer } = useContext(AuthContext);
  const [itemLocations, setItemLocations] = useState([]);
  const [filteredItemLoc, setFilteredItemLoc] = useState([]);
  const [itemVendors, setItemVendors] = useState([]);
  const [filteredItemVendors, setFilteredItemVendors] = useState([]);
  const [itemActivityLogs, setItemActivityLogs] = useState([]);
  const [filteredItemActivityLogs, setFilteredActivityLogs] = useState([]);
  const [urlSearchParams] = useSearchParams();

  const { loading, updateData, onOpenTransferModal, onOpenAdjustModal } =
    useContext(GlobalContext);
  const [locationDataSelected, setLocationDataSelected] = useState({
    itemSKU: '',
    binName: '',
  });
  const [vendorDataSelected, setVendorDataSelected] = useState({
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
    searchFacilityName: '',
    searchZoneName: '',
    searchBinName: '',
    searchQtyOnHand: '',
    searchName: '',
    searchVendorNo: '',
    searchPointOfContact: '',
    searchAddress: '',
    searchPhoneNumber: '',
    searchActivityType: '',
    searchActivityUser: '',
    searchActivityDate: '',
    searchActivityQty: '',
    searchActivityZone: '',
    searchActivityBinLocation: '',
    searchLotNo: '',
    searchExpirationDate: '',
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

  const onLoadItemDetails = async () => {
    try {
      const itemVendorsAndLocationsFromApi = await getItemDetails(
        currentUser.Claim_CustomerId,
        urlSearchParams.get(`itemId`),
      );

      setItemLocations(itemVendorsAndLocationsFromApi.customerLocations);
      setFilteredItemLoc(itemVendorsAndLocationsFromApi.customerLocations);
      setItemVendors(itemVendorsAndLocationsFromApi.vendors);
      setFilteredItemVendors(itemVendorsAndLocationsFromApi.vendors);
      setItemActivityLogs(itemVendorsAndLocationsFromApi.activities);
      setFilteredActivityLogs(itemVendorsAndLocationsFromApi.activities);

      return true;
    } catch (error) {
      return error;
    }
  };

  useEffect(() => {
    setItemLocations([]);
    setFilteredItemLoc([]);
    setItemVendors([]);
    setFilteredItemVendors([]);
    setItemActivityLogs([]);
    setFilteredActivityLogs([]);
    onLoadItemDetails();
  }, [loading, updateData]);

  const filterLocations = () => {
    const customerFacilityNameResult = itemLocations.filter((f) =>
      f.facility.toLowerCase().includes(searchParams.searchFacilityName),
    );
    const searchZoneNameResult = customerFacilityNameResult.filter((f) =>
      f.zone.toLowerCase().includes(searchParams.searchFacilityName),
    );
    const searchBinNameResult = searchZoneNameResult.filter((f) =>
      f.bin.toLowerCase().includes(searchParams.searchBinName),
    );
    const qtyOnHandResult = searchBinNameResult.filter((f) =>
      f.qtyOnHand
        .toString()
        .toLowerCase()
        .includes(searchParams.searchQtyOnHand),
    );
    const lotNoSearch = qtyOnHandResult.filter((u) =>
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

    setFilteredItemLoc(expirationDateSearch);
  };

  const filterVendors = () => {
    const vendorNoResult = itemVendors.filter((v) =>
      v.vendorNo.toString().toLowerCase().includes(searchParams.searchVendorNo),
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

    setFilteredItemVendors(finalResult);
  };

  const filterActivityLogs = () => {
    const searchActivityTypeResult = itemActivityLogs.filter((v) =>
      v.type.toString().toLowerCase().includes(searchParams.searchActivityType),
    );
    const searchActivityUserResult = searchActivityTypeResult.filter((v) =>
      v.user.toLowerCase().includes(searchParams.searchActivityUser),
    );
    const searchActivityZoneResult = searchActivityUserResult.filter((v) =>
      v.zone.toLowerCase().includes(searchParams.searchActivityZone),
    );
    const searchActivityQtyResult = searchActivityZoneResult.filter((v) =>
      v.qty.toString().toLowerCase().includes(searchParams.searchActivityQty),
    );
    const searchActivityDateResult = searchActivityQtyResult.filter((v) =>
      moment(v.date)
        .format('MM/DD/YY')
        .includes(searchParams.searchActivityDate),
    );
    const finalResult = searchActivityDateResult.filter((v) =>
      v.binLocation
        .toLowerCase()
        .includes(searchParams.searchActivityBinLocation),
    );

    setFilteredActivityLogs(finalResult);
  };

  const handleResetDataSelected = () => {
    setLocationDataSelected({
      itemSKU: '',
      binName: '',
    });
  };

  const handleOpenTransferModal = (rowData) => {
    setLocationDataSelected(rowData);
    onOpenTransferModal();
  };

  const handleOpenAdjustModal = (rowData) => {
    setLocationDataSelected(rowData);
    onOpenAdjustModal();
  };

  type ItemRow = typeof itemLocations[number];

  const itemLocationColumn: GridColumns<ItemRow> = [
    {
      field: 'actions',
      type: 'actions',
      hide: isInventoryViewer,
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="Transfer"
          onClick={() => handleOpenTransferModal(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Adjust"
          onClick={() => handleOpenAdjustModal(params.row)}
          showInMenu
        />,
      ],
    },
    {
      field: 'facility',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Facility</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchFacilityName}
            onChange={(value) => inputHandler('searchFacilityName', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterLocations();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.facility}</Typography>,
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
            value={searchParams.searchZoneName}
            onChange={(value) => inputHandler('searchZoneName', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterLocations();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.zone}</Typography>,
    },
    {
      field: 'bin',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Bin</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchBinName}
            onChange={(value) => inputHandler('searchBinName', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterLocations();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography sx={{ color: [theme.palette.primary.main] }}>
          {params.row.bin}
        </Typography>
      ),
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
            value={searchParams.searchQtyOnHand}
            onChange={(value) => inputHandler('searchQtyOnHand', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterLocations();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.qtyOnHand}</Typography>,
    },
    {
      field: 'lotNo',
      width: 160,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography fontWeight="bold">Lot Number.</Typography>
          <Input
            placeholder="Filter"
            value={searchParams.searchLotNo}
            onChange={(value) => inputHandler('searchLotNo', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterLocations();
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
      width: 160,
      renderHeader: () => (
        <Box sx={{ gap: '8px' }}>
          <Typography fontWeight="bold">Expiration Date</Typography>
          <Input
            placeholder="Filter"
            value={searchParams.searchExpirationDate}
            onChange={(value) => inputHandler('searchExpirationDate', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterLocations();
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

  const itemVendorColumns: GridColDef[] = [
    {
      field: 'vendorNo',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Vendor No.</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchVendorNo}
            onChange={(value) => inputHandler('searchVendorNo', value)}
            rightIcon={<Search />}
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
          {params.row.vendorNo}
        </Typography>
      ),
    },
    {
      field: 'name',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Name</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchName}
            onChange={(value) => inputHandler('searchName', value)}
            rightIcon={<Search />}
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
          <Typography fontWeight="bold">Contact</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchPointOfContact}
            onChange={(value) => inputHandler('searchPointOfContact', value)}
            rightIcon={<Search />}
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
          <Typography fontWeight="bold">Address</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchAddress}
            onChange={(value) => inputHandler('searchAddress', value)}
            rightIcon={<Search />}
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
          <Typography fontWeight="bold">Phone Number</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchPhoneNumber}
            onChange={(value) => inputHandler('searchPhoneNumber', value)}
            rightIcon={<Search />}
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

  const itemActivityColumn: GridColDef[] = [
    {
      field: 'type',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Type</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchActivityType}
            onChange={(value) => inputHandler('searchActivityType', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterActivityLogs();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.type}</Typography>,
    },
    {
      field: 'user',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">User</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchActivityUser}
            onChange={(value) => inputHandler('searchActivityUser', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterActivityLogs();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.user}</Typography>,
    },
    {
      field: 'date',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Date</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchActivityDate}
            onChange={(value) => inputHandler('searchActivityDate', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterActivityLogs();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{moment(params.row.date).format('MM/DD/YY')}</Typography>
      ),
    },
    {
      field: 'qty',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Qty</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchActivityQty}
            onChange={(value) => inputHandler('searchActivityQty', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterActivityLogs();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.qty}</Typography>,
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
            value={searchParams.searchActivityZone}
            onChange={(value) => inputHandler('searchActivityZone', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterActivityLogs();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.zone}</Typography>,
    },
    {
      field: 'binLocation',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Bin Location </Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchActivityBinLocation}
            onChange={(value) =>
              inputHandler('searchActivityBinLocation', value)
            }
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterActivityLogs();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.binLocation}</Typography>,
    },
  ];

  return (
    <MainContainer>
      <Header />
      {!isInventoryViewer && (
        <>
          <TransferBinModal
            ItemDetails={locationDataSelected}
            callBack={handleResetDataSelected}
          />
          <AdjustQtyModal
            locationDetails={locationDataSelected}
            itemDetails={urlSearchParams.get(`itemId`)}
          />
          <CustomerBinModal itemDetails={urlSearchParams.get(`itemId`)} />
        </>
      )}
      <ContentContainer>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
          <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredItemLoc}
              columns={itemLocationColumn}
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              pageSize={6}
              density="compact"
              rowsPerPageOptions={[6]}
              getRowId={(row) => row.binId}
              checkboxSelection
              onSelectionModelChange={(customerLocationId) => {
                const selectedId = customerLocationId[0];
                const selectedRowData = itemLocations.filter(
                  (f) => f.customerLocationId === selectedId,
                );
                if (selectedId === undefined) {
                  setLocationDataSelected({
                    itemSKU: '',
                    binName: '',
                  });
                } else {
                  setLocationDataSelected(selectedRowData[0]);
                }
              }}
              components={{ Toolbar: CustomGridToolbar }}
              componentsProps={{
                toolbar: {
                  title: 'Locations',
                },
              }}
            />
          </Card>
          <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredItemVendors}
              columns={itemVendorColumns}
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              pageSize={15}
              density="compact"
              rowsPerPageOptions={[15]}
              getRowId={(row) => row.vendorId}
              checkboxSelection
              onSelectionModelChange={(vendorId) => {
                const selectedId = vendorId[0];
                const selectedRowData = itemVendors.filter(
                  (c) => c.vendorId === selectedId,
                );
                if (selectedId === undefined) {
                  setVendorDataSelected({
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
                  setVendorDataSelected(selectedRowData[0]);
                }
              }}
              components={{ Toolbar: CustomGridToolbar }}
              componentsProps={{
                toolbar: {
                  title: 'Vendors',
                },
              }}
            />
          </Card>
          <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
            <Grid
              autoHeight
              headerHeight={120}
              rows={filteredItemActivityLogs}
              columns={itemActivityColumn}
              disableColumnFilter
              disableColumnSelector
              disableDensitySelector
              disableColumnMenu
              pageSize={6}
              density="compact"
              rowsPerPageOptions={[6]}
              getRowId={(row) => row.activityLogId}
              checkboxSelection
              components={{ Toolbar: CustomGridToolbar }}
              componentsProps={{
                toolbar: {
                  title: 'Activity',
                },
              }}
            />
          </Card>
        </Box>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(ItemView);
