namespace Indra.Astra.Rules {
  public partial class Rule {
    public static partial class Parser {

      public record Failure
        : Result {

        public Exception Exception { get; }
        internal Failure(
          Grammar? grammar,
          Exception exception,
          Rule[] rules
        ) : base(grammar, rules)
          => Exception = exception;
      }
    }
  }
}