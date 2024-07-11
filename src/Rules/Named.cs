namespace Indra.Astra.Rules {
  public class Named
    : Custom {

    public override bool IsPartial
      => false;

    protected internal Named(string name, Rule rule)
      : base(name, rule) { }
  }
}