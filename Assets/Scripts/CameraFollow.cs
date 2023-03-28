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

    public float move = 5.0f;
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
    bool isTrackedVisible;

    private void movingUpdate() {
        /*
            On first update, lastPosition will be Vector3.zero.
                Set to trackedObject.position, b/c deltaPosition on first frame should be ~0,0,0
            Find the change in position (newPos - oldPos)
            Update last position for next frame
        */
        if (lastPosition == Vector3.zero) lastPosition = trackedObject.position;
        Vector3 deltaPosition = trackedObject.position - lastPosition;
        lastPosition = trackedObject.position;
        // The deltaVector but with a magnitude of 1, used for adding to other vectors
        // trackedObject.position where y compoonent is 0

        isTrackedVisible = trackedObject.GetComponent<Renderer>().isVisible;

        //
        Vector3 position = trackedObject.position - (deltaPosition * move / Time.deltaTime);
        // position -= trackedObject.forward;
        // Position calculation
        /*
            Position is the trackedObject's position subtracted by the change in position ()
        */
        // Removed: position -= trackedObject.forward;
        position.y += offsetY;
        position.y -= deltaPosition.magnitude * move / Time.deltaTime;
        Vector3 rotation = Quaternion.LookRotation(((deltaPosition / Time.deltaTime + trackedObject.position) - position).normalized).eulerAngles;
        //
        /*
            Use the deltaVector 
        */

        rotLerpMulti = Mathf.Lerp(rotLerpMulti, deltaPosition.magnitude > 0 ? 0.0f : 0.75f, Time.deltaTime * trackSpeed);
        /*
            Rotation stops when moving b/c it can lead to weird movement errors.
            Lerp is used because of the nice cinematic effect it creates.
        */
        //
        Vector3 newPosition = Vector3.Lerp(cameraTransform.position, position, Time.deltaTime * trackSpeed);
        newPosition.y = Mathf.Lerp(cameraTransform.position.y, position.y, Time.deltaTime * trackSpeed);
        cameraTransform.position = newPosition;
        
        //
        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(rotation), Time.deltaTime * trackSpeed * rotLerpMulti);
    }

    private void stationaryUpdate() {
        var (position, rotation) = calculateCameraPosAndRot();
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, position, Time.deltaTime * trackSpeed);
        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(rotation), Time.deltaTime * trackSpeed);
    }
}
