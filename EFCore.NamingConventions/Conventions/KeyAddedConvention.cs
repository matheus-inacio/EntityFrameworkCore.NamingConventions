using EFCore.NamingConventions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

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
