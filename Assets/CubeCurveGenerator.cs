using UnityEngine;

public class CubeCurveGenerator : MonoBehaviour
{
    [Header("Cube Settings")]
    public GameObject cubePrefab;   // Your cube prefab.
    public float cubeSize = 1f;     // Cube's side length.

    [Header("Arc Settings")]
    public float angleStepDeg = 10f;   // Angular step (in degrees) between cubes.
    public float arcSpanDeg = 180f;      // Total arc span (in degrees).
    public float tiltAngle = 0f;         // Optional tilt (in degrees) for ramp effect.

    void Start()
    {
        // Number of cubes: include both endpoints of the arc.
        int count = Mathf.RoundToInt(arcSpanDeg / angleStepDeg) + 1;
        float angleStepRad = angleStepDeg * Mathf.Deg2Rad;
        float startAngleRad = 0f; // Starting angle relative to object's forward.

        // Calculate the required radius so that the chord (distance between cube centers) equals cubeSize:
        float radius = cubeSize / (2 * Mathf.Sin(angleStepRad / 2));

        for (int i = 0; i < count; i++)
        {
            float angleRad = startAngleRad + i * angleStepRad;
            
            // Position cubes on an arc in the XZ-plane
            Vector3 pos = transform.position + new Vector3(radius * Mathf.Cos(angleRad), 0, radius * Mathf.Sin(angleRad));
            
            // Instantiate the cube.
            GameObject cube = Instantiate(cubePrefab, pos, Quaternion.identity, transform);
            
            // Compute the tangent (direction along the arc) so each cube faces along the arc.
            Vector3 tangent = new Vector3(-Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad));
            Quaternion baseRot = Quaternion.LookRotation(tangent, Vector3.up);
            
            // Apply additional tilt for a ramp effect by rotating around the local X-axis.
            cube.transform.rotation = baseRot * Quaternion.Euler(tiltAngle, 0, 0);
        }
    }

    // This method draws the predicted positions in the Scene view.
    private void OnDrawGizmos()
    {
        // Calculate the number of cubes.
        int count = Mathf.RoundToInt(arcSpanDeg / angleStepDeg) + 1;
        float angleStepRad = angleStepDeg * Mathf.Deg2Rad;
        float startAngleRad = 0f; // Starting from this object's forward.
        float radius = cubeSize / (2 * Mathf.Sin(angleStepRad / 2));

        // Set the Gizmos color.
        Gizmos.color = Color.cyan;

        // Draw wire cubes at calculated positions.
        for (int i = 0; i < count; i++)
        {
            float angleRad = startAngleRad + i * angleStepRad;
            Vector3 pos = transform.position + new Vector3(radius * Mathf.Cos(angleRad), 0, radius * Mathf.Sin(angleRad));
            Gizmos.DrawWireCube(pos, new Vector3(cubeSize, cubeSize, cubeSize));
            // Optionally, draw a line from the origin to each cube.
            Gizmos.DrawLine(transform.position, pos);
        }

        // Draw the full arc with a series of connected line segments.
        int arcDetail = 30;
        float endAngleRad = startAngleRad + arcSpanDeg * Mathf.Deg2Rad;
        Vector3 previousPoint = transform.position + new Vector3(radius * Mathf.Cos(startAngleRad), 0, radius * Mathf.Sin(startAngleRad));
        for (int j = 1; j <= arcDetail; j++)
        {
            float t = j / (float)arcDetail;
            float angleRad = Mathf.Lerp(startAngleRad, endAngleRad, t);
            Vector3 newPoint = transform.position + new Vector3(radius * Mathf.Cos(angleRad), 0, radius * Mathf.Sin(angleRad));
            Gizmos.DrawLine(previousPoint, newPoint);
            previousPoint = newPoint;
        }
    }
}
