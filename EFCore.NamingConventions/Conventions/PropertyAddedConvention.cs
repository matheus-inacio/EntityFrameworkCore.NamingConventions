using EFCore.NamingConventions.Extensions;

namespace EFCore.NamingConventions.Conventions;

public class PropertyAddedConvention(INameRewriter nameRewriter) : IPropertyAddedConvention
{
    public virtual void ProcessPropertyAdded(
        IConventionPropertyBuilder propertyBuilder,
        IConventionContext<IConventionPropertyBuilder> context)
    {
        propertyBuilder.RewriteColumnName(nameRewriter);
    }
}
