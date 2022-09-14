
using Packem.Integrations.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Packem.Integrations.Entities
{
    public partial class PurchaseOrderHeader
    {
        [Column(TypeName = "decimal(19, 2)")]
        public decimal po_no { get; set; }

        [Column(TypeName = "decimal(19, 2)")]
        public decimal vendor_id { get; set; }

        [ForeignKey("vendor_id")]
        public Vendor Vendors { get; set; }

        public DateTime? order_date { get; set; }

        [Column(TypeName = "varchar(8)")]
        public string company_no { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ship2_name { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string packing_slip_number { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ship2_add1 { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ship2_add2 { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ship2_city { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ship2_state { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ship2_country { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string requested_by { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string fob { get; set; }

        public DateTime? date_due { get; set; }

        public DateTime? receipt_date { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string po_desc { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string ship2_zip { get; set; }

        [Column(TypeName = "char(1)")]
        public char delete_flag { get; set; }

        [Column(TypeName = "char(1)")]
        public char complete { get; set; }

        public DateTime date_created { get; set; }

        public DateTime date_last_modified { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string last_maintained_by { get; set; }

        [Column(TypeName = "decimal(19, 0)")]
        public int location_id { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string terms { get; set; }

        [Column(TypeName = "varchar(8)")]
        public string carrier_id { get; set; }

        [Column(TypeName = "char(1)")]
        public char vouch_flag { get; set; }

        [Column(TypeName = "decimal(3, 0)")]
        public decimal period { get; set; }

        [Column(TypeName = "decimal(4, 0)")]
        public decimal year_for_period { get; set; }

        [Column(TypeName = "char(1)")]
        public char cancel_flag { get; set; }

        [Column(TypeName = "decimal(19, 0)")]
        public char currency_id { get; set; }

        [Column(TypeName = "char(1)")]
        public string printed { get; set; }

        public DateTime? ack_date { get; set; }

        [Column(TypeName = "varchar(2)")]
        public string ack_code { get; set; }

        [Column(TypeName = "char(1)")]
        public char closed_flag { get; set; }

        [Column(TypeName = "decimal(19, 0)")]
        public decimal supplier_id { get; set; }

        [Column(TypeName = "decimal(19, 0)")]
        public decimal division_id { get; set; }

        [Column(TypeName = "varchar(8)")]
        public string branch_id { get; set; }

        [Column(TypeName = "char(1)")]
        public char approved { get; set; }

        [Column(TypeName = "varchar(8)")]
        public string purchase_group_id { get; set; }

        [Column(TypeName = "varchar(8)")]
        public string po_class1 { get; set; }

        [Column(TypeName = "varchar(8)")]
        public string po_class2 { get; set; }

        [Column(TypeName = "varchar(8)")]
        public string po_class3 { get; set; }

        [Column(TypeName = "varchar(8)")]
        public string po_class4 { get; set; }

        [Column(TypeName = "varchar(8)")]
        public string po_class5 { get; set; }

        [Column(TypeName = "varchar(8)")]
        public string sales_order_number { get; set; }

        [Column(TypeName = "char(1)")]
        public char po_type { get; set; }

        [Column(TypeName = "decimal(19, 6)")]
        public decimal exchange_rate { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string external_po_no { get; set; }

        [Column(TypeName = "char(1)")]
        public char exclude_from_lead_time { get; set; }


        public int? source_type { get; set; }

        public int? transmission_method { get; set; }

        public int po_hdr_uid { get; set; }

        public int edi_edit_count { get; set; }

        [Column(TypeName = "char(1)")]
        public char revised_po { get; set; }

        [Column(TypeName = "varchar(16)")]
        public string contact_id { get; set; }

        [Column(TypeName = "char(255)")]
        public string created_by { get; set; }

        [Column(TypeName = "decimal(19, 6)")]
        public decimal total_iva_tax_amt { get; set; }

        [Column(TypeName = "char(1)")]
        public char expedite_flag { get; set; }

        [Column(TypeName = "char(1)")]
        public char vendor_pays_freight_flag { get; set; }

        [Column(TypeName = "char(1)")]
        public char retrieved_by_wms { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string supplier_release_no { get; set; }

        [Column(TypeName = "char(1)")]
        public char historical_complete_flag { get; set; }


        public DateTime? expected_date { get; set; }

        public DateTime? sent_to_carrier_date { get; set; }

        [Column(TypeName = "char(1)")]
        public char? print_canadian_b3_forms_flag { get; set; }

        public DateTime? canadian_b3_forms_date { get; set; }

        public int? cad_cha_contract_cd { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string cad_cha_quote_no { get; set; }

        [Column(TypeName = "char(40)")]
        public char? tracking_no { get; set; }

        [Column(TypeName = "char(1)")]
        public char? net_billing_flag { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ship2_add3 { get; set; }

        [Column(TypeName = "char(1)")]
        public char? do_not_export_carrier_po_flag { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string packing_basis { get; set; }

        public int? packing_basis_violation_cd { get; set; }

        [Column(TypeName = "char(1)")]
        public char? transmit_print { get; set; }

        [Column(TypeName = "char(1)")]
        public char? transmit_fax { get; set; }

        [Column(TypeName = "char(1)")]
        public char? transmit_email { get; set; }

        [Column(TypeName = "char(1)")]
        public char? transmit_edi { get; set; }

        [Column(TypeName = "decimal(19, 9)")]
        public decimal dflt_list_price_multiplier { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string ship_via_desc { get; set; }

        [Column(TypeName = "char(1)")]
        public char? blind_ship_flag { get; set; }

        public int? po_qty_change_count { get; set; }

        [Column(TypeName = "char(1)")]
        public char? bulk_discount_flag { get; set; }

        [Column(TypeName = "decimal(19, 9)")]
        public decimal estimated_value { get; set; }

        [Column(TypeName = "char(1)")]
        public string estimated_value_edit_flag { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string bid_no { get; set; }

        public DateTime? ExpectedShipDate { get; set; }

        [Column(TypeName = "char(1)")]
        public char auto_vouch_except_flag { get; set; }
    }
}