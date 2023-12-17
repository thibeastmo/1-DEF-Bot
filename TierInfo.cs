namespace NLBE_Bot
{
    public class TierInfo
    {
        public short Tier { get; }
        public int Avarage_damage { get; }
        public double Winrate { get; }
        public int Battles { get; }

        public TierInfo(short tier, int avarage_damage, double winrate, int battles)
        {
            Tier = tier;
            Avarage_damage = avarage_damage;
            Winrate = winrate;
            Battles = battles;
        }
    }
}
