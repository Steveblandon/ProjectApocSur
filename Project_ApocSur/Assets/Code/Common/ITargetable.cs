namespace Projapocsur
{
    using UnityEngine;

    public interface ITargetable
    {
        string UniqueID { get; set; }

        string FactionID { get; set; }

        Vector3 Position { get; }

        Vector3 Size { get; }
    }
}
