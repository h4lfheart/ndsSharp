using ndsSharp.Core.Data;

namespace ndsSharp.Core.Objects;

public abstract class BaseDeserializable
{
    public abstract void Deserialize(BaseReader reader);
}