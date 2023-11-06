namespace EFCore.NamingConventions.Conventions;

public class EntityTypeAddedConvention(INameRewriter nameRewriter) : IEntityTypeAddedConvention
{
    public virtual void ProcessEntityTypeAdded(
        IConventionEntityTypeBuilder entityTypeBuilder,
        IConventionContext<IConventionEntityTypeBuilder> context)
    {
        var entityType = entityTypeBuilder.Metadata;

        // Note that the base type is null when the entity type is first added - a base type only gets added later
        // (see ProcessEntityTypeBaseTypeChanged). But we still have this check for safety.
        if (entityType.BaseType is not null || entityType.ClrType.IsAbstract)
        {
            return;
        }

        if (entityType.GetTableName() is { } tableName)
        {
            entityTypeBuilder.ToTable(nameRewriter.RewriteName(tableName), entityType.GetSchema());
        }

        if (entityType.GetViewNameConfigurationSource() == ConfigurationSource.Convention
            && entityType.GetViewName() is { } viewName)
        {
            entityTypeBuilder.ToView(nameRewriter.RewriteName(viewName), entityType.GetViewSchema());
        }
    }
}
