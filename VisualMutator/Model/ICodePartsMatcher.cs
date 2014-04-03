namespace VisualMutator.Model
{
    using Microsoft.Cci;

    public interface ICodePartsMatcher
    {
        bool Matches(IMethodReference method);
        bool Matches(ITypeReference typeReference);

    }
    public static class CodePartsMatcherExt
    {
        public static ICodePartsMatcher Join(this ICodePartsMatcher matcher, ICodePartsMatcher matcher2)
            {
                return new DelegatingMatcher(
                    reff => matcher.Matches(reff) && matcher2.Matches(reff),
                    reff => matcher.Matches(reff) && matcher2.Matches(reff));
            }
    }
}