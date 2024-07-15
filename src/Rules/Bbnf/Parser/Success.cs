namespace Indra.Astra.Rules
{
  public partial class Rule
  {
    public static partial class Parser
    {
#endregion

      public record Success
  : Result
      {
        public new Rule[] Rules
          => base.Rules!;

        internal Success(
          Grammar? grammar,
          Rule[] rules
        ) : base(grammar, rules) { }
      }
    }
  }
}