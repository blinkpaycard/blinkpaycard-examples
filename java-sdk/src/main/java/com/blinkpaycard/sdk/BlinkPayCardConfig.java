package com.blinkpaycard.sdk;

public class BlinkPayCardConfig {

    /**
     * 此为测试账号的开发配置，申请正式账号后请替换
     */
    private String appId = "appEqMuvZ088";
    private String mchId = "blEAgkNtLm8";
    private String appKey = "keyW8LNi4GJzyMqEsnjS2ZMw6kkNtDUObxB";

    public String getAppId() {
        return appId;
    }

    public void setAppId(String appId) {
        this.appId = appId;
    }

    public String getMchId() {
        return mchId;
    }

    public void setMchId(String mchId) {
        this.mchId = mchId;
    }

    public String getAppKey() {
        return appKey;
    }

    public void setAppKey(String appKey) {
        this.appKey = appKey;
    }
}
