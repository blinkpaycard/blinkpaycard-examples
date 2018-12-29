<?php
require_once "BlinkPayCard.Exception.php";
require_once "BlinkPayCard.Config.php";


/**
 * 
 * 接口访问类，包含所有 BlinkPay API列表的封装，类中方法为static方法，
 *
 */
class BlinkPayCardApi
{
	/**
	 * 
     * 充值下单
     * 该接口用于平台方从BlinkPayCard系统获取充值预下单号。
     * 平台方通过Post请求调用该接口，从返回结果中解析过预下单号（pre_order）。
	 * @param string $app_id 
	 * @param string $mch_id
	 * @param string $app_key
	 * @throws BlinkPayCardException
	 * @return 成功时返回，其他抛异常
	 */
	public static function preOrder($app_id, $mch_id, $app_key)
	{
		$url = "https://pay.blinkpaycard.com/pay/preOrder";
		
		$amount = "100";
		$currency = "CNY";
		$sign_type = 'md5';
		$osu_number = self::getNonceStr(32);

		//sign
		$params = array(
			"app_id" => $app_id,
			"mch_id" => $mch_id,
			"osu_number" => $osu_number,
			"amount" => $amount,
			"currency" => $currency,
			"sign_type" => $sign_type
		);

		//签名
		$sign = self::makeSign($params, $app_key, $sign_type);
		$params['sign'] = $sign;
		$content = json_encode($params);
		
		$response = self::postCurl($content, $url);
		echo "res: " . $response  . "\n </br>";
		$result = json_decode($response, true);

		$pre_order = $result['result']['pre_order'];

		return $pre_order;
	}

	/**
	 * 
     * 网页充值
     * 合作平台⽅方系统根据充值下单接⼝口获取的预下单号(pre_order)与BlinkPay Card系统分配的商户 ID(mch_id)获取BlinkPay Card⽹网⻚页充值⻚页⾯面
	 * @param string $mch_id
	 * @param string $pre_order
	 * @return 
	 */
	public static function webPay($mch_id, $pre_order)
	{
		return sprintf("https://pay.blinkpaycard.com/?mch_id=%s&pre_order=%s",$mch_id, $pre_order);
	}

	
	/**
	 * 
     * 订单查询
     * 该接口提供所有BlinkPayCard充值订单的查询，平台方可以通过查询订单接口主动查询订单状态，完成下一步的业务逻辑。
	 * @param string $app_id 
	 * @param string $mch_id
	 * @param string $app_key
	 * @param string $pre_order
	 * @throws BlinkPayCardException
	 * @return 成功时返回，其他抛异常
	 */
	public static function orderQuery($app_id, $mch_id, $app_key, $pre_order)
	{
		$url = "https://pay.api.blinkpaycard.com/pay/queryResult";

		$sign_type = 'md5';


		//sign
		$params = array(
			"app_id" => $app_id,
			"mch_id" => $mch_id,
			"pre_order" => $pre_order,
			"sign_type" => $sign_type

		);

		//签名
		$sign  = self::makeSign($params, $app_key, $sign_type);
		$params['sign'] = $sign;
		$content = json_encode($params);
		
		$response = self::postCurl($content, $url);
		// $result = json_decode($response);

		echo "res: " . $response  . "\n </br>";
	}

	/**
     * 接收通知消息
     * @return
     */
    public static function receiveNotify($notify)  {

		 //TODO 接收到通知后，平台方异步处理订单业务逻辑，请立即返回nonce_str的值，作为接收成功的标识。
        echo "receive notify : " . $notify  . "\n </br>";

        $content = json_decode($notify, true);
        $nonce_str = $content["nonce_str"];
        echo "nonce_str : " . $nonce_str  . "\n </br>";

        return $nonce_str;
    }

	
	/**
	 * 以post方式提交对应的接口url
	 * 
	 * @param string $content content
	 * @param string $url url
	 * @throws BlinkPayCardException
	 */
	private static function postCurl($content, $url)
	{		
		$curl = curl_init($url);

		curl_setopt($curl, CURLOPT_CUSTOMREQUEST, "POST");
		curl_setopt($curl, CURLOPT_HEADER, false);
		curl_setopt($curl, CURLOPT_RETURNTRANSFER, true);
		curl_setopt($curl, CURLOPT_SSL_VERIFYPEER, false);
		curl_setopt($curl, CURLOPT_HTTPHEADER, array("Content-type: application/json"));
		// curl_setopt($curl, CURLOPT_POST, true);
		curl_setopt($curl, CURLOPT_POSTFIELDS, $content);
		echo "url :" . $url . "\n </br>";
		echo "request params : " . $content  . "\n </br>";
		//运行curl
		$data = curl_exec($curl);
		echo "response data : " . $data  . "\n </br>";
		//返回结果
		if($data){
			curl_close($curl);
			return $data;
		} else { 
			$error = curl_errno($curl);
			echo curl_error($curl);
			curl_close($curl);
			throw new BlinkPayCardException("curl出错，错误码:$error");
		}
	}
	
	
	/**
	 * 生成签名
	 * @param array $content  配置对象
	 * @param string $appKey 
	 * @param string $signtype
	 * @return 签名
	 */
	private static function makeSign($content, $appKey, $signtype)
	{
		//签名步骤一：按字典序排序参数
		ksort($content);

		//签名步骤二：在string后加入KEY
		$sort_params = http_build_query($content);
		$sort_params .= "&key=" . $appKey;

		//签名步骤三：MD5加密或者HMAC-SHA256
		if($signtype == "md5"){
			$sort_params = md5($sort_params);
		} else if($csigntype == "HMAC-SHA256") {
			$strsort_paramsing = hash_hmac("sha256",$sort_params ,$appKey);
		} else {
			throw new BlinkPayCardException("签名类型不支持！");
		}
		
		//签名步骤四：所有字符转为大写
		$result = strtoupper($sort_params);
		return $result;
	}

	/**
	 * 
	 * 产生随机字符串，不长于32位
	 * @param int $length
	 * @return 产生的随机字符串
	 */
	private static function getNonceStr($length = 32) 
	{
		$chars = "abcdefghijklmnopqrstuvwxyz0123456789";  
		$str ="";
		for ( $i = 0; $i < $length; $i++ )  {  
			$str .= substr($chars, mt_rand(0, strlen($chars)-1), 1);  
		} 
		return $str;
	}
	
}