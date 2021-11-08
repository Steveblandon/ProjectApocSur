namespace Projapocsur.Scripts
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.World;

    public interface IBodyPartsViewManager
    {
        event Action<BodyPart> OnSelectedBodyPartChangeEvent;

        BodyPart CurrentSelected { get; }

        void SetBodyParts(IReadOnlyCollection<BodyPart> bodyParts);
    }
}
