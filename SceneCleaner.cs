using System.Collections.Generic;
using UnityEngine;

public class SceneCleaner : MonoBehaviour
{
    List<GameObject> inactiveObjects;
    // removes all inactive objects in the scene
    [ExecuteInEditMode]
    [ContextMenu("Remove Inactive Objects")]
    void RemoveInactiveObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);
        foreach (GameObject obj in allObjects)
            if (!obj.activeInHierarchy) inactiveObjects.Add(obj);

        foreach (GameObject obj in inactiveObjects)
            if (inactiveObjects != null)
                DestroyImmediate(obj);
    }
}
