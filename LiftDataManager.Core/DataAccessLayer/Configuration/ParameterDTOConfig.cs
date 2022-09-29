using static LiftDataManager.Core.Models.ParameterBase;

namespace LiftDataManager.Core.DataAccessLayer.Configuration;

public class ParameterDTOConfig : BaseModelBuilder<ParameterDTO>
{
    public override void Configure(EntityTypeBuilder<ParameterDTO> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.ParameterTyp)
                         .IsRequired()
                         .HasConversion(
                          v => v.ToString(),
                          v => (ParameterTypValue)Enum.Parse(typeof(ParameterTypValue), v));
        builder.Property(x => x.ParameterCategory)
                 .IsRequired()
                 .HasConversion(
                 v => v.ToString(),
                 v => (ParameterCategoryValue)Enum.Parse(typeof(ParameterCategoryValue), v));
        builder.Property(x => x.TypeCode)
                 .IsRequired()
                 .HasConversion(
                 v => v.ToString(),
                 v => (TypeCodeValue)Enum.Parse(typeof(TypeCodeValue), v));
        builder.Property(x => x.Name)
                 .HasMaxLength(50)
                 .IsRequired();
        builder.Property(x => x.DisplayName)
                 .HasMaxLength(50)
                 .IsRequired();
        builder.Property(x => x.Value);
        builder.Property(x => x.IsKey);
        builder.Property(x => x.DefaultUserEditable);
        builder.Property(x => x.Comment)
                 .IsRequired()
                 .HasMaxLength(50);
        //parameter.Property(x => x.DropDownList);
    }
}
