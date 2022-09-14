import React, { ReactNode, useContext } from 'react';
import Barcode from 'react-jsbarcode';

import moment from 'moment';
import { AuthContext } from 'store/contexts/AuthContext';

import { Box, Button as MUIButton, Typography } from '@mui/material';

import { PrintDivider, WMSButton } from '../styles';

interface TicketProps {
  salesOrder?: boolean;
  purchaseOrder?: boolean;
  data: any;
  innerRef?: any;
  children?: ReactNode;
}

function Ticket({
  salesOrder,
  purchaseOrder,
  data,
  innerRef,
  children,
  ...props
}: TicketProps) {
  const { currentUser, currentLocationAndFacility } = useContext(AuthContext);
  if (purchaseOrder && data && Object.keys(data).length !== 0) {
    return (
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'center',
          alignItems: 'center',
          width: '100%',
        }}
        ref={innerRef}
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            justifyContent: 'space-between',
            alignItems: 'center',
            padding: '24px',
            gap: '80px',
            width: '100%',
          }}
        >
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'center',
              alignItems: 'left',
            }}
          >
            <Typography variant="h5">
              {currentUser.Claim_CustomerName}
            </Typography>
            <Typography variant="subtitle1">
              {`${currentUser.Claim_CustomerAddress1} ${currentUser.Claim_CustomerAddress2}`}
            </Typography>
            <Typography variant="subtitle1">{`${currentUser.Claim_CustomerCity}, ${currentUser.Claim_CustomerStateOrProvince} ${currentUser.Claim_CustomerZip}`}</Typography>
            <Typography variant="subtitle1">
              {currentUser.Claim_CustomerPhoneNumber}
            </Typography>
          </Box>
          <Barcode value={data.purchaseOrderDetail.purchaseOrderNo} />
        </Box>
        <Box sx={{ display: 'flex', width: '95%' }}>
          <PrintDivider />
        </Box>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            justifyContent: 'flex-start',
            alignItems: 'start',
            padding: '24px',
            gap: '80px',
            width: '100%',
          }}
        >
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'center',
              alignItems: 'left',
            }}
          >
            <Typography
              sx={{ fontWeight: 'bold', marginBottom: '12px' }}
              variant="h6"
            >
              Purchase From
            </Typography>
            <Typography variant="subtitle1">
              {`Vendor: ${data.purchaseOrderDetail.vendorName}`}
            </Typography>
            <Typography variant="subtitle1">
              {`${data.purchaseOrderDetail.vendorAddress1} ${data.purchaseOrderDetail.vendorAddress2}`}
            </Typography>
            <Typography variant="subtitle1">{`${data.purchaseOrderDetail.vendorCity}, ${data.purchaseOrderDetail.vendorStateOrProvince} ${data.purchaseOrderDetail.vendorZip}`}</Typography>
            <Typography variant="subtitle1">{`${data.purchaseOrderDetail.vendorPhoneNumber}`}</Typography>
          </Box>
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'center',
              alignItems: 'left',
            }}
          >
            <Typography
              sx={{ fontWeight: 'bold', marginBottom: '12px' }}
              variant="h6"
            >
              Ship To
            </Typography>
            <Typography variant="subtitle1">
              {`Facility: ${currentLocationAndFacility.facilityName}`}
            </Typography>
            <Typography variant="subtitle1">
              {`${currentLocationAndFacility.facilityAddress} ${
                currentLocationAndFacility.facilityAddress2
                  ? currentLocationAndFacility.facilityAddress2
                  : ''
              }`}
            </Typography>
            <Typography variant="subtitle1">{`${currentLocationAndFacility.facilityCity}, ${currentLocationAndFacility.facilityStateProvince} ${currentLocationAndFacility.facilityZip}`}</Typography>
            <Typography variant="subtitle1">
              {currentLocationAndFacility.facilityPhoneNumber
                ? currentLocationAndFacility.facilityPhoneNumber
                : ''}
            </Typography>
          </Box>
        </Box>

        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            justifyContent: 'space-between',
            alignItems: 'center',
            padding: '24px',
            gap: '80px',
            width: '100%',
          }}
        >
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'center',
              alignItems: 'left',
              width: '100%',
            }}
          >
            <Typography
              sx={{ fontWeight: 'bold', marginBottom: '12px' }}
              variant="h6"
            >
              Purchase Order Details
            </Typography>
            <Box sx={{ display: 'flex', flexDirection: 'row', width: '100%' }}>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '15%' }}>
                Order Date
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '15%' }}>
                Order Qty
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '20%' }}>
                PO Number
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '20%' }}>
                Vendor Name
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '15%' }}>
                Vendor Acct.
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                Carrier / Ship Via
              </Box>
            </Box>
            <Box sx={{ display: 'flex', width: '100%' }}>
              <PrintDivider />
            </Box>
            <Box sx={{ display: 'flex', flexDirection: 'row', width: '100%' }}>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '15%' }}>
                {moment(data.purchaseOrderDetail.orderDate).format('MM/DD/YY')}
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '15%' }}>
                {data.purchaseOrderDetail.orderQty}
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '20%' }}>
                {data.purchaseOrderDetail.purchaseOrderNo}
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '20%' }}>
                {data.purchaseOrderDetail.vendorName}
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '15%' }}>
                {data.purchaseOrderDetail.vendorAccount}
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                {data.purchaseOrderDetail.shipVia}
              </Box>
            </Box>
          </Box>
        </Box>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            justifyContent: 'space-between',
            alignItems: 'center',
            padding: '24px',
            gap: '80px',
            width: '100%',
          }}
        >
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'center',
              alignItems: 'left',
              width: '100%',
            }}
          >
            <Typography
              sx={{ fontWeight: 'bold', marginBottom: '12px' }}
              variant="h6"
            >
              Receives
            </Typography>
            <Box sx={{ display: 'flex', flexDirection: 'row', width: '100%' }}>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                Item SKU
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                Description
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                UoM
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                Order Qty
              </Box>
            </Box>
            <Box sx={{ display: 'flex', width: '100%' }}>
              <PrintDivider />
            </Box>
            <Box
              sx={{ display: 'flex', flexDirection: 'column', width: '100%' }}
            >
              {data.items.map((item, index) => (
                <>
                  <Box
                    sx={{
                      display: 'flex',
                      flexDirection: 'row',
                      width: '100%',
                    }}
                    key={item.itemId}
                  >
                    <Box
                      sx={{ display: 'flex', aignItems: 'start', width: '25%' }}
                    >
                      {item.itemSKU}
                    </Box>
                    <Box
                      sx={{ display: 'flex', aignItems: 'start', width: '25%' }}
                    >
                      {item.description}
                    </Box>
                    <Box
                      sx={{ display: 'flex', aignItems: 'start', width: '25%' }}
                    >
                      {item.uom}
                    </Box>
                    <Box
                      sx={{ display: 'flex', aignItems: 'start', width: '25%' }}
                    >
                      {item.orderQty}
                    </Box>
                  </Box>
                  <Box sx={{ display: 'flex', width: '100%' }}>
                    <PrintDivider />
                  </Box>
                </>
              ))}
            </Box>
          </Box>
        </Box>
      </Box>
    );
  }
  if (salesOrder && data && Object.keys(data).length !== 0)
    return (
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'center',
          alignItems: 'center',
          width: '100%',
        }}
        ref={innerRef}
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            justifyContent: 'space-between',
            alignItems: 'center',
            padding: '24px',
            gap: '80px',
            width: '100%',
          }}
        >
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'center',
              alignItems: 'left',
            }}
          >
            <Typography variant="h5">
              {currentUser.Claim_CustomerName}
            </Typography>
            <Typography variant="subtitle1">
              {`${currentUser.Claim_CustomerAddress1} ${currentUser.Claim_CustomerAddress2}`}
            </Typography>
            <Typography variant="subtitle1">{`${currentUser.Claim_CustomerCity}, ${currentUser.Claim_CustomerStateOrProvince} ${currentUser.Claim_CustomerZip}`}</Typography>
            <Typography variant="subtitle1">
              {currentUser.Claim_CustomerPhoneNumber}
            </Typography>
          </Box>
          <Barcode value={data.orderDetail.saleOrderNo} />
        </Box>
        <Box sx={{ display: 'flex', width: '95%' }}>
          <PrintDivider />
        </Box>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            justifyContent: 'flex-start',
            alignItems: 'start',
            padding: '24px',
            gap: '80px',
            width: '100%',
          }}
        >
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'center',
              alignItems: 'left',
            }}
          >
            <Typography
              sx={{ fontWeight: 'bold', marginBottom: '12px' }}
              variant="h6"
            >
              Bill To
            </Typography>
            <Typography variant="subtitle1">
              {`${data.orderDetail.billToAddress1} ${data.orderDetail.billToAddress2}`}
            </Typography>
            <Typography variant="subtitle1">
              {`${data.orderDetail.billToCity}, ${data.orderDetail.billToStateProvince} ${data.orderDetail.billToZipPostalCode}`}
            </Typography>
            <Typography variant="subtitle1">
              {data.orderDetail.billToPhoneNumber}
            </Typography>
          </Box>
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'center',
              alignItems: 'left',
            }}
          >
            <Typography
              sx={{ fontWeight: 'bold', marginBottom: '12px' }}
              variant="h6"
            >
              Ship To
            </Typography>
            <Typography variant="subtitle1">
              {`${data.orderDetail.shipToAddress1} ${data.orderDetail.shipToAddress2}`}
            </Typography>
            <Typography variant="subtitle1">
              {`${data.orderDetail.shipToCity}, ${data.orderDetail.shipToStateProvince} ${data.orderDetail.shipToZipPostalCode}`}
            </Typography>
            <Typography variant="subtitle1">
              {data.orderDetail.shipToPhoneNumber}
            </Typography>
          </Box>
        </Box>

        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            justifyContent: 'space-between',
            alignItems: 'center',
            padding: '24px',
            gap: '80px',
            width: '100%',
          }}
        >
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'center',
              alignItems: 'left',
              width: '100%',
            }}
          >
            <Typography
              sx={{ fontWeight: 'bold', marginBottom: '12px' }}
              variant="h6"
            >
              Order Details
            </Typography>
            <Box sx={{ display: 'flex', flexDirection: 'row', width: '100%' }}>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                Order Date
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                Promised Date
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                SO Number
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                Order Qty
              </Box>
            </Box>
            <Box sx={{ display: 'flex', width: '100%' }}>
              <PrintDivider />
            </Box>
            <Box sx={{ display: 'flex', flexDirection: 'row', width: '100%' }}>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                {moment(data.orderDetail.orderDate).format('MM/DD/YY')}
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                {moment(data.orderDetail.promisedDate).format('MM/DD/YY')}
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                {data.orderDetail.saleOrderNo}
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                {data.orderDetail.orderQty}
              </Box>
            </Box>
          </Box>
        </Box>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'row',
            justifyContent: 'space-between',
            alignItems: 'center',
            padding: '24px',
            gap: '80px',
            width: '100%',
          }}
        >
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'center',
              alignItems: 'left',
              width: '100%',
            }}
          >
            <Typography
              sx={{ fontWeight: 'bold', marginBottom: '12px' }}
              variant="h6"
            >
              Receives
            </Typography>
            <Box sx={{ display: 'flex', flexDirection: 'row', width: '100%' }}>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                Item SKU
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                Description
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                UoM
              </Box>
              <Box sx={{ display: 'flex', aignItems: 'start', width: '25%' }}>
                Order Qty
              </Box>
            </Box>
            <Box sx={{ display: 'flex', width: '100%' }}>
              <PrintDivider />
            </Box>
            <Box
              sx={{ display: 'flex', flexDirection: 'column', width: '100%' }}
            >
              {data.orderLines &&
                data.orderLines.map((item, index) => (
                  <>
                    <Box
                      sx={{
                        display: 'flex',
                        flexDirection: 'row',
                        width: '100%',
                      }}
                      key={item.itemId}
                    >
                      <Box
                        sx={{
                          display: 'flex',
                          aignItems: 'start',
                          width: '25%',
                        }}
                      >
                        {item.itemSKU}
                      </Box>
                      <Box
                        sx={{
                          display: 'flex',
                          aignItems: 'start',
                          width: '25%',
                        }}
                      >
                        {item.description}
                      </Box>
                      <Box
                        sx={{
                          display: 'flex',
                          aignItems: 'start',
                          width: '25%',
                        }}
                      >
                        {item.uom}
                      </Box>
                      <Box
                        sx={{
                          display: 'flex',
                          aignItems: 'start',
                          width: '25%',
                        }}
                      >
                        {item.orderQty}
                      </Box>
                    </Box>
                    <Box sx={{ display: 'flex', width: '100%' }}>
                      <PrintDivider />
                    </Box>
                  </>
                ))}
            </Box>
          </Box>
        </Box>
      </Box>
    );
}

export default React.memo(Ticket);
