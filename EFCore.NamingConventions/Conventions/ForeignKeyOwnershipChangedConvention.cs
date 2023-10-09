using EFCore.NamingConventions.Extensions;
using EFCore.NamingConventions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EFCore.NamingConventions.Conventions;

public class ForeignKeyOwnershipChangedConvention(INameRewriter nameRewriter)
    : IForeignKeyOwnershipChangedConvention
{
    public void ProcessForeignKeyOwnershipChanged(
        IConventionForeignKeyBuilder relationshipBuilder,
        IConventionContext<bool?> context)
    {
        var foreignKey = relationshipBuilder.Metadata;

        if (!foreignKey.IsOwnership)
        {
            return;
        }

        var ownedEntityType = foreignKey.DeclaringEntityType;

        if (!foreignKey.IsUnique && string.IsNullOrEmpty(ownedEntityType.GetContainerColumnName()))
        {
            return;
        }

        // An entity type is becoming owned - this is a bit complicated.
        // This is a trigger for table splitting - unless the foreign key
        // is non-unique (collection navigation), it's JSON ownership, or the
        // owned entity table name was explicitly set by the user.
        // If this is table splitting, we need to undo rewriting which we've
        // done previously.
        if (ownedEntityType.GetTableNameConfigurationSource() == ConfigurationSource.Explicit)
        {
            return;
        }

        // Reset the table name which we've set when the entity type was added.
        // If table splitting was configured by explicitly setting the table
        // name, the following does nothing.
        ownedEntityType.Builder.HasNoAnnotation(RelationalAnnotationNames.TableName);
        ownedEntityType.Builder.HasNoAnnotation(RelationalAnnotationNames.Schema);

        ownedEntityType.FindPrimaryKey()?.Builder.HasNoAnnotation(RelationalAnnotationNames.Name);

        // We've previously set rewritten column names when the entity was
        // originally added (before becoming owned). These need to be rewritten
        // again to include the owner prefix.
        foreach (var property in ownedEntityType.GetProperties())
        {
            property.Builder.RewriteColumnName(nameRewriter);
        }
    }
}
