namespace Indra.Astra {

  public partial class Grammar {
    public static class Sources {
      public const string Default
        = "default";

      public static Source Shared { get; }
        = new("./rules/shared", "shared");

      public static Source Astra { get; }
        = new("./rules/axa", "astra");

      public static Source Markup { get; }
        = new("./rules/mup", "mup");

      public static Source XLLogic { get; }
        = new("./rules/xll", "xll");

      public static Source Prox { get; }
        = new("./rules/prx", "prox");

      public static Source Strux { get; }
        = new("./rules/stx", "stx");

      public static Source Blox { get; }
        = new("./rules/blx", "blx");
    }
  }
}