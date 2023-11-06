namespace EFCore.NamingConventions.Conventions;

public class IndexAddedConvention(INameRewriter nameRewriter) : IIndexAddedConvention
{
    public void ProcessIndexAdded(
        IConventionIndexBuilder indexBuilder,
        IConventionContext<IConventionIndexBuilder> context)
    {
        if (indexBuilder.Metadata.GetDefaultDatabaseName() is { } indexName)
        {
            indexBuilder.HasDatabaseName(nameRewriter.RewriteName(indexName));
        }
    }
}
