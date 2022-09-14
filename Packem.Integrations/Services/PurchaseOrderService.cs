using FluentResults;
using Microsoft.EntityFrameworkCore;
using Packem.Integrations.Entities;
using Packem.Integrations.Interfaces;
using Packem.Integrations.Models;

namespace Packem.Integrations.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IntegrationDbContext _context;

        public PurchaseOrderService(IntegrationDbContext context
            )
        {
            _context = context;
           
        }

        public async Task<Result<PurchaseOrderGetModel>> CreatePurchaseOrderAsync(PurchaseOrderCreateModel model)
        {
            try
            {
                var entity = new PurchaseOrderHeader
                {
                    po_no = model.po_no,
                    vendor_id = model.vendor_id,
                    order_date = model.order_date,
                    company_no = model.company_no,
                    ship2_name = model.ship2_name,
                    packing_slip_number = model.packing_slip_number,
                    ship2_add1 = model.ship2_add1,
                    ship2_add2 = model.ship2_add2,
                    ship2_city = model.ship2_city,
                    ship2_state = model.ship2_state,
                    ship2_country = model.ship2_country,
                    requested_by = model.requested_by,
                    fob = model.fob,
                    date_due = model.date_due,
                    receipt_date = model.receipt_date,
                    po_desc = model.po_desc,
                    ship2_zip = model.ship2_zip,
                    delete_flag = model.delete_flag,
                    complete = model.complete,
                    date_created = model.date_created,
                    date_last_modified = model.date_last_modified,
                    last_maintained_by = model.last_maintained_by,
                    location_id = model.location_id,
                    terms = model.terms,
                    carrier_id = model.carrier_id,
                    vouch_flag = model.vouch_flag,
                    period = model.period,
                    year_for_period = model.year_for_period,
                    cancel_flag = model.cancel_flag,
                    currency_id = model.currency_id,
                    printed = model.printed,
                    ack_date = model.ack_date,
                    ack_code = model.ack_code,
                    closed_flag = model.closed_flag,
                    supplier_id = model.supplier_id,
                    division_id = model.division_id,
                    branch_id = model.branch_id,
                    approved = model.approved,
                    purchase_group_id = model.purchase_group_id,
                    po_class1 = model.po_class1,
                    po_class2 = model.po_class2,
                    po_class3 = model.po_class3,
                    po_class4 = model.po_class4,
                    po_class5 = model.po_class5,
                    sales_order_number = model.sales_order_number,
                    po_type = model.po_type,
                    exchange_rate = model.exchange_rate,
                    external_po_no = model.external_po_no,
                    exclude_from_lead_time = model.exclude_from_lead_time,
                    source_type = model.source_type,
                    transmission_method = model.transmission_method,
                    po_hdr_uid = model.po_hdr_uid,
                    edi_edit_count = model.edi_edit_count,
                    revised_po = model.revised_po,
                    contact_id = model.contact_id,
                    created_by = model.created_by,
                    total_iva_tax_amt = model.total_iva_tax_amt,
                    expedite_flag = model.expedite_flag,
                    vendor_pays_freight_flag = model.vendor_pays_freight_flag,
                    retrieved_by_wms = model.retrieved_by_wms,
                    supplier_release_no = model.supplier_release_no,
                    historical_complete_flag = model.historical_complete_flag,
                    expected_date = model.expected_date,
                    sent_to_carrier_date = model.sent_to_carrier_date,
                    print_canadian_b3_forms_flag = model.print_canadian_b3_forms_flag,
                    canadian_b3_forms_date = model.canadian_b3_forms_date,
                    cad_cha_contract_cd = model.cad_cha_contract_cd,
                    cad_cha_quote_no = model.cad_cha_quote_no,
                    tracking_no = model.tracking_no,
                    net_billing_flag = model.net_billing_flag,
                    ship2_add3 = model.ship2_add3,
                    do_not_export_carrier_po_flag = model.do_not_export_carrier_po_flag,
                    packing_basis = model.packing_basis,
                    packing_basis_violation_cd = model.packing_basis_violation_cd,
                    transmit_print = model.transmit_print,
                    transmit_fax = model.transmit_fax,
                    transmit_email = model.transmit_email,
                    transmit_edi = model.transmit_edi,
                    dflt_list_price_multiplier = model.dflt_list_price_multiplier,
                    ship_via_desc = model.ship_via_desc,
                    blind_ship_flag = model.blind_ship_flag,
                    po_qty_change_count = model.po_qty_change_count,
                    bulk_discount_flag = model.bulk_discount_flag,
                    estimated_value = model.estimated_value,
                    estimated_value_edit_flag = model.estimated_value_edit_flag,
                    bid_no = model.bid_no,
                    ExpectedShipDate = model.ExpectedShipDate,
                    auto_vouch_except_flag = model.auto_vouch_except_flag,
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();

                return Result.Ok(new PurchaseOrderGetModel
                {
                    po_no = entity.po_no,
                    vendor_id = entity.vendor_id,
                    order_date = entity.order_date,
                    company_no = entity.company_no,
                    ship2_name = entity.ship2_name,
                    packing_slip_number = entity.packing_slip_number,
                    ship2_add1 = entity.ship2_add1,
                    ship2_add2 = entity.ship2_add2,
                    ship2_city = entity.ship2_city,
                    ship2_state = entity.ship2_state,
                    ship2_country = entity.ship2_country,
                    requested_by = entity.requested_by,
                    fob = entity.fob,
                    date_due = entity.date_due,
                    receipt_date = entity.receipt_date,
                    po_desc = entity.po_desc,
                    ship2_zip = entity.ship2_zip,
                    delete_flag = entity.delete_flag,
                    complete = entity.complete,
                    date_created = entity.date_created,
                    date_last_modified = entity.date_last_modified,
                    last_maintained_by = entity.last_maintained_by,
                    location_id = entity.location_id,
                    terms = entity.terms,
                    carrier_id = entity.carrier_id,
                    vouch_flag = entity.vouch_flag,
                    period = entity.period,
                    year_for_period = entity.year_for_period,
                    cancel_flag = entity.cancel_flag,
                    currency_id = entity.currency_id,
                    printed = entity.printed,
                    ack_date = entity.ack_date,
                    ack_code = entity.ack_code,
                    closed_flag = entity.closed_flag,
                    supplier_id = entity.supplier_id,
                    division_id = entity.division_id,
                    branch_id = entity.branch_id,
                    approved = entity.approved,
                    purchase_group_id = entity.purchase_group_id,
                    po_class1 = entity.po_class1,
                    po_class2 = entity.po_class2,
                    po_class3 = entity.po_class3,
                    po_class4 = entity.po_class4,
                    po_class5 = entity.po_class5,
                    sales_order_number = entity.sales_order_number,
                    po_type = entity.po_type,
                    exchange_rate = entity.exchange_rate,
                    external_po_no = entity.external_po_no,
                    exclude_from_lead_time = entity.exclude_from_lead_time,
                    source_type = entity.source_type,
                    transmission_method = entity.transmission_method,
                    po_hdr_uid = entity.po_hdr_uid,
                    edi_edit_count = entity.edi_edit_count,
                    revised_po = entity.revised_po,
                    contact_id = entity.contact_id,
                    created_by = entity.created_by,
                    total_iva_tax_amt = entity.total_iva_tax_amt,
                    expedite_flag = entity.expedite_flag,
                    vendor_pays_freight_flag = entity.vendor_pays_freight_flag,
                    retrieved_by_wms = entity.retrieved_by_wms,
                    supplier_release_no = entity.supplier_release_no,
                    historical_complete_flag = entity.historical_complete_flag,
                    expected_date = entity.expected_date,
                    sent_to_carrier_date = entity.sent_to_carrier_date,
                    print_canadian_b3_forms_flag = entity.print_canadian_b3_forms_flag,
                    canadian_b3_forms_date = entity.canadian_b3_forms_date,
                    cad_cha_contract_cd = entity.cad_cha_contract_cd,
                    cad_cha_quote_no = entity.cad_cha_quote_no,
                    tracking_no = entity.tracking_no,
                    net_billing_flag = entity.net_billing_flag,
                    ship2_add3 = entity.ship2_add3,
                    do_not_export_carrier_po_flag = entity.do_not_export_carrier_po_flag,
                    packing_basis = entity.packing_basis,
                    packing_basis_violation_cd = entity.packing_basis_violation_cd,
                    transmit_print = entity.transmit_print,
                    transmit_fax = entity.transmit_fax,
                    transmit_email = entity.transmit_email,
                    transmit_edi = entity.transmit_edi,
                    dflt_list_price_multiplier = entity.dflt_list_price_multiplier,
                    ship_via_desc = entity.ship_via_desc,
                    blind_ship_flag = entity.blind_ship_flag,
                    po_qty_change_count = entity.po_qty_change_count,
                    bulk_discount_flag = entity.bulk_discount_flag,
                    estimated_value = entity.estimated_value,
                    estimated_value_edit_flag = entity.estimated_value_edit_flag,
                    bid_no = entity.bid_no,
                    ExpectedShipDate = entity.ExpectedShipDate,
                    auto_vouch_except_flag = entity.auto_vouch_except_flag,
                });
            }
            catch (Exception ex)
            {
                //ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }


        public async Task<Result<PurchaseOrderGetModel>> GetPurchaseOrderDetailAsync( int purchaseOrderId)
        {
            try
            {


                var query = _context.PurchaseOrderHeaders
                    .Include(x => x.Vendors)
                    .Where(e => e.po_no == purchaseOrderId)
                    .AsQueryable();



                var model = await query
                    .AsNoTracking()
                    .Select(entity => new PurchaseOrderGetModel
                    {
                        po_no = entity.po_no,
                        vendor_id = entity.vendor_id,
                        order_date = entity.order_date,
                        company_no = entity.company_no,
                        ship2_name = entity.ship2_name,
                        packing_slip_number = entity.packing_slip_number,
                        ship2_add1 = entity.ship2_add1,
                        ship2_add2 = entity.ship2_add2,
                        ship2_city = entity.ship2_city,
                        ship2_state = entity.ship2_state,
                        ship2_country = entity.ship2_country,
                        requested_by = entity.requested_by,
                        fob = entity.fob,
                        date_due = entity.date_due,
                        receipt_date = entity.receipt_date,
                        po_desc = entity.po_desc,
                        ship2_zip = entity.ship2_zip,
                        delete_flag = entity.delete_flag,
                        complete = entity.complete,
                        date_created = entity.date_created,
                        date_last_modified = entity.date_last_modified,
                        last_maintained_by = entity.last_maintained_by,
                        location_id = entity.location_id,
                        terms = entity.terms,
                        carrier_id = entity.carrier_id,
                        vouch_flag = entity.vouch_flag,
                        period = entity.period,
                        year_for_period = entity.year_for_period,
                        cancel_flag = entity.cancel_flag,
                        currency_id = entity.currency_id,
                        printed = entity.printed,
                        ack_date = entity.ack_date,
                        ack_code = entity.ack_code,
                        closed_flag = entity.closed_flag,
                        supplier_id = entity.supplier_id,
                        division_id = entity.division_id,
                        branch_id = entity.branch_id,
                        approved = entity.approved,
                        purchase_group_id = entity.purchase_group_id,
                        po_class1 = entity.po_class1,
                        po_class2 = entity.po_class2,
                        po_class3 = entity.po_class3,
                        po_class4 = entity.po_class4,
                        po_class5 = entity.po_class5,
                        sales_order_number = entity.sales_order_number,
                        po_type = entity.po_type,
                        exchange_rate = entity.exchange_rate,
                        external_po_no = entity.external_po_no,
                        exclude_from_lead_time = entity.exclude_from_lead_time,
                        source_type = entity.source_type,
                        transmission_method = entity.transmission_method,
                        po_hdr_uid = entity.po_hdr_uid,
                        edi_edit_count = entity.edi_edit_count,
                        revised_po = entity.revised_po,
                        contact_id = entity.contact_id,
                        created_by = entity.created_by,
                        total_iva_tax_amt = entity.total_iva_tax_amt,
                        expedite_flag = entity.expedite_flag,
                        vendor_pays_freight_flag = entity.vendor_pays_freight_flag,
                        retrieved_by_wms = entity.retrieved_by_wms,
                        supplier_release_no = entity.supplier_release_no,
                        historical_complete_flag = entity.historical_complete_flag,
                        expected_date = entity.expected_date,
                        sent_to_carrier_date = entity.sent_to_carrier_date,
                        print_canadian_b3_forms_flag = entity.print_canadian_b3_forms_flag,
                        canadian_b3_forms_date = entity.canadian_b3_forms_date,
                        cad_cha_contract_cd = entity.cad_cha_contract_cd,
                        cad_cha_quote_no = entity.cad_cha_quote_no,
                        tracking_no = entity.tracking_no,
                        net_billing_flag = entity.net_billing_flag,
                        ship2_add3 = entity.ship2_add3,
                        do_not_export_carrier_po_flag = entity.do_not_export_carrier_po_flag,
                        packing_basis = entity.packing_basis,
                        packing_basis_violation_cd = entity.packing_basis_violation_cd,
                        transmit_print = entity.transmit_print,
                        transmit_fax = entity.transmit_fax,
                        transmit_email = entity.transmit_email,
                        transmit_edi = entity.transmit_edi,
                        dflt_list_price_multiplier = entity.dflt_list_price_multiplier,
                        ship_via_desc = entity.ship_via_desc,
                        blind_ship_flag = entity.blind_ship_flag,
                        po_qty_change_count = entity.po_qty_change_count,
                        bulk_discount_flag = entity.bulk_discount_flag,
                        estimated_value = entity.estimated_value,
                        estimated_value_edit_flag = entity.estimated_value_edit_flag,
                        bid_no = entity.bid_no,
                        ExpectedShipDate = entity.ExpectedShipDate,
                        auto_vouch_except_flag = entity.auto_vouch_except_flag,
                    })
                    .SingleOrDefaultAsync();

                if (model is null)
                {
                    return Result.Fail($"{nameof(PurchaseOrderHeader)} not found.");
                }

                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                //ex = await _exceptionService.HandleExceptionAsync(ex);
                return Result.Fail(ex.ToString());
            }
        }

    }
}