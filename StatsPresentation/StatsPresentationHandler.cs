using DiscordHelper;
using DSharpPlus.Entities;
using FMWOTB.Account;
using FMWOTB.Tools.Replays;
using NLBE_Bot.Users;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NLBE_Bot.StatsPresentation
{
    public class StatsPresentationHandler
    {
        public static List<Tuple<string, long>> LatestWinrates;
        static StatsPresentationHandler()
        {
            LatestWinrates = new List<Tuple<string, long>>();
        }

        private DiscordMessageBuilder dmb;
        private string fastStatsFilePath = string.Empty;
        private string statsFilePath = string.Empty;
        private string moreFilePath = string.Empty;
        private string badgeFilePath = string.Empty;

        public string LastPlayed { get; set; }
        public string StartedOn { get; set; }
        public string LatestWinrate { get; set; }
        public string Winrate { get; set; }
        public string Clan { get; set; }
        public string Accuracy { get; set; }
        public string Battles { get; set; }
        public string AvgDamage { get; set; }
        public string Defeats { get; set; }
        public string Victories { get; set; }
        public string AmountOfTanks { get; set; }
        public string MostKills { get; set; }
        public string Masteries { get; set; }
        public string BattlersPerDay { get; set; }
        public string Role { get; set; }
        public string ClanJoined { get; set; }
        public string XpPerBattle { get; set; }
        public string RatingScore { get; set; }
        public string RatingRank { get; set; }
        public string PlayerName { get; set; }

        public StatsPresentationHandler()
        {
            //set default empty string values
            foreach (var property in this.GetType().GetProperties())
            {
                property.SetValue(this, ""); //originally stirng empty
            }
        }

        public void SetDataFromAccount(WGAccount account)
        {
            if (account.clan != null)
            {
                if (account.clan.tag != null)
                {
                    Clan = account.clan.tag;

                    Role = account.clan.role.ToString().adaptToDiscordChat();

                    string[] splitted = Bot.convertToDate(account.clan.joined_at.Value).Split(' ');
                    ClanJoined = splitted[0];
                }
            }
            if (account.created_at.HasValue)
            {
                string[] splitted = Bot.convertToDate(account.created_at.Value).Split(' ');
                StartedOn = splitted[0];
            }
            if (account.last_battle_time.HasValue)
            {
                string[] splitted = Bot.convertToDate(account.last_battle_time.Value).Split(' ');
                LastPlayed = splitted[0];
            }
            if (account.statistics != null)
            {
                if (account.statistics.all != null)
                {
                    var winrate = 100 * ((double)account.statistics.all.wins / (double)account.statistics.all.battles);
                    var accuracy = 100 * ((double)account.statistics.all.hits / (double)account.statistics.all.shots);
                    var avgBattlesPerDay = (int)(account.statistics.all.battles / (DateTime.Now - account.created_at.Value).TotalDays);

                    Winrate = String.Format("{0:.##} %", winrate);

                    for (int i = 0; i < LatestWinrates.Count; i++)
                    {
                        if (LatestWinrates[i].Item2 == account.account_id)
                        {
                            LatestWinrate = LatestWinrates[i].Item1;
                            LatestWinrates.RemoveAt(i);
                            break;
                        }
                    }
                    LatestWinrates.Add(new Tuple<string, long>(Winrate, account.account_id));

                    Accuracy = String.Format("{0:.##} %", accuracy);

                    AvgDamage = (account.statistics.all.damage_dealt / account.statistics.all.battles).ToString();

                    int defeats = (int)(account.statistics.all.battles * ((100 - winrate)/100));
                    Defeats = defeats.ToString();
                    Victories = (account.statistics.all.battles-defeats).ToString();

                    Battles = account.statistics.all.battles.ToString();

                    if (account.VehiclesOfPlayers != null)
                    {
                        AmountOfTanks = account.VehiclesOfPlayers.Count.ToString();
                    }
                    BattlersPerDay = String.Format("{0}", avgBattlesPerDay);
                    XpPerBattle = (account.statistics.all.xp / account.statistics.all.battles).ToString();
                    MostKills = account.statistics.all.max_frags.ToString();
                }
            }

            var ratingResult = Bot.GetRatingById(account.account_id, account.nickname);
            RatingRank = ratingResult.Rank.ToString();
            RatingScore = ratingResult.Score.ToString();

            PlayerName = account.nickname;
        }

        public void CreateStats(long wargamingId)
        {
            //load the image file
            Bitmap statsBitmap = (Bitmap)Image.FromFile(Bot.debuggingPath + Constants.DIR_IMAGES + Constants.IMAGE_STATS);

            using (Graphics graphics = Graphics.FromImage(statsBitmap))
            {
                using (Font arialFont = new Font(Constants.DEFAULT_FONT_FAMILY, 35f, FontStyle.Bold))
                {
                    CenterDrawer cd = new CenterDrawer(graphics, arialFont);

                    cd.Draw(StartedOn, Constants.COORD_STATS_STARTED_X, Constants.COORD_STATS_STARTED_Y);
                    cd.Draw(RatingScore, Constants.COORD_STATS_RATED_LEFT_X, Constants.COORD_STATS_RATED_Y);
                    cd.Draw(RatingRank, Constants.COORD_STATS_RATED_RIGHT_X, Constants.COORD_STATS_RATED_Y);
                    cd.Draw(Winrate, Constants.COORD_STATS_COLUMN_2, Constants.COORD_STATS_ROW_1);
                    cd.Draw(Accuracy, Constants.COORD_STATS_COLUMN_1, Constants.COORD_STATS_ROW_2);
                    cd.Draw(Battles, Constants.COORD_STATS_COLUMN_2, Constants.COORD_STATS_ROW_2);
                    cd.Draw(AvgDamage, Constants.COORD_STATS_COLUMN_3, Constants.COORD_STATS_ROW_2);
                    cd.Draw(Defeats, Constants.COORD_STATS_COLUMN_1, Constants.COORD_STATS_ROW_3);
                    cd.Draw(AmountOfTanks, Constants.COORD_STATS_COLUMN_2, Constants.COORD_STATS_ROW_3);
                    cd.Draw(BattlersPerDay, Constants.COORD_STATS_COLUMN_3, Constants.COORD_STATS_ROW_3);
                    cd.Draw(Role, Constants.COORD_STATS_COLUMN_1, Constants.COORD_STATS_ROW_4);
                    cd.Draw(ClanJoined, Constants.COORD_STATS_COLUMN_2, Constants.COORD_STATS_ROW_4);
                    cd.Draw(XpPerBattle, Constants.COORD_STATS_COLUMN_3, Constants.COORD_STATS_ROW_4);
                }
            }
            string path = Bot.debuggingPath + Constants.DIR_IMAGES + Constants.IMAGE_STATS.Replace(".png", "_" + wargamingId + ".png");
            statsBitmap.Save(path);
            this.statsFilePath = path;
        }
        public void CreateFastStats(long wargamingId, DiscordMember member)
        {
            //load the image file
            string statsImagePath = Bot.debuggingPath + Constants.DIR_IMAGES;
            float winrate = float.Parse(Winrate.Split(' ')[0]);
            string wn8 = "0";
            if (winrate < 46)
            {
                statsImagePath += "beginner";
                wn8 = "< 300";
            }
            else if (winrate < 47)
            {
                statsImagePath += "basic";
                wn8 = "> 300";
            }
            else if (winrate < 48)
            {
                statsImagePath += "below_average";
                wn8 = "> 450";
            }
            else if (winrate < 50)
            {
                statsImagePath += "average";
                wn8 = "> 650";
            }
            else if (winrate < 52)
            {
                statsImagePath += "above_average";
                wn8 = "> 900";
            }
            else if (winrate < 54)
            {
                statsImagePath += "good";
                wn8 = "> 1200";
            }
            else if (winrate < 56)
            {
                statsImagePath += "very_good";
                wn8 = "> 1600";
            }
            else if (winrate < 60)
            {
                statsImagePath += "great";
                wn8 = "> 2000";
            }
            else if (winrate < 65)
            {
                statsImagePath += "unicum";
                wn8 = "> 2450";
            }
            else
            {
                statsImagePath += "super_unicum";
                wn8 = "> 2900";
            }
            statsImagePath += ".png";
            Bitmap statsBitmap = (Bitmap)Image.FromFile(statsImagePath);

            using (Graphics graphics = Graphics.FromImage(statsBitmap))
            {
                using (Font arialFont = new Font(Constants.DEFAULT_FONT_FAMILY, 18f, FontStyle.Bold))
                {
                    CenterDrawer cd = new CenterDrawer(graphics, arialFont);

                    int column1 = 105;
                    int column2 = 330;
                    int column3 = 555;
                    int row0 = 55;
                    int row1 = 158;
                    int row2 = 241;
                    int row3 = 325;
                    int row4 = 408;

                    cd.Draw(RatingScore, 215, row0);
                    cd.Draw(RatingRank, 450, row0);

                    int rating = RatingScore == null || RatingScore == string.Empty ? 0 : Int32.Parse(RatingScore);
                    string ratingImagePath = Bot.debuggingPath + Constants.DIR_IMAGES;
                    if (rating == 0)
                    {
                        ratingImagePath += "calibrationemblem";
                    }
                    else if (rating < 2000)
                    {
                        ratingImagePath += "bronzeemblem";
                    }
                    else if (rating < 3000)
                    {
                        ratingImagePath += "silveremblem";
                    }
                    else if (rating < 4000)
                    {
                        ratingImagePath += "goldemblem";
                    }
                    else if (rating < 5000)
                    {
                        ratingImagePath += "platinumemblem";
                    }
                    else
                    {
                        ratingImagePath += "diamondemblem";
                    }
                    ratingImagePath += ".png";
                    var ratingImage = (Bitmap)Image.FromFile(ratingImagePath);
                    int width = 80;
                    graphics.DrawImage(ratingImage, new Rectangle(289, 30, width, width));

                    cd.Draw(LatestWinrate, column1, row1);
                    cd.Draw(Winrate, column3, row1);
                    cd.Draw(Accuracy, column1, row2);
                    cd.Draw(Battles, 597, 35);
                    cd.Draw(AvgDamage, column3, row2);
                    cd.Draw(Defeats, column3, row3);
                    cd.Draw(Victories, column1, row3);
                    cd.Draw(AmountOfTanks, 235, row4); //tanks met masteries
                    cd.Draw(Masteries, 428, row4); //masteries
                    cd.Draw(MostKills, 90, 665);
                    //cd.Draw(TimesMostKills, 90, 680);
                    cd.Draw(wn8, 580, 617);
                    cd.Draw(BattlersPerDay, column2, row3);
                    cd.Draw(XpPerBattle, column2, row2);

                    bool roleFound = false;
                    string roleImagePath = Bot.debuggingPath + Constants.DIR_IMAGES;
                    var roles = member.Roles.OrderBy(r => r.Position).Reverse();
                    var player = new WGAccount(Bot.WG_APPLICATION_ID, wargamingId, loadStatistics: true);
                    foreach (var role in roles)
                    {
                        roleFound = true;
                        switch (role.Id)
                        {
                            case Constants.ROLE_LEIDER: roleImagePath += "a_leider_rank"; break;
                            case Constants.ROLE_DEPUTY: roleImagePath += "b_deputy_rank"; break;
                            case Constants.ROLE_KAPITEIN: roleImagePath += "c_kapitein_rank.png"; break;
                            case Constants.ROLE_FIELD_COMMANDER: roleImagePath += "d_field_commander_rank"; break;
                            case Constants.ROLE_RESERVE_FIELD_COMMANDER: roleImagePath += "Reserve_field_commander"; break;
                            case Constants.ROLE_RECRUITER: roleImagePath += "f_recruiter_rank"; break;
                            case Constants.ROLE_TRAINER: roleImagePath += "g_trainer_rank"; break;
                            case Constants.ROLE_A_TOURNAMENT: roleImagePath += "h_tournament_team_a_rank"; break;
                            case Constants.ROLE_B_TOURNAMENT: roleImagePath += "i_tournament_team_b_rank"; break;
                            case Constants.ROLE_CREW: roleImagePath += "j_crew_rank"; break;
                            default: roleFound = false; break;
                        }
                        if (roleFound)
                        {
                            break;
                        }
                    }
                    if (!roleFound)
                    {
                        roleImagePath += "k_guest_rank";
                    }
                    roleImagePath += ".png";

                    var roleImage = (Bitmap)Image.FromFile(roleImagePath);
                    //overlay image
                    graphics.DrawImage(roleImage, new Rectangle(column2-60, 440- roleImage.Height/2, roleImage.Width, roleImage.Height));

                    var dtLastPlayed = Bot.convertShortStringToDateTime(LastPlayed);
                    int days = (DateTime.Now - dtLastPlayed).Days;
                    string activityImagePath = Bot.debuggingPath + Constants.DIR_IMAGES;
                    if (days < 2)
                    {
                        activityImagePath += "11_green_activity";
                    }
                    else if (days < 7)
                    {
                        activityImagePath += "12_jellow_activity";
                    }
                    else if (days < 31)
                    {
                        activityImagePath += "13_orange_activity";
                    }
                    else
                    {
                        activityImagePath += "14_red_activity";
                    }
                    activityImagePath += ".png";
                    var activityImage = (Bitmap)Image.FromFile(activityImagePath);
                    graphics.DrawImage(activityImage, new Rectangle(15, 35, activityImage.Width, activityImage.Height));


                    int substringLength = PlayerName.Length;
                    int nameHeightLess = 0;
                    bool goodSubstringLength = false;
                    while (!goodSubstringLength)
                    {
                        var tempMeasurements = graphics.MeasureString(PlayerName.Substring(0, substringLength), arialFont);
                        if (tempMeasurements.Width > 180)
                        {
                            substringLength--;
                        }
                        else
                        {
                            goodSubstringLength = true;
                            nameHeightLess = (int)(tempMeasurements.Height / 2);
                        }
                    }
                    string name = PlayerName.Substring(0, substringLength);
                    if (PlayerName.Length != substringLength)
                    {
                        name = name.Substring(0, name.Length - 4) + "...";
                    }
                    cd.Draw(name, 150, 440- nameHeightLess);
                }
            }
            string path = Bot.debuggingPath + Constants.DIR_IMAGES + Constants.IMAGE_FAST_STATS.Replace(".png", "_" + wargamingId + ".png");
            statsBitmap.Save(path);
            this.fastStatsFilePath = path;
        }

        public async Task ShowBoth(DiscordChannel channel, bool delete = true)
        {
            if (statsFilePath.Length > 0 && File.Exists(statsFilePath))
            {
                using (FileStream fs = new FileStream(statsFilePath, FileMode.Open))
                {
                    using (FileStream fsMore = new FileStream(moreFilePath, FileMode.Open))
                    {
                        if (badgeFilePath.Length > 0 && File.Exists(badgeFilePath))
                        {
                            using (FileStream fsBadge = new FileStream(badgeFilePath, FileMode.Open))
                            {
                                var dict = new Dictionary<string, Stream>();
                                dict.Add(badgeFilePath, fsBadge);
                                dict.Add(statsFilePath, fs);
                                dict.Add(moreFilePath, fsMore);
                                dmb = new DiscordMessageBuilder();
                                dmb.WithFiles(dict);
                                await channel.SendMessageAsync(dmb);
                            }
                        }
                        else
                        {
                            var dict = new Dictionary<string, Stream>();
                            dict.Add(statsFilePath, fs);
                            dict.Add(moreFilePath, fsMore);
                            dmb = new DiscordMessageBuilder();
                            dmb.WithFiles(dict);
                            await channel.SendMessageAsync(dmb);
                        }
                    }
                }
                if (delete)
                {
                    File.Delete(statsFilePath);
                    statsFilePath = string.Empty;
                }
            }
        }
        public async Task ShowStats(DiscordChannel channel, bool delete = true)
        {
            await ShowSomething(statsFilePath, channel, delete);
            statsFilePath = string.Empty;
        }
        public async Task ShowFastStats(DiscordChannel channel, bool delete = true)
        {
            await ShowSomething(fastStatsFilePath, channel, delete);
            fastStatsFilePath = string.Empty;
        }

        public async Task ShowMore(DiscordChannel channel, bool delete = true)
        {
            await ShowSomething(moreFilePath, channel, delete);
            moreFilePath = string.Empty;
        }
        private async Task ShowSomething(string path, DiscordChannel channel, bool delete = true)
        {
            if (path.Length > 0 && File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    dmb = new DiscordMessageBuilder();
                    dmb.WithFile(path, fs);
                    await channel.SendMessageAsync(dmb);
                }
                if (delete)
                {
                    File.Delete(path);
                }
            }
        }

        public void CreateMoreInfo(long wargamingId,
            string tanksWithAce,
            string mostDestroyed,
            string noInfoFor,
            TierInfo[] tiers)
        {
            //load the image file
            Bitmap moreBitmap = (Bitmap)Image.FromFile(Bot.debuggingPath + Constants.DIR_IMAGES + Constants.IMAGE_MORE_INFO);

            using (Graphics graphics = Graphics.FromImage(moreBitmap))
            {
                using (Font arialFont = new Font(Constants.DEFAULT_FONT_FAMILY, 35f, FontStyle.Bold))
                {
                    var measurements = graphics.MeasureString(tanksWithAce, arialFont);
                    graphics.DrawString(tanksWithAce, arialFont, Brushes.White, new PointF(Constants.COORD_MORE_TOP_INFO_X - measurements.Width / 2, Constants.COORD_MORE_TOP_INFO_1_Y));
                    measurements = graphics.MeasureString(mostDestroyed, arialFont);
                    graphics.DrawString(mostDestroyed, arialFont, Brushes.White, new PointF(Constants.COORD_MORE_TOP_INFO_X - measurements.Width / 2, Constants.COORD_MORE_TOP_INFO_2_Y));
                    measurements = graphics.MeasureString(tanksWithAce, arialFont);
                    graphics.DrawString(noInfoFor, arialFont, Brushes.White, new PointF(Constants.COORD_MORE_NO_INFO_TANKS_X - measurements.Width / 2, Constants.COORD_MORE_NO_INFO_TANKS_Y));
                    for (int i = 0; i < tiers.Length; i++)
                    {
                        int x = Constants.COORD_TIER_FIRST_COLUMN + Constants.COORD_TIER_COLUMN_DISTANCE * i;
                        graphics.DrawString(tiers[i].Battles.ToString(), arialFont, Brushes.White,
                            new PointF(x, Constants.COORD_TIER_FIRST_ROW));
                        graphics.DrawString(String.Format("{0:##0.00}", tiers[i].Winrate).Replace(',', '.').ToString(), arialFont, Brushes.White,
                            new PointF(x, Constants.COORD_TIER_FIRST_ROW + Constants.COORD_TIER_ROW_DISTANCE));
                        graphics.DrawString(tiers[i].Avarage_damage.ToString(), arialFont, Brushes.White,
                            new PointF(x, Constants.COORD_TIER_FIRST_ROW + Constants.COORD_TIER_ROW_DISTANCE * 2));
                    }
                }
            }

            string pathMore = Bot.debuggingPath + Constants.DIR_IMAGES + Constants.IMAGE_MORE_INFO.Replace(".png", "_" + wargamingId + ".png");
            moreBitmap.Save(pathMore);
            this.moreFilePath = pathMore;
        }



        public async Task<string> ShowUserBadge(DiscordChannel channel)
        {
            using (FileStream fs = new FileStream(badgeFilePath, FileMode.Open))
            {

                dmb = new DiscordMessageBuilder();
                dmb.WithFile(badgeFilePath, fs);
                await channel.SendMessageAsync(dmb);
            }
            File.Delete(badgeFilePath);
            return string.Empty;
        }

        /// <summary>
        /// UserRelations must be complete before passing it.
        /// </summary>
        /// <param name="userRelations"></param>
        /// <returns>the filepath</returns>
        public string CreateUserBadge(UserRelations userRelations, ulong guildId)
        {
            if (userRelations.DiscordID == 0)
            {
                return "DiscordID moet nog toegevoegd worden!";
            }
            else if (userRelations.WargamingID == 0)
            {
                return "WargamingID moet nog toegevoegd worden!";
            }
            var curDir = Directory.GetCurrentDirectory();
            string imagePath = Bot.debuggingPath + "images/";
            bool roleFound = false;
            int yDiffForWinrateAndRanked = 0;
            IReadOnlyCollection<DiscordMember> members = null;
            try
            {
                 members = Bot.discordClient.GetGuildAsync(guildId).Result.GetAllMembersAsync().Result;
            }
            catch
            {
                return string.Empty;
            }
            var member = members.Where(m => m.Id == userRelations.DiscordID).FirstOrDefault();
            if (member == null)
            {
                return "Geen lid van deze server.";
            }
            var roles = member.Roles.OrderBy(r => r.Position).Reverse();
            var player = new WGAccount(Bot.WG_APPLICATION_ID, userRelations.WargamingID, loadStatistics: true);
            foreach (var role in roles)
            {
                roleFound = true;
                switch (role.Id)
                {
                    case Constants.ROLE_LEIDER: imagePath += "Leider"; break;
                    case Constants.ROLE_DEPUTY: imagePath += "Deputy"; yDiffForWinrateAndRanked = Constants.COORD_BADGE_BORDER_WINRATE_Y_DIFF; break;
                    case Constants.ROLE_KAPITEIN: imagePath += "Kapitein"; yDiffForWinrateAndRanked = Constants.COORD_BADGE_BORDER_WINRATE_Y_DIFF; break;
                    case Constants.ROLE_FIELD_COMMANDER: imagePath += "Field_commander"; yDiffForWinrateAndRanked = Constants.COORD_BADGE_BORDER_WINRATE_Y_DIFF; break;
                    case Constants.ROLE_RESERVE_FIELD_COMMANDER: imagePath += "Reserve_field_commander"; yDiffForWinrateAndRanked = Constants.COORD_BADGE_BORDER_WINRATE_Y_DIFF; break;
                    case Constants.ROLE_RECRUITER: imagePath += "Recruiter"; break;
                    case Constants.ROLE_TRAINER: imagePath += "Trainer"; break;
                    case Constants.ROLE_A_TOURNAMENT: imagePath += "A_Tournament"; yDiffForWinrateAndRanked = Constants.COORD_BADGE_BORDER_WINRATE_Y_DIFF_TOURNAMENT; break;
                    case Constants.ROLE_B_TOURNAMENT: imagePath += "B_Tournament"; yDiffForWinrateAndRanked = Constants.COORD_BADGE_BORDER_WINRATE_Y_DIFF_TOURNAMENT; break;
                    case Constants.ROLE_CREW: imagePath += "Crew"; yDiffForWinrateAndRanked = Constants.COORD_BADGE_BORDER_WINRATE_Y_DIFF; break;
                    default: roleFound = false; break;
                }
                if (roleFound)
                {
                    break;
                }
            }
            if (!roleFound)
            {
                yDiffForWinrateAndRanked = Constants.COORD_BADGE_BORDER_WINRATE_Y_DIFF;
                imagePath += "Guest";
            }
            imagePath += ".png";
            Bitmap bitmap = null;
            try
            {
                bitmap = (Bitmap)Image.FromFile(imagePath);//load the image file
            }
            catch (Exception ex)
            {

            }

            //spelerinfo
            var playerRating = Bot.GetRatingById(userRelations.WargamingID, player.nickname);
            string rankPath = Bot.debuggingPath + Constants.DIR_IMAGES;
            if (playerRating != null)
            {
                switch (playerRating.Rank)
                {
                    case NLBE_Bot.RatingRank.Diamond: rankPath += Constants.IMAGE_DIAMOND_EMBLEM; break;
                    case NLBE_Bot.RatingRank.Platinum: rankPath += Constants.IMAGE_PLATINUM_EMBLEM; break;
                    case NLBE_Bot.RatingRank.Gold: rankPath += Constants.IMAGE_GOLD_EMBLEM; break;
                    case NLBE_Bot.RatingRank.Silver: rankPath += Constants.IMAGE_SILVER_EMBLEM; break;
                    case NLBE_Bot.RatingRank.Bronze: rankPath += Constants.IMAGE_BRONZE_EMBLEM; break;
                    case NLBE_Bot.RatingRank.Calib: rankPath += Constants.IMAGE_CALIB_EMBLEM; break;
                }
            }
            string borderPath = Bot.debuggingPath + Constants.DIR_IMAGES;
            var winrate = 100 * ((double)player.statistics.all.wins / (double)player.statistics.all.battles);
            string winrateString = String.Format("{0:N2}", winrate);

            if (winrate < 50)
            {
                borderPath += Constants.IMAGE_SILVER_BORDER;
            }
            else if (winrate < 60)
            {
                borderPath += Constants.IMAGE_GOLDSILVER_BORDER;
            }
            else
            {
                borderPath += Constants.IMAGE_GOLD_BORDER;
            }

            WebClient client = new WebClient();
            Stream stream = client.OpenRead(member.AvatarUrl);
            Bitmap avatarBitmap = new Bitmap(stream);
            avatarBitmap = Helper.ClipToCircle(avatarBitmap);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                bool nameFits = false;
                float fontSize = 35f;
                while (!nameFits)
                {
                    using (Font arialFont = new Font(Constants.DEFAULT_FONT_FAMILY, fontSize, FontStyle.Bold))
                    {
                        var measurements = graphics.MeasureString(player.nickname, arialFont);
                        if (measurements.Width <= 580)
                        {
                            nameFits = true;
                            graphics.DrawString(player.nickname, arialFont, Brushes.White, new PointF(170, 440));
                        }
                        else
                        {
                            fontSize--;
                        }
                    }
                }
                int y = Constants.COORD_BADGE_BORDER_WINRATE_Y - yDiffForWinrateAndRanked;
                int yText = y + 2;
                using (Font arialFont = new Font(Constants.DEFAULT_FONT_FAMILY, 35f, FontStyle.Bold))
                {
                    if (File.Exists(borderPath))
                    {
                        var classImage = (Bitmap)Image.FromFile(borderPath);
                        //overlay image
                        graphics.DrawImage(classImage, new Rectangle(475, y, classImage.Width, classImage.Height));
                        graphics.DrawImage(classImage, new Rectangle(662, y, classImage.Width, classImage.Height));
                    }
                    if (playerRating != null && File.Exists(rankPath))
                    {
                        var classImage = (Bitmap)Image.FromFile(rankPath);
                        //overlay image
                        graphics.DrawImage(classImage, new Rectangle(612 - classImage.Width / 2, y + 25 - classImage.Height / 2, classImage.Width, classImage.Height));
                    }
                }
                using (Font arialFont = new Font(Constants.DEFAULT_FONT_FAMILY, 28f, FontStyle.Regular))
                {
                    graphics.DrawString("WR", arialFont, Brushes.Black, new PointF(482, yText));
                    graphics.DrawString(winrateString, arialFont, Brushes.Black, new PointF(655, yText));
                }
                //overlay image
                graphics.DrawImage(avatarBitmap, new Rectangle(140, 100, 256, 256));
            }
            string endPath = Bot.debuggingPath + Constants.DIR_IMAGES + "badge.png";
            bitmap.Save(endPath);
            badgeFilePath = endPath;
            return string.Empty;
        }

        public string replayInfoPath;
        public Dictionary<string, string> getSomeReplayInfo(WGBattle battle, int position, ulong guildId)
        {
            var dic = new Dictionary<string, string>();
            dic.Add(StatsPresentationHandler.LINK, battle.view_url);
            dic.Add(StatsPresentationHandler.PLAYER, battle.player_name);
            dic.Add(StatsPresentationHandler.CLAN, battle.details.clan_tag);
            dic.Add(StatsPresentationHandler.TANK, battle.vehicle);
            dic.Add(StatsPresentationHandler.TIER, battle.vehicle_tier.ToString());
            dic.Add(StatsPresentationHandler.DAMAGE, battle.details.damage_made.ToString());
            dic.Add(StatsPresentationHandler.DAMAGE_BOUNCED, battle.details.damage_blocked.ToString());
            dic.Add(StatsPresentationHandler.ASSIST_DAMAGE, (battle.details.damage_assisted + battle.details.damage_assisted_track).ToString());
            dic.Add(StatsPresentationHandler.EXPERIENCE, battle.details.exp.ToString());
            dic.Add(StatsPresentationHandler.HITS, battle.details.shots_pen.ToString());
            dic.Add(StatsPresentationHandler.TANKS_DESTROYED, battle.details.enemies_destroyed.ToString());
            dic.Add(StatsPresentationHandler.KILLS, battle.details.enemies_destroyed.ToString());
            dic.Add(StatsPresentationHandler.MAP, battle.map_name);
            string resultaat = "Gewonnen";
            if (battle.protagonist_team != battle.winner_team)
            {
                if (battle.winner_team != 2 && battle.winner_team != 1)
                {
                    resultaat = "Gelijk gespeeld";
                }
                else
                {
                    resultaat = "Verloren";
                }
            }
            dic.Add(StatsPresentationHandler.RESULT, resultaat);
            if (battle.battle_start_time.HasValue)
            {
                dic.Add(StatsPresentationHandler.DATE, (battle.battle_start_time.Value.Day < 10 ? "0" : string.Empty) + battle.battle_start_time.Value.Day + "-" + battle.battle_start_time.Value.Month + "-" + battle.battle_start_time.Value.Year + " " + battle.battle_start_time.Value.Hour + ":" + (battle.battle_start_time.Value.Minute < 10 ? "0" : string.Empty) + battle.battle_start_time.Value.Minute + ":" + (battle.battle_start_time.Value.Second < 10 ? "0" : string.Empty) + battle.battle_start_time.Value.Second);
            }
            dic.Add(StatsPresentationHandler.TYPE, WGBattle.getBattleType(battle.battle_type));
            dic.Add(StatsPresentationHandler.MODE, WGBattle.getBattleRoom(battle.room_type));
            dic.Add(StatsPresentationHandler.HOF, position.ToString());
            if (battle.details.achievements != null && battle.details.achievements.Count > 0)
            {
                List<FMWOTB.Achievement> achievementList = new List<FMWOTB.Achievement>();
                var fields = new List<string>()
                {
                    "-image_big",
                    "-section",
                    "-options",
                    "-image",
                    "-condition",
                    "-description",
                    "-order"
                };
                for (int i = 0; i < battle.details.achievements.Count; i++)
                {
                    FMWOTB.Achievement tempAchievement = FMWOTB.Achievement.getAchievement(Bot.WG_APPLICATION_ID, battle.details.achievements.ElementAt(i).t, fields).Result;
                    if (tempAchievement != null)
                    {
                        achievementList.Add(tempAchievement);
                    }
                }
                if (achievementList.Count > 0)
                {
                    achievementList = achievementList.OrderBy(x => x.order).ToList();
                    StringBuilder sbAchievements = new StringBuilder();
                    foreach (FMWOTB.Achievement tempAchievement in achievementList)
                    {
                        sbAchievements.AppendLine(tempAchievement.name.Replace("\n", string.Empty).Replace("(" + tempAchievement.achievement_id + ")", string.Empty));
                    }
                    dic.Add(StatsPresentationHandler.ACHIEVEMENTS, sbAchievements.ToString());
                }
                else
                {
                    dic.Add(StatsPresentationHandler.ACHIEVEMENTS, string.Empty);
                }
            }
            else
            {
                dic.Add(StatsPresentationHandler.ACHIEVEMENTS, string.Empty);
            }

            var ur = UserRelations.GetRelations(new UserRelations() { WargamingID = battle.protagonist });
            if (ur.DiscordID > 0)
            {
                CreateUserBadge(ur, guildId);
            }

            return dic;
        }
        public void CreateReplayInfo(Dictionary<string, string> info, bool isMasteryReplay)
        {
            //get path
            string path = isMasteryReplay ? Constants.IMAGE_MASTERY_REPLAY : Constants.IMAGE_REGULAR_REPLAY;

            //load the image file
            Bitmap replayBitmap = (Bitmap)Image.FromFile(Bot.debuggingPath + Constants.DIR_IMAGES + path);
            Bitmap badgeImage = null;
            Rectangle badgeRect = new Rectangle(60, 200, 450, 200);
            if (badgeFilePath.Length > 0 && File.Exists(badgeFilePath))
            {
                badgeImage = (Bitmap)Image.FromFile(badgeFilePath);

            }
            using (Graphics graphics = Graphics.FromImage(replayBitmap))
            {
                using (Font arialFont = new Font(Constants.DEFAULT_FONT_FAMILY, 35f, FontStyle.Bold))
                {
                    CenterDrawer cd = new CenterDrawer(graphics, arialFont);
                    cd.Draw(info[RESULT], replayBitmap.Width / 2, badgeRect.Y + (isMasteryReplay ? 33 : 85));
                    cd.Draw(info[TANK], Constants.COORD_FIRST_COLUMN_X, 450);
                    cd.Draw(info[TIER], Constants.COORD_FIRST_COLUMN_X, 530);
                    cd.Draw(info[MAP], Constants.COORD_FIRST_COLUMN_X, 610);
                    cd.Draw(info[TYPE], Constants.COORD_FIRST_COLUMN_X, 750);
                    cd.Draw(info[DAMAGE], Constants.COORD_FIRST_COLUMN_X, 890);
                    cd.Draw(info[DAMAGE_BOUNCED], Constants.COORD_FIRST_COLUMN_X, 970);
                    cd.Draw(info[ASSIST_DAMAGE], Constants.COORD_FIRST_COLUMN_X, 1060);
                    cd.Draw(info[EXPERIENCE], Constants.COORD_SECOND_COLUMN_X, 890);
                    cd.Draw(info[HITS], Constants.COORD_SECOND_COLUMN_X, 970);
                    cd.Draw(info[KILLS], Constants.COORD_SECOND_COLUMN_X, 1060);
                    cd.Draw(info[MODE], 1600, 750);
                    graphics.DrawString(info[PLAYER], arialFont, Brushes.White, 200, 1210);
                    graphics.DrawString(info[CLAN], arialFont, Brushes.White, 200, 1260);
                    graphics.DrawString(info[DATE], arialFont, Brushes.White, 1340, 1240);
                    if (Int32.Parse(info[HOF]) > 0)
                    {
                        cd.Draw(info[HOF], 1170, 325);
                    }
                }
                if (info[ACHIEVEMENTS] != null && info[ACHIEVEMENTS].Length > 0)
                {
                    var splitted = info[ACHIEVEMENTS].Split("\n");
                    using (Font arialFont = new Font(Constants.DEFAULT_FONT_FAMILY, 140f / splitted.Length > 35f ? 35f : 140 / splitted.Length, FontStyle.Bold))
                    {
                        CenterDrawer cd = new CenterDrawer(graphics, arialFont);
                        cd.Draw(info[ACHIEVEMENTS], 1425, 940);
                    }
                }
                string classPath = string.Empty;
                if (info[ACHIEVEMENTS].Contains("Mastery badge: Ace Tanker"))
                {
                    classPath += Constants.IMAGE_ACE;
                }
                else if (info[ACHIEVEMENTS].Contains("Mastery badge: Class III"))
                {
                    classPath += Constants.IMAGE_THIRD_CLASS;
                }
                else if (info[ACHIEVEMENTS].Contains("Mastery badge: Class II"))
                {
                    classPath += Constants.IMAGE_SECOND_CLASS;
                }
                else if (info[ACHIEVEMENTS].Contains("Mastery badge: Class I")) {
                    classPath += Constants.IMAGE_FIRST_CLASS;
                }
                if (classPath.Length > 0)
                {
                    var classImage = (Bitmap)Image.FromFile(Bot.debuggingPath + Constants.DIR_IMAGES + classPath);
                    //overlay image
                    graphics.DrawImage(classImage, new Rectangle(1350, 565, 130, 130));
                }
                if (badgeImage != null)
                {
                    //overlay image
                    graphics.DrawImage(badgeImage, badgeRect);
                }
            }

            replayInfoPath = Bot.debuggingPath + Constants.DIR_IMAGES + path.Replace(".png", "_.png");
            replayBitmap.Save(replayInfoPath);
            if (badgeFilePath.Length > 0 && File.Exists(badgeFilePath))
            {
                try
                {
                    File.Delete(badgeFilePath);
                }
                catch
                {
                    //Bot.handleError("StatsPresentationHandler > Deleting badgeImage", ex.Message, ex.StackTrace).Wait();
                }
            }
        }

        public async Task<DiscordMessage> ShowReplayInfoAsEmbed(DiscordChannel channel, WGBattle battle, string eventDescription, DiscordMessage uploadMessage, DiscordColor color)
        {
            var testReplaysChannel = await Bot.GetTestReplaysChannel();
            var message = await ShowReplayInfo(testReplaysChannel);
            //await message.DeleteAsync();
            var deb = new DiscordEmbedBuilder();
            deb.Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = Bot.DEF_LOGO_IMGUR };
            deb.Color = color;
            deb.ImageUrl = message.Attachments[0].Url;
            deb.Title = "Download link";
            deb.Url = uploadMessage.Attachments != null && uploadMessage.Attachments.Count > 0 ? uploadMessage.Attachments[0].Url : uploadMessage.Content;
            StringBuilder sb = new StringBuilder();
            sb.Append(DiscordHelper.Helper.CreateTitle("Statistieken online", battle.view_url));
            sb.Append(" | ");
            sb.Append(DiscordHelper.Helper.CreateTitle("Bekijk online", battle.view_online));
            if (eventDescription != null && eventDescription.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(eventDescription);
            }
            deb.Description = sb.ToString();
            var embed = deb.Build();
            //staat in commentaar omdat de download link anders niet beschikbaar is
            //try
            //{
            //    //await uploadMessage.DeleteAsync();
            //}
            //catch
            //{

            //}
            return await channel.SendMessageAsync(embed: embed);
        }
        private async Task<DiscordMessage> ShowReplayInfo(DiscordChannel channel)
        {
            DiscordMessage message = null;
            using (FileStream fs = new FileStream(replayInfoPath, FileMode.Open))
            {
                dmb = new DiscordMessageBuilder();
                dmb.WithFile(replayInfoPath, fs);
                message = await channel.SendMessageAsync(dmb);
            }
            File.Delete(replayInfoPath);
            return message;
        }

        public const string PLAYER = "speler";
        public const string CLAN = "Clan";
        public const string LINK = "Link";
        public const string TANK = "Tank";
        public const string TIER = "Tier";
        public const string DAMAGE = "Damage";
        public const string DAMAGE_BOUNCED = "Damage bounced";
        public const string ASSIST_DAMAGE = "Assist damage";
        public const string EXPERIENCE = "exp";
        public const string HITS = "Hits";
        public const string TANKS_DESTROYED = "Tanks vernietigd";
        public const string MAP = "Map";
        public const string RESULT = "Resultaat";
        public const string DATE = "Datum";
        public const string TYPE = "Type";
        public const string MODE = "Mode";
        public const string HOF = "Positie in HOF";
        public const string ACHIEVEMENTS = "Achievements";
        public const string KILLS = "Kills";
    }
}
