using UnityEngine;

namespace EGF.UI
{
    public class BillboardFacing : MonoBehaviour
    {
        private Camera _mainCam;
        private Quaternion _originalRotation;
        
        void Start()
        {
            _originalRotation = transform.rotation;
            _mainCam = Camera.main;
        }

        void LateUpdate()
        {
            if (!_mainCam) _mainCam = Camera.main;
            if(!_mainCam) return;
            transform.rotation = _mainCam.transform.rotation; 
        }
    }
}