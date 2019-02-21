package com.blinkpaycard.sdk;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.JSONObject;

import java.util.HashMap;
import java.util.Map;

/**
 * 该示例包含BlinkPayCard的充值下单，网页充值，订单查询，接收通知消息
 */
public class BlinkPayCardApi {

    public static void main(String[] args) throws Exception{

        BlinkPayCardConfig config = new BlinkPayCardConfig();
        String appId = config.getAppId();
        String mchId = config.getMchId();
        String appKey = config.getAppKey();


        String notify = "{\"nonce_str\":\"0F77ezcval5lEYb6FKPtCMqwMMqzHHLm\"," +
                "\"amount\":\"100\"," +
                "\"event_type\":\"order_payed\"," +
                "\"sign\":\"64963552561AD42D1D842271B7123D02\"," +
                "\"currency\":\"CNY\"," +
                "\"pre_order\":\"dae80fd0a98e4ba6bc4a77a3bac64d2c\"," +
                "\"sign_type\":\"md5\"," +
                "\"osu_number\":\"efaa8247f91748468e8ae541e563cb39\"}";


        System.out.println("=========== 充值下单 ===========");
        String pre_order = preOrder(appId, mchId, appKey);

        System.out.println("=========== 网页充值 ===========");
        String webPayUrl = webPay(mchId, pre_order);
        System.out.println("请用浏览器访问：" + webPayUrl);

        System.out.println("=========== 订单查询 ===========");
        orderQuery(appId, mchId, appKey, pre_order);

        System.out.println("=========== 模拟接收通知消息 ===========");
        receiveNotify(notify);

    }

    /**
     * 充值下单
     * 该接口用于平台方从BlinkPayCard系统获取充值预下单号。
     * 平台方通过Post请求调用该接口，从返回结果中解析过预下单号（pre_order）。
     * @param appId
     * @param mchId
     * @param appKey
     * @return
     * @throws Exception
     */
    private static String preOrder(String appId, String mchId, String appKey) throws Exception {

        String osuNumber = BlinkPayCardUtil.generateNonceStr();
        final String signType = "md5";

        Map<String,String> params = new HashMap<>();
        params.put("app_id", appId);
        params.put("mch_id", mchId);
        params.put("osu_number",osuNumber);
        params.put("amount", "100");
        params.put("currency", "CNY");
        params.put("sign_type", signType);

        String sign = BlinkPayCardUtil.generateSignature(params,appKey,"md5");
        params.put("sign",sign);

        String resp = BlinkPayCardRequest.requestOnce("https://pay.api.blinkpaycard.com","/pay/preOrder", JSON.toJSONString(params),6*1000 ,8*1000);
        System.out.println(String.format("resp : %s", resp));

        JSONObject jsonObject = (JSONObject)JSON.parse(resp);

        JSONObject result = (JSONObject)jsonObject.get("result");

        String preOrder = result.getString("pre_order");

        System.out.println(String.format("pre_order : %s", preOrder));

        return preOrder;

    }

    /**
     * 网页充值
     *合作平台⽅方系统根据充值下单接⼝口获取的预下单号(pre_order)与BlinkPay Card系统分配的商户 ID(mch_id)获取BlinkPay Card⽹网⻚页充值⻚页⾯面
     * @param mchId
     * @param preOrder 预下单号
     * @return
     * @throws Exception
     */
    private static String webPay(String mchId, String preOrder) {

        return String.format("http://pay.blinkpaycard.com/?mch_id=%s&pre_order=%s",mchId , preOrder);
    }


    /**
     * 订单查询
     * 该接口提供所有BlinkPayCard充值订单的查询，平台方可以通过查询订单接口主动查询订单状态，完成下一步的业务逻辑。
     * @param appId
     * @param mchId
     * @param appKey
     * @return
     * @throws Exception
     */
    private static void orderQuery(String appId, String mchId, String appKey, String preOrder) throws Exception {

        final String signType = "md5";

        Map<String,String> params = new HashMap<>();
        params.put("app_id", appId);
        params.put("mch_id", mchId);
        params.put("pre_order", preOrder);
        params.put("sign_type", signType);

        String sign = BlinkPayCardUtil.generateSignature(params,appKey,"md5");
        params.put("sign",sign);

        String resp = BlinkPayCardRequest.requestOnce("https://pay.api.blinkpaycard.com","/pay/queryResult", JSON.toJSONString(params),6*1000 ,8*1000);
        System.out.println(String.format("resp : %s", resp));


    }

    /**
     * 接收通知消息
     * @return
     */
    public static String receiveNotify(String notify)  {
        //TODO 接收到通知后，平台方异步处理订单业务逻辑，请立即返回nonce_str的值，作为接收成功的标识。
        System.out.println("receive notify : " + notify);

        JSONObject res = JSON.parseObject(notify);
        String nonce_str = res.getString("nonce_str");
        System.out.println("nonce_str : " + nonce_str);
        try {
            //业务处理
            //1、验签
            //2、 检查订单存在否
            //3、... ...
        }catch (Exception e) {
            e.printStackTrace();
        }

        return nonce_str;
    }

}
