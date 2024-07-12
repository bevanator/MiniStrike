using UnityEngine;

namespace ProjectC.Armory
{
    public interface IThrowable
    {
        int CurrentClipCount { get; }
        public void Throw(Quaternion transformRotation, float power);
        public int UpdateClip();

        public void AddClip();
    }
}