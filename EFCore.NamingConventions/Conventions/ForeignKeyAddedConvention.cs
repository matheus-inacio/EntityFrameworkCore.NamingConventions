using EFCore.NamingConventions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EFCore.NamingConventions.Conventions;

public class ForeignKeyAddedConvention(INameRewriter nameRewriter) : IForeignKeyAddedConvention
{
    public void ProcessForeignKeyAdded(
        IConventionForeignKeyBuilder foreignKeyBuilder,
        IConventionContext<IConventionForeignKeyBuilder> context)
    {
        if (foreignKeyBuilder.Metadata.GetDefaultName() is { } constraintName)
        {
            foreignKeyBuilder.HasConstraintName(nameRewriter.RewriteName(constraintName));
        }
    }
}
