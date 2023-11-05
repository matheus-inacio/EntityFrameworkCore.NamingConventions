using EFCore.NamingConventions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EFCore.NamingConventions.Conventions;

public class ForeignKeyPropertiesChangedConvention(INameRewriter nameRewriter) : IForeignKeyPropertiesChangedConvention
{
    public void ProcessForeignKeyPropertiesChanged(
        IConventionForeignKeyBuilder relationshipBuilder,
        IReadOnlyList<IConventionProperty> oldDependentProperties,
        IConventionKey oldPrincipalKey,
        IConventionContext<IReadOnlyList<IConventionProperty>> context)
    {
        if (relationshipBuilder.Metadata.GetDefaultName() is { } constraintName)
        {
            relationshipBuilder.HasConstraintName(nameRewriter.RewriteName(constraintName));
        }
    }
}
