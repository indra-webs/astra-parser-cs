namespace Indra.Astra {
  public partial class Parser {
    public abstract partial record Result {
      public abstract record AstrA
        : Result {

        internal AstrA(Grammar grammar)
          : base(grammar) { }
      }
    }
  }
}