using UnityEngine;

public class InterpolatedTransformUpdater : MonoBehaviour
{
    private InterpolatedTransform interpolatedTransform;

    void OnEnable()
    {
        interpolatedTransform = GetComponent<InterpolatedTransform>();
    }

    void FixedUpdate()
    {
        interpolatedTransform.SetInterpolation();
    }
}