package com.doradopay.example;

import com.alibaba.fastjson.JSON;

import java.util.HashMap;
import java.util.Map;

/**
 * 该示例演示DoradoPay的下单API 调用
 */
public class DoradoPayApi {

    public static void main(String[] args) throws Exception{

        DoradoPayConfig config = new DoradoPayConfig();
        String merchant_key = config.getMerchant_key();
        String merchant_sk = config.getMerchant_sk();
        long amount = 500;
        String currency = "USD";
        String osu_number = "1xss2343d3";
        String email = "gim.yum20@icloud.com";
        String ip_address = "142.136.62.200";
        String card_exp_month = "03";
        String card_exp_year = "2023";
        String card_number = "4385211206195406";
        String card_cvc = "890";

        String product_info = "casio 2019 newest";
        String country = "US";
        String state = "CA";
        String city = "Covina";
        String address = "137 W San Bernardino Rd.";
        String zip_code = "91723";
        String phone = "(909) 394-9899 ";
        String shipping_first_name = "gim";
        String shipping_last_name = "yum";
        String shipping_state = "CA";
        String shipping_country = "US";
        String shipping_city = "Covina";
        String shipping_address = "137 W San Bernardino Rd.";
        String shipping_zip_code = "91723";
        String shipping_email = "gim.yum20@icloud.com";
        String shipping_phone = "(909) 394-9899";

        Map<String, Object> params = new HashMap<>();

        params.put("merchant_key", merchant_key);
        params.put("amount", String.valueOf(amount));
        params.put("currency", currency);
        params.put("osu_number", osu_number);
        params.put("email", email);
        params.put("card_number", card_number);
        params.put("card_exp_month", card_exp_month);
        params.put("card_exp_year", card_exp_year);
        params.put("card_cvc", card_cvc);
        params.put("ip_address", ip_address);

        // 校验签名
        Map<String, String> signData = new HashMap<>();
        signData.put("merchant_key", merchant_key);
        signData.put("amount", String.valueOf(amount));
        signData.put("currency", currency);
        signData.put("osu_number", osu_number);
        signData.put("card_number", card_number);
        signData.put("card_exp_year", card_exp_year);
        signData.put("card_exp_month", card_exp_month);
        signData.put("card_cvc", card_cvc);
        signData.put("email", email);
        if (ip_address != null) {
            signData.put("ip_address", ip_address);
        }

        String sign = DoradoPayUtil.generateSignature(signData, merchant_sk, "md5");
        params.put("sign", sign);

        Map<String, Object> orderInfo = new HashMap<>();
        orderInfo.put("product_info", product_info);
        orderInfo.put("country", country);
        orderInfo.put("state", state);
        orderInfo.put("city", city);
        orderInfo.put("address", address);
        orderInfo.put("zip_code", zip_code);
        orderInfo.put("phone", phone);
        orderInfo.put("shipping_first_name", shipping_first_name);
        orderInfo.put("shipping_last_name", shipping_last_name);
        orderInfo.put("shipping_state", shipping_state);
        orderInfo.put("shipping_country", shipping_country);
        orderInfo.put("shipping_city", shipping_city);
        orderInfo.put("shipping_address", shipping_address);
        orderInfo.put("shipping_zip_code", shipping_zip_code);
        orderInfo.put("shipping_email", shipping_email);
        orderInfo.put("shipping_phone", shipping_phone);

        params.put("order_info", orderInfo);


        System.out.println("=========== 下单 ===========");
        String resp = DoradoPayRequest.requestOnce("https://uat.doradopay.com","/api/cpi/pay", JSON.toJSONString(params),6*1000 ,8*1000);
        System.out.println(String.format("resp : %s", resp));

    }

}
