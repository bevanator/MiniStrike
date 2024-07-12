using UnityEngine;

namespace ProjectC.Constants
{
    public static class AnimHash
    {
        public static readonly int EquipGrenade = Animator.StringToHash("EquipGrenade");
        public static readonly int Running = Animator.StringToHash("IsRunning");
        public static readonly int Throw = Animator.StringToHash("Throw");
        public static readonly int EquipGun = Animator.StringToHash("EquipGun");
        public static readonly int BlendX = Animator.StringToHash("BlendX");
        public static readonly int BlendY = Animator.StringToHash("BlendY");
        public static readonly int Reload = Animator.StringToHash("Reload");
        public static readonly int Death = Animator.StringToHash("Death");
    }
}