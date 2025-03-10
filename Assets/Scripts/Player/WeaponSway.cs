using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] private Transform weaponTransform;

    [Header("Sway Properties")]
    [SerializeField] private float swaySmooth = 20f;
    [SerializeField] private AnimationCurve swayCurve;
    [SerializeField] private float posToRotAmount = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float swaySmoothCounteraction = 1f;

    [Header("Rotation Sway")]
    [SerializeField] private float rotationSwayMultiplier = -1f;
    [SerializeField] private float maxRotSwayAmount = 5f;

    [Header("Position Sway")]
    [SerializeField] private float movementSwayMultiplier = -0.1f;
    [SerializeField] private float maxPosSwayAmount = 0.01f;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private Vector2 _rotSway;
    private Vector3 _posSway;
    private Quaternion _lastRotation;
    private Vector3 _lastPosition;

    private void Reset()
    {
        // Initialize the sway curve with default linear behavior
        swayCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    }

    private void Start()
    {
        if (!weaponTransform)
            weaponTransform = transform.GetChild(0);

        CacheInitialTransforms();
    }

    private void CacheInitialTransforms()
    {
        _lastRotation = transform.localRotation;
        _lastPosition = transform.localPosition;
        _initialPosition = weaponTransform.localPosition;
        _initialRotation = weaponTransform.localRotation;
    }

    private void LateUpdate()
    {
        CalculateSway();
        ApplySway();
    }

    private void CalculateSway()
    {
        CalculateRotationalSway();
        CalculatePositionalSway();
        ClampSwayValues();
    }

    private void CalculateRotationalSway()
    {
        Quaternion angularVelocity = Quaternion.Inverse(_lastRotation) * transform.rotation;
        _lastRotation = transform.rotation;

        float mouseX = FixAngle(angularVelocity.eulerAngles.y) * rotationSwayMultiplier;
        float mouseY = -FixAngle(angularVelocity.eulerAngles.x) * rotationSwayMultiplier;

        _rotSway += new Vector2(mouseX, mouseY);
    }

    private void CalculatePositionalSway()
    {
        Vector3 positionDelta = transform.position - _lastPosition;
        _lastPosition = transform.position;

        positionDelta = weaponTransform.InverseTransformDirection(positionDelta) * movementSwayMultiplier;
        _posSway += positionDelta;
    }

    private void ClampSwayValues()
    {
        _rotSway = Vector2.ClampMagnitude(_rotSway, maxRotSwayAmount);
        _posSway = Vector3.ClampMagnitude(_posSway, maxPosSwayAmount);
    }

    private void ApplySway()
    {
        float deltaRot = CalculateDelta(_rotSway.magnitude / maxRotSwayAmount);
        float deltaPos = CalculateDelta(_posSway.magnitude / maxPosSwayAmount);

        ApplyRotationSway(deltaRot + deltaPos);
        ApplyPositionSway(deltaPos);

        ReduceSwayOverTime(deltaRot, deltaPos);
    }

    private float CalculateDelta(float swayRatio)
    {
        return swayCurve.Evaluate(swayRatio) * swaySmooth * Time.deltaTime;
    }

    private void ApplyRotationSway(float deltaRot)
    {
        Quaternion targetRotation = _initialRotation * Quaternion.Euler(new Vector3(-_rotSway.y - _posSway.y * posToRotAmount, _rotSway.x + _posSway.x * posToRotAmount, 0));
        weaponTransform.localRotation = Quaternion.Lerp(weaponTransform.localRotation, targetRotation, swaySmooth * Time.deltaTime);
    }

    private void ApplyPositionSway(float deltaPos)
    {
        Vector3 targetPosition = _initialPosition + _posSway;
        weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, targetPosition, swaySmooth * Time.deltaTime);
    }

    private void ReduceSwayOverTime(float deltaRot, float deltaPos)
    {
        _rotSway = Vector2.Lerp(_rotSway, Vector2.zero, deltaRot);
        _posSway = Vector3.Lerp(_posSway, Vector3.zero, deltaPos);
    }

    private float FixAngle(float angle)
    {
        // Normalize the angle to be within -180 to 180 degrees
        return Mathf.Repeat(angle + 180f, 360f) - 180f;
    }
}