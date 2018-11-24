<?php
require_once "BlinkPayCard.Config.php"; 
require_once "BlinkPayCard.Api.php"; 

$config = new BlinkPayCardConfig;
$api = new BlinkPayCardApi;

$app_id = $config -> getAppKey();
$mch_id = $config -> getMerchantId();
$app_key = $config -> getAppId();

$notify = "{\"nonce_str\":\"0F77ezcval5lEYb6FKPtCMqwMMqzHHLm\"," .
	"\"amount\":\"100\"," .
	"\"event_type\":\"order_payed\"," .
	"\"sign\":\"64963552561AD42D1D842271B7123D02\"," .
	"\"currency\":\"CNY\"," .
	"\"pre_order\":\"dae80fd0a98e4ba6bc4a77a3bac64d2c\"," .
	"\"sign_type\":\"md5\"," .
	"\"osu_number\":\"efaa8247f91748468e8ae541e563cb39\"}";

$card_number = "1510000003360001";
$password = "091086";

echo "=========== 充值下单 ===========" . "\n </br>";
$pre_order = $api -> preOrder($app_id, $mch_id, $app_key);

echo "=========== 网页充值 =========== " . "\n </br>";
$webPayUrl = $api -> webPay($mch_id, $pre_order);
echo "请用浏览器访问：" . $webPayUrl  . "\n </br>";

echo "=========== 接口充值 ===========" . "\n </br>";
$api -> innerPay($app_id, $mch_id, $app_key, $card_number, $password, $pre_order);

echo "=========== 订单查询 ===========" . "\n </br>";
$api -> orderQuery($app_id, $mch_id, $app_key, $pre_order);

echo "=========== 模拟接收通知消息 ===========" . "\n </br>";
$api -> receiveNotify($notify);

?>