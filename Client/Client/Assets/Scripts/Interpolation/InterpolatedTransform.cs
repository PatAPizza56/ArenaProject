using UnityEngine;

[RequireComponent(typeof(InterpolatedTransformUpdater))]
public class InterpolatedTransform : MonoBehaviour
{
    TransformData[] previousTransforms = new TransformData[2];
    int transformIndex = 0;

    void OnEnable()
    {
        ResetInterpolation();
    }

    void Update()
    {
        SetTransform();
    }

    void FixedUpdate()
    {
        SetFixedTransform();
    }

    public void SetInterpolation()
    {
        transformIndex = transformIndex == 0 ? 1 : 0;
        previousTransforms[transformIndex] = new TransformData(transform.localPosition, transform.localRotation, transform.localScale);
    }

    public void ResetInterpolation()
    {
        previousTransforms = new TransformData[2];
        TransformData t = new TransformData(transform.localPosition, transform.localRotation, transform.localScale);

        previousTransforms[0] = t;
        previousTransforms[1] = t;
        transformIndex = 0;
    }

    void SetTransform()
    {
        TransformData newestTransform = previousTransforms[transformIndex];
        TransformData olderTransform = previousTransforms[transformIndex == 0 ? 1 : 0];

        transform.localPosition = Vector3.Lerp(olderTransform.position, newestTransform.position, InterpolationController.InterpolationFactor);
        transform.localRotation = Quaternion.Slerp(olderTransform.rotation, newestTransform.rotation, InterpolationController.InterpolationFactor);
        transform.localScale = Vector3.Lerp(olderTransform.scale, newestTransform.scale, InterpolationController.InterpolationFactor);
    }

    void SetFixedTransform()
    {
        TransformData newestTransform = previousTransforms[transformIndex];

        transform.localPosition = newestTransform.position;
        transform.localRotation = newestTransform.rotation;
        transform.localScale = newestTransform.scale;
    }

    private struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
}