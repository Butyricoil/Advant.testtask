public static class BusinessUtils
{
    public static int CalculateIncome(Business business, BusinessConfig config)
    {
        float multiplier = 1f;
        if (business.Upgrade1Purchased) multiplier += config.Upgrade1Multipliers[business.Id];
        if (business.Upgrade2Purchased) multiplier += config.Upgrade2Multipliers[business.Id];

        return (int)(business.Level * config.BaseIncomes[business.Id] * multiplier);
    }
} 