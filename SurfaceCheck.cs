using System.Diagnostics;
using UnityEngine;
/// <summary>
/// Casts rays in a grid pattern in front of the player. 
/// If a ray hits a surface, it displays a colored ray 
/// (blue or red based on surface alignment). Rays without a hit are displayed in yellow.
/// </summary>
public class SurfaceCheck : MonoBehaviour
{
  public Transform playerTransform;
  public int rowCount = 3;
  public int rayCount = 5;
  public float coverageWidth = 2.0f; 
  public float coverageHeight = 1.0f;
  public float forwardOffsetBetweenRows = 0.5f;
  public float forwardDistance = 1.0f;
  public float startHeight = 0.5f;
  public LayerMask surfaceLayerMask;
  private Vector3 castDirection => -playerTransform.up;

  private void Update()
  {
    if (playerTransform == null) return;
    RaycastHit hit;
    Vector3 startPosition = playerTransform.position + playerTransform.forward * forwardDistance;
    float rowSpacing = coverageHeight / (rowCount - 1);
    // Iterate over rows
    for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
    {
      // Calculate the center offset for the current row
      float currentForwardOffset = forwardOffsetBetweenRows * rowIndex;
      Vector3 centerOffset = startPosition + playerTransform.forward * currentForwardOffset + Vector3.up * startHeight;

      float spacing = coverageWidth / (rayCount - 1);
      Vector3 startOffset = centerOffset - playerTransform.right * (coverageWidth * 0.5f);

      // Iterate over rays in the current row
      for (int i = 0; i < rayCount; i++)
      {
        // Cast the ray
        if (Physics.Raycast(startOffset, castDirection, out hit, coverageHeight, surfaceLayerMask))
        {
          Debug.DrawRay(hit.point, hit.normal * 0.5f, Vector3.Dot(hit.normal, Vector3.up) > 0.5f ? Color.blue : Color.red);
        }
        else
        {
          Debug.DrawRay(startOffset, castDirection * coverageHeight, Color.yellow);
        }
        startOffset += playerTransform.right * spacing;
      }
    }
  }
}
