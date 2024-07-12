using Unity.Cinemachine;
using UnityEngine;

namespace Other
{
    public class CameraController : MonoBehaviour
    {
        private static CameraController _instance;
        [SerializeField] private CinemachineFollow m_FollowChar;
        [SerializeField] private CinemachineCamera m_CinemachineCamera;
        [SerializeField] private Transform m_Player;
        [SerializeField] private Transform m_Target;
        bool _isPlayer;

        public void SwitchCameraMode()
        {
            _isPlayer = !_isPlayer;
            var newTarget = _isPlayer ? m_Player : m_Target;
            m_CinemachineCamera.Target.TrackingTarget = newTarget;
        }

        private void Awake()
        {
            _instance = this;
        }

        public static void SwitchCamera(float value)
        {
            _instance.m_FollowChar.FollowOffset = value * (Vector3.up + Vector3.forward);
        }
        
    }
}