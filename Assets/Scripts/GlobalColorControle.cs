using Unity.VisualScripting;
using UnityEngine;

public class GlobalColorControle : MonoBehaviour
{
    [Tooltip("This color will be applied globally to all shaders using this property.")]
    public Color globalColor = Color.white;

    [Tooltip("Name of the global shader property (e.g., '_GlobalColor').")]
    public string globalColorPropertyName = "_GlobalColor";

    void Update()
    {
        // Set the global color property so that any shader referencing this property gets updated
        Shader.SetGlobalColor(globalColorPropertyName, globalColor);
    }
}