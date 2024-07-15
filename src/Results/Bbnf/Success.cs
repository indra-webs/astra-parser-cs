using Indra.Astra.Rules;

namespace Indra.Astra {

  public partial class Parser {

    public abstract partial record Result {
      public abstract partial record Bbnf {
        public record Success
          : Bbnf {

          public override bool IsSuccess
            => true;

          public new IReadOnlyList<Rule> Values
            => base.Values!;

          internal Success(Grammar grammar, Rule.Parser.Success success)
            : base(grammar, success.Rules) { }
        }
      }
    }
  }
}