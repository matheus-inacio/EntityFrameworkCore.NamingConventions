using EFCore.NamingConventions.Extensions;
using EFCore.NamingConventions.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

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
