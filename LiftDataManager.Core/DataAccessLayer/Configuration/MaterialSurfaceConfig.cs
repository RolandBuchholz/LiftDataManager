namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class MaterialSurfaceConfig : BaseModelBuilder<MaterialSurface>
{
    public override void Configure(EntityTypeBuilder<MaterialSurface> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();
        builder.Property(x => x.CarMaterialFrontBackWalls);
        builder.Property(x => x.CarMaterialSideWalls);
        builder.Property(x => x.CarPanelMaterial);
        builder.Property(x => x.LiftDoorMaterial);
        builder.Property(x => x.ControlCabinetMaterial);
    }
}
