using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Kavenegar.Core.Exceptions;
using Kavenegar.Core.Models;
using Kavenegar.Core.Models.Enums;
using Kavenegar.Core.Utils;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kavenegar
{
    public class KavenegarApi
    {
        private const string BASE_URL = "https://api.kavenegar.com/v1";
        private const string API_PATH = "{0}/{1}.{2}";

        private static HttpClient _client;

        public string ApiKey { get; private set; }

        public KavenegarApi(string apikey)
        {
            ApiKey = apikey;

            _client = new HttpClient
            {
                BaseAddress = new Uri($"{BASE_URL}/{ApiKey}/")
            };
        }

        #region << Execute >>
        public async Task<T> Execute<T>(string scope, string method, Dictionary<string, object> _params = null)
        {
            var path = string.Format(API_PATH, scope, method, "json");

            var nvc = _params?.Select(x => new KeyValuePair<string, string>(x.Key, x.Value?.ToString()));

            var postdata = nvc == null ? null : new FormUrlEncodedContent(nvc);

            var response = await _client.PostAsync(path, postdata);

            var strResponseBody = await response.Content.ReadAsStringAsync();

            // System.Diagnostics.Debug.WriteLine(responseBody);

            try
            {
                var result = JsonConvert.DeserializeObject<ReturnGeneric<T>>(strResponseBody);

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new ApiException(result.Return.Message, result.Return.Status);

                return result.Entries;
            }
            catch (HttpException)
            {
                throw;
            }
        }
        #endregion

        #region Send
        public async Task<List<SendResult>> Send(string sender, List<string> receptor, string message)
            => await Send(sender, receptor, message, MessageType.MobileMemory, DateTime.MinValue);
        public async Task<SendResult> Send(string sender, string receptor, string message)
            => await Send(sender, receptor, message, MessageType.MobileMemory, DateTime.MinValue);
        public async Task<SendResult> Send(string sender, string receptor, string message, MessageType type, DateTime date)
            => (await Send(sender, new List<string> { receptor }, message, type, date))[0];
        public async Task<List<SendResult>> Send(string sender, List<string> receptor, string message, MessageType type, DateTime date)
            => await Send(sender, receptor, message, type, date, null);
        public async Task<SendResult> Send(string sender, string receptor, string message, MessageType type, DateTime date, string localid)
            => (await Send(sender, new List<string> { receptor }, message, type, date, new List<string> { localid }))[0];
        public async Task<SendResult> Send(string sender, string receptor, string message, string localid)
            => await Send(sender, receptor, message, MessageType.MobileMemory, DateTime.MinValue, localid);
        public async Task<List<SendResult>> Send(string sender, List<string> receptors, string message, string localid)
        {
            List<string> localids = new List<string>();
            for (var i = 0; i <= receptors.Count - 1; i++)
            {
                localids.Add(localid);
            }
            return await Send(sender, receptors, message, MessageType.MobileMemory, DateTime.MinValue, localids);
        }
        public async Task<List<SendResult>> Send(string sender, List<string> receptor, string message, MessageType type, DateTime date, List<string> localids)
        {
            var param = new Dictionary<string, object>
            {
                {"sender", System.Net.WebUtility.HtmlEncode(sender)},
                {"receptor", System.Net.WebUtility.HtmlEncode(StringHelper.Join(",", receptor.ToArray()))},
                {"message", System.Net.WebUtility.HtmlEncode(message)},
                {"type", (int) type},
                {"date", date == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(date)}
            };
            if (localids != null && localids.Count > 0)
            {
                param.Add("localid", StringHelper.Join(",", localids.ToArray()));
            }
            return await Execute<List<SendResult>>("sms", "send", param);
        }
        #endregion

        #region SendArray
        public async Task<List<SendResult>> SendArray(List<string> senders, List<string> receptors, List<string> messages)
        {
            var types = new List<MessageType>();
            for (var i = 0; i <= senders.Count - 1; i++)
            {
                types.Add(MessageType.MobileMemory);
            }
            return await SendArray(senders, receptors, messages, types, DateTime.MinValue, null);
        }
        public async Task<List<SendResult>> SendArray(string sender, List<string> receptors, List<string> messages, MessageType type, DateTime date)
        {
            var senders = new List<string>();
            for (var i = 0; i < receptors.Count; i++)
            {
                senders.Add(sender);
            }
            var types = new List<MessageType>();
            for (var i = 0; i <= senders.Count - 1; i++)
            {
                types.Add(MessageType.MobileMemory);
            }
            return await SendArray(senders, receptors, messages, types, date, null);
        }
        public async Task<List<SendResult>> SendArray(string sender, List<string> receptors, List<string> messages, MessageType type, DateTime date, string localmessageids)
        {
            var senders = new List<string>();
            for (var i = 0; i < receptors.Count; i++)
            {
                senders.Add(sender);
            }
            List<MessageType> types = new List<MessageType>();
            for (var i = 0; i <= senders.Count - 1; i++)
            {
                types.Add(MessageType.MobileMemory);
            }
            return await SendArray(senders, receptors, messages, types, date, new List<string>() { localmessageids });
        }
        public async Task<List<SendResult>> SendArray(string sender, List<string> receptors, List<string> messages, string localmessageid)
        {
            List<string> senders = new List<string>();
            for (var i = 0; i < receptors.Count; i++)
            {
                senders.Add(sender);
            }

            return await SendArray(senders, receptors, messages, localmessageid);
        }
        public async Task<List<SendResult>> SendArray(List<string> senders, List<string> receptors, List<string> messages, string localmessageid)
        {
            var types = new List<MessageType>();
            for (var i = 0; i <= receptors.Count - 1; i++)
            {
                types.Add(MessageType.MobileMemory);
            }
            var localmessageids = new List<string>();
            for (var i = 0; i <= receptors.Count - 1; i++)
            {
                localmessageids.Add(localmessageid);
            }
            return await SendArray(senders, receptors, messages, types, DateTime.MinValue, localmessageids);
        }
        public async Task<List<SendResult>> SendArray(List<string> senders, List<string> receptors, List<string> messages, List<MessageType> types, DateTime date, List<string> localmessageids)
        {
            var jsonSenders = JsonConvert.SerializeObject(senders);
            var jsonReceptors = JsonConvert.SerializeObject(receptors);
            var jsonMessages = JsonConvert.SerializeObject(messages);
            var jsonTypes = JsonConvert.SerializeObject(types);
            var param = new Dictionary<string, object>
            {
                {"message", jsonMessages},
                {"sender", jsonSenders},
                {"receptor", jsonReceptors},
                {"type", jsonTypes},
                {"date", date == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(date)}
            };
            if (localmessageids != null && localmessageids.Count > 0)
            {
                param.Add("localmessageids", StringHelper.Join(",", localmessageids.ToArray()));
            }

            var data = await Execute<List<SendResult>>("sms", "sendarray", param);
            if (data == null)
            {
                return new List<SendResult>();
            }
            return data;
        }
        #endregion

        #region Status
        public async Task<StatusResult> Status(string messageid)
        {
            var result = await Status(new List<string> { messageid });
            return result.Count == 1 ? result[0] : null;
        }
        public async Task<List<StatusResult>> Status(List<string> messageids)
        {
            var param = new Dictionary<string, object>
            {
                {"messageid", StringHelper.Join(",", messageids.ToArray())}
            };
            
            var data = await Execute<List<StatusResult>>("sms", "status", param);
            return data == null ? new List<StatusResult>() : data;
        }
        #endregion

        #region StatusLocalMessageId
        public async Task<StatusLocalMessageIdResult> StatusLocalMessageId(string messageid)
        {
            var result = await StatusLocalMessageId(new List<string>() { messageid });
            return result.Count == 1 ? result[0] : null;
        }
        public async Task<List<StatusLocalMessageIdResult>> StatusLocalMessageId(List<string> messageids)
        {
            var param = new Dictionary<string, object> { { "localid", StringHelper.Join(",", messageids.ToArray()) } };

            return await Execute<List<StatusLocalMessageIdResult>>("sms", "statuslocalmessageid", param);
        }
        #endregion

        #region Select
        public async Task<SendResult> Select(string messageid)
        {
            var result = await Select(new List<string> { messageid });
            return result.Count == 1 ? result[0] : null;
        }
        public async Task<List<SendResult>> Select(List<string> messageids)
        {
            var param = new Dictionary<string, object> { { "messageid", StringHelper.Join(",", messageids.ToArray()) } };

            var data = await Execute<List<SendResult>>("sms", "select", param);
            if (data == null)
            {
                return new List<SendResult>();
            }
            return data;
        }
        #endregion

        #region SelectOutbox
        public async Task<List<SendResult>> SelectOutbox(DateTime startdate)
            => await SelectOutbox(startdate, DateTime.MaxValue);
        public async Task<List<SendResult>> SelectOutbox(DateTime startdate, DateTime enddate)
            => await SelectOutbox(startdate, enddate, null);
        public async Task<List<SendResult>> SelectOutbox(DateTime startdate, DateTime enddate, string sender)
        {
            var param = new Dictionary<string, object>
             {
                 {"startdate", startdate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(startdate)},
                 {"enddate", enddate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(enddate)},
                 {"sender", sender}
             };
            return await Execute<List<SendResult>>("sms", "selectoutbox", param);
        }
        #endregion

        #region LatestOutbox
        public async Task<List<SendResult>> LatestOutbox(long pagesize)
            => await LatestOutbox(pagesize, "");
        public async Task<List<SendResult>> LatestOutbox(long pagesize, string sender)
        {
            var param = new Dictionary<string, object> { { "pagesize", pagesize }, { "sender", sender } };

            return await Execute<List<SendResult>>("sms", "latestoutbox", param);
        }
        #endregion

        #region CountOutbox
        public async Task<CountOutboxResult> CountOutbox(DateTime startdate)
            => await CountOutbox(startdate, DateTime.MaxValue, 10);
        public async Task<CountOutboxResult> CountOutbox(DateTime startdate, DateTime enddate)
            => await CountOutbox(startdate, enddate, 0);
        public async Task<CountOutboxResult> CountOutbox(DateTime startdate, DateTime enddate, int status)
        {
            var param = new Dictionary<string, object>
            {
                {"startdate", startdate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(startdate)},
                {"enddate", enddate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(enddate)},
                {"status", status}
            };

            var data = await Execute<List<CountOutboxResult>>("sms", "countoutbox", param);
            return data == null ? new CountOutboxResult() : (data[0] == null ? new CountOutboxResult() : data[0]);
        }
        #endregion

        #region Cancel
        public async Task<StatusResult> Cancel(string messageid)
        {
            var result = await Cancel(new List<string> { messageid });
            return result.Count == 1 ? result[0] : null;
        }
        public async Task<List<StatusResult>> Cancel(List<string> ids)
        {
            var param = new Dictionary<string, object>
            {
                {"messageid", StringHelper.Join(",", ids.ToArray())}
            };

            return await Execute<List<StatusResult>>("sms", "cancel", param);
        }
        #endregion

        #region Receive
        public async Task<List<ReceiveResult>> Receive(string line, int isread)
        {
            var param = new Dictionary<string, object> {
                { "linenumber", line },
                { "isread", isread }
            };

            var data = await Execute<List<ReceiveResult>>("sms", "receive", param);
            return data == null ? new List<ReceiveResult>() : data;
        }
        #endregion

        #region CountInbox
        public async Task<CountInboxResult> CountInbox(DateTime startdate, string linenumber)
            => await CountInbox(startdate, DateTime.MaxValue, linenumber, 0);
        public async Task<CountInboxResult> CountInbox(DateTime startdate, DateTime enddate, string linenumber)
            => await CountInbox(startdate, enddate, linenumber, 0);

        public async Task<CountInboxResult> CountInbox(DateTime startdate, DateTime enddate, string linenumber, int isread)
        {
            var param = new Dictionary<string, object>
            {
                {"startdate", startdate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(startdate)},
                {"enddate", enddate == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(enddate)},
                {"linenumber", linenumber},
                {"isread", isread}
            };
            return await Execute<CountInboxResult>("sms", "countoutbox", param);
        }
        #endregion

        #region CountPostalCode
        public async Task<List<CountPostalCodeResult>> CountPostalCode(long postalcode)
        {
            var param = new Dictionary<string, object> { { "postalcode", postalcode } };

            return await Execute<List<CountPostalCodeResult>>("sms", "countpostalcode", param);
        }
        #endregion

        #region SendByPostalCode
        public async Task<List<SendResult>> SendByPostalCode(long postalcode, string sender, string message, long mcistartIndex, long mcicount, long mtnstartindex, long mtncount)
            => await SendByPostalCode(postalcode, sender, message, mcistartIndex, mcicount, mtnstartindex, mtncount, DateTime.MinValue);

        public async Task<List<SendResult>> SendByPostalCode(long postalcode, string sender, string message, long mcistartIndex, long mcicount, long mtnstartindex, long mtncount, DateTime date)
        {
            var param = new Dictionary<string, object>
            {
                {"postalcode", postalcode},
                {"sender", sender},
                {"message", System.Net.WebUtility.HtmlEncode(message)},
                {"mcistartIndex", mcistartIndex},
                {"mcicount", mcicount},
                {"mtnstartindex", mtnstartindex},
                {"mtncount", mtncount},
                {"date", date == DateTime.MinValue ? 0 : DateHelper.DateTimeToUnixTimestamp(date)}
            };
            return await Execute<List<SendResult>>("sms", "sendbypostalcode", param); ;
        }
        #endregion

        #region AccountInfo
        public async Task<AccountInfoResult> AccountInfo()
            => await Execute<AccountInfoResult>("account", "info");
        #endregion

        #region UtilsGetDate
        public async Task<DateResult> UtilsGetDate()
            => await Execute<DateResult>("utils", "getdate");
        #endregion

        #region AccountConfig
        public async Task<AccountConfigResult> AccountConfig(string apilogs, string dailyreport, string debugmode, string defaultsender, int? mincreditalarm, string resendfailed)
        {
            var param = new Dictionary<string, object>
            {
                {"apilogs", apilogs},
                {"dailyreport", dailyreport},
                {"debugmode", debugmode},
                {"defaultsender", defaultsender},
                {"mincreditalarm", mincreditalarm},
                {"resendfailed", resendfailed}
            };
            return await Execute<AccountConfigResult>("account", "config", param);
        }
        #endregion

        #region VerifyLookup
        public async Task<SendResult> VerifyLookup(string receptor, string token, string template)
            => await VerifyLookup(receptor, token, null, null, template, VerifyLookupType.Sms);
        public async Task<SendResult> VerifyLookup(string receptor, string token, string template, VerifyLookupType type)
            => await VerifyLookup(receptor, token, null, null, template, type);
        public async Task<SendResult> VerifyLookup(string receptor, string token, string token2, string token3, string template)
            => await VerifyLookup(receptor, token, token2, token3, template, VerifyLookupType.Sms);
        public async Task<SendResult> VerifyLookup(string receptor, string token, string token2, string token3, string token10, string template)
            => await VerifyLookup(receptor, token, token2, token3, token10, template, VerifyLookupType.Sms);
        public async Task<SendResult> VerifyLookup(string receptor, string token, string token2, string token3, string template, VerifyLookupType type)
            => await VerifyLookup(receptor, token, token2, token3, null, template, type);
        public async Task<SendResult> VerifyLookup(string receptor, string token, string token2, string token3, string token10, string template, VerifyLookupType type)
            => await VerifyLookup(receptor, token, token2, token3, token10, null, template, type);

        public async Task<SendResult> VerifyLookup(string receptor, string token, string token2, string token3, string token10, string token20, string template, VerifyLookupType type)
        {
            var param = new Dictionary<string, object>
            {
                {"receptor", receptor},
                {"template", template},
                {"token", token},
                {"token2", token2},
                {"token3", token3},
                {"token10", token10},
                {"token20", token20},
                {"type", type},
            };
            return await Execute<SendResult>("verify", "lookup", param);
        }
        #endregion

        #region CallMakeTTS
        public async Task<SendResult> CallMakeTTS(string message, string receptor)
            => (await CallMakeTTS(message, new List<string> { receptor }, null, null))[0];
        public async Task<List<SendResult>> CallMakeTTS(string message, List<string> receptor)
            => await CallMakeTTS(message, receptor, null, null);
        public async Task<List<SendResult>> CallMakeTTS(string message, List<string> receptor, DateTime? date, List<string> localid)
        {
            var param = new Dictionary<string, object>
            {
                {"receptor", StringHelper.Join(",", receptor.ToArray())},
                {"message", System.Net.WebUtility.HtmlEncode(message)},
            };
            if (date != null)
                param.Add("date", DateHelper.DateTimeToUnixTimestamp(date.Value));
            if (localid != null && localid.Count > 0)
                param.Add("localid", StringHelper.Join(",", localid.ToArray()));

            return await Execute<List<SendResult>>("call", "maketts", param);
        }
        #endregion

        internal class ReturnGeneric<T>
        {
            public Result Return { get; set; }
            public T Entries { get; set; }
            internal class Result
            {
                public int Status { get; set; }
                public string Message { get; set; }
            }
        }
    }
}