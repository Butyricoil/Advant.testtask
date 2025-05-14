using UnityEngine;

[CreateAssetMenu(fileName = "BusinessNamesConfig", menuName = "Configs/Business Names Config")]
public class BusinessNamesConfig : ScriptableObject
{
    public string[] BusinessNames;
    public string[] Upgrade1Names;
    public string[] Upgrade2Names;

    private void OnValidate()
    {
        if (BusinessNames == null || Upgrade1Names == null || Upgrade2Names == null)
        {
            Debug.LogError("All arrays in BusinessNamesConfig must be initialized!");
            return;
        }

        int length = BusinessNames.Length;
        if (Upgrade1Names.Length != length || Upgrade2Names.Length != length)
        {
            Debug.LogError("All arrays in BusinessNamesConfig must have the same length!");
        }

        // Validate that names are not empty
        for (int i = 0; i < length; i++)
        {
            if (string.IsNullOrEmpty(BusinessNames[i]) || 
                string.IsNullOrEmpty(Upgrade1Names[i]) || 
                string.IsNullOrEmpty(Upgrade2Names[i]))
            {
                Debug.LogError($"Business names at index {i} cannot be empty!");
            }
        }
    }
}