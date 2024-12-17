using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.SDF;

public class SDFBaker : MonoBehaviour
{
  private MeshToSDFBaker RuntimeBaker;
  public Transform TestMesh;
  private Mesh sdfInput;
  public VisualEffect sdfOutput;
  void Start()
  {
    sdfInput = TakeGeometrySnapShot(new Vector3(20f, 20f, 20f), transform.position);
    Bake();
    Log();
  }
  void Bake()
  {
    RuntimeBaker = new MeshToSDFBaker(sdfInput.bounds.size, sdfInput.bounds.center, 64, sdfInput);
    RuntimeBaker.BakeSDF();
    sdfOutput.SetTexture("SDF", RuntimeBaker.SdfTexture);
    sdfOutput.SetVector3("SDFSize", sdfInput.bounds.size);
    sdfOutput.SetMesh("SDFVisual", sdfInput);
    UpdateSDFVectors();
    Log();
  }
  private void UpdateSDFVectors()
  {
    sdfOutput.SetVector3("SDFPosition", sdfInput.bounds.center);
    sdfOutput.SetVector3("SDFAngles", TestMesh.localEulerAngles);
  }
  public Mesh TakeGeometrySnapShot(Vector3 customBoundsSizeXYZ, Vector3 playerPosition)
  {
    // Step 1: Define bounds and initialize collections
    Bounds customBounds = new Bounds(playerPosition, customBoundsSizeXYZ);
    List<Mesh> collectedMeshes = new List<Mesh>();
    List<Transform> collectedTransforms = new List<Transform>();
    List<CombineInstance> combineInstances = new List<CombineInstance>();

    // Step 2: Initial low-resolution raycasting
    int initialResolution = 50;
    float gridSpacingX = customBounds.size.x / initialResolution;
    float gridSpacingZ = customBounds.size.z / initialResolution;

    for (int x = 0; x < initialResolution; x++)
    {
      for (int z = 0; z < initialResolution; z++)
      {
        // Calculate raycast position
        Vector3 startPoint = new Vector3(
            customBounds.min.x + x * gridSpacingX,
            customBounds.max.y, // Start from top of bounds
            customBounds.min.z + z * gridSpacingZ
        );

        if (Physics.Raycast(startPoint, Vector3.down, out RaycastHit hit, customBounds.size.y))
        {
          Collider hitCollider = hit.collider;
          Debug.Log("Raycast hit: " + hitCollider.name);
          if (customBounds.Intersects(hitCollider.bounds))
          {
            MeshFilter meshFilter = hitCollider.GetComponent<MeshFilter>();
            if (collectedMeshes.Contains(meshFilter.mesh)) { continue; }
            if (meshFilter != null && customBounds.Contains(hitCollider.bounds.min) && customBounds.Contains(hitCollider.bounds.max))
            {
              Debug.Log("Adding mesh: " + meshFilter.mesh);
              // Directly add the mesh if fully contained
              collectedMeshes.Add(meshFilter.mesh);
              collectedTransforms.Add(hitCollider.transform);
            }
            else { continue; }
          }
        }
      }
    }
    for (int i = 0; i < collectedMeshes.Count; i++)
    {
      CombineInstance combineInstance = new CombineInstance
      {
        mesh = collectedMeshes[i],
        transform = collectedTransforms[i].localToWorldMatrix // Apply the collider's transform (position, rotation, scale)
      };
      combineInstances.Add(combineInstance);
    }

    Mesh combinedMesh = new Mesh();
    combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
    combinedMesh.RecalculateBounds();

    Debug.Log("Geometry snapshot taken with combined mesh center at: " + combinedMesh.bounds.center);
 
    return combinedMesh;
    
  }


  private void Log()
  {
    Debug.Log(RuntimeBaker.SdfTexture.format);
    Debug.Log("Box Dimensions: " + sdfInput.bounds.size.x + " WIDTH " + sdfInput.bounds.size.y + " HEIGHT " + sdfInput.bounds.size.z + " LENGTH");
    Debug.Log("Texture Dimensions: " + RuntimeBaker.SdfTexture.width + " WIDTH " + RuntimeBaker.SdfTexture.height + " HEIGHT " + RuntimeBaker.SdfTexture.volumeDepth + " DEPTH" + "\nActual Box Size: " + RuntimeBaker.GetActualBoxSize());
  }
  private void Update()
  {
    sdfOutput.SetVector3("PlayerPosition", transform.position);
    UpdateSDFVectors();
  }
  private void OnDestroy()
  {
    if (RuntimeBaker != null)
    {
      RuntimeBaker.Dispose();
      RuntimeBaker = null;
    }
  }
}
