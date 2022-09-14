import React, { ReactNode, useContext } from 'react';
import { Routes, Route, Navigate, useLocation } from 'react-router-dom';

import { snackActions } from 'config/snackbar.js';
import BinManagement from 'pages/bins';
import BinView from 'pages/bins/binview';
import Transfers from 'pages/bins/transfers';
import Dashboard from 'pages/dashboard';
import DeviceManagement from 'pages/devices';
import DeviceView from 'pages/devices/deviceview';
import FacilityManagement from 'pages/facilities';
import Inventory from 'pages/inventory';
import ItemView from 'pages/inventory/itemview';
import Login from 'pages/login';
import OrderCustomers from 'pages/ordercustomers';
import OrderCustomerDetails from 'pages/ordercustomers/ordercustomerdetails';
import PasswordReset from 'pages/passwordreset';
import Picking from 'pages/picking';
import PutAway from 'pages/putaway';
import Receipts from 'pages/receiving';
import ReceiptItemView from 'pages/receiving/itemview';
import Sales from 'pages/sales';
import SalesOrderView from 'pages/sales/salesorderview';
import Settings from 'pages/settings';
import CustomerManagement from 'pages/superadmin/customermanagement';
import CustomerView from 'pages/superadmin/customermanagement/customerview';
import UserManagement from 'pages/users';
import Vendors from 'pages/vendors';
import { AuthContext } from 'store/contexts/AuthContext';

import KitchenSink from './pages/kitchensink';
import StyledKitchenSink from './pages/styledkitchensink';
import EmptyLayout from './styles/emptylayout';
import MainLayout from './styles/mainlayout';

interface RouteProps {
  element?: any;
  path?: string;
  superAdminPath?: boolean;
  adminPath?: boolean;
  inventoryViewerPath?: boolean;
  children?: ReactNode;
}

function ProtectedRoute({
  element,
  path,
  superAdminPath,
  adminPath,
  inventoryViewerPath,
  children,
}: RouteProps) {
  const {
    isAuth,
    isSuperAdmin,
    isAdmin,
    isOpManager,
    isOperator,
    isInventoryViewer,
  } = useContext(AuthContext);

  if (superAdminPath && !isSuperAdmin) {
    snackActions.warning('Not authorized to access this page.');
    return <Navigate to="/inventory" />;
  }

  if ((adminPath && isOperator) || (adminPath && isOpManager)) {
    snackActions.warning('You must be an admin to access this portal.');
    return <Navigate to="/login" />;
  }
  if (adminPath && !isAdmin) {
    snackActions.warning('You must be an admin to access this portal.');
    return <Navigate to="/login" />;
  }
  if (inventoryViewerPath && !isInventoryViewer) {
    snackActions.warning('You do not have permission to access this portal.');
    return <Navigate to="/login" />;
  }

  return isAuth ? (
    <Route path={path} element={element}>
      {children}
    </Route>
  ) : (
    <Navigate to="/login" />
  );
}

export default function MainRoutes() {
  return (
    <Routes>
      <Route path="/demo" element={<MainLayout />}>
        <Route path="/kitchen" element={<KitchenSink />} />
        <Route path="/styled-kitchen" element={<StyledKitchenSink />} />
      </Route>

      <ProtectedRoute adminPath element={<MainLayout />}>
        <Route path="/inventory" element={<Inventory />} />
        <Route path="/inventory/item/:itemSKU" element={<ItemView />} />
        <Route path="/inventory/vendors" element={<Vendors />} />
        <Route path="/bins" element={<BinManagement />} />
        <Route path="/bins/bin/:binName" element={<BinView />} />
        <Route path="/bins/transfer-queue" element={<Transfers />} />
        <Route path="/sales" element={<Sales />} />
        <Route path="/sales/order/:salesOrderId" element={<SalesOrderView />} />
        <Route path="/customers" element={<OrderCustomers />} />
        <Route path="/put-away" element={<PutAway />} />
        <Route
          path="/customers/:customerId"
          element={<OrderCustomerDetails />}
        />
        <Route path="/receiving" element={<Receipts />} />
        <Route
          path="/receiving/item/:purchaseOrderNo"
          element={<ReceiptItemView />}
        />
        <Route path="/facilities" element={<FacilityManagement />} />
        <Route path="/devices" element={<DeviceManagement />} />
        <Route path="/users" element={<UserManagement />} />
        <Route path="/devices/device/:deviceId" element={<DeviceView />} />
        <Route path="/receiving/:poNo" element={<ReceiptItemView />} />
        <Route path="/settings" element={<Settings />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/picking" element={<Picking />} />
        <Navigate to="/devices" />
      </ProtectedRoute>

      <ProtectedRoute superAdminPath element={<MainLayout />}>
        <Route path="/customer-management" element={<CustomerManagement />} />
        <Route
          path="/customer-management/customer/:customerId"
          element={<CustomerView />}
        />
      </ProtectedRoute>

      <ProtectedRoute inventoryViewerPath element={<MainLayout />}>
        <Route path="/customer-inventory" element={<Inventory />} />
        <Route
          path="/customer-inventory/item/:itemSKU"
          element={<ItemView />}
        />
        <Route path="/customer-inventory/vendors" element={<Vendors />} />
        <Navigate to="/customer-inventory" />
      </ProtectedRoute>

      <Route element={<EmptyLayout />}>
        <Route path="/login" element={<Login />} />
        <Route path="/password-reset" element={<PasswordReset />} />
        <Navigate to="/login" />
      </Route>
    </Routes>
  );
}
