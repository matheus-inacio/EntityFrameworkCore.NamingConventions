namespace EFCore.NamingConventions.Rewriters;

public class CamelCaseNameRewriter(CultureInfo culture) : INameRewriter
{
    public string RewriteName(string name)
        => string.IsNullOrEmpty(name) ? name: char.ToLower(name[0], culture) + name[1..];
}
