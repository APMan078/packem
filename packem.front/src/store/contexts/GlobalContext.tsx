import React, { createContext, useState, useMemo } from 'react';

import { useTheme } from '@mui/material/styles';

export const GlobalContext = createContext(null);

const GlobalProvider = ({ children }) => {
  const theme = useTheme();
  const [loading, setLoading] = useState(false);
  const [updateData, setUpdateData] = useState(false);
  const [isResetPasswordModalOpen, setResetPasswordModalOpen] =
    React.useState(false);
  const [isConfirmDeleteDialogOpen, setConfirmDeleteDialogOpen] =
    React.useState(false);
  const [isAddOrderLineItemModalOpen, setOnOpenAddOrderLineItemModal] =
    useState(false);
  const [isConfirmationModalOpen, setConfirmationModalOpen] =
    React.useState(false);
  const [isFileInputModalOpen, setFileInputModalOpen] = React.useState(false);
  const [isLocationAndFacilityModalOpen, setLocationAndFacilityModalOpen] =
    React.useState(false);
  const [isManualSalesOrderOpen, setManualSalesOrderOpen] =
    React.useState(false);
  const [isSOPrintAndQueueModalOpen, setSOPrintAndQueueModalOpen] =
    React.useState(false);
  const [isItemModalOpen, setItemModalOpen] = React.useState(false);
  const [isVendorModalOpen, setVendorModalOpen] = React.useState(false);
  const [isBinModalOpen, setBinModalOpen] = React.useState(false);
  const [isCreateBinModalOpen, setCreateBinModalOpen] = React.useState(false);
  const [isAdjustModalOpen, setAdjustModalOpen] = React.useState(false);
  const [isTransferModalOpen, setTransferModalOpen] = React.useState(false);
  const [isCustomerModalOpen, setCustomerModalOpen] = React.useState(false);
  const [isOrderCustomerModalOpen, setOrderCustomerModalOpen] =
    React.useState(false);
  const [isOrderCustomerAddressModalOpen, setOrderCustomerAddressModalOpen] =
    React.useState(false);
  const [isUserModalOpen, setUserModalOpen] = React.useState(false);
  const [isFacilityModalOpen, setFacilityModalOpen] = React.useState(false);
  const [isFacilityZoneModalOpen, setFacilityZoneModalOpen] =
    React.useState(false);
  const [isDeviceModalOpen, setDeviceModalOpen] = React.useState(false);
  const [isDeviceTokenModalOpen, setDeviceTokenModalOpen] =
    React.useState(false);
  const [isCustomerLocationModalOpen, setCustomerLocationModalOpen] =
    React.useState(false);
  const [isPurchaseOrderItemModalOpen, setPurchaseOrderItemModalOpen] =
    React.useState(false);
  const [isPurchaseOrderModalOpen, setPurchaseOrderModalOpen] =
    React.useState(false);
  const [isManualReceiptModalOpen, setManualReceiptModalOpen] =
    React.useState(false);
  const [isAdjustPurchaseOrderModalOpen, setAdjustPurchaseOrderModalOpen] =
    React.useState(false);
  const [isEditExpirationDateModalOpen, setEditExpirationDateModalOpen] =
    React.useState(false);
  const [isEditThresholdModalOpen, setEditThresholdModalOpen] =
    React.useState(false);

  const updateLoading = (value) => {
    setLoading(value);
  };

  const onLoading = () => Boolean(loading);

  const handleUpdateData = () => {
    setUpdateData(!updateData);
  };

  const onOpenOrderCustomerModal = () => {
    setOrderCustomerModalOpen(true);
  };

  const onCloseOrderCustomerModal = () => {
    setOrderCustomerModalOpen(false);
  };

  const onOpenOrderCustomerAddressModal = () => {
    setOrderCustomerAddressModalOpen(true);
  };

  const onCloseOrderCustomerAddressModal = () => {
    setOrderCustomerAddressModalOpen(false);
  };

  const onOpenAddOrderLineItemModal = () => {
    setOnOpenAddOrderLineItemModal(true);
  };

  const onCloseAddOrderLineItemModal = () => {
    setOnOpenAddOrderLineItemModal(false);
  };

  const onOpenManualSalesOrderModal = () => {
    setManualSalesOrderOpen(true);
  };

  const onCloseManualSalesOrderModal = () => {
    setManualSalesOrderOpen(false);
  };

  const onOpenResetPasswordModal = () => {
    setResetPasswordModalOpen(true);
  };

  const onCloseResetPasswordModal = () => {
    setResetPasswordModalOpen(false);
  };

  const onOpenConfirmDeleteDialog = () => {
    setConfirmDeleteDialogOpen(true);
  };

  const onCloseConfirmDeleteDialog = () => {
    setConfirmDeleteDialogOpen(false);
  };

  const onOpenConfirmationModal = () => {
    setConfirmationModalOpen(true);
  };

  const onCloseConfirmationModal = () => {
    setConfirmationModalOpen(false);
  };
  const onOpenFileInputModal = () => {
    setFileInputModalOpen(true);
  };

  const onCloseInputFilenModal = () => {
    setFileInputModalOpen(false);
  };

  const onOpenLocationAndFacilityModal = () => {
    setLocationAndFacilityModalOpen(true);
  };

  const onCloseLocationAndFacilityModal = () => {
    setLocationAndFacilityModalOpen(false);
  };

  const onOpenSOPrintAndQueueModal = () => {
    setSOPrintAndQueueModalOpen(true);
  };

  const onCloseSOPrintAndQueueModal = () => {
    setSOPrintAndQueueModalOpen(false);
  };

  const onOpenItemModal = () => {
    setItemModalOpen(true);
  };

  const onCloseItemModal = () => {
    setItemModalOpen(false);
  };

  const onOpenTransferModal = () => {
    setTransferModalOpen(true);
  };

  const onCloseTransferModal = () => {
    setTransferModalOpen(false);
  };

  const onOpenAdjustModal = () => {
    setAdjustModalOpen(true);
  };

  const onCloseAdjustModal = () => {
    setAdjustModalOpen(false);
  };

  const onOpenEditExpirationDateModal = () => {
    setEditExpirationDateModalOpen(true);
  };

  const onCloseExpirationDateModal = () => {
    setEditExpirationDateModalOpen(false);
  };

  const onOpenCreateBinModal = () => {
    setCreateBinModalOpen(true);
  };

  const onCloseCreateBinModal = () => {
    setCreateBinModalOpen(false);
  };

  const onOpenBinModal = () => {
    setBinModalOpen(true);
  };

  const onCloseBinModal = () => {
    setBinModalOpen(false);
  };

  const onOpenVendorModal = () => {
    setVendorModalOpen(true);
  };

  const onCloseVendorModal = () => {
    setVendorModalOpen(false);
  };

  const onOpenDeviceModal = () => {
    setDeviceModalOpen(true);
  };

  const onCloseDeviceModal = () => {
    setDeviceModalOpen(false);
  };

  const onOpenDeviceTokenModal = () => {
    setDeviceTokenModalOpen(true);
  };

  const onCloseDeviceTokenModal = () => {
    setDeviceTokenModalOpen(false);
  };

  const onOpenFacilityModal = () => {
    setFacilityModalOpen(true);
  };

  const onCloseFacilityModal = () => {
    setFacilityModalOpen(false);
  };

  const onOpenFacilityZoneModal = () => {
    setFacilityZoneModalOpen(true);
  };

  const onCloseFacilityZoneModal = () => {
    setFacilityZoneModalOpen(false);
  };

  const onOpenCustomerLocationModal = () => {
    setCustomerLocationModalOpen(true);
  };

  const onCloseCustomerLocationModal = () => {
    setCustomerLocationModalOpen(false);
  };

  const onOpenUserModal = () => {
    setUserModalOpen(true);
  };

  const onCloseUserModal = () => {
    setUserModalOpen(false);
  };

  const onOpenCustomerModal = () => {
    setCustomerModalOpen(true);
  };

  const onCloseCustomerModal = () => {
    setCustomerModalOpen(false);
  };

  const onOpenManualReceiptModal = () => {
    setManualReceiptModalOpen(true);
  };

  const onCloseManualReceiptModal = () => {
    setManualReceiptModalOpen(false);
  };

  const onOpenPurchaseOrderModal = () => {
    setPurchaseOrderModalOpen(true);
  };

  const onClosePurchaseOrderModal = () => {
    setPurchaseOrderModalOpen(false);
  };

  const onOpenPurchaseOrderItemModal = () => {
    setPurchaseOrderItemModalOpen(true);
  };

  const onClosePurchaseOrderItemModal = () => {
    setPurchaseOrderItemModalOpen(false);
  };

  const onOpenPurchaseOrderItemAdjustModal = () => {
    setAdjustPurchaseOrderModalOpen(true);
  };

  const onClosePurchaseOrderItemAdjustModal = () => {
    setAdjustPurchaseOrderModalOpen(false);
  };

  const onOpenEditThresholdModal = () => {
    setEditThresholdModalOpen(true);
  };

  const onCloseEditThresholdModal = () => {
    setEditThresholdModalOpen(false);
  };

  const props = useMemo(
    () => ({
      isAddOrderLineItemModalOpen,
      isManualSalesOrderOpen,
      isResetPasswordModalOpen,
      isConfirmDeleteDialogOpen,
      isConfirmationModalOpen,
      isFileInputModalOpen,
      isLocationAndFacilityModalOpen,
      isSOPrintAndQueueModalOpen,
      isItemModalOpen,
      isTransferModalOpen,
      isAdjustModalOpen,
      isCreateBinModalOpen,
      isBinModalOpen,
      isVendorModalOpen,
      isCustomerModalOpen,
      isUserModalOpen,
      isCustomerLocationModalOpen,
      isFacilityModalOpen,
      isFacilityZoneModalOpen,
      isDeviceModalOpen,
      isDeviceTokenModalOpen,
      onOpenLocationAndFacilityModal,
      onCloseLocationAndFacilityModal,
      isManualReceiptModalOpen,
      isPurchaseOrderModalOpen,
      isPurchaseOrderItemModalOpen,
      isAdjustPurchaseOrderModalOpen,
      isOrderCustomerModalOpen,
      isOrderCustomerAddressModalOpen,
      onOpenOrderCustomerModal,
      onCloseOrderCustomerModal,
      onOpenOrderCustomerAddressModal,
      onCloseOrderCustomerAddressModal,
      isEditExpirationDateModalOpen,
      isEditThresholdModalOpen,
      onOpenAddOrderLineItemModal,
      onCloseAddOrderLineItemModal,
      onOpenManualSalesOrderModal,
      onCloseManualSalesOrderModal,
      onOpenResetPasswordModal,
      onCloseResetPasswordModal,
      onOpenConfirmDeleteDialog,
      onCloseConfirmDeleteDialog,
      onOpenConfirmationModal,
      onCloseConfirmationModal,
      onOpenFileInputModal,
      onCloseInputFilenModal,
      onOpenSOPrintAndQueueModal,
      onCloseSOPrintAndQueueModal,
      onOpenItemModal,
      onCloseItemModal,
      onOpenTransferModal,
      onCloseTransferModal,
      onOpenAdjustModal,
      onCloseAdjustModal,
      onOpenCreateBinModal,
      onCloseCreateBinModal,
      onOpenBinModal,
      onCloseBinModal,
      onOpenVendorModal,
      onCloseVendorModal,
      onOpenFacilityModal,
      onCloseFacilityModal,
      onOpenFacilityZoneModal,
      onCloseFacilityZoneModal,
      onOpenCustomerModal,
      onCloseCustomerModal,
      onOpenUserModal,
      onCloseUserModal,
      onOpenDeviceModal,
      onCloseDeviceModal,
      onOpenDeviceTokenModal,
      onCloseDeviceTokenModal,
      onOpenCustomerLocationModal,
      onCloseCustomerLocationModal,
      onOpenManualReceiptModal,
      onCloseManualReceiptModal,
      onOpenPurchaseOrderModal,
      onClosePurchaseOrderModal,
      onOpenPurchaseOrderItemModal,
      onClosePurchaseOrderItemModal,
      onOpenPurchaseOrderItemAdjustModal,
      onClosePurchaseOrderItemAdjustModal,
      onOpenEditExpirationDateModal,
      onCloseExpirationDateModal,
      onOpenEditThresholdModal,
      onCloseEditThresholdModal,
      updateLoading,
      updateData,
      handleUpdateData,
      onLoading: Boolean(onLoading()),
    }),
    [
      isOrderCustomerModalOpen,
      isOrderCustomerAddressModalOpen,
      isAddOrderLineItemModalOpen,
      isManualSalesOrderOpen,
      isSOPrintAndQueueModalOpen,
      isResetPasswordModalOpen,
      isConfirmDeleteDialogOpen,
      isConfirmationModalOpen,
      isFileInputModalOpen,
      isLocationAndFacilityModalOpen,
      isItemModalOpen,
      isTransferModalOpen,
      isCreateBinModalOpen,
      isBinModalOpen,
      isAdjustModalOpen,
      isVendorModalOpen,
      isCustomerModalOpen,
      isCustomerLocationModalOpen,
      isUserModalOpen,
      isFacilityModalOpen,
      isFacilityZoneModalOpen,
      isDeviceModalOpen,
      isDeviceTokenModalOpen,
      isManualReceiptModalOpen,
      isPurchaseOrderModalOpen,
      isPurchaseOrderItemModalOpen,
      isAdjustPurchaseOrderModalOpen,
      isEditExpirationDateModalOpen,
      isEditThresholdModalOpen,
      updateData,
    ],
  );

  return (
    <GlobalContext.Provider value={props}>{children}</GlobalContext.Provider>
  );
};

export default React.memo(GlobalProvider);
