using UnityEngine;
using UnityEditor;

public class TextureArrayCreator : MonoBehaviour
{
  [MenuItem("Assets/Create/Texture2D Array")]
  public static void CreateTextureArray()
  {
    // Select
    Texture2D[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

    if (textures.Length == 0)
    {
      Debug.LogError("No textures selected! Please select some PNGs.");
      return;
    }
    int width = textures[0].width;
    int height = textures[0].height;
    TextureFormat format = textures[0].format;

    foreach (var tex in textures)
    {
      if (tex.width != width || tex.height != height)
      {
        Debug.LogError("All textures must have the same dimensions!");
        return;
      }
    }

    // Create the Texture2DArray
    Texture2DArray textureArray = new Texture2DArray(width, height, textures.Length, format, true);
    textureArray.filterMode = FilterMode.Bilinear;
    textureArray.wrapMode = TextureWrapMode.Clamp;
    for (int i = 0; i < textures.Length; i++)
    {
      int mipCount = textures[i].mipmapCount;

      for (int mip = 0; mip < mipCount; mip++)
      {
        // Copy each mip level
        Graphics.CopyTexture(textures[i], 0, mip, textureArray, i, mip);
      }
    }
    textureArray.Apply(true);
    // Show a Save File dialog
    string path = EditorUtility.SaveFilePanel(
        "Save Texture2D Array",        // Window title
        "Assets",                     // Default folder
        "NewTextureArray.asset",      // Default file name
        "asset"                       // File extension
    );

    if (string.IsNullOrEmpty(path))
    {
      Debug.Log("Save canceled.");
      return;
    }

    // Convert absolute path to relative path for Unity
    path = FileUtil.GetProjectRelativePath(path);

    // Save
    AssetDatabase.CreateAsset(textureArray, path);
    AssetDatabase.SaveAssets();

    Debug.Log($"Texture2DArray created at {path}");
  }
}
