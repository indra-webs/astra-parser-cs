namespace Indra.Astra {
  public partial class Parser {
    public abstract partial record Result {

      public Grammar Grammar { get; }

      public abstract bool IsSuccess { get; }

      internal Result(Grammar grammar)
        => Grammar = grammar;
    }
  }
}