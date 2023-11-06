namespace EFCore.NamingConventions.Rewriters;

public class UpperSnakeCaseNameRewriter(CultureInfo culture) : SnakeCaseNameRewriter(culture)
{
    public override string RewriteName(string name)
        => base.RewriteName(name).ToUpper(culture);
}
