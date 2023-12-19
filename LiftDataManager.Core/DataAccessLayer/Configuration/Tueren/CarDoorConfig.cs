using LiftDataManager.Core.DataAccessLayer.Models.Tueren;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LiftDataManager.Core.DataAccessLayer.Configuration.Tueren;

public class CarDoorConfig : BaseModelBuilder<CarDoor>
{
    public override void Configure(EntityTypeBuilder<CarDoor> builder)
    {
        var intArrayConverter = new ValueConverter<int[], string>(
                v => string.Join(";", v),
                v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => int.Parse(val)).ToArray());

        var intArrayComparer = new ValueComparer<int[]>(
                (c1, c2) => ReferenceEquals(c1, c2),
                c1 => c1.GetHashCode(),
                c1 => c1);

        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.Manufacturer)
               .HasMaxLength(30)
               .IsRequired();
        builder.Property(x => x.SillWidth);
        builder.Property(x => x.MinimalMountingSpace);
        builder.Property(x => x.DoorPanelWidth);
        builder.Property(x => x.DoorPanelSpace);
        builder.Property(x => x.DoorPanelCount);
        builder.Property(x => x.LiftDoorOpeningDirectionId);
        builder.Property(x => x.TypeExaminationCertificateId);
        builder.HasMany(t => t.LiftDoorGroups)
               .WithOne(g => g.CarDoor)
               .HasForeignKey(t => t.CarDoorId);
        builder.Property(x => x.CarDoorHeaderDepth)
               .HasConversion(intArrayConverter, intArrayComparer);
        builder.Property(x => x.CarDoorHeaderHeight)
               .HasConversion(intArrayConverter, intArrayComparer);
    }
}


