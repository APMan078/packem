import React from 'react';
import { Outlet } from 'react-router-dom';

import Navbar from 'components/navbar';
import CustomerModal from 'pages/shared/customermodal';

import Box from '@mui/material/Box';

import { MainContainer, Content } from './styles';

function MainLayout() {
  return (
    <MainContainer>
      <CustomerModal />
      <Navbar />
      <Content>
        <Outlet />
      </Content>
    </MainContainer>
  );
}

export default React.memo(MainLayout);
