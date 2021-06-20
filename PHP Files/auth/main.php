<?php
	include "class.php";
	$auth = new auth;
	$var = $_POST['type'];
	$var1 = $_POST['username'];
	$var2 = $_POST['password'];
	$var3 = $_POST['hwid'];
	$var4 = $_POST['email'];
	$var6 = $_POST['token'];
	if(isset($var) || !empty($var)){
		if($var == "login" || $var == "freemode" || $var == "register")
			switch(strip_tags($var)){
				case "login": $auth->Login($var1, $var2, $var3); break;
				case "freemode": $auth->Freemode() ? die('{"freemdoe_status":"1", "username":"Freemode User", "password":"freemode", "email":"freemode_user@email.com", "user_level":"0"}') : die('{"freemdoe_status":"0"}'); break;
				case "register": $auth->Register($var1, $var2, $var3, $var4, $var6); break;
			}
		else
			die('{"result":"Error [0x00000007]"}');//invalid params
	}
	else{
		die('{"result":"Error [0x00000006]"}');//empty params
	}