namespace EFCore.NamingConventions.Rewriters;

public class UpperCaseNameRewriter(CultureInfo culture) : INameRewriter
{
    public string RewriteName(string name) => name.ToUpper(culture);
}
