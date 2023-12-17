using DiscordHelper;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;

namespace NLBE_Bot
{
    public class WeeklyEvent
    {
        public string Tank { get; private set; }
        public List<WeeklyEventItem> WeeklyEventItems { get; set; }
        public DateTime StartDate { get; set; }

        private static string DATETIME_FORMAT = "dd/MM/yy HH:mm";
        private static string DATETIME_FORMAT_SHORT = "dd/MM/yy";

        public WeeklyEvent(string tank, List<WeeklyEventItem> weeklyEventItems)
        {
            this.Tank = tank;
            this.WeeklyEventItems = weeklyEventItems;
            StartDate = StartOfWeek(DateTime.Now);
        }

        public WeeklyEvent(DiscordMessage message)
        {
            Tank = message.Embeds[0].Title;
            WeeklyEventItems = new List<WeeklyEventItem>();
            foreach (DiscordEmbedField embedField in message.Embeds[0].Fields)
            {
                WeeklyEventItems.Add(new WeeklyEventItem(embedField));
            }
            StartDate = StartOfWeek(message.CreationTimestamp.DateTime);
        }

        public DiscordEmbed GenerateEmbed(string winner = "")
        {
            try
            {
                DiscordEmbedBuilder newDiscEmbedBuilder = new DiscordEmbedBuilder();
                newDiscEmbedBuilder.Color = Bot.WEEKLY_EVENT_COLOR;
                string preDescription = "```ini\nWinnaar: [ " + winner + " ] ```";
                //string preDescription = "```fix\nWinnaar = " + winner + " ```";
                string description = String.Format("**Start op {0}\nVereiste tank: {1}**\n```css\nVerslaag deze score voor {2}```",
                    StartDate.ToString(DATETIME_FORMAT_SHORT), this.Tank, GetEndDate().ToString(DATETIME_FORMAT).TrimStart('0'));
                newDiscEmbedBuilder.Description = (winner.Length == 0 ? string.Empty : preDescription + "\n") + description;
                newDiscEmbedBuilder.Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail();
                newDiscEmbedBuilder.Thumbnail.Url = Bot.DEF_LOGO_IMGUR;

                foreach (WeeklyEventItem weeklyEventItem in WeeklyEventItems)
                {
                    DEF def = weeklyEventItem.GenerateDEF();
                    newDiscEmbedBuilder.AddField(def.Name, def.Value.adaptToDiscordChat(), def.Inline);
                }

                newDiscEmbedBuilder.Title = this.Tank;

                return newDiscEmbedBuilder.Build();
            }
            catch (Exception e)
            {
                Bot.handleError("While editing HOF message: ", e.Message, e.StackTrace).Wait();
            }
            return null;
        }

        public DateTime GetEndDate()
        {
            return this.StartDate.AddDays(7);
        }

        public static DateTime StartOfWeek(DateTime dt)
        {
            dt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
            dt = dt.AddDays(-1 * diff).Date;
            dt = dt.AddHours(14);
            return dt;
        }
    }
}
