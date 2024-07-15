using Indra.Astra.Rules;

namespace Indra.Astra {
  public partial class Parser {
    public abstract partial record Result {
      public abstract partial record Bbnf {
        public record Failure
          : Bbnf {

          public override bool IsSuccess
            => false;

          public Exception Exception { get; }

          internal Failure(Grammar grammar, Rule.Parser.Failure failure)
            : base(grammar, failure.Rules)
            => Exception = failure.Exception;
        }
      }
    }
  }
}