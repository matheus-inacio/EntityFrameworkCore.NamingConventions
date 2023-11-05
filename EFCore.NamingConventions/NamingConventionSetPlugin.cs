using System.Globalization;
using EFCore.NamingConventions.Conventions;
using EFCore.NamingConventions.Interfaces;
using EFCore.NamingConventions.Rewriters;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace EFCore.NamingConventions;

public class NamingConventionSetPlugin(IDbContextOptions options) : IConventionSetPlugin
{
    public ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        var extension = options.FindExtension<NamingConventionsOptionsExtension>()!;

        if (extension.NamingConvention == NamingConvention.None)
        {
            return conventionSet;
        }

        var culture = extension.Culture  ?? CultureInfo.InvariantCulture;
        INameRewriter rewriter = extension.NamingConvention switch
        {
            NamingConvention.SnakeCase => new SnakeCaseNameRewriter(culture),
            NamingConvention.LowerCase => new LowerCaseNameRewriter(culture),
            NamingConvention.CamelCase => new CamelCaseNameRewriter(culture),
            NamingConvention.UpperCase => new UpperCaseNameRewriter(culture),
            NamingConvention.UpperSnakeCase => new UpperSnakeCaseNameRewriter(culture),
            _ => throw new ArgumentOutOfRangeException("Unhandled enum value: " + extension.NamingConvention)
        };

        conventionSet.EntityTypeAddedConventions.Add(new EntityTypeAddedConvention(rewriter));
        conventionSet.EntityTypeAnnotationChangedConventions.Add(new EntityTypeAnnotationChangedConvention(rewriter));
        conventionSet.PropertyAddedConventions.Add(new PropertyAddedConvention(rewriter));
        conventionSet.ForeignKeyOwnershipChangedConventions.Add(new ForeignKeyOwnershipChangedConvention(rewriter));
        conventionSet.KeyAddedConventions.Add(new KeyAddedConvention(rewriter));
        conventionSet.ForeignKeyAddedConventions.Add(new ForeignKeyAddedConvention(rewriter));
        conventionSet.IndexAddedConventions.Add(new IndexAddedConvention(rewriter));
        conventionSet.EntityTypeBaseTypeChangedConventions.Add(new EntityTypeBaseTypeChangedConvention(rewriter));
        conventionSet.ModelFinalizingConventions.Add(new ModelFinalizingConvention(rewriter));

        return conventionSet;
    }
}
