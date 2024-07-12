using System;
using DG.Tweening;
using ProjectC.Constants;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace PlayerController
{
    public class AnimationHandler : MonoBehaviour
    {
        private Animator _animator;
        private RigBuilder _rigBuilder;
        private Tweener _floatTween;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _rigBuilder = GetComponentInChildren<RigBuilder>();
        }

        public void PlayKnockBack()
        {
            Rig aimRig = _rigBuilder.layers[0].rig;
            _floatTween.Kill();
            aimRig.weight = 1f;
            aimRig.weight = 0f;
            _floatTween = DOVirtual.Float(0, 1f, 0.3f, value => aimRig.weight = value);
        }

        public void PlayGunEquipAnim(int layerIndex)
        {
            ResetAllWeaponRigs();
            _animator.SetLayerWeight(1,1f);
            _animator.SetTrigger(AnimHash.EquipGun);
            //TODO: change logic later
            RigLayer weaponRig = _rigBuilder.layers[layerIndex+1];
            DOVirtual.Float(0, 1f, 0.2f, value => weaponRig.rig.weight = value)
                .SetDelay(0.4f);
        }

        public void PlayThrowableEquipAnim()
        {
            _animator.SetLayerWeight(1,1f);
            _animator.SetTrigger(AnimHash.EquipGrenade);
            ResetAllWeaponRigs();
        }

        public void ResetAllWeaponRigs()
        {
            for (var i = 1; i < _rigBuilder.layers.Count; i++)
            {
                RigLayer layer = _rigBuilder.layers[i];
                layer.rig.weight = 0f;
            }
        }

        public void SetRunningState(bool state)
        {
            _animator.SetBool(AnimHash.Running, state);
        }

        public void PlayThrowAnim()
        {
            _animator.SetTrigger(AnimHash.Throw);
        }

        public void UpdateLocomotionBlends(float blendX, float blendY)
        {
            _animator.SetFloat(AnimHash.BlendX, blendX);
            _animator.SetFloat(AnimHash.BlendY, blendY);;
        }

        public void PlayReloadAnim()
        {
            _animator.SetTrigger(AnimHash.Reload);
        }

        public void PlayDeathAnim()
        {
            _rigBuilder.enabled = false;
            _animator.SetLayerWeight(1, 0f);
            _animator.SetLayerWeight(2, 0f);
            _animator.SetTrigger(AnimHash.Death);
        }
        
    }
}