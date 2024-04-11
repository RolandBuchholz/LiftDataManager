namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class SafetyComponentTypConfig : BaseModelBuilder<SafetyComponentTyp>
{
    public override void Configure(EntityTypeBuilder<SafetyComponentTyp> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Name)
               .HasMaxLength(20)
               .IsRequired();
        builder.HasMany(t => t.TypeExaminationCertificates)
               .WithOne(g => g.SafetyComponentTyp)
               .HasForeignKey(t => t.SafetyComponentTypId);
    }
}
