<?php
/**
*
* 实际部署时，请务必保管自己的商户密钥，证书等
* 
**/

class BlinkPayCardConfig
{


	//=======【BlinkPayCard信息配置】=====================================
	/**
     *
	 * TODO: 此为测试账号的开发配置，申请正式账号后请替换
	 * 
	 * APPID：绑定支付的APPID
	 * 
	 * MCHID：商户号
	 * 
	 * KEY：商户支付密钥
	 * 
	 **/


	public function getAppId()
	{
		return "appEqMuvZ088";
	}

	public function getMerchantId()
	{
		return "blEAgkNtLm8";
	}
	
	public function getAppKey()
	{
		return "keyW8LNi4GJzyMqEsnjS2ZMw6kkNtDUObxB";
	}


}
