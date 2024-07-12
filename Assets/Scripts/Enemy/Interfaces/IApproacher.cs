using UnityEngine;

namespace Enemy.Interfaces
{
    public interface IApproacher
    {
        public void MoveToTarget(Vector3 target);
    }
}