import React, { useEffect, useState, useContext, ChangeEvent } from 'react';

import Card from 'components/card';
import CustomGridToolbar from 'components/gridtoolbar';
import Header from 'components/header';
import Input from 'components/input/Input';
import { Grid } from 'components/styles';
import { snackActions } from 'config/snackbar.js';
import UserRoles from 'helpers/userrolehelper';
import UserCreationModal from 'pages/shared/customerusermodal';
import { MainContainer, ContentContainer } from 'pages/styles';
import {
  getUsersByCustomerId,
  changeUserActiveStatus,
} from 'services/api/user/users.api';
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

function UserManagement() {
  const theme = useTheme();
  const { currentUser } = useContext(AuthContext);
  const {
    loading,
    updateLoading,
    updateData,
    handleUpdateData,
    onOpenUserModal,
  } = useContext(GlobalContext);
  const [users, setUsers] = useState([]);
  const [editUser, setEditUser] = useState(false);
  const [filteredUsers, setFilteredUsers] = useState([]);
  const [dataSelected, setDataSelected] = useState({
    userId: '',
    name: '',
    username: '',
    email: '',
    roleId: '',
  });
  const initialSearchState = {
    searchName: '',
    searchUserId: '',
    searchUsername: '',
    searchEmail: '',
    searchRoleId: '',
    searchIsActive: '',
  };
  const [searchParams, setSearchParams] = useState(initialSearchState);

  function getStatus(params) {
    let status = '';
    if (!params.row.isActive) {
      status = 'Inactive';
    } else {
      status = 'Active';
    }
    return status;
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

  const filterUsers = () => {
    const userIdSearchResults = users.filter((u) =>
      u.userId.toString().includes(searchParams.searchUserId),
    );
    const userNameSearch = userIdSearchResults.filter((u) =>
      u.name.toString().toLowerCase().includes(searchParams.searchName),
    );
    const usernameSearch = userNameSearch.filter((u) =>
      u.username.toString().toLowerCase().includes(searchParams.searchUsername),
    );
    const emailSearchResults = usernameSearch.filter((u) =>
      u.email.toString().toLowerCase().includes(searchParams.searchEmail),
    );
    const roleSearchResults = emailSearchResults.filter((u) =>
      UserRoles[u.roleId].toLowerCase().includes(searchParams.searchRoleId),
    );
    const finalResult = roleSearchResults.filter((u) =>
      u.isActive.toString().includes(searchParams.searchIsActive),
    );

    setFilteredUsers(finalResult);
  };

  const onLoadCustomerData = async () => {
    try {
      const customerUsersFromApi = await getUsersByCustomerId(
        currentUser.Claim_CustomerId,
      );

      setUsers(customerUsersFromApi);
      setFilteredUsers(customerUsersFromApi);

      return true;
    } catch (error) {
      return error;
    }
  };

  useEffect(() => {
    setUsers([]);
    onLoadCustomerData();
  }, [updateData]);

  const handleDeactivateUser = async (rowData) => {
    try {
      if (rowData.isActive === true) {
        const form = {
          userId: rowData.userId,
          isActive: false,
        };
        const response = await changeUserActiveStatus(form);
        handleUpdateData();
        return snackActions.success(
          `Successfully deactivated user: ${rowData.username}`,
        );
      }
      const form = {
        userId: rowData.userId,
        isActive: true,
      };
      const response = await changeUserActiveStatus(form);
      handleUpdateData();
      return snackActions.success(
        `Successfully reactivated user: ${rowData.username}`,
      );
    } catch (error) {
      return snackActions.error(
        `Unable to deactivate user: ${rowData.username}`,
      );
    }
  };

  type UserRow = typeof users[number];

  const handleEditUser = (rowData) => {
    setDataSelected(rowData);
    setEditUser(true);
    onOpenUserModal();
  };

  const handleResetDataSelected = () => {
    setDataSelected({
      userId: '',
      name: '',
      username: '',
      email: '',
      roleId: '',
    });
    setEditUser(false);
  };

  const userColumns: GridColumns<UserRow> = [
    {
      field: 'actions',
      type: 'actions',
      width: 34,
      // eslint-disable-next-line react/no-unstable-nested-components
      getActions: (params) => [
        <GridActionsCellItem
          label="Edit"
          onClick={() => handleEditUser(params.row)}
          showInMenu
        />,
        <GridActionsCellItem
          label="Deactivate/Reactivate"
          onClick={() => handleDeactivateUser(params.row)}
          showInMenu
        />,
      ],
    },
    {
      field: 'userId',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">User Id</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchUserId}
            onChange={(value) => inputHandler('searchUserId', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.userId}</Typography>,
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
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.name}</Typography>,
    },
    {
      field: 'username',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Username</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchUsername}
            onChange={(value) => inputHandler('searchUsername', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.username}</Typography>,
    },
    {
      field: 'email',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Email</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchEmail}
            onChange={(value) => inputHandler('searchEmail', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{params.row.email}</Typography>,
    },
    {
      field: 'roleId',
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Role</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchRoleId}
            onChange={(value) => inputHandler('searchRoleId', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => (
        <Typography>{UserRoles[params.row.roleId]}</Typography>
      ),
    },
    {
      field: 'isActive',
      sortable: true,
      width: 200,
      renderHeader: () => (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Typography fontWeight="bold">Status</Typography>
          <Input
            sx={{ maxWidth: '140px' }}
            placeholder="Search"
            value={searchParams.searchIsActive}
            onChange={(value) => inputHandler('searchIsActive', value)}
            rightIcon={<Search />}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                filterUsers();
              }
            }}
          />
        </Box>
      ),
      renderCell: (params) => <Typography>{getStatus(params)}</Typography>,
    },
  ];

  return (
    <MainContainer>
      {editUser ? (
        <UserCreationModal
          userData={dataSelected}
          callBack={handleResetDataSelected}
          edit
          admin
        />
      ) : (
        <UserCreationModal
          userData={dataSelected}
          callBack={handleResetDataSelected}
          admin
        />
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
              {users.length}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Total Users
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
              {users.filter((u) => u.roleId === 2).length}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Admins
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
              {users.filter((u) => u.roleId === 3).length}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Ops Manager
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
              {users.filter((u) => u.roleId === 4).length}
            </Typography>
            <Typography variant="caption" fontWeight="bold">
              Operator
            </Typography>
          </Card>
        </Box>
        <Card sx={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
          <Grid
            autoHeight
            headerHeight={120}
            rows={filteredUsers}
            onSelectionModelChange={(id) => {
              const selectedId = new Set(id);
              const selectedRowData = users.filter((ca) =>
                selectedId.has(ca.userId),
              );
              /* setDataSelected(selectedRowData); */
            }}
            density="compact"
            disableColumnFilter
            disableColumnSelector
            disableDensitySelector
            disableColumnMenu
            columns={userColumns}
            pageSize={15}
            rowsPerPageOptions={[15]}
            getRowId={(row) => row.userId}
            checkboxSelection
            components={{ Toolbar: CustomGridToolbar }}
            componentsProps={{
              toolbar: {
                title: 'Users',
              },
            }}
          />
        </Card>
      </ContentContainer>
    </MainContainer>
  );
}

export default React.memo(UserManagement);
