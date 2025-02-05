using LiftDataManager.Core.DataAccessLayer.Models.Fahrkorb;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Fahrkorb;

public class LiftBufferConfig : BaseModelBuilder<LiftBuffer>
{
    public override void Configure(EntityTypeBuilder<LiftBuffer> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(25)
               .IsRequired();
        builder.Property(x => x.DisplayName)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.IsFavorite);
        builder.Property(x => x.IsObsolete);
        builder.Property(x => x.SchindlerCertified);
        builder.Property(x => x.OrderSelection);
        builder.Property(x => x.Manufacturer)
               .HasMaxLength(25)
               .IsRequired();
        builder.Property(x => x.TypeExaminationCertificateId);
        builder.Property(x => x.Diameter);
        builder.Property(x => x.Height);
        builder.Property(x => x.BufferStroke);
        builder.Property(x => x.MinLoad063);
        builder.Property(x => x.MaxLoad063);
        builder.Property(x => x.MinLoad100);
        builder.Property(x => x.MaxLoad100);
        builder.Property(x => x.MinLoad130);
        builder.Property(x => x.MaxLoad130);
        builder.Property(x => x.MinLoad160);
        builder.Property(x => x.MaxLoad160);
        builder.Property(x => x.MinLoad180);
        builder.Property(x => x.MaxLoad180);
        builder.Property(x => x.MinLoad200);
        builder.Property(x => x.MaxLoad200);
        builder.Property(x => x.MinLoad250);
        builder.Property(x => x.MaxLoad250);
    }
}