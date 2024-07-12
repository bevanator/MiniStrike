
using UnityEngine;

namespace ProjectC.Armory.Interfaces
{
    public interface IShooting
    {
        public void Shoot(Vector3 forward, float damageMultiplier = 1f, float range = 50, DamagerType damagerType = DamagerType.Player);
        public int UpdateClipCount();
        int CurrentClipCount { get; }
        int RemainingClipSize { get; }
        public void RefillAmmo(bool maxClip = false);
    }
}