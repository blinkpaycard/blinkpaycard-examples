package com.doradopay.example;

public class DoradoPayConfig {

    /**
     * 此为测试账号的开发配置，申请正式账号后请替换
     */
    private String merchant_key =  "vSObKXFcdHh7nCg3daB7dTU3XHneKbd0";

    private String merchant_sk = "mskLTvmS7enfKDCoOPhcED3tAsBYW3zaV9c";

    public String getMerchant_key() {
        return merchant_key;
    }

    public void setMerchant_key(String merchant_key) {
        this.merchant_key = merchant_key;
    }

    public String getMerchant_sk() {
        return merchant_sk;
    }

    public void setMerchant_sk(String merchant_sk) {
        this.merchant_sk = merchant_sk;
    }
}
