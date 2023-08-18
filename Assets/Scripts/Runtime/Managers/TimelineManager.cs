using UnityEngine;
using UnityEngine.Playables;

namespace IGJ.SIMP.Runtime.Managers
{
    public class TimelineManager : MonoBehaviour
    {
        [System.Serializable]
        public struct TimelineManagerStruct
        {
            public string TimelineName;
            public PlayableAsset Playable;

            public TimelineManagerStruct(string timelineName, PlayableAsset playable)
            {
                TimelineName = timelineName;
                Playable = playable;
            }
        }

        [SerializeField] TimelineManagerStruct[] timelineList;

        public PlayableAsset GetPlayableByName(string name)
        {
            if (System.Array.Find(timelineList, x => x.TimelineName == name).Playable == null) return null;
            return System.Array.Find(timelineList, x => x.TimelineName == name).Playable;
        }
    }
}
