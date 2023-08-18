using UnityEngine;
using DG.Tweening;

namespace IGJ.SIMP.Runtime.AI
{
    public class ThrowableGalonBehaviour : MonoBehaviour
    {
        Transform self;
        Transform target;

        public void LaunchToTarget(Transform t, System.Action v)
        {
            if (self == null)
                self = transform;

            target = t;

            self.DOJump(target.localPosition, 6f, 1, 1f).OnComplete(() => { v?.Invoke(); });
        }
    }
}
