using DSharpPlus.Entities;
using FMWOTB.Account;
using FMWOTB.Clans;
using FMWOTB.Tools;
using JsonObjectConverter;
using NLBE_Bot.StatsPresentation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NLBE_Bot.Activity
{
    public class ActivityBoard
    {
        private DiscordMessageBuilder dmb;
        public List<WGAccountShort>[] PlayersActivities { get; set; }
        private string filePath;
        private static string GIF_FILE_PATH = Bot.debuggingPath + Constants.DIR_IMAGES + Constants.GIF_CLAN_NAME;
        private static string TOP_IMAGE_FILE_PATH = Bot.debuggingPath + Constants.DIR_IMAGES + Constants.IMAGE_CLAN_NAME;
        private static string IMAGE_LEGEND_FILE_PATH = Bot.debuggingPath + Constants.DIR_IMAGES + Constants.IMAGE_ACTIVITY_LEGEND;
        private static string IMAGE_CHECK_LEGEND_FILE_PATH = Bot.debuggingPath + Constants.DIR_IMAGES + Constants.IMAGE_ACTIVITY_CHECK_LEGEND;

        public void Initialize(string clan)
        {
            var clanResults = WGClan.searchByName(SearchAccuracy.EXACT, clan, Bot.WG_APPLICATION_ID, true).Result;

            if (clanResults != null && clanResults.Count > 0)
            {
                Initialize(clanResults.First());
            }
        }
        public void Initialize(WGClan clan)
        {
            int categories = 4;
            PlayersActivities = new List<WGAccountShort>[categories];
            for (int i = 0; i < categories; i++)
            {
                PlayersActivities[i] = new List<WGAccountShort>();
            }

            if (clan != null && clan.members != null && clan.members.Count > 0)
            {
                var players = new List<WGAccountShort>();

                string url = "https://api.wotblitz.eu/wotb/account/info/?application_id=1d6ad555a9f55cf57d94abe71a106640";
                var fields = new List<string>()
                {
                    "-created_at",
                    "-last_battle_time",
                    "-private",
                    "-statistics"
                };
                StringBuilder sbIds = new StringBuilder();
                for (int i = 0; i < clan.members.Count; i++)
                {
                    if (i > 0)
                    {
                        sbIds.Append(Constants.URL_VALUE_SPLITTER);
                    }
                    sbIds.Append(clan.members[i].account_id);
                }
                StringBuilder sbFields = new StringBuilder();
                for (int i = 0; i < fields.Count; i++)
                {
                    if (i > 0)
                    {
                        sbFields.Append(Constants.URL_VALUE_SPLITTER);
                    }
                    sbFields.Append(fields[i]);
                }
                //var headers = new List<Tuple<string, string>>()
                //{
                //    new Tuple<string, string>("fields", sbFields.ToString()),
                //    new Tuple<string, string>("account_id", sbIds.ToString())
                //};

                StringBuilder sbUrl = new StringBuilder(url);
                sbUrl.Append(Constants.URL_PARAM_SPLITTER);
                sbUrl.Append("fields");
                sbUrl.Append(Constants.URL_KEY_VALUE_SPLITTER);
                sbUrl.Append(sbFields);
                sbUrl.Append(Constants.URL_PARAM_SPLITTER);
                sbUrl.Append("account_id");
                sbUrl.Append(Constants.URL_KEY_VALUE_SPLITTER);
                sbUrl.Append(sbIds);

                Json json = HttpClientRequester.SendRequest(sbUrl.ToString(), HttpMethod.Get);
                if (json != null && json.subJsons != null && json.subJsons.Count > 1 && json.subJsons[1].subJsons != null && json.subJsons[1].subJsons.Count > 0)
                {
                    for (int i = 0; i < json.subJsons[1].subJsons.Count; i++)
                    {
                        players.Add(new WGAccountShort()
                        {
                            Nickname = json.subJsons[1].subJsons[i].tupleList[0].Item2.Item1,
                            Account_id = long.Parse(json.subJsons[1].subJsons[i].tupleList[1].Item2.Item1),
                            Last_time_played = Json.convertStringToDateTime(json.subJsons[1].subJsons[i].tupleList[2].Item2.Item1)
                        });
                    }
                    for (int i = 0; i < players.Count; i++)
                    {
                        int days = (DateTime.Now - players[i].Last_time_played).Days;
                        if (days < 2)
                        {
                            PlayersActivities[0].Add(players[i]);
                        }
                        else if (days < 7)
                        {
                            PlayersActivities[1].Add(players[i]);
                        }
                        else if (days < 31)
                        {
                            PlayersActivities[2].Add(players[i]);
                        }
                        else
                        {
                            PlayersActivities[3].Add(players[i]);
                        }
                    }
                }
            }
            for (int i = 0; i < categories; i++)
            {
                PlayersActivities[i] = PlayersActivities[i].OrderBy(n => n.Nickname).ToList();
            }
        }

        public void CreateActivityBoard()
        {
            string srcPath = Bot.debuggingPath + Constants.DIR_IMAGES + Constants.IMAGE_ACTIVITY_TABLE;
            Bitmap originalImage = (Bitmap)Image.FromFile(srcPath);
            int[] xCoords = new int[3] { 130, 335, 537 };
            float fontSize = 7f;
            int columns = 3;
            int rows = 17;
            int yStarter = 44;
            int totalCount = Count();
            using (Graphics graphics = Graphics.FromImage(originalImage))
            {
                using (Font arialFont = new Font(Constants.DEFAULT_FONT_FAMILY, fontSize, FontStyle.Bold))
                {
                    for (int i = 0; i < columns; i++)
                    {
                        for (int j = 0; j < rows; j++)
                        {
                            int calculatedCounter = i * rows + j;
                            if (totalCount > calculatedCounter)
                            {
                                var playerTuple = GetWGAccountShort(calculatedCounter);
                                var player = playerTuple.Item1;
                                int substringLength = player.Nickname.Length;
                                bool goodSubstringLength = false;
                                while (!goodSubstringLength)
                                {
                                    var tempMeasurements = graphics.MeasureString(player.Nickname.Substring(0, substringLength), arialFont);
                                    if (tempMeasurements.Width > 180)
                                    {
                                        substringLength--;
                                    }
                                    else
                                    {
                                        goodSubstringLength = true;
                                    }
                                }
                                string name = player.Nickname.Substring(0, substringLength);
                                if (player.Nickname.Length != substringLength)
                                {
                                    name = name.Substring(0, name.Length - 4) + "...";
                                }
                                var measurements = graphics.MeasureString(name, arialFont);
                                int calculatedY = (int)(yStarter + j * 20.20);
                                int calculatedX = (int)(xCoords[i] - measurements.Width / 2);
                                graphics.DrawString(name, arialFont, Brushes.White, calculatedX, calculatedY);
                                //overlay image
                                string dotImageFileName = string.Empty;
                                switch (playerTuple.Item2)
                                {
                                    case 0: dotImageFileName = Constants.IMAGE_DOT_GREEN; break;
                                    case 1: dotImageFileName = Constants.IMAGE_DOT_YELLOW; break;
                                    case 2: dotImageFileName = Constants.IMAGE_DOT_ORANGE; break;
                                    case 3: dotImageFileName = Constants.IMAGE_DOT_RED; break;
                                }
                                Bitmap dotBitmap = (Bitmap)Image.FromFile(Bot.debuggingPath + Constants.DIR_IMAGES + dotImageFileName);//load the image file
                                graphics.DrawImage(dotBitmap, new Rectangle(xCoords[i] - 90, calculatedY + 1, 8, 8));
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    for (int i = 0; i < PlayersActivities.Length; i++)
                    {
                        graphics.DrawString(PlayersActivities[i].Count.ToString(), arialFont, Brushes.White, xCoords[2], 383 + i * 17);
                    }
                    graphics.DrawString(totalCount.ToString(), arialFont, Brushes.White, 147, 412);
                }
            }
            filePath = srcPath.Replace(".png", "_temp.png");
            originalImage.Save(filePath);
        }
        private Tuple<WGAccountShort, int> GetWGAccountShort(int index)
        {
            for (int i = 0; i < PlayersActivities.Length; i++)
            {
                if (PlayersActivities[i].Count > index)
                {
                    return new Tuple<WGAccountShort, int>(PlayersActivities[i][index], i);
                }
                else
                {
                    index -= PlayersActivities[i].Count;
                }
            }
            return null;
        }
        private int Count()
        {
            int counter = 0;
            for (int i = 0; i < PlayersActivities.Length; i++)
            {
                counter += PlayersActivities[i].Count;
            }
            return counter;
        }
        public void CreateActivityBoard_Old()
        {
            string srcPath = Bot.debuggingPath + Constants.DIR_IMAGES + Constants.IMAGE_ACTIVITY_CHECK_TABLES;
            Bitmap originalImage = (Bitmap)Image.FromFile(srcPath);
            int[] xCoords = new int[3] { 120, 335, 543 };
            int padding = 2;
            using (Graphics graphics = Graphics.FromImage(originalImage))
            {
                for (int i = 0; i < 3; i++)
                {
                    float fontSize = 15f;
                    bool isOkGoodFontSize = false;
                    while (!isOkGoodFontSize)
                    {
                        var measurements = graphics.MeasureString("T", new Font(Constants.DEFAULT_FONT_FAMILY, fontSize, FontStyle.Bold));
                        if ((measurements.Height + padding*2) * PlayersActivities[i].Count < 315)
                        {
                            isOkGoodFontSize = true;
                        }
                        else
                        {
                            fontSize-=0.1f;
                        }
                    }
                    int yTotal = 90;
                    for (int j = 0; j < PlayersActivities[i].Count; j++)
                    {
                        using (Font arialFont = new Font(Constants.DEFAULT_FONT_FAMILY, fontSize, FontStyle.Bold))
                        {
                            yTotal += padding; //padding
                            int substringLength = PlayersActivities[i][j].Nickname.Length;
                            bool goodSubstringLength = false;
                            while (!goodSubstringLength)
                            {
                                var tempMeasurements = graphics.MeasureString(PlayersActivities[i][j].Nickname.Substring(0, substringLength), arialFont);
                                if (tempMeasurements.Width > 180)
                                {
                                    substringLength--;
                                }
                                else
                                {
                                    goodSubstringLength = true;
                                }
                            }
                            string name = PlayersActivities[i][j].Nickname.Substring(0, substringLength);
                            if (PlayersActivities[i][j].Nickname.Length != substringLength)
                            {
                                name = name.Substring(0, name.Length - 4) + "...";
                            }
                            var measurements = graphics.MeasureString(name, arialFont);
                            graphics.DrawString(name, arialFont, Brushes.White,
                                xCoords[i] - measurements.Width / 2, yTotal);
                            yTotal += (int)measurements.Height;
                        }
                    }
                }
            }
            filePath = srcPath.Replace(".png", "_temp.png");
            originalImage.Save(filePath);
        }

        public async Task ShowActivity(DiscordChannel channel, bool delete = true)
        {
            if (filePath != null && filePath.Length > 0 && File.Exists(filePath))
            {
                using (FileStream fsTables = new FileStream(filePath, FileMode.Open))
                {
                    string topFilePath = TOP_IMAGE_FILE_PATH;
                    if (!File.Exists(topFilePath))
                    {
                        topFilePath = GIF_FILE_PATH;
                    }
                    using (FileStream fsGif = new FileStream(topFilePath, FileMode.Open))
                    {
                        using (FileStream fsLegend = new FileStream(IMAGE_LEGEND_FILE_PATH, FileMode.Open))
                        {
                            var dict = new Dictionary<string, Stream>();
                            dict.Add(GIF_FILE_PATH, fsGif);
                            dict.Add(filePath, fsTables);
                            dict.Add(IMAGE_LEGEND_FILE_PATH, fsLegend);
                            dmb = new DiscordMessageBuilder();
                            dmb.WithFiles(dict);
                            await channel.SendMessageAsync(dmb);
                        }
                    }
                }
                if (delete)
                {
                    File.Delete(filePath);
                    filePath = null;
                }
            }
        }
        public async Task ShowActivity_Old(DiscordChannel channel, bool delete = true)
        {
            if (filePath != null && filePath.Length > 0 && File.Exists(filePath))
            {
                using (FileStream fsTables = new FileStream(filePath, FileMode.Open))
                {
                    using (FileStream fsGif = new FileStream(GIF_FILE_PATH, FileMode.Open))
                    {
                        using (FileStream fsLegend = new FileStream(IMAGE_CHECK_LEGEND_FILE_PATH, FileMode.Open))
                        {
                            var dict = new Dictionary<string, Stream>();
                            dict.Add(GIF_FILE_PATH, fsGif);
                            dict.Add(filePath, fsTables);
                            dict.Add(IMAGE_LEGEND_FILE_PATH, fsLegend);
                            dmb = new DiscordMessageBuilder();
                            dmb.WithFiles(dict);
                            await channel.SendMessageAsync(dmb);
                        }
                    }
                }
                if (delete)
                {
                    File.Delete(filePath);
                    filePath = null;
                }
            }
        }
    }
}
