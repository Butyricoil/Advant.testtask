using UnityEngine;

[CreateAssetMenu(fileName = "BusinessConfig", menuName = "Configs/Business Config")]
public class BusinessConfig : ScriptableObject
{
    public float[] IncomeDelays;
    public int[] BaseCosts;
    public int[] BaseIncomes;
    public int[] Upgrade1Prices;
    public float[] Upgrade1Multipliers;
    public int[] Upgrade2Prices;
    public float[] Upgrade2Multipliers;
}