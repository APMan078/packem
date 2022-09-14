using System.ComponentModel.DataAnnotations.Schema;

namespace Packem.Integrations.Entities
{
    public partial class PurchaseOrderLine
    {
        public decimal po_no { get; set; }
        [ForeignKey("po_no")]
        public PurchaseOrderHeader PurchaseOrderHeader { get; set; }
        public decimal qty_ordered { get; set; }
        public decimal qty_received { get; set; }
        public decimal received_date { get; set; }
        public decimal unit_price { get; set; }
        public string company_no { get; set; }
        public string mfg_part_no { get; set; }
        public string delete_flag { get; set; }
        public string date_due { get; set; }
        public string date_created { get; set; }
        public string date_last_modified { get; set; }
        public string last_maained_by { get; set; }
        public string next_due_in_po_cost { get; set; }
        public string complete { get; set; }
        public string vouch_completed { get; set; }
        public string cancel_flag { get; set; }
        public string in_bound_curry_id { get; set; }
        public string account_no { get; set; }
        public string qty_to_vouch { get; set; }
        public string closed_flag { get; set; }
        public string item_description { get; set; }
        public string unit_of_measure { get; set; }
        public string unit_size { get; set; }
        public string unit_quantity { get; set; }
        public string line_no { get; set; }
        public string pricing_book_id { get; set; }
        public string pricing_book_item_id { get; set; }
        public string pricing_book_supplier_id { get; set; }
        public string pricing_book_disc_grp_id { get; set; }
        public string pricing_book_effective_date { get; set; }
        public string combinable { get; set; }
        public string calc_type { get; set; }
        public string calc_value { get; set; }
        public string required_date { get; set; }
        public string next_break { get; set; }
        public string next_ut_price { get; set; }
        public string base_ut_price { get; set; }
        public string price_edit { get; set; }
        public string new_item { get; set; }
        public string quantity_changed { get; set; }
        public string pricing_unit { get; set; }
        public string pricing_unit_size { get; set; }
        public string extended_desc { get; set; }
        public string unit_price_display { get; set; }
        public string inv_mast_uid { get; set; }
        public string exclude_from_lead_time { get; set; }
        public string source_type { get; set; }
        public string exp_date_updates { get; set; }
        public string po_line_uid { get; set; }
        public string edi_new_status { get; set; }
        public string line_type { get; set; }
        public string contract_number { get; set; }
        public string created_by { get; set; }
        public string parent_po_line_no { get; set; }
        public string supplier_ship_date { get; set; }
        public string entered_as_code { get; set; }
        public string gpor_run_uid { get; set; }
        public string purchase_pricing_page_uid { get; set; }
        public string expedite_flag { get; set; }
        public string original_unit_price_display { get; set; }
        public string retrieved_by_wms { get; set; }
        public string expedite_es { get; set; }
        public string expedite_followup_flag { get; set; }
        public string desired_receipt_location_id { get; set; }
        public string country_of_origin { get; set; }
        public string acknowledged_date { get; set; }
        public string b3_qty { get; set; }
        public string qty_ready { get; set; }
        public string qty_ready_unit_size { get; set; }
        public string qty_ready_uom { get; set; }
        public string unit_qty_ready { get; set; }
        public string bulk_buy_flag { get; set; }
        public string cad_purchase_cost { get; set; }
        public string list_price_multiplier { get; set; }
        public string carrier_status { get; set; }
        public string expected_ship_date { get; set; }
        public string date_due_last_modified { get; set; }
    }
}
