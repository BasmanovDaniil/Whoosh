using UnityEngine;

public class DollyZoom : MonoBehaviour
{
    public Vector3 target = Vector3.zero;
    public float lerpDistance = -60;
    public float lerpStep = 0.5f;

    private float initHeightAtDist;

    private void Start()
    {
        transform.position = new Vector3(0, 0, lerpDistance);
        initHeightAtDist = FrustumHeightAtDistance(Vector3.Distance(transform.position, target));
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(0, 0, lerpDistance), lerpStep*Time.deltaTime);
        // Measure the new distance and readjust the FOV accordingly.
        camera.fieldOfView = FOVForHeightAndDistance(initHeightAtDist, Vector3.Distance(transform.position, target));
    }

    // Calculate the frustum height at a given distance from the camera.
    private float FrustumHeightAtDistance(float distance)
    {
        return 2*distance*Mathf.Tan(camera.fieldOfView*0.5f*Mathf.Deg2Rad);
    }

    // Calculate the FOV needed to get a given frustum height at a given distance.
    private float FOVForHeightAndDistance(float height, float distance)
    {
        return 2*Mathf.Rad2Deg*Mathf.Atan(height*0.5f/distance);
    }
}