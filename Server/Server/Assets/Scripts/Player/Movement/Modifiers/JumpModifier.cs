using UnityEngine;

public class JumpModifier : MonoBehaviour, IMovementModifier
{
    public Vector3 Modify(ModifierInfo info, PlayerMoverConfig config)
    {
        if (info.Input.JumpInput < 1f) return Vector3.zero;

        if (!info.IsGrounded) return Vector3.zero;
        if (info.IsCrouching) return Vector3.zero;

        return new Vector3(0, Mathf.Sqrt(config.jumpForce * 2f * -config.gravity), 0);
    }
}