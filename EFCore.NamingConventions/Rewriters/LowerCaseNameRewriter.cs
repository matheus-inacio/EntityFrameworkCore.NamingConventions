namespace EFCore.NamingConventions.Rewriters;

public class LowerCaseNameRewriter(CultureInfo culture) : INameRewriter
{
    public string RewriteName(string name) => name.ToLower(culture);
}
