import React from 'react';
import { Outlet } from 'react-router-dom';

import { Container, MainContainer } from './styles';

function EmptyLayout() {
  return (
    <MainContainer>
      <Container>
        <Outlet />
      </Container>
    </MainContainer>
  );
}

export default React.memo(EmptyLayout);
