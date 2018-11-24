using System;
using System.Collections.Generic;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using LitJson;
using System.Linq;

namespace BlinkPayCard
{
    public class BlinkPayCardData
    {
        public const string SIGN_TYPE_MD5 = "md5";
        public const string SIGN_TYPE_HMAC_SHA256 = "HMAC-SHA256";
        public BlinkPayCardData()
        {

        }

        //采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

        /**
        * 设置某个字段的值
        * @param key 字段名
         * @param value 字段值
        */
        public void SetValue(string key, object value)
        {
            m_values[key] = value;
        }

        /**
        * 根据字段名获取某个字段的值
        * @param key 字段名
         * @return key对应的字段值
        */
        public object GetValue(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return o;
        }

        /**
         * 判断某个字段是否已设置
         * @param key 字段名
         * @return 若字段key已被设置，则返回true，否则返回false
         */
        public bool IsSet(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            if (null != o)
                return true;
            else
                return false;
        }


        /**
        * @Dictionary格式转化成url参数格式
        * @ return url格式串, 该串不包含sign字段值
        */
        public string ToUrl()
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    Log.Error(this.GetType().ToString(), "BlinkPayCardData内部含有值为null的字段!");
                    throw new BlinkPayCardException("BlinkPayCardData内部含有值为null的字段!");
                }

                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }


        /**
        * @Dictionary格式化成Json
         * @return json串数据
        */
        public string ToJson()
        {
            string jsonStr = JsonMapper.ToJson(m_values);
            return jsonStr;

        }



        /**
        * @生成签名，详见签名生成算法
        * @return 签名, sign字段不参加签名
        */
        public string MakeSign(string signType)
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += "&key=" + BlinkPayCardConfig.GetConfig().GetAppKey();
            if (signType == SIGN_TYPE_MD5)
            {
                var md5 = MD5.Create();
                var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                var sb = new StringBuilder();
                foreach (byte b in bs)
                {
                    sb.Append(b.ToString("x2"));
                }
                //所有字符转为大写
                return sb.ToString().ToUpper();
            }
            else if (signType == SIGN_TYPE_HMAC_SHA256)
            {
                return CalcHMACSHA256Hash(str, BlinkPayCardConfig.GetConfig().GetAppKey());
            }
            else
            {
                throw new BlinkPayCardException("sign_type 不合法");
            }
        }

        /**
        * @生成签名，详见签名生成算法
        * @return 签名, sign字段不参加签名 SHA256
        */
        public string MakeSign()
        {
            return MakeSign(SIGN_TYPE_HMAC_SHA256);
        }



        /**
        * 
        * 检测签名是否正确
        * 正确返回true，错误抛异常
        */
        public bool CheckSign(string signType)
        {
            //如果没有设置签名，则跳过检测
            if (!IsSet("sign"))
            {
                Log.Error(this.GetType().ToString(), "BlinkPayCardData签名存在但不合法!");
                throw new BlinkPayCardException("BlinkPayCardData签名存在但不合法!");
            }
            //如果设置了签名但是签名为空，则抛异常
            else if (GetValue("sign") == null || GetValue("sign").ToString() == "")
            {
                Log.Error(this.GetType().ToString(), "BlinkPayCardData签名存在但不合法!");
                throw new BlinkPayCardException("BlinkPayCardData签名存在但不合法!");
            }

            //获取接收到的签名
            string return_sign = GetValue("sign").ToString();

            //在本地计算新的签名
            string cal_sign = MakeSign(signType);

            if (cal_sign == return_sign)
            {
                return true;
            }

            Log.Error(this.GetType().ToString(), "BlinkPayCardData签名验证错误!");
            throw new BlinkPayCardException("BlinkPayCardData签名验证错误!");
        }



        /**
        * 
        * 检测签名是否正确
        * 正确返回true，错误抛异常
        */
        public bool CheckSign()
        {
            return CheckSign(SIGN_TYPE_HMAC_SHA256);
        }

        /**
        * @获取Dictionary
        */
        public SortedDictionary<string, object> GetValues()
        {
            return m_values;
        }


        private string CalcHMACSHA256Hash(string plaintext, string salt)
        {
            string result = "";
            var enc = Encoding.Default;
            byte[]
            baText2BeHashed = enc.GetBytes(plaintext),
            baSalt = enc.GetBytes(salt);
            System.Security.Cryptography.HMACSHA256 hasher = new HMACSHA256(baSalt);
            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return result;
        }

    }
}