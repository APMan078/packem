import React from 'react';

import Button from 'components/button';
import Card from 'components/card';
import Input from 'components/input';
import { Grid } from 'components/styles';

import { TextField, Box, Typography } from '@mui/material';
import { GridColDef, GridValueGetterParams } from '@mui/x-data-grid';

const columns: GridColDef[] = [
  { field: 'id', headerName: 'ID', width: 70 },
  { field: 'firstName', headerName: 'First name', width: 130 },
  { field: 'lastName', headerName: 'Last name', width: 130 },
  {
    field: 'age',
    headerName: 'Age',
    type: 'number',
    width: 90,
  },
  {
    field: 'fullName',
    headerName: 'Full name',
    description: 'This column has a value getter and is not sortable.',
    sortable: false,
    width: 160,
    valueGetter: (params: GridValueGetterParams) =>
      `${params.row.firstName || ''} ${params.row.lastName || ''}`,
  },
];

const rows = [
  { id: 1, lastName: 'Snow', firstName: 'Jon', age: 35 },
  { id: 2, lastName: 'Lannister', firstName: 'Cersei', age: 42 },
  { id: 3, lastName: 'Lannister', firstName: 'Jaime', age: 45 },
  { id: 4, lastName: 'Stark', firstName: 'Arya', age: 16 },
  { id: 5, lastName: 'Targaryen', firstName: 'Daenerys', age: null },
  { id: 6, lastName: 'Melisandre', firstName: null, age: 150 },
  { id: 7, lastName: 'Clifford', firstName: 'Ferrara', age: 44 },
  { id: 8, lastName: 'Frances', firstName: 'Rossini', age: 36 },
  { id: 9, lastName: 'Roxie', firstName: 'Harvey', age: 65 },
];

function StyledKitchenSink() {
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        gap: '16px',
        width: '100%',
        height: '100vh',
      }}
    >
      <Typography variant="h1">h1</Typography>
      <Typography variant="h2">h2</Typography>
      <Typography variant="h3">h3</Typography>
      <Typography variant="h4">h4</Typography>
      <Typography variant="h5">h5</Typography>
      <Typography variant="h6">h6</Typography>
      <Typography variant="subtitle1">subtitle1</Typography>
      <Typography variant="subtitle2">subtitle2</Typography>
      <Typography variant="body1">body1</Typography>
      <Typography variant="body2">body2</Typography>
      <Typography variant="button">button</Typography>
      <Typography variant="caption">caption</Typography>
      <Typography variant="overline">overline</Typography>

      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Card>
          <Box sx={{ display: 'flex', flexDirection: 'column' }}>
            <Typography variant="h1">header1</Typography>
            <Typography variant="h2">header2</Typography>
            <Typography variant="h3">header3</Typography>
            <Typography variant="h4">header4</Typography>
            <Typography variant="h5">header5</Typography>
            <Typography variant="h6">header6</Typography>
            <Typography variant="subtitle1">
              Lorem ipsum dolor sit amet, consectetur adipiscing elit,
            </Typography>
            <Typography variant="subtitle2">
              Lorem ipsum dolor sit amet, consectetur adipiscing elit,
            </Typography>
            <Typography variant="body1">
              Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
              eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut
              enim ad minim veniam, quis nostrud exercitation ullamco laboris
              nisi ut aliquip ex ea commodo consequat.
            </Typography>
            <Typography variant="body2">
              Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
              eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut
              enim ad minim veniam, quis nostrud exercitation ullamco laboris
              nisi ut aliquip ex ea commodo consequat.
            </Typography>
            {/* <Button>
              <Typography variant="button">Button</Typography>
            </Button> */}
            <Typography variant="caption">
              Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
              eiusmod tempor incididunt ut labore et dolore magna aliqua.
            </Typography>
            <Typography variant="overline">
              Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
              eiusmod tempor incididunt ut labore et dolore magna aliqua.
            </Typography>
            <TextField placeholder="Placeholder" />
          </Box>
        </Card>
      </Box>

      {/* CARDS */}
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Card>Card</Card>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Card>Card</Card>
        <Card>Card</Card>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
        <Card>Card</Card>
      </Box>
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'row',
          width: '100%',
          gap: '16px',
        }}
      >
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
      </Box>
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'row',
          width: '100%',
          gap: '16px',
        }}
      >
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
      </Box>
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'row',
          width: '100%',
          gap: '16px',
        }}
      >
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
      </Box>
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'row',
          width: '100%',
          gap: '16px',
        }}
      >
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
      </Box>
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'row',
          width: '100%',
          gap: '16px',
        }}
      >
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
      </Box>
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'row',
          width: '100%',
          gap: '16px',
        }}
      >
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
        <TextField sx={{ width: '100%' }} placeholder="TextField" />
      </Box>

      {/* GRIDS */}
      <Box sx={{ display: 'flex', minHeight: '400px' }}>
        <Grid
          rows={rows}
          columns={columns}
          pageSize={5}
          rowsPerPageOptions={[5]}
          checkboxSelection
        />
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Grid
          rows={rows}
          columns={columns}
          pageSize={5}
          rowsPerPageOptions={[5]}
          checkboxSelection
        />
        <Grid
          rows={rows}
          columns={columns}
          pageSize={5}
          rowsPerPageOptions={[5]}
          checkboxSelection
        />
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Grid
          rows={rows}
          columns={columns}
          pageSize={5}
          rowsPerPageOptions={[5]}
          checkboxSelection
        />
        <Grid
          rows={rows}
          columns={columns}
          pageSize={5}
          rowsPerPageOptions={[5]}
          checkboxSelection
        />
        <Grid
          rows={rows}
          columns={columns}
          pageSize={5}
          rowsPerPageOptions={[5]}
          checkboxSelection
        />
      </Box>

      {/* CONTAINED BUTTONS */}
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="contained" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
        <Button variant="contained" size="large">
          Button
        </Button>
      </Box>

      {/* OUTLINED BUTTONS */}
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="outlined" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
        <Button variant="outlined" size="large">
          Button
        </Button>
      </Box>

      {/* TEXT BUTTONS */}
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="text" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'row', gap: '16px' }}>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
        <Button variant="text" size="large">
          Button
        </Button>
      </Box>
    </Box>
  );
}

export default React.memo(StyledKitchenSink);
