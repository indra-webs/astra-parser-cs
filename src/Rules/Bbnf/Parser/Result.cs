namespace Indra.Astra.Rules
{
  public partial class Rule
  {
    public static partial class Parser
    {
#endregion

      public abstract record Result
      {
        public Grammar? Grammar { get; }
        public Rule[]? Rules { get; }
        internal Result(
          Grammar? grammar,
          Rule[] rules
        ) => (Grammar, Rules) = (grammar, rules);
      }
    }
  }
}