using UnityEngine;

public class InterpolationController : MonoBehaviour
{
    float[] fixedUpdateTimes = new float[2];
    int timeIndex = 0;

    public static float InterpolationFactor { get; private set; }

    void FixedUpdate()
    {
        SetTimeIndex();
    }

    void Update()
    {
        SetInterpolationFactor();
    }

    void SetTimeIndex()
    {
        timeIndex = timeIndex == 0 ? 1 : 0;
        fixedUpdateTimes[timeIndex] = Time.fixedTime;
    }

    void SetInterpolationFactor()
    {
        float newerTime = fixedUpdateTimes[timeIndex];
        float olderTime = fixedUpdateTimes[timeIndex == 0 ? 1 : 0];

        if (newerTime != olderTime)
        {
            InterpolationFactor = (Time.time - newerTime) / (newerTime - olderTime);
        }
        else
        {
            InterpolationFactor = 1;
        }
    }
}