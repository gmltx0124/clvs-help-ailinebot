using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Web;

namespace isRock.Template
{
    public class LineLUISWebHookController : isRock.LineBot.LineWebHookControllerBase
    {
        const string key = "cb0a35116a0a4c2e85becec24f47c2a4";
        const string endpoint = "john73825618.cognitiveservices.azure.com";
        const string appId = "5a81e035-7820-45a9-b503-15653ecb6337";

        [Route("api/LineLUIS")]
        [HttpPost]
        public IActionResult POST()
        {
            var AdminUserId = "Ub7330f5cf227d3901e6eff9e8d87c61b";

            try
            {
                //設定ChannelAccessToken
                this.ChannelAccessToken = "HkL737NM9Xq/4j01v7q7DAJttxSwLPExdKwmCsDeJrL5cE8f9PPwYD4cMAI6Z9LO4mQP6RiYn7sKe3bZ+TZbyUYCYdjsX4OB/FlOFLd6i5rAftaqHVVYVsCY6aWbkRTZctgJ8w+9Jfj3g4/WoD4z/wdB04t89/1O/w1cDnyilFU=";
                //取得Line Event
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                //配合Line verify 
                if (LineEvent.replyToken == "00000000000000000000000000000000") return Ok();
                var responseMsg = "";
                //準備回覆訊息
                if (LineEvent.type.ToLower() == "message" && LineEvent.message.type == "text")
                {
                    var ret = MakeRequest(LineEvent.message.text);
                    
                    if (ret.topScoringIntent.intent == "上放學時間")
                        responseMsg = $"上學7點50分前 \n放學4點但若有第八節則為4點50分 ";
                    if (ret.topScoringIntent.intent == "愛校時間")
                        responseMsg = $"星期一、三、五拿報名表 10點下課去教官室 \n當天中午即可做愛校 ";
                    if (ret.topScoringIntent.intent == "校規有什麼")
                        responseMsg = $"請參考高一進校時發的學生手冊 如果沒有找學弟妹借";
                    if (ret.topScoringIntent.intent == "午餐是統一訂餐還是訂外賣")
                        responseMsg = $"基本上都是合菜，不可叫外賣，但可以叫家長送";
                    if (ret.topScoringIntent.intent == "None")
                        responseMsg = $"抱歉目前沒有新增此問題";
                    if (ret.topScoringIntent.intent == "交糾在幹嘛")
                        responseMsg = $"負責配合教官管理上下學動線以及辛苦的保護壢家學生的安全 ";
                    if (ret.topScoringIntent.intent == "什麼是自治")
                        responseMsg = $"負責配合教官管理學校大小事，像是：服儀檢查、升旗典禮、管理校園秩序 ";
                    if (ret.topScoringIntent.intent == "什麼時候知道自己班導")
                        responseMsg = $"開學就會知道了喔不用緊張啦 ";
                    if (ret.topScoringIntent.intent == "作業會不會很多")
                        responseMsg = $"每科、每位老師都不太一樣，但如果有按照老師安排的話就一定寫的完的 ";
                    if (ret.topScoringIntent.intent == "升學管道有哪些")
                        responseMsg =  $"https://web.tchcvs.tc.edu.tw/resource/openfid.php?id=14021" ;
                    if (ret.topScoringIntent.intent == "可不可以化妝")
                        responseMsg = $"不會被記過但要去找教官複檢喔 ";
                    if (ret.topScoringIntent.intent == "可不可以戴耳環")
                        responseMsg = $"總長1公分以下，不是可以帶垂吊飾 ";
                    if (ret.topScoringIntent.intent == "可以訂外食嗎")
                        responseMsg = $"抱歉不行喔，如果被學校抓到會被記過喔 ";
                    if (ret.topScoringIntent.intent == "商經科")
                        responseMsg = $"這個問題可以問問看學長姐喔 ";
                    if (ret.topScoringIntent.intent == "始業考難不難")
                        responseMsg = $"因人而異喔但如果暑假有複習以及認真寫功課一定不難的喔";
                    if (ret.topScoringIntent.intent == "學校制服長怎樣")
                        responseMsg = $"https://image.peoplenews.tw/news/9d3d5320-51a3-4363-9fe0-f199077c19e6.jpg ";
                    
                    
                    

                }
                else if (LineEvent.type.ToLower() == "message")
                    responseMsg = $"收到 event : {LineEvent.type} type: {LineEvent.message.type} ";
                else
                    responseMsg = $"收到 event : {LineEvent.type} ";
                //回覆訊息
                this.ReplyMessage(LineEvent.replyToken, responseMsg);
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //回覆訊息
                this.PushMessage(AdminUserId, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }

        static LUISResult MakeRequest(string utterance)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            var endpointUri = String.Format(
                "https://{0}/luis/v2.0/apps/{1}?verbose=true&timezoneOffset=0&subscription-key={3}&q={2}",
                endpoint, appId, utterance, key);

            var response = client.GetAsync(endpointUri).Result;

            var strResponseContent = response.Content.ReadAsStringAsync().Result;
            var Result = Newtonsoft.Json.JsonConvert.DeserializeObject<LUISResult>(strResponseContent);
            // Display the JSON result from LUIS
            return Result;
        }
    }

    #region "LUIS Model"

    public class TopScoringIntent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }

    public class Resolution
    {
        public string value { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public double score { get; set; }
        public Resolution resolution { get; set; }
    }

    public class LUISResult
    {
        public string query { get; set; }
        public TopScoringIntent topScoringIntent { get; set; }
        public List<Intent> intents { get; set; }
        public List<Entity> entities { get; set; }
    }
    #endregion
}