using System;
using System.Collections.Generic;
using System.Text;

namespace NLBE_Bot
{
    public class PlayerRatingResult
    {
        public string Name { get; set; }
        public int Score { get; set; }
        private bool StillCallibrating { get; set; }
        public RatingRank Rank { 
            get
            {
                if (StillCallibrating)
                {
                    return RatingRank.Calib;
                }
                if (Score >= 5000)
                {
                    return RatingRank.Diamond;
                }
                else if (Score >= 4000)
                {
                    return RatingRank.Platinum;
                }
                else if (Score >= 3000)
                {
                    return RatingRank.Gold;
                }
                else if (Score >= 2000)
                {
                    return RatingRank.Silver;
                }
                else
                {
                    return RatingRank.Bronze;
                }
            } 
        }

        public PlayerRatingResult(string name, int score, bool stillCallibrating)
        {
            Name = name;
            Score = score;
            StillCallibrating = stillCallibrating;
        }
    }
}
