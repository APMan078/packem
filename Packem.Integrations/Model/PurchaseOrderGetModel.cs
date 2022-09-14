using System;

namespace Packem.Integrations.Models
{
    public class PurchaseOrderGetModel
    {
        public decimal po_no { get; set; }

        public decimal vendor_id { get; set; }

        public DateTime? order_date { get; set; }

        public string company_no { get; set; }

        public string ship2_name { get; set; }

        public string packing_slip_number { get; set; }

        public string ship2_add1 { get; set; }

        public string ship2_add2 { get; set; }

        public string ship2_city { get; set; }

        public string ship2_state { get; set; }

        public string ship2_country { get; set; }

        public string requested_by { get; set; }

        public string fob { get; set; }

        public DateTime? date_due { get; set; }

        public DateTime? receipt_date { get; set; }

        public string po_desc { get; set; }

        public string ship2_zip { get; set; }

        public char delete_flag { get; set; }

        public char complete { get; set; }

        public DateTime date_created { get; set; }

        public DateTime date_last_modified { get; set; }

        public string last_maintained_by { get; set; }

        public int location_id { get; set; }

        public string terms { get; set; }

        public string carrier_id { get; set; }

        public char vouch_flag { get; set; }

        public decimal period { get; set; }

        public decimal year_for_period { get; set; }

        public char cancel_flag { get; set; }

        public char currency_id { get; set; }

        public string printed { get; set; }

        public DateTime? ack_date { get; set; }

        public string ack_code { get; set; }

        public char closed_flag { get; set; }

        public decimal supplier_id { get; set; }

        public decimal division_id { get; set; }

        public string branch_id { get; set; }

        public char approved { get; set; }

        public string purchase_group_id { get; set; }

        public string po_class1 { get; set; }

        public string po_class2 { get; set; }

        public string po_class3 { get; set; }

        public string po_class4 { get; set; }

        public string po_class5 { get; set; }

        public string sales_order_number { get; set; }

        public char po_type { get; set; }

        public decimal exchange_rate { get; set; }

        public string external_po_no { get; set; }

        public char exclude_from_lead_time { get; set; }

        public int? source_type { get; set; }

        public int? transmission_method { get; set; }

        public int po_hdr_uid { get; set; }

        public int edi_edit_count { get; set; }

        public char revised_po { get; set; }

        public string contact_id { get; set; }

        public string created_by { get; set; }

        public decimal total_iva_tax_amt { get; set; }

        public char expedite_flag { get; set; }

        public char vendor_pays_freight_flag { get; set; }

        public char retrieved_by_wms { get; set; }

        public string supplier_release_no { get; set; }

        public char historical_complete_flag { get; set; }

        public DateTime? expected_date { get; set; }

        public DateTime? sent_to_carrier_date { get; set; }

        public char? print_canadian_b3_forms_flag { get; set; }

        public DateTime? canadian_b3_forms_date { get; set; }

        public int? cad_cha_contract_cd { get; set; }

        public string cad_cha_quote_no { get; set; }

        public char? tracking_no { get; set; }

        public char? net_billing_flag { get; set; }

        public string ship2_add3 { get; set; }

        public char? do_not_export_carrier_po_flag { get; set; }

        public string packing_basis { get; set; }

        public int? packing_basis_violation_cd { get; set; }

        public char? transmit_print { get; set; }

        public char? transmit_fax { get; set; }

        public char? transmit_email { get; set; }

        public char? transmit_edi { get; set; }

        public decimal dflt_list_price_multiplier { get; set; }

        public string ship_via_desc { get; set; }

        public char? blind_ship_flag { get; set; }

        public int? po_qty_change_count { get; set; }

        public char? bulk_discount_flag { get; set; }

        public decimal estimated_value { get; set; }

        public string estimated_value_edit_flag { get; set; }

        public string bid_no { get; set; }

        public DateTime? ExpectedShipDate { get; set; }

        public char auto_vouch_except_flag { get; set; }
    }
}
