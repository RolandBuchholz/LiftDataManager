using LiftDataManager.Core.DataAccessLayer.Models.Tueren;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Tueren;

public class ShaftDoorConfig : BaseModelBuilder<ShaftDoor>
{
    public override void Configure(EntityTypeBuilder<ShaftDoor> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.DisplayName)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.IsFavorite);
        builder.Property(x => x.IsObsolete);
        builder.Property(x => x.SchindlerCertified);
        builder.Property(x => x.OrderSelection);
        builder.Property(x => x.Manufacturer)
               .HasMaxLength(30)
               .IsRequired();
        builder.Property(x => x.DoorPanelCount);
        builder.Property(x => x.SillWidth);
        builder.Property(x => x.DoorPanelWidth);
        builder.Property(x => x.DoorPanelSpace);
        builder.Property(x => x.DefaultFrameWidth);
        builder.Property(x => x.DefaultFrameDepth);
        builder.Property(x => x.LiftDoorOpeningDirectionId);
        builder.Property(x => x.TypeExaminationCertificateId);
        builder.HasMany(t => t.LiftDoorGroups)
               .WithOne(g => g.ShaftDoor)
               .HasForeignKey(t => t.ShaftDoorId);
    }
}