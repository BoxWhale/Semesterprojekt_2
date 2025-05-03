using UnityEngine;

public class CubeCurveGenerator : MonoBehaviour
{
    [Header("Cube Settings")] public GameObject cubePrefab; // Your cube prefab.

    public float cubeSize = 1f; // Cube's side length.

    [Header("Arc Settings")] public float angleStepDeg = 10f; // Angular step (in degrees) between cubes.

    public float arcSpanDeg = 180f; // Total arc span (in degrees).
    public float tiltAngle; // Optional tilt (in degrees) for ramp effect.

    private void Start()
    {
        // Number of cubes: include both endpoints of the arc.
        var count = Mathf.RoundToInt(arcSpanDeg / angleStepDeg) + 1;
        var angleStepRad = angleStepDeg * Mathf.Deg2Rad;
        var startAngleRad = 0f; // Starting angle relative to object's forward.

        // Calculate the required radius so that the chord (distance between cube centers) equals cubeSize:
        var radius = cubeSize / (2 * Mathf.Sin(angleStepRad / 2));

        for (var i = 0; i < count; i++)
        {
            var angleRad = startAngleRad + i * angleStepRad;

            // Position cubes on an arc in the XZ-plane
            var pos = transform.position + new Vector3(radius * Mathf.Cos(angleRad), 0, radius * Mathf.Sin(angleRad));

            // Instantiate the cube.
            var cube = Instantiate(cubePrefab, pos, Quaternion.identity, transform);

            // Compute the tangent (direction along the arc) so each cube faces along the arc.
            var tangent = new Vector3(-Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad));
            var baseRot = Quaternion.LookRotation(tangent, Vector3.up);

            // Apply additional tilt for a ramp effect by rotating around the local X-axis.
            cube.transform.rotation = baseRot * Quaternion.Euler(tiltAngle, 0, 0);
        }
    }

    // This method draws the predicted positions in the Scene view.
    private void OnDrawGizmos()
    {
        // Calculate the number of cubes.
        var count = Mathf.RoundToInt(arcSpanDeg / angleStepDeg) + 1;
        var angleStepRad = angleStepDeg * Mathf.Deg2Rad;
        var startAngleRad = 0f; // Starting from this object's forward.
        var radius = cubeSize / (2 * Mathf.Sin(angleStepRad / 2));

        // Set the Gizmos color.
        Gizmos.color = Color.cyan;

        // Draw wire cubes at calculated positions.
        for (var i = 0; i < count; i++)
        {
            var angleRad = startAngleRad + i * angleStepRad;
            var pos = transform.position + new Vector3(radius * Mathf.Cos(angleRad), 0, radius * Mathf.Sin(angleRad));
            Gizmos.DrawWireCube(pos, new Vector3(cubeSize, cubeSize, cubeSize));
            // Optionally, draw a line from the origin to each cube.
            Gizmos.DrawLine(transform.position, pos);
        }

        // Draw the full arc with a series of connected line segments.
        var arcDetail = 30;
        var endAngleRad = startAngleRad + arcSpanDeg * Mathf.Deg2Rad;
        var previousPoint = transform.position +
                            new Vector3(radius * Mathf.Cos(startAngleRad), 0, radius * Mathf.Sin(startAngleRad));
        for (var j = 1; j <= arcDetail; j++)
        {
            var t = j / (float)arcDetail;
            var angleRad = Mathf.Lerp(startAngleRad, endAngleRad, t);
            var newPoint = transform.position +
                           new Vector3(radius * Mathf.Cos(angleRad), 0, radius * Mathf.Sin(angleRad));
            Gizmos.DrawLine(previousPoint, newPoint);
            previousPoint = newPoint;
        }
    }
}