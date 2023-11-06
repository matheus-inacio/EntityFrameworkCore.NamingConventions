namespace EFCore.NamingConventions.Conventions;

public class EntityTypeBaseTypeChangedConvention(INameRewriter nameRewriter) : IEntityTypeBaseTypeChangedConvention
{
    public void ProcessEntityTypeBaseTypeChanged(
        IConventionEntityTypeBuilder entityTypeBuilder,
        IConventionEntityType? newBaseType,
        IConventionEntityType? oldBaseType,
        IConventionContext<IConventionEntityType> context)
    {
        var entityType = entityTypeBuilder.Metadata;

        if (newBaseType is null || entityType.GetMappingStrategy() == RelationalAnnotationNames.TpcMappingStrategy)
        {
            // The entity is getting removed from a hierarchy. Set the (rewritten) TableName.
            if (entityType.GetTableName() is { } tableName && !entityType.ClrType.IsAbstract)
            {
                entityTypeBuilder.ToTable(nameRewriter.RewriteName(tableName), entityType.GetSchema());
            }
        }
        else
        {
            // The entity is getting a new base type (e.g. joining a hierarchy).
            // If this is TPH, we remove the previously rewritten TableName
            // (and non-rewritten Schema) which we set when the entity type
            // was first added to the model (see ProcessEntityTypeAdded).

            // If this is TPT, TableName and Schema are set explicitly,
            // so the following will be ignored. TPC is handled above
            // (we need to rewrite just like with a normal table that
            // isn't in an inheritance hierarchy)
            entityTypeBuilder.HasNoAnnotation(RelationalAnnotationNames.TableName);
            entityTypeBuilder.HasNoAnnotation(RelationalAnnotationNames.Schema);
        }
    }
}
