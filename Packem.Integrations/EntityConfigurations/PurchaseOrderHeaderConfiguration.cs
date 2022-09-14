using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Packem.Integrations.Entities;

namespace Packem.Integrations.EntityConfigurations
{
    public class PurchaseOrderHeaderConfiguration : IEntityTypeConfiguration<PurchaseOrderHeader>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderHeader> builder)
        {
            builder.ToTable("po_hdr");

            builder.HasKey(x => x.po_no);

            builder.Property(x => x.vendor_id)
             .IsRequired();

            builder.Property(x => x.company_no)
             .IsRequired();

            builder.Property(x => x.delete_flag)
             .IsRequired();

            builder.Property(x => x.date_created)
             .IsRequired();

            builder.Property(x => x.date_last_modified)
             .IsRequired();

            builder.Property(x => x.last_maintained_by)
             .IsRequired();


            builder.Property(x => x.location_id)
             .IsRequired();

            builder.Property(x => x.branch_id)
             .IsRequired();

            builder.Property(x => x.approved)
             .IsRequired();

            builder.Property(x => x.po_type)
             .IsRequired();

            builder.Property(x => x.purchase_group_id)
             .IsRequired();

            builder.Property(x => x.exclude_from_lead_time)
             .IsRequired(); 

            builder.Property(x => x.po_hdr_uid)
             .IsRequired();

            builder.Property(x => x.retrieved_by_wms)
             .IsRequired();


            builder.Property(x => x.print_canadian_b3_forms_flag)
             .IsRequired();

            builder.Property(x => x.auto_vouch_except_flag)
             .IsRequired();

            builder.HasOne(x => x.Vendors)
          .WithMany(x => x.PurchaseOrderHeaders)
          .HasForeignKey(x => x.vendor_id)
          .OnDelete(DeleteBehavior.ClientSetNull)
          .HasConstraintName("FK_po_hdr_vendor");
        }
    }
}