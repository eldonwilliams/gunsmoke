using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // The type of camera following algorithm to use
    public enum FollowType
    {
        // This is recommended for moving targets, such as the player. It tracks movement over time.
        MOVING,
        // This is recommended for a stationary or a seldomly moving target, such as a box. It will track movement, but not accurately.
        STATIONARY,
    }

    // The type of following currently active
    public FollowType followType = FollowType.MOVING;
    public Transform trackedObject;
    public float offsetY = 2.0f;
    public float trackSpeed = 2.0f;

    private Transform cameraTransform;

    /*
        FOR FollowType.STATIONARY

        Calculates the Camera's position and rotation.
        This an instantaneous rate of change, no lerp.
        First value is Pos, second is Rotation.
    */
    private (Vector3, Vector3) calculateCameraPosAndRot() {
        Vector3 position = trackedObject.position - Vector3Utils.Abs(trackedObject.forward * trackedObject.localScale.magnitude * 3);
        position.y += offsetY;
        Vector3 rotation = Quaternion.LookRotation(((trackedObject.forward + trackedObject.position) - position).normalized).eulerAngles;
        return (position, rotation);
    }

    void Start()
    {
        cameraTransform = gameObject.GetComponent<Camera>().transform;
        var (position, rotation) = calculateCameraPosAndRot();
        cameraTransform.position = position;
        cameraTransform.rotation = Quaternion.Euler(rotation);
    }

    void Update()
    {
        switch (followType)
        {
            case FollowType.MOVING:
                movingUpdate();
                break;
            case FollowType.STATIONARY:
                stationaryUpdate();
                break;
            default:
                break;
        }
    }

    Vector3 lastPosition = Vector3.zero;
    float rotLerpMulti = 0.75f;

    private void movingUpdate() {
        if (lastPosition == Vector3.zero) lastPosition = trackedObject.position;
        Vector3 deltaPosition = trackedObject.position - lastPosition;
        lastPosition = trackedObject.position;

        //
        Vector3 position = trackedObject.position - deltaPosition;
        position -= trackedObject.forward;
        position.y += offsetY;
        Vector3 rotation = Quaternion.LookRotation(((trackedObject.forward + trackedObject.position) - position).normalized).eulerAngles;
        //

        rotLerpMulti = Mathf.Lerp(rotLerpMulti, deltaPosition.magnitude > 0 ? 0.2f : 0.75f, Time.deltaTime * trackSpeed);
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, position, Time.deltaTime * trackSpeed);
        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(rotation), Time.deltaTime * trackSpeed * rotLerpMulti);
    }

    private void stationaryUpdate() {
        var (position, rotation) = calculateCameraPosAndRot();
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, position, Time.deltaTime * trackSpeed);
        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(rotation), Time.deltaTime * trackSpeed);
    }
}
