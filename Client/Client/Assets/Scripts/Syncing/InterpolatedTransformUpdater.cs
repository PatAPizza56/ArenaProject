using UnityEngine;

public class InterpolatedTransformUpdater : MonoBehaviour
{
    InterpolatedTransform interpolatedTransform = null;

    void OnEnable()
    {
        interpolatedTransform = GetComponent<InterpolatedTransform>();
    }

    void FixedUpdate()
    {
        interpolatedTransform.SetInterpolation();
    }
}