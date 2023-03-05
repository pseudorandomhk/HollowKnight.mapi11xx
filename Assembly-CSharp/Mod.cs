using Modding.Logging;

namespace Modding;

public abstract class Mod : Logger
{
    public virtual string Version => "UNKNOWN";

    protected Mod(string name) : base(name) { }

    public virtual void Initialize() { }
}