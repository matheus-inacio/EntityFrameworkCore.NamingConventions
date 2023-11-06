using EFCore.NamingConventions.Extensions;

namespace EFCore.NamingConventions.Conventions;

public class EntityTypeAnnotationChangedConvention(INameRewriter nameRewriter)
    : IEntityTypeAnnotationChangedConvention
{
    public void ProcessEntityTypeAnnotationChanged(
        IConventionEntityTypeBuilder entityTypeBuilder,
        string name,
        IConventionAnnotation? annotation,
        IConventionAnnotation? oldAnnotation,
        IConventionContext<IConventionAnnotation> context)
    {
        var entityType = entityTypeBuilder.Metadata;

        // If the View/SqlQuery/Function name is being set on the entity type, and its table name is set by convention, then we assume
        // we're the one who set the table name back when the entity type was originally added. We now undo this as the entity type
        // should only be mapped to the View/SqlQuery/Function.
        if (name is RelationalAnnotationNames.ViewName or RelationalAnnotationNames.SqlQuery or RelationalAnnotationNames.FunctionName
            && annotation?.Value is not null
            && entityType.GetTableNameConfigurationSource() == ConfigurationSource.Convention)
        {
            entityType.SetTableName(null);
        }

        if (name != RelationalAnnotationNames.TableName
            || StoreObjectIdentifier.Create(entityType, StoreObjectType.Table) is not { } tableIdentifier)
        {
            return;
        }

        // The table's name is changing - rewrite keys, index names

        if (entityType.FindPrimaryKey() is { } primaryKey)
        {
            // We need to rewrite the PK name.
            // However, this isn't yet supported with TPT, see https://github.com/dotnet/efcore/issues/23444.
            // So we need to check if the entity is within a TPT hierarchy, or is an owned entity within a TPT hierarchy.

            var rootType = entityType.GetRootType();
            var isTPT = rootType.GetDerivedTypes().FirstOrDefault() is { } derivedType
                        && derivedType.GetTableName() != rootType.GetTableName();

            if (entityType.FindRowInternalForeignKeys(tableIdentifier).FirstOrDefault() is null && !isTPT)
            {
                if (primaryKey.GetDefaultName() is { } primaryKeyName)
                {
                    primaryKey.Builder.HasName(nameRewriter.RewriteName(primaryKeyName));
                }
            }
            else
            {
                // This hierarchy is being transformed into TPT via the explicit setting of the table name.
                // We not only have to reset our own key name, but also the parents'. Otherwise, the parent's key name
                // is used as the child's (see RelationalKeyExtensions.GetName), and we get a "duplicate key name in database" error
                // since both parent and child have the same key name;
                foreach (var type in entityType.GetRootType().GetDerivedTypesInclusive())
                {
                    if (type.FindPrimaryKey() is { } pk)
                    {
                        pk.Builder.HasNoAnnotation(RelationalAnnotationNames.Name);
                    }
                }
            }
        }

        foreach (var foreignKey in entityType.GetForeignKeys())
        {
            if (foreignKey.GetDefaultName() is { } foreignKeyName)
            {
                foreignKey.Builder.HasConstraintName(nameRewriter.RewriteName(foreignKeyName));
            }
        }

        foreach (var index in entityType.GetIndexes())
        {
            if (index.GetDefaultDatabaseName() is { } indexName)
            {
                index.Builder.HasDatabaseName(nameRewriter.RewriteName(indexName));
            }
        }

        if (annotation?.Value is not null
            && entityType.FindOwnership() is { } ownership
            && (string)annotation.Value != ownership.PrincipalEntityType.GetTableName())
        {
            // An owned entity's table is being set explicitly -
            // this is the trigger to undo table splitting (which is the default).

            // When the entity became owned, we prefixed all of
            // its properties - we must now undo that.
            foreach (var property in entityType.GetProperties()
                         .Except(entityType.FindPrimaryKey()?.Properties ?? Array.Empty<IConventionProperty>())
                         .Where(p => p.Builder.CanSetColumnName(null)))
            {
                property.Builder.RewriteColumnName(nameRewriter);
            }

            // We previously rewrote the owned entity's primary key name, when the
            // owned entity was still in table splitting.
            // Now that its getting its own table, rewrite the primary key constraint
            // name again.
            if (entityType.FindPrimaryKey() is { } key&& key.GetDefaultName() is { } keyName)
            {
                key.Builder.HasName(nameRewriter.RewriteName(keyName));
            }
        }
    }
}