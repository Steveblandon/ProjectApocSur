﻿namespace Projapocsur.World
{
    using System;

    public interface IRangedWeapon
    {
        event Action WeaponFiredEvent;

        event Action WeaponReloadedEvent;

        int Ammo { get; }

        int EffectiveRange { get; }

        void FireProjectile();

        void Reload();
    }
}
