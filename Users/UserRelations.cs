using JsonObjectConverter;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace NLBE_Bot.Users
{
    public class UserRelations
    {
        public ulong DiscordID { get; set; }
        public long WargamingID { get; set; }


        public static Tuple<UserRelations, int> GetRelations(ulong discordID)
        {
            Json json = GetExternalJson();
            if (json != null && json.jsonArray != null && json.jsonArray.Count > 0)
            {
                //use filled in value
                if (json.jsonArray != null && json.jsonArray.Count > 0)
                {
                    for (int i = 0; i < json.jsonArray.Count; i++)
                    {
                        if (json.jsonArray[i].Item2.tupleList != null && json.jsonArray[i].Item2.tupleList.Count > 0)
                        {
                            foreach (var tuple in json.jsonArray[i].Item2.tupleList)
                            {
                                if (tuple.Item1.ToLower().Contains("discordid"))
                                {
                                    if (tuple.Item2.Item1 == discordID.ToString())
                                    {
                                        Json userJson = json.jsonArray[i].Item2;
                                        var userRelations = new UserRelations();

                                        foreach (var property in userRelations.GetType().GetProperties())
                                        {
                                            var value = property.GetValue(userRelations);
                                            ulong variable = GetNumberFromObject(value);
                                            if (value == null || variable == 0)
                                            {//fill in value from tuplelist
                                                foreach (var tupleItem in userJson.tupleList)
                                                {
                                                    if (tupleItem.Item1 == property.Name.ToString())
                                                    {// its the correct property
                                                        object tempValue = Convert.ChangeType(tupleItem.Item2.Item1, property.PropertyType);
                                                        property.SetValue(userRelations, tempValue);
                                                    }
                                                }
                                            }
                                        }
                                        return new Tuple<UserRelations, int>(userRelations, i);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        public static UserRelations GetRelations(UserRelations userRelations)
        {
            bool atLeastOneNull = false;
            bool atLeastOneNotNull = false;
            foreach (var property in userRelations.GetType().GetProperties())
            {
                var value = property.GetValue(userRelations);
                ulong variable = GetNumberFromObject(value);
                if (value == null || variable > 0)
                {
                    atLeastOneNull = true;
                }
                else
                {
                    atLeastOneNotNull = true;
                }
            }
            if (atLeastOneNotNull && atLeastOneNull)
            {
                Json json = GetExternalJson();
                if (json.jsonArray != null && json.jsonArray.Count > 0)
                {
                    //use filled in value
                    Json userJson = null;
                    foreach (var property in userRelations.GetType().GetProperties())
                    {
                        var value = property.GetValue(userRelations);
                        ulong variable = GetNumberFromObject(value);
                        if (value != null && variable != 0)
                        {
                            foreach (var user in json.jsonArray)
                            {
                                foreach (var item in user.Item2.tupleList)
                                {
                                    if (value != null && item.Item2.Item1.ToString() == value.ToString())
                                    {//this is the user u were looking for
                                        userJson = user.Item2;
                                        break;
                                    }
                                }
                                if (userJson != null)
                                {
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    if (userJson != null)
                    {
                        foreach (var property in userRelations.GetType().GetProperties())
                        {
                            var value = property.GetValue(userRelations);
                            ulong variable = GetNumberFromObject(value);
                            if (value == null || variable == 0)
                            {//fill in value from tuplelist
                                foreach (var item in userJson.tupleList)
                                {
                                    if (item.Item1 == property.Name.ToString())
                                    {// its the correct property
                                        object tempValue = Convert.ChangeType(item.Item2.Item1, property.PropertyType);
                                        property.SetValue(userRelations, tempValue);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return userRelations;
        }
        private static ulong GetNumberFromObject(object value)
        {
            ulong variable = 0;
            if (value is ulong)
            {
                variable = (ulong)value;
            }
            else if (value is long)
            {
                variable = (ulong)(long)value;
            }
            else if (value is int)
            {
                variable = (ulong)(long)(int)value;
            }
            else if (value is string)
            {
                variable = ulong.Parse(value.ToString());
            }
            return variable;
        }
        public static Json GetExternalJson()
        {
            return HttpClientRequester.SendRequest(Constants.JSON_RELATIONS_URL, HttpMethod.Get);
        }
        public static Json PostExternalJson(string content)
        {
            return HttpClientRequester.SendRequest(Constants.JSON_RELATIONS_URL, HttpMethod.Post, content: content);
        }
        public string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var property in this.GetType().GetProperties())
            {
                sb.AppendLine(property.Name + ": " + property.GetValue(this));
            }
            return sb.ToString();
        }
    }
}
