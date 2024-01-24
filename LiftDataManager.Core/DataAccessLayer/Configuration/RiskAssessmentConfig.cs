namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class RiskAssessmentConfig : BaseModelBuilder<RiskAssessment>
{
    public override void Configure(EntityTypeBuilder<RiskAssessment> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(20)
               .IsRequired();
        builder.Property(x => x.VaultDocument)
               .IsRequired();
        builder.Property(x => x.Description)
               .IsRequired();
    }
}