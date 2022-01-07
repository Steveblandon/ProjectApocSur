namespace Projapocsur.World
{
    using System.Collections.Generic;

    public class RelationsTracker
    {
        public ISet<string> HostileIndividualsById { get; } = new HashSet<string>();

        public ISet<string> FriendlyIndividualsById { get; } = new HashSet<string>();

        // when working on factions system, add additional properties for **FactionsById... each creature should contain its own relationsTracker
    }
}