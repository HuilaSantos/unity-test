using UnityEngine;

/// <summary>
/// Add this component to any GameObject that should NOT be destroyed
/// when the maze instantiator clears its root children.
/// Useful if your GameSystems is parented under the maze root.
/// </summary>
public class KeepAliveDuringMazeBuild : MonoBehaviour
{
}
