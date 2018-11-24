using System;
namespace BlinkPayCard
{
    public class DemoConfig : IConfig
    {
        //=======【BlinkPayCard信息配置】=====================================
        /* TODO: 此为测试账号的开发配置，申请正式账号后请替换
        * APPID：绑定支付的APPID（必须配置）
        * MCHID：商户号（必须配置）
        * APPKEY：商户支付密钥
        */

        public string GetAppID()
        {
            return "appEqMuvZ088";
        }
        public string GetMchID()
        {
            return "blEAgkNtLm8";
        }
        public string GetAppKey()
        {
            return "keyW8LNi4GJzyMqEsnjS2ZMw6kkNtDUObxB";
        }

        //=======【日志级别】===================================
        /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
        */
        public int GetLogLevel()
        {
            return 1;
        }
    }
}
