using Indra.Astra.Rules;

namespace Indra.Astra {

  public partial class Parser {

    public abstract partial record Result {
      public abstract partial record Bbnf
        : Result {

        public IReadOnlyList<Rule>? Values { get; }

        internal Bbnf(Grammar grammar, IReadOnlyList<Rule>? values)
          : base(grammar)
          => Values = values;
      }
    }
  }
}