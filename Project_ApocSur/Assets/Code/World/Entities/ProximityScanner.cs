namespace Projapocsur.World
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ProximityScanner
    {
        private ITargetable origin;

        public ProximityScanner(ITargetable origin)
        {
            this.origin = origin;
        }

        public ITargetable GetNearestTargetByIdWithinRadius(ISet<string> idsToLookFor, float scanRadius)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(this.origin.Position, scanRadius, Vector2.zero);
            ITargetable nearestTarget = null;
            float nearestTargetDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent(out ITargetable target) && idsToLookFor.Contains(target.UniqueID))
                {
                    // NOTE: since the cast is stationary (no direction), hits won't actually be sorted by distance
                    // because in this case everything is at a distance of zero. Therefore we have to do our own distance checking.
                    float targetDistance = Vector3.Distance(origin.Position, target.Position);
                    
                    if (targetDistance < nearestTargetDistance)
                    {
                        nearestTarget = target;
                        nearestTargetDistance = targetDistance;
                    }
                }
            }

            return nearestTarget;
        }

        public bool IsTargetWithinDistance(ITargetable target, float distance) => target != null && Vector3.Distance(origin.Position, target.Position) < distance;
    }
}