using System.Drawing;
using System.Linq;

namespace NLBE_Bot
{
    public static class Constants
    {
        static Constants()
        {
            DEFAULT_FONT_FAMILY = FontFamily.Families.Where(f => f.Name == "Arial").FirstOrDefault();
        }

        //for server 1-DEF
        public const string DEF_CLAN_NAME = "1-DEF";
        //clan id
        public const int DEF_CLAN_ID = 100634;

        //server id
        public const ulong DEF_SERVER_ID = 642780115138379828;

        //textchannels
        public const ulong REPLAYS_CHANNEL = 678004225095237642;
        public const ulong MASTERY_REPLAYS_CHANNEL = 940002599766220900;
        public const ulong HOF_CHANNEL = 940003302781882368;
        public const ulong WEKELIJKSE_EVENT_CHANNEL = 940004127168143501;
        public const ulong TEST2_CHANNEL = 939871663573569556;
        public const ulong TEST_REPLAYS_CHANNEL = 956100151183835136;
        public const ulong RATING_CHANNEL = 940005503164108831;
        public const ulong ACTIVITY_CHANNEL = 956929960419950592;

        //player id's
        public const ulong IAN = 632986606575288360; //1-DEF leider
        public const ulong BOT_ID = 939869373819150376;

        //role id's
        public const ulong ROLE_LEIDER = 642831366781665292;
        public const ulong ROLE_DEPUTY = 642844906980638740;
        public const ulong ROLE_KAPITEIN = 757180951347331163;
        public const ulong ROLE_FIELD_COMMANDER = 651477229850591250;
        public const ulong ROLE_RESERVE_FIELD_COMMANDER = 947131118812016671;
        public const ulong ROLE_TRAINER = 749329661565337610;
        public const ulong ROLE_RECRUITER = 749631233155268670;
        public const ulong ROLE_A_TOURNAMENT = 781960049102684170;
        public const ulong ROLE_B_TOURNAMENT = 651478615799889920;
        public const ulong ROLE_CREW = 651474113054900274;
        public const ulong ROLE_GUEST = 662984835836542997;


        //emojies
        public const string EMOJI_GOLD_MEDAL = ":first_place:";
        public const string EMOJI_SILVER_MEDAL = ":second_place:";
        public const string EMOJI_BRONZE_MEDAL = ":third_place:";
        public const string EMOJI_CLAN_LOGO = ":clanlogo1DEF:";
        
        //json link
        public const string JSON_RELATIONS_URL = "https://Simple-API.thibeastmo.repl.co";

        //fonts
        public static FontFamily DEFAULT_FONT_FAMILY;

        //images
        public const string PATH_DEBUGGING = "../../../";
        public const string DIR_IMAGES = "images/";
        public const string IMAGE_STATS = "stats_badge.png";
        public const string IMAGE_FAST_STATS = "super_unicum.png";
        public const string IMAGE_MORE_INFO = "more_info.png";
        public const string IMAGE_MASTERY_REPLAY = "mastery_replay_info.png";
        public const string IMAGE_REGULAR_REPLAY = "regular_replay_info.png";
        public const string IMAGE_ACE = "Ace_tanker.png";
        public const string IMAGE_FIRST_CLASS = "I_class.png";
        public const string IMAGE_SECOND_CLASS = "II_class.png";
        public const string IMAGE_THIRD_CLASS = "III_class.png";
        public const string IMAGE_DIAMOND_EMBLEM = "diamondemblem.png";
        public const string IMAGE_PLATINUM_EMBLEM = "platinumemblem.png";
        public const string IMAGE_GOLD_EMBLEM = "goldemblem.png";
        public const string IMAGE_SILVER_EMBLEM = "silveremblem.png";
        public const string IMAGE_BRONZE_EMBLEM = "bronzeemblem.png";
        public const string IMAGE_CALIB_EMBLEM = "calibrationemblem.png";
        public const string IMAGE_GOLD_BORDER = "goldborder.png";
        public const string IMAGE_GOLDSILVER_BORDER = "goldsilverborder.png";
        public const string IMAGE_SILVER_BORDER = "silverborder.png";
        public const string IMAGE_BRONZE_BORDER = "bronzeborder.png";
        public const string GIF_CLAN_NAME = "activity_check_1.gif";
        public const string IMAGE_CLAN_NAME = "activity_check_1.png";
        public const string IMAGE_ACTIVITY_CHECK_TABLES = "activity_check_tables.png";
        public const string IMAGE_ACTIVITY_CHECK_LEGEND = "activity_check_legend.png";
        public const string IMAGE_ACTIVITY_TABLE = "activity_table.png";
        public const string IMAGE_ACTIVITY_LEGEND = "activity_legend.png";
        public const string IMAGE_DOT_GREEN = "dot_green.png";
        public const string IMAGE_DOT_YELLOW = "dot_yellow.png";
        public const string IMAGE_DOT_ORANGE = "dot_orange.png";
        public const string IMAGE_DOT_RED = "dot_red.png";

        //coords
        //stats
        public const int COORD_STATS_COLUMN_1 = 380;
        public const int COORD_STATS_COLUMN_2 = 890;
        public const int COORD_STATS_COLUMN_3 = 1410;
        public const int COORD_STATS_ROW_1 = 340;
        public const int COORD_STATS_ROW_2 = 525;
        public const int COORD_STATS_ROW_3 = 712;
        public const int COORD_STATS_ROW_4 = 900;
        public const int COORD_STATS_STARTED_X = 600;
        public const int COORD_STATS_STARTED_Y = 160;
        public const int COORD_STATS_RATED_Y = 1055;
        public const int COORD_STATS_RATED_LEFT_X = 475;
        public const int COORD_STATS_RATED_RIGHT_X = 1310;
        //MOREINFO
        public const int COORD_TIER_COLUMN_DISTANCE = 165;
        public const int COORD_TIER_ROW_DISTANCE = 100;
        public const int COORD_TIER_FIRST_ROW = 700;
        public const int COORD_TIER_FIRST_COLUMN = 130;
        public const int COORD_MORE_NO_INFO_TANKS_X = 1570;
        public const int COORD_MORE_NO_INFO_TANKS_Y = 1160;
        public const int COORD_MORE_TOP_INFO_X = 930;
        public const int COORD_MORE_TOP_INFO_1_Y = 260;
        public const int COORD_MORE_TOP_INFO_2_Y = 340;
        //replay
        public const int COORD_FIRST_COLUMN_X = 670;
        public const int COORD_SECOND_COLUMN_X = 975;
        //badge
        public const int COORD_BADGE_BORDER_WINRATE_Y = 318;
        public const int COORD_BADGE_BORDER_WINRATE_Y_DIFF = 87;
        public const int COORD_BADGE_BORDER_WINRATE_Y_DIFF_TOURNAMENT = 55;

        public const string URL_VALUE_SPLITTER = "%2C";
        public const string URL_PARAM_SPLITTER = "&";
        public const string URL_KEY_VALUE_SPLITTER = "=";
    }
}
