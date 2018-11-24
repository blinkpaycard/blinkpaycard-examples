<?php
/**
 * 
 * BlinkPayCard API异常类
 * @author finch
 *
 */
class BlinkPayCardException extends Exception {
	public function errorMessage()
	{
		return $this->getMessage();
	}
}