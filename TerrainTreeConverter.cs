using UnityEditor;
using UnityEngine;

public class TerrainTreeConverter : MonoBehaviour
{
    [SerializeField]
    private Terrain terrain;
    [ContextMenu("Convert All Trees To Object")]
    public void ConvertAllTreesToObject()
    {// Editor ONLY
        foreach (TreeInstance tree in terrain.terrainData.treeInstances)
        {
            GameObject gameObject = PrefabUtility.InstantiatePrefab(terrain.terrainData.treePrototypes[tree.prototypeIndex].prefab) as GameObject;
            gameObject.transform.localScale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
            Vector3 terrainLocalPosition = new(tree.position.x * terrain.terrainData.size.x, tree.position.y * terrain.terrainData.size.y, tree.position.z * terrain.terrainData.size.z);
            Vector3 worldPosition = terrain.transform.TransformPoint(terrainLocalPosition);
            gameObject.transform.SetPositionAndRotation(worldPosition, Quaternion.Euler(0, tree.rotation, 0));
            gameObject.transform.SetParent(transform);
        }
    }
}
