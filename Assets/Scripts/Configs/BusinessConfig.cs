using UnityEngine;

[CreateAssetMenu(fileName = "BusinessConfig", menuName = "Configs/Business Config")]
public class BusinessConfig : ScriptableObject
{
    [Range(0.1f, 10f)]
    public float[] IncomeDelays;
    
    [Range(1, 1000)]
    public int[] BaseCosts;
    
    [Range(1, 1000)]
    public int[] BaseIncomes;
    
    [Range(1, 1000)]
    public int[] Upgrade1Prices;
    
    [Range(0.1f, 10f)]
    public float[] Upgrade1Multipliers;
    
    [Range(1, 1000)]
    public int[] Upgrade2Prices;
    
    [Range(0.1f, 10f)]
    public float[] Upgrade2Multipliers;

    private void OnValidate()
    {
        if (IncomeDelays == null || BaseCosts == null || BaseIncomes == null || 
            Upgrade1Prices == null || Upgrade1Multipliers == null || 
            Upgrade2Prices == null || Upgrade2Multipliers == null)
        {
            Debug.LogError("All arrays in BusinessConfig must be initialized!");
            return;
        }

        int length = IncomeDelays.Length;
        if (BaseCosts.Length != length || BaseIncomes.Length != length || 
            Upgrade1Prices.Length != length || Upgrade1Multipliers.Length != length || 
            Upgrade2Prices.Length != length || Upgrade2Multipliers.Length != length)
        {
            Debug.LogError("All arrays in BusinessConfig must have the same length!");
        }
    }
}