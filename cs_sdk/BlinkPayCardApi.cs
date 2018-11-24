using System;
using System.Collections.Generic;
using LitJson;
using System.Linq;


namespace BlinkPayCard
{
    class BlinkPayCardApplication
    {
        public static void Main(string[] args)
        {
            String notify = "{\"nonce_str\":\"0F77ezcval5lEYb6FKPtCMqwMMqzHHLm\"," +
                                "\"amount\":\"100\"," +
                                "\"event_type\":\"order_payed\"," +
                                "\"sign\":\"64963552561AD42D1D842271B7123D02\"," +
                                "\"currency\":\"CNY\"," +
                                "\"pre_order\":\"dae80fd0a98e4ba6bc4a77a3bac64d2c\"," +
                                "\"sign_type\":\"md5\"," +
                                "\"osu_number\":\"efaa8247f91748468e8ae541e563cb39\"}";

            String card_number = "1510000003360001";
            String password = "091086";


            Console.WriteLine("=========== 充值下单 ===========");
            String pre_order = PreOrder(new BlinkPayCardData());
                
            Console.WriteLine("=========== 订单查询 ===========");
            OrderQuery(new BlinkPayCardData());

            Console.WriteLine("=========== 网页充值 ===========");
            String webPayUrl = WebPay(pre_order);
            Console.WriteLine("请用浏览器访问：" + webPayUrl);

            Console.WriteLine("=========== 接口充值 ===========");
            InnerPay(new BlinkPayCardData(), card_number, password, pre_order);

            Console.WriteLine("=========== 模拟接收通知消息 ===========");
            ReceiveNotify(notify);
        }

        /**
        *    
        * 充值下单
        * 该接口用于平台方从BlinkPayCard系统获取充值预下单号。
        * 平台方通过Post请求调用该接口，从返回结果中解析过预下单号（pre_order）。
        * @param BlinkPayCardData inputObj 提交给查询订单API的参数
        * @throws BlinkPayCardException
        * @return 预下单号
        */
        public static string PreOrder(BlinkPayCardData inputObj)
        {
            string url = "https://pay.api.blinkpaycard.com/pay/preOrder";

            inputObj.SetValue("app_id", BlinkPayCardConfig.GetConfig().GetAppID());
            inputObj.SetValue("mch_id", BlinkPayCardConfig.GetConfig().GetMchID());
            inputObj.SetValue("osu_number", GenerateNonceStr());
            inputObj.SetValue("sign_type", BlinkPayCardData.SIGN_TYPE_MD5);
            inputObj.SetValue("sign", inputObj.MakeSign(BlinkPayCardData.SIGN_TYPE_MD5));//签名


            string json = inputObj.ToJson();

            Console.WriteLine("PreOrder request : " + json);
            string response = HttpService.Post(json, url);//调用HTTP通信接口提交数据
            Console.WriteLine("PreOrder response : " + response);

            JsonData resultObj = JsonMapper.ToObject(response);
            string pre_order = (string)resultObj["result"]["pre_order"];
            //JsonData preOrderObj = JsonMapper.ToObject(response);
            //string pre_order = (string)preOrderObj["pre_order"];

            return pre_order;
        }


        /**
         * 网页充值
         *合作平台⽅方系统根据充值下单接⼝口获取的预下单号(pre_order)与BlinkPay Card系统分配的商户 ID(mch_id)获取BlinkPay Card⽹网⻚页充值⻚页⾯面
         * @param mchId
         * @param preOrder 预下单号
         * @return
         * @throws Exception
         */
        private static String WebPay(String preOrder)
        {
            string mch_id = BlinkPayCardConfig.GetConfig().GetMchID();
            return "http://pay.blinkpaycard.com/?mch_id=" + mch_id + "&pre_order=" + preOrder;

        }


        /**
         * 接口充值
         * 该接口用于平台方站内完成充值操作。
         * 平台方通过Post请求调用该接口，BlinkPayCard系统将返回充值结果。
         * @param appId
         * @param mchId
         * @param appKey
         * @param card_number 卡号
         * @param password 卡密码
         * @param preOrder 预下单号
         * @return
         * @throws Exception
         */
        public static void InnerPay(BlinkPayCardData inputObj, String card_number, String password, String preOrder)
        {
            string url = "https://pay.api.blinkpaycard.com/pay/stationPay";

            inputObj.SetValue("app_id", BlinkPayCardConfig.GetConfig().GetAppID());
            inputObj.SetValue("mch_id", BlinkPayCardConfig.GetConfig().GetMchID());
            inputObj.SetValue("card_number", card_number);
            inputObj.SetValue("password", password);
            inputObj.SetValue("pre_order", preOrder);
            inputObj.SetValue("sign_type", BlinkPayCardData.SIGN_TYPE_MD5);
            inputObj.SetValue("sign", inputObj.MakeSign(BlinkPayCardData.SIGN_TYPE_MD5));//签名

            string json = inputObj.ToJson();
            string response = HttpService.Post(json, url);
            Console.WriteLine("PreOrder response : " + response);

         }

        /**
        *    
        * 查询订单
        * @param BlinkPayCardData inputObj 提交给查询订单API的参数
        * @throws BlinkPayCardException
        * @return 成功时返回订单查询结果，其他抛异常
        */
        public static BlinkPayCardData OrderQuery(BlinkPayCardData inputObj)
        {
            string url = "https://pay.api.blinkpaycard.com/pay/queryResult";

            inputObj.SetValue("app_id", BlinkPayCardConfig.GetConfig().GetAppID());
            inputObj.SetValue("mch_id", BlinkPayCardConfig.GetConfig().GetMchID());
            inputObj.SetValue("pre_order", "dae80fd0a98e4ba6bc4a77a3bac64d2c");
            inputObj.SetValue("sign_type", BlinkPayCardData.SIGN_TYPE_MD5);
            inputObj.SetValue("sign", inputObj.MakeSign(BlinkPayCardData.SIGN_TYPE_MD5));//签名


            string json = inputObj.ToJson();

            Console.WriteLine("OrderQuery request : " + json);
            string response = HttpService.Post(json, url);//调用HTTP通信接口提交数据
            Console.WriteLine("OrderQuery response : " + response);

            BlinkPayCardData result = new BlinkPayCardData();

            return result;
        }

        /**
         * 接收通知消息
         * @return
         */
        public static String ReceiveNotify(String notify)
        {

            Console.WriteLine("receive notify : " + notify);

            JsonData jsonData = JsonMapper.ToObject(notify);
            String nonce_str = (string)jsonData["nonce_str"];
            Console.WriteLine("nonce_str : " + nonce_str);

            return nonce_str;
        }

        /**
        * 生成时间戳，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数
         * @return 时间戳
        */
        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /**
        * 生成随机串，随机串包含字母或数字
        * @return 随机串
        */
        public static string GenerateNonceStr()
        {
            RandomGenerator randomGenerator = new RandomGenerator();
            return randomGenerator.GetRandomUInt().ToString();
        }
    }
}
