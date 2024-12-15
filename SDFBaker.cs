using System.Drawing;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.SDF;

public class SDFBaker : MonoBehaviour
{
  private MeshToSDFBaker RuntimeBaker;
  public Transform TestMesh;
  public Mesh sdfInput;
  public VisualEffect sdfOutput;
  void Start()
  {
    RuntimeBaker = new MeshToSDFBaker(new Vector3(2,2,2), new Vector3(0,0,0),64, sdfInput);
    RuntimeBaker.BakeSDF();
   // var size = RuntimeBaker.GetActualBoxSize() / 2f; // optional way to get the bounding box size
    sdfOutput.SetTexture("SDF", RuntimeBaker.SdfTexture);
    sdfOutput.SetVector3("Size", sdfInput.bounds.size);
    sdfOutput.SetVector3("Position", TestMesh.transform.position);
    sdfOutput.SetVector3("Angles", TestMesh.localEulerAngles);
  }
  private void Update()
  {
    sdfOutput.SetVector3("Position", TestMesh.position);
    sdfOutput.SetVector3("Angles", TestMesh.localEulerAngles);
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
