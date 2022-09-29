namespace LiftDataManager.Core.DataAccessLayer.Configuration.AllgemeineDaten;

public class ElevatorStandardConfig : BaseModelBuilder<ElevatorStandard>
{
        public override void Configure(EntityTypeBuilder<ElevatorStandard> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.Name)
                        .HasMaxLength(50)
                        .IsRequired();
        }
}
