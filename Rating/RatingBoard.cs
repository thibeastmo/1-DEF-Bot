using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using FMWOTB.Clans;
using FMWOTB.Tools;

namespace NLBE_Bot.Rating
{
    public class RatingBoard
    {
        private DiscordMessageBuilder dmb;
        public List<PlayerRatingResult> ClanMemberRatings;
        public List<PlayerRatingResult> ClanMemberRatingsCalibrationNeeded;
        public void InitializeBoard(WGClan clan)
        {
            ClanMemberRatings = new List<PlayerRatingResult>();
            ClanMemberRatingsCalibrationNeeded = new List<PlayerRatingResult>();
            Parallel.For(0, clan.members.Count, intCounter => {
                var ratingResult = Bot.GetRatingById(clan.members[(int)intCounter].account_id, clan.members[intCounter].account_name);
                if (ratingResult != null)
                {
                    if (ratingResult.Rank == RatingRank.Calib)
                    {
                        ClanMemberRatingsCalibrationNeeded.Add(ratingResult);
                    }
                    else
                    {
                        ClanMemberRatings.Add(ratingResult);
                    }
                }
            });
            ClanMemberRatings = ClanMemberRatings.OrderBy(t => t.Score).Reverse().ToList();
            ClanMemberRatingsCalibrationNeeded = ClanMemberRatingsCalibrationNeeded.OrderBy(t => t.Score).Reverse().ToList();
            ClanMemberRatings.AddRange(ClanMemberRatingsCalibrationNeeded);
        }
        public void InitializeBoard(string clan)
        {
            var clanResults = WGClan.searchByName(SearchAccuracy.EXACT, clan, Bot.WG_APPLICATION_ID, true).Result;

            if (clanResults != null && clanResults.Count > 0)
            {
                InitializeBoard(clanResults.First());
            }
        }

