using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Animations;

[ExecuteInEditMode]
public class ToArray : MonoBehaviour
{
  //Options
  [Range(0, 20)]
  public int ArraySize = 0;
  [Range(-40, 20)]
  public float Distance = 0;
  public Axis axis;
  private List<GameObject> Objects;
  private GameObject fab;
  private int lastSizeValue;
  private float lastDistanceValue;
  private Axis lastAxis;
  private void Awake()
  {
    if (Application.isPlaying)
      return;
    string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this.gameObject);
    if (!string.IsNullOrEmpty(prefabPath))
      fab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
    else fab = gameObject;
    Objects = new(0);
  }
  void ApplyOvs(GameObject newObj)
  {
    var prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(gameObject);

    if (prefabInstanceStatus == PrefabInstanceStatus.Connected)
    {
      var propertyModifications = PrefabUtility.GetPropertyModifications(gameObject);

      if (propertyModifications != null && propertyModifications.Length > 0)
      {
        PrefabUtility.SetPropertyModifications(newObj, propertyModifications);
      }
      void ApplyOvsRecursive(Transform t)
      {
        if (t.childCount == 0)
          return;
        for (int i = 0; i < t.childCount; i++)
        {
          Transform child = t.GetChild(i);
          if (child.TryGetComponent<ToArray>(out var toArray))
            DestroyImmediate(toArray);
          if (PrefabUtility.GetPrefabAssetType(child.gameObject) == PrefabAssetType.NotAPrefab)
          {
            GameObject newChild = Instantiate(child.gameObject, newObj.transform);
            newChild.transform.SetPositionAndRotation(child.position, child.rotation);
            newChild.transform.localScale = child.localScale;
            //if (newChild.TryGetComponent<ToArray>(out var toArrayTwo))
            //  DestroyImmediate(toArrayTwo);
          }
          ApplyOvsRecursive(child);
        }
      }
      ApplyOvsRecursive(transform);
    }
    else if (prefabInstanceStatus == PrefabInstanceStatus.NotAPrefab)
    {
      Debug.LogWarning("The instantiated object is not a prefab.");
    }
  }
  private void Update()
  {
    if (Application.isPlaying)
      return;
    if (PrefabStageUtility.GetCurrentPrefabStage() != null)
      return;
    if (lastSizeValue != ArraySize)
    {
      OnValueChange();
      lastSizeValue = ArraySize;
    }
    if (lastDistanceValue != Distance)
    {
      OnValueChange();
      lastDistanceValue = Distance;
    }
    if (lastAxis != axis)
    {
      OnValueChange();
      lastAxis = axis;
    }
  }
  Bounds GetSize()
  {
    Bounds totalArea = new() {center = transform.position};
    if (!transform.TryGetComponent<Collider>(out var collision))
    {
      if (transform.TryGetComponent<MeshRenderer>(out var mesh))
        totalArea.Encapsulate(mesh.bounds);
    }
    else
      totalArea.Encapsulate(collision.bounds);
    var csize = GetChildSizes(transform);
    if (csize.size != Vector3.zero)
      totalArea.Encapsulate(csize);
    return totalArea;
  }
  Bounds GetChildSizes(Transform t)
  {
    Bounds bounds = new();
    bool initialized = false;

    void CalculateBoundsRecursive(Transform currentTransform)
    {
      if (currentTransform.childCount == 0)
      {
        return;
      }
      for (int i = 0; i < currentTransform.childCount; i++)
      {
        if (currentTransform.GetChild(i).TryGetComponent<Collider>(out var childCollision))
        {
          if (!initialized)
          {
            bounds = childCollision.bounds;
            initialized = true;
          }
          else
          {
            bounds.Encapsulate(childCollision.bounds);
          }
        }
        else if (currentTransform.GetChild(i).TryGetComponent<MeshRenderer>(out var childMesh))
        {
          if (!initialized)
          {
            bounds = childMesh.bounds;
            initialized = true;
          }
          else
          {
            bounds.Encapsulate(childMesh.bounds);
          }
        }
        CalculateBoundsRecursive(currentTransform.GetChild(i));
      }
    }
    CalculateBoundsRecursive(t);
    return bounds;
  }
  public void OnValueChange()
  {
    if (Application.isPlaying)
      return;
    if (ArraySize == 0 || axis == Axis.None)
    {
      if (Objects != null && Objects.Count > 0)
      {
        foreach (var item in Objects)
          DestroyImmediate(item);
        Objects = new(0);
      }
    }
    else
    {
      int count = 0;
      if ((axis & Axis.X) != 0) count++;
      if ((axis & Axis.Y) != 0) count++;
      if ((axis & Axis.Z) != 0) count++;

      if (ArraySize * count != Objects.Count)
      {
        if (ArraySize * count > Objects.Count)
        {
          for (int i = Objects.Count; i < ArraySize * count; i++)
          {
            GameObject newObj = PrefabUtility.InstantiatePrefab(fab) as GameObject;
            if (newObj == null)
            {
              newObj = Instantiate(fab);
            }
            if (newObj.TryGetComponent< ToArray>(out var toArray))
              DestroyImmediate(toArray);
            if (transform.parent != null)
              newObj.transform.SetParent(transform.parent);

            newObj.transform.SetPositionAndRotation(transform.position, transform.rotation);
            newObj.transform.localScale = transform.localScale;
            ApplyOvs(newObj);
            Objects.Add(newObj);
          }

        }
        else if (ArraySize * count < Objects.Count)
        {
          for (int i = Objects.Count - 1; i >= ArraySize * count; i--)
          {
            DestroyImmediate(Objects[i]);
            Objects.RemoveAt(i);
          }
        }
      }
      PositionObjects(count);
    }
  }
  private void PositionObjects(int axisCount)
  {

    var totalsize = GetSize().size;
    var targetAmount = Objects.Count / axisCount;
    float between;
    int arrayPoint = 0;
    int added = 0;
    if ((axis & Axis.X) != 0)
    {
      added++;
      for (int i = arrayPoint; i < Objects.Count; i++)
      {
        if (i >= targetAmount)
        {
          break;
        }
        arrayPoint++;
        var sizePerObject = totalsize.x * (i + 1);
        between = Distance * (i + 1) + sizePerObject;
        Objects[i].transform.position = transform.position +
          new Vector3(between, 0, 0);
      }
    }
    if ((axis & Axis.Y) != 0)
    {
      var yI = 0;
      added++;
      for (int i = arrayPoint; i < Objects.Count; i++)
      {
        if (i >= targetAmount * added)
        {
          break;
        }
        arrayPoint++;
        var sizePerObject = totalsize.y * (yI + 1);
        between = Distance * (yI + 1) + sizePerObject;
        Objects[i].transform.position = transform.position +
          new Vector3(0, between, 0);
        yI++;
      }
    }
    if ((axis & Axis.Z) != 0)
    {
      var zI = 0;
      added++;
      for (int i = arrayPoint; i < Objects.Count; i++)
      {
        if (i >= targetAmount * added)
        {
          break;
        }
        arrayPoint++;
        var sizePerObject = totalsize.z * (zI + 1);
        between = Distance * (zI + 1) + sizePerObject;
        Objects[i].transform.position = transform.position +
          new Vector3(0, 0, between);
        zI++;
      }
    }
    Debug.Log(arrayPoint);
  }
  private void OnDrawGizmos()
  {
    Bounds b = GetSize();
    Gizmos.color = Color.magenta;
   // Gizmos.DrawCube(b.center, b.size);
  }
}
