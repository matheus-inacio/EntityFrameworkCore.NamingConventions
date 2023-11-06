namespace EFCore.NamingConventions.Conventions;

public class KeyAddedConvention(INameRewriter nameRewriter) : IKeyAddedConvention
{
    public void ProcessKeyAdded(
        IConventionKeyBuilder keyBuilder,
        IConventionContext<IConventionKeyBuilder> context)
    {
        if (keyBuilder.Metadata.GetName() is { } keyName)
        {
            keyBuilder.HasName(nameRewriter.RewriteName(keyName));
        }
    }
}
