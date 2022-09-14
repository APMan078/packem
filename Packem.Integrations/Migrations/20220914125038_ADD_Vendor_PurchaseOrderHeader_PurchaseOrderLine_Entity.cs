using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.Integrations.Migrations
{
    public partial class ADD_Vendor_PurchaseOrderHeader_PurchaseOrderLine_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vendor",
                columns: table => new
                {
                    vendor_id = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vendor", x => x.vendor_id);
                });

            migrationBuilder.CreateTable(
                name: "po_hdr",
                columns: table => new
                {
                    po_no = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    vendor_id = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    order_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    company_no = table.Column<string>(type: "varchar(8)", nullable: false),
                    ship2_name = table.Column<string>(type: "varchar(50)", nullable: false),
                    packing_slip_number = table.Column<string>(type: "varchar(50)", nullable: false),
                    ship2_add1 = table.Column<string>(type: "varchar(50)", nullable: false),
                    ship2_add2 = table.Column<string>(type: "varchar(50)", nullable: false),
                    ship2_city = table.Column<string>(type: "varchar(50)", nullable: false),
                    ship2_state = table.Column<string>(type: "varchar(50)", nullable: false),
                    ship2_country = table.Column<string>(type: "varchar(50)", nullable: false),
                    requested_by = table.Column<string>(type: "varchar(50)", nullable: false),
                    fob = table.Column<string>(type: "varchar(20)", nullable: false),
                    date_due = table.Column<DateTime>(type: "datetime2", nullable: true),
                    receipt_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    po_desc = table.Column<string>(type: "varchar(255)", nullable: false),
                    ship2_zip = table.Column<string>(type: "varchar(10)", nullable: false),
                    delete_flag = table.Column<string>(type: "char(1)", nullable: false),
                    complete = table.Column<string>(type: "char(1)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    date_last_modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    last_maintained_by = table.Column<string>(type: "varchar(30)", nullable: false),
                    location_id = table.Column<decimal>(type: "decimal(19,0)", nullable: false),
                    terms = table.Column<string>(type: "varchar(20)", nullable: false),
                    carrier_id = table.Column<string>(type: "varchar(8)", nullable: false),
                    vouch_flag = table.Column<string>(type: "char(1)", nullable: false),
                    period = table.Column<decimal>(type: "decimal(3,0)", nullable: false),
                    year_for_period = table.Column<decimal>(type: "decimal(4,0)", nullable: false),
                    cancel_flag = table.Column<string>(type: "char(1)", nullable: false),
                    currency_id = table.Column<decimal>(type: "decimal(19,0)", nullable: false),
                    printed = table.Column<string>(type: "char(1)", nullable: false),
                    ack_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ack_code = table.Column<string>(type: "varchar(2)", nullable: false),
                    closed_flag = table.Column<string>(type: "char(1)", nullable: false),
                    supplier_id = table.Column<decimal>(type: "decimal(19,0)", nullable: false),
                    division_id = table.Column<decimal>(type: "decimal(19,0)", nullable: false),
                    branch_id = table.Column<string>(type: "varchar(8)", nullable: false),
                    approved = table.Column<string>(type: "char(1)", nullable: false),
                    purchase_group_id = table.Column<string>(type: "varchar(8)", nullable: false),
                    po_class1 = table.Column<string>(type: "varchar(8)", nullable: false),
                    po_class2 = table.Column<string>(type: "varchar(8)", nullable: false),
                    po_class3 = table.Column<string>(type: "varchar(8)", nullable: false),
                    po_class4 = table.Column<string>(type: "varchar(8)", nullable: false),
                    po_class5 = table.Column<string>(type: "varchar(8)", nullable: false),
                    sales_order_number = table.Column<string>(type: "varchar(8)", nullable: false),
                    po_type = table.Column<string>(type: "char(1)", nullable: false),
                    exchange_rate = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    external_po_no = table.Column<string>(type: "varchar(40)", nullable: false),
                    exclude_from_lead_time = table.Column<string>(type: "char(1)", nullable: false),
                    source_type = table.Column<int>(type: "int", nullable: true),
                    transmission_method = table.Column<int>(type: "int", nullable: true),
                    po_hdr_uid = table.Column<int>(type: "int", nullable: false),
                    edi_edit_count = table.Column<int>(type: "int", nullable: false),
                    revised_po = table.Column<string>(type: "char(1)", nullable: false),
                    contact_id = table.Column<string>(type: "varchar(16)", nullable: false),
                    created_by = table.Column<string>(type: "char(255)", nullable: false),
                    total_iva_tax_amt = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    expedite_flag = table.Column<string>(type: "char(1)", nullable: false),
                    vendor_pays_freight_flag = table.Column<string>(type: "char(1)", nullable: false),
                    retrieved_by_wms = table.Column<string>(type: "char(1)", nullable: false),
                    supplier_release_no = table.Column<string>(type: "varchar(255)", nullable: false),
                    historical_complete_flag = table.Column<string>(type: "char(1)", nullable: false),
                    expected_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    sent_to_carrier_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    print_canadian_b3_forms_flag = table.Column<string>(type: "char(1)", nullable: false),
                    canadian_b3_forms_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    cad_cha_contract_cd = table.Column<int>(type: "int", nullable: true),
                    cad_cha_quote_no = table.Column<string>(type: "varchar(255)", nullable: false),
                    tracking_no = table.Column<string>(type: "char(40)", nullable: true),
                    net_billing_flag = table.Column<string>(type: "char(1)", nullable: true),
                    ship2_add3 = table.Column<string>(type: "varchar(50)", nullable: false),
                    do_not_export_carrier_po_flag = table.Column<string>(type: "char(1)", nullable: true),
                    packing_basis = table.Column<string>(type: "varchar(255)", nullable: false),
                    packing_basis_violation_cd = table.Column<int>(type: "int", nullable: true),
                    transmit_print = table.Column<string>(type: "char(1)", nullable: true),
                    transmit_fax = table.Column<string>(type: "char(1)", nullable: true),
                    transmit_email = table.Column<string>(type: "char(1)", nullable: true),
                    transmit_edi = table.Column<string>(type: "char(1)", nullable: true),
                    dflt_list_price_multiplier = table.Column<decimal>(type: "decimal(19,9)", nullable: false),
                    ship_via_desc = table.Column<string>(type: "varchar(255)", nullable: false),
                    blind_ship_flag = table.Column<string>(type: "char(1)", nullable: true),
                    po_qty_change_count = table.Column<int>(type: "int", nullable: true),
                    bulk_discount_flag = table.Column<string>(type: "char(1)", nullable: true),
                    estimated_value = table.Column<decimal>(type: "decimal(19,9)", nullable: false),
                    estimated_value_edit_flag = table.Column<string>(type: "char(1)", nullable: false),
                    bid_no = table.Column<string>(type: "varchar(255)", nullable: false),
                    ExpectedShipDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    auto_vouch_except_flag = table.Column<string>(type: "char(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_po_hdr", x => x.po_no);
                    table.ForeignKey(
                        name: "FK_po_hdr_vendor",
                        column: x => x.vendor_id,
                        principalTable: "vendor",
                        principalColumn: "vendor_id");
                });

            migrationBuilder.CreateTable(
                name: "po_line",
                columns: table => new
                {
                    po_no = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    qty_ordered = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    qty_received = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    received_date = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    company_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mfg_part_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    delete_flag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_due = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_created = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_last_modified = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    last_maained_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    next_due_in_po_cost = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    complete = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    vouch_completed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cancel_flag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    in_bound_curry_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    account_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qty_to_vouch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    closed_flag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    item_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    unit_of_measure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    unit_size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    unit_quantity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    line_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pricing_book_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pricing_book_item_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pricing_book_supplier_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pricing_book_disc_grp_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pricing_book_effective_date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    combinable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    calc_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    calc_value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    required_date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    next_break = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    next_ut_price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    base_ut_price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price_edit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    new_item = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    quantity_changed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pricing_unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pricing_unit_size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    extended_desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    unit_price_display = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    inv_mast_uid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    exclude_from_lead_time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    source_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    exp_date_updates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    po_line_uid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    edi_new_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    line_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    contract_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    parent_po_line_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    supplier_ship_date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    entered_as_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gpor_run_uid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    purchase_pricing_page_uid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expedite_flag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    original_unit_price_display = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    retrieved_by_wms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expedite_es = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expedite_followup_flag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    desired_receipt_location_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    country_of_origin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    acknowledged_date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    b3_qty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qty_ready = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qty_ready_unit_size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qty_ready_uom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    unit_qty_ready = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bulk_buy_flag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cad_purchase_cost = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    list_price_multiplier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    carrier_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expected_ship_date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_due_last_modified = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_po_line", x => x.po_no);
                    table.ForeignKey(
                        name: "FK_po_line_po_hdr_po_no",
                        column: x => x.po_no,
                        principalTable: "po_hdr",
                        principalColumn: "po_no",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_po_hdr_vendor_id",
                table: "po_hdr",
                column: "vendor_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "po_line");

            migrationBuilder.DropTable(
                name: "po_hdr");

            migrationBuilder.DropTable(
                name: "vendor");
        }
    }
}
