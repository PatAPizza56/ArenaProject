using UnityEngine;

public class AccelerationModifier : MonoBehaviour, IMovementModifier
{
    public Vector3 Modify(ModifierInfo info, PlayerMoverConfig config)
    {
        Vector3 wishDir = transform.right * info.Input.HorizontalInput + transform.forward * info.Input.VerticalInput;

        Vector3 wishVel = wishDir * info.CurrentMaxMoveSpeed;
        Vector3 newHorVel;

        if (info.IsGrounded)
        {
            newHorVel = Vector3.ClampMagnitude(info.CurrentHorizontalVelocity + wishVel * config.groundAcceleration * Time.fixedDeltaTime, info.CurrentMaxMoveSpeed);
        }
        else
        {
            float speed = Mathf.Clamp(info.CurrentMaxMoveSpeed - Vector3.Dot(info.CurrentHorizontalVelocity, wishVel), 0f, config.airAcceleration * Time.fixedDeltaTime);
            Vector3 horVelAfterAcceleration = info.CurrentHorizontalVelocity + wishVel * speed;

            newHorVel = Vector3.ClampMagnitude(horVelAfterAcceleration, Mathf.Max(info.CurrentHorizontalVelocity.magnitude, info.CurrentMaxMoveSpeed));
        }

        return new Vector3(newHorVel.x, 0, newHorVel.z) - info.CurrentHorizontalVelocity;
    }
}