        public List<string> ratingBoardPanels;
        public void CreateRatingBoard()
        {
            ratingBoardPanels = new List<string>();
            //load the image file
            List<PlayerRatingResult> tempPlayerRatingResults = new List<PlayerRatingResult>(ClanMemberRatings);

            //image 1
            tempPlayerRatingResults = CreateBoard(1, tempPlayerRatingResults, 550, 8);
            //image 2
            if (tempPlayerRatingResults.Count > 0)
            {
                tempPlayerRatingResults = CreateBoard(2, tempPlayerRatingResults, 50, 11);
            }
            //image 3
            if (tempPlayerRatingResults.Count > 0)
            {
                tempPlayerRatingResults = CreateBoard(3, tempPlayerRatingResults, 50, 11);
            }
            //image 4
            if (tempPlayerRatingResults.Count > 0)
            {
                tempPlayerRatingResults = CreateBoard(4, tempPlayerRatingResults, 50, 11);
            }
            //image 5
            if (tempPlayerRatingResults.Count > 0)
            {
                tempPlayerRatingResults = CreateBoard(5, tempPlayerRatingResults, 50, 11);
            }
            //statistics
            string statisticsPath = Bot.debuggingPath + Constants.DIR_IMAGES + "ratingboard" + 6 + ".png";
            if (File.Exists(statisticsPath))
            {
                Bitmap originalImage = (Bitmap)Image.FromFile(statisticsPath);
                using (Graphics graphics = Graphics.FromImage(originalImage))
                {
                    using (Font arialFont = new Font(Constants.DEFAULT_FONT_FAMILY, 35f, FontStyle.Bold))
                    {
                        graphics.DrawString(String.Format("{0}/{1}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year), arialFont, Brushes.White, 220, 30);
                        int[] counters = new int[6];
                        for (int i = 0; i < ClanMemberRatings.Count; i++)
                        {
                            switch(ClanMemberRatings[i].Rank)
                            {
                                case RatingRank.Calib: counters[0]++; break;
                                case RatingRank.Bronze: counters[1]++; break;
                                case RatingRank.Silver: counters[2]++; break;
                                case RatingRank.Gold: counters[3]++; break;
                                case RatingRank.Platinum: counters[4]++; break;
                                case RatingRank.Diamond: counters[5]++; break;
                            }
                        }

                        for (int i = 0; i < counters.Length; i++)
                        {
                            var measurements = graphics.MeasureString(counters[i].ToString(), arialFont);
                            graphics.DrawString(counters[i].ToString(), arialFont, Brushes.White, new PointF(245 + 358 * i - measurements.Width / 2, 130));
                        }
                    }
                }
                string pathRating = statisticsPath.Replace(".png", "_temp.png");
                originalImage.Save(pathRating);
                ratingBoardPanels.Add(pathRating);
            }

        }
        private List<PlayerRatingResult> CreateBoard(int boardNumber, List<PlayerRatingResult> tempPlayerRatingResults, int startY, int maxHere)
        {
            string destPath = Bot.debuggingPath + Constants.DIR_IMAGES + "ratingboard" + boardNumber + ".png";
            Bitmap originalImage = (Bitmap)Image.FromFile(destPath);
            int[] xCoords = new int[3] { 725, 1195, 1655 };
            if (boardNumber == 1)
            {
                //1ste is afwijkend
                xCoords = new int[3] { 775, 1275, 1770 };
            }
            int counter = 0;
            using (Graphics graphics = Graphics.FromImage(originalImage))
            {
                using (Font arialFont = new Font(Constants.DEFAULT_FONT_FAMILY, 35f, FontStyle.Bold))
                {
                    if (boardNumber == 1)
                    {
                        graphics.DrawString(String.Format("{0}/{1}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year), arialFont, Brushes.White, 240, 10);
                    }
                    while (tempPlayerRatingResults.Count > 0 && counter < maxHere)
                    {
                        int y = startY + counter * (boardNumber == 1 ? 160 : 153);
                        PlayerRatingResult player = tempPlayerRatingResults[0];
                        var measurements = graphics.MeasureString(player.Name, arialFont);
                        graphics.DrawString(player.Name, arialFont, Brushes.White, new PointF(xCoords[0] - measurements.Width / 2, y));
                        graphics.DrawString(player.Score.ToString(), arialFont, Brushes.White, new PointF(xCoords[2], y));

                        string rankPath = Bot.debuggingPath + Constants.DIR_IMAGES;
                        switch (player.Rank)
                        {
                            case RatingRank.Diamond: rankPath += Constants.IMAGE_DIAMOND_EMBLEM; break;
                            case RatingRank.Platinum: rankPath += Constants.IMAGE_PLATINUM_EMBLEM; break;
                            case RatingRank.Gold: rankPath += Constants.IMAGE_GOLD_EMBLEM; break;
                            case RatingRank.Silver: rankPath += Constants.IMAGE_SILVER_EMBLEM; break;
                            case RatingRank.Bronze: rankPath += Constants.IMAGE_DIAMOND_EMBLEM; break;
                            case RatingRank.Calib: rankPath += Constants.IMAGE_CALIB_EMBLEM; break;
                        }
                        if (File.Exists(rankPath))
                        {
                            var classImage = (Bitmap)Image.FromFile(rankPath);
                            //overlay image
                            graphics.DrawImage(classImage, new Rectangle(xCoords[1] - classImage.Width / 2, y - classImage.Height / 2, (int)(classImage.Width * 1.5), (int)(classImage.Height * 1.5)));
                        }

                        //remove
                        tempPlayerRatingResults.RemoveAt(0);
                        counter++;
                    }
                }
            }
            string pathRating = destPath.Replace(".png", "_temp.png");
            originalImage.Save(pathRating);
            ratingBoardPanels.Add(pathRating);
            return tempPlayerRatingResults;
        }
        public async Task<string> ShowRatingBoard(DiscordChannel channel)
        {
            dmb = new DiscordMessageBuilder();
            await NewUsing(channel, ratingBoardPanels, null);
            return string.Empty;
        }
        private async Task NewUsing(DiscordChannel channel, List<string> boardPanels, Dictionary<string, Stream> fileStreams)
        {
            string thisFilePath = boardPanels[0];
            using (FileStream fs = new FileStream(boardPanels[0], FileMode.Open))
            {
                if (fileStreams == null)
                {
                    fileStreams = new Dictionary<string, Stream>();
                }
                dmb.WithFile(boardPanels[0], fs);
                fileStreams.Add(boardPanels[0], fs);
                boardPanels.RemoveAt(0);
                if (boardPanels.Count > 0)
                {
                    NewUsing(channel, boardPanels, fileStreams).Wait();
                }
                else
                {
                    await channel.SendMessageAsync(dmb);
                }
            }
            File.Delete(thisFilePath);
        }
    }
}
