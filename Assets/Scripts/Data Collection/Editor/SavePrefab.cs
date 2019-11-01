using UnityEditor;
using UnityEngine;

/// <summary>
/// Saves mesh and entire prefab from gameview, your procedural mesh prefab is saved.
/// </summary>
class CreatePrefabFromSelected
{
    const string menuName = "GameObject/Create Prefab From Selected";

    /// <summary>
    /// Adds a menu named "Create Prefab From Selected" to the GameObject menu.
    /// </summary>
    [MenuItem(menuName)]
    static void CreatePrefabMenu()
    {
        var go = Selection.activeGameObject;

        Mesh m1 = go.GetComponent<SkinnedMeshRenderer>().sharedMesh;//update line1
        AssetDatabase.CreateAsset(m1, "Assets/savedMesh/" + go.name + "_M" + ".asset"); // update line2


        var prefab = EditorUtility.CreateEmptyPrefab("Assets/savedMesh/" + go.name + ".prefab");
        EditorUtility.ReplacePrefab(go, prefab);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Validates the menu.
    /// The item will be disabled if no game object is selected.
    /// </summary>
    /// <returns>True if the menu item is valid.</returns>
    [MenuItem(menuName, true)]
    static bool ValidateCreatePrefabMenu()
    {
        return Selection.activeGameObject != null;
    }
}