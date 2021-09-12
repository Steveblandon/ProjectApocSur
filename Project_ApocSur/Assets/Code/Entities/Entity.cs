namespace Projapocsur.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Serializable]
    public abstract class Entity
    {
        // Override this to complete any construction of the entity once it has been loaded and deserialized
        public virtual void PostLoad() { }
    }
}
