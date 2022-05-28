using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "FingerBend", menuName = "ScriptableObjects/Hand Finger Bend", order = 80)]
    public class FingerBend : ScriptableObject
    {
        public enum FingersId
        {
            ThumbFingerId = 0,
            IndexFingerId = 1,
            MiddleFingerId = 2,
            RingFingerId = 3,
            PinkyFingerId = 4
        }
        
        [SerializeField] private float thumb = 0.03f;
        [SerializeField] private float index = 0f;
        [SerializeField] private float middle = 0f;
        [SerializeField] private float ring = 0f;
        [SerializeField] private float pinky = 0f;

        public float GetBendById(int fingerId)
        {
            switch (fingerId)
            {
                case (int)FingersId.ThumbFingerId:
                    return thumb;
                case (int)FingersId.IndexFingerId:
                    return index;
                case (int)FingersId.MiddleFingerId:
                    return middle;
                case (int)FingersId.RingFingerId:
                    return ring;
                case (int)FingersId.PinkyFingerId:
                    return pinky;
                default:
                    Debug.LogError("Unsupport finger id " + fingerId, this);
                    return 0;
            }
        }
    }
}