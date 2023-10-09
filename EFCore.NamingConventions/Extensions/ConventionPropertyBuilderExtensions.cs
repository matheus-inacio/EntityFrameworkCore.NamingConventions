using EFCore.NamingConventions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.NamingConventions.Extensions;

public static class ConventionPropertyBuilderExtensions
{
    internal static void RewriteColumnName(this IConventionPropertyBuilder propertyBuilder, INameRewriter nameRewriter)
    {
        var property = propertyBuilder.Metadata;
        var entityType = property.DeclaringType;

        // Remove any previous setting of the column name
        // we may have done, so we can get the default
        // recalculated below.
        property.Builder.HasNoAnnotation(RelationalAnnotationNames.ColumnName);

        // TODO: The following is a temporary hack. We should probably just always set the relational override below,
        // but https://github.com/dotnet/efcore/pull/23834
        string? baseColumnName;
        if (StoreObjectIdentifier.Create(property.DeclaringType, StoreObjectType.Table) is { } tableIdentifier)
        {
            baseColumnName = property.GetDefaultColumnName(tableIdentifier);
        }
        else
        {
            baseColumnName = property.GetDefaultColumnName();
        }

        if (baseColumnName is not null)
        {
            propertyBuilder.HasColumnName(nameRewriter.RewriteName(baseColumnName));
        }

        foreach (var storeObjectType in _storeObjectTypes)
        {
            var identifier = StoreObjectIdentifier.Create(entityType, storeObjectType);
            if (identifier is null)
            {
                continue;
            }

            if (property.GetColumnNameConfigurationSource(identifier.Value) != ConfigurationSource.Convention)
            {
                continue;
            }

            if (property.GetColumnName(identifier.Value) is { } columnName)
            {
                propertyBuilder.HasColumnName(nameRewriter.RewriteName(columnName), identifier.Value);
            }
        }
    }

    private static readonly StoreObjectType[] _storeObjectTypes =
    {
        StoreObjectType.Table,
        StoreObjectType.View,
        StoreObjectType.Function,
        StoreObjectType.SqlQuery
    };
}
