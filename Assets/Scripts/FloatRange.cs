using UnityEngine;

[System.Serializable]
public struct FloatRange
{
    public float Min;
    public float Max;

    public FloatRange(float min, float max)
    {
        Min = min;
        Max = max;
    }

    public float RandomInRange()
    {
        return Random.Range(Min, Max);
    }
}