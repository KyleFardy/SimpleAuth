<?php
	include "../includes/database.php";
	class auth extends database{
		public function __construct(){
			$this->connect_to_database();
		}
		public function PasswordCheck($var, $var1){
			if(password_verify($var, $var1))
				return true;
			else
				return false;
		}
		public function PasswordCheck1($var, $var1){
			if($var == $var1)
				return true;
			else
				return false;
		}
		public function EmailCheck($var){
			$query = $this->db->prepare("SELECT * FROM `users` WHERE `email` = :emailAddr");
			$query->execute(array("emailAddr"=>self::sanitize($var)));
			$result = $query->fetch(PDO::FETCH_ASSOC);
			if($result)
				return true;
			else
				return false;
		}
		public function HWIDCheck($var, $var1){
			if($var == $var1)
				return true;
			else
				return false;
		}
		public function HWIDEmpty($var){
			if(empty($var))
				return true;
			else
				return false;
		}
		public function BanCheck($var){
			if($var == "1")
				return true;
			else
				return false;
		}
		
		public function Register($var, $var1, $var2, $var3, $var5){/* username, password, hwid, email, password1, token*/
			$query = $this->db->prepare("SELECT * FROM `users` WHERE `username` = :uname");
			$query->execute(array("uname"=>self::sanitize($var)));
			$result = $query->fetch(PDO::FETCH_ASSOC);
			if($this->TokenCheck(self::sanitize($var5))){
				if($result){
					die("Error [0x00000001]");//username
				}
				else{
					if(!$this->EmailCheck(self::sanitize($var3))){
						$this->update("register_tokens", array("used"=>1, "used_by"=>self::sanitize($var)), "token", self::sanitize($var5));
						$data =  array(
							"username"=>self::sanitize($var), 
							"password"=>password_hash($var1, PASSWORD_BCRYPT), 
							"email"=>self::sanitize($var3), 
							"hwid"=>self::sanitize($var2), 
							"user_ip"=>(isset($_SERVER["HTTP_CF_CONNECTING_IP"])?$_SERVER["HTTP_CF_CONNECTING_IP"]:$_SERVER['REMOTE_ADDR'])
						);
						$this->insert_query("users", $data);
						die('{"result":"Success [0x00001337]"}');//register success
					}
					else{
						die('{"result":"Error [0x00000002]"}');//email alredy in db
					}
				}
			}
			else{
				die('{"result":"Error [0x00000004]"}');//invalid token
			}
		}
		
		public function Login($var, $var1, $var3){
			$query = $this->db->prepare("SELECT * FROM `users` WHERE `username` = :uname");
			$query->execute(array("uname"=>self::sanitize($var)));
			$result = $query->fetch(PDO::FETCH_ASSOC);
			if($result){
				if($this->PasswordCheck(self::sanitize($var1), $result['password'])){
					if(!$this->HWIDEmpty($result['hwid'])){
						if($this->HWIDCheck(self::sanitize($var3), $result['hwid'])){
							if(!$this->BanCheck($result['banned'])){
								$this->update("users", array("user_ip"=>(isset($_SERVER["HTTP_CF_CONNECTING_IP"])?$_SERVER["HTTP_CF_CONNECTING_IP"]:$_SERVER['REMOTE_ADDR'])), "id", $result['id']);
								$this->insert_query("logins", array("userid"=>$result['id'], "ip_addr"=>(isset($_SERVER["HTTP_CF_CONNECTING_IP"])?$_SERVER["HTTP_CF_CONNECTING_IP"]:$_SERVER['REMOTE_ADDR']), "country"=>$this->getCountry(), "city"=>$this->getCity()), "id", $result['id']);
								die('{"result":"Success [0x00001337]",
							        "username":"'.$result['username'].'",
							        "password":"'.$var1.'",
							        "email":"'.$result['email'].'",
							        "user_level":"'.$result['user_level'].'"
								}');  //successfull login
							}
							else{
								die('{"result":"Error [0x00000004]"}');//banned  
							}
						}
						else{
							die('{"result":"Error [0x00000003]"}');//hwid no match 
						}
					}
					else{
						$this->update("users", array("hwid"=>self::sanitize($var3), "user_ip"=>(isset($_SERVER["HTTP_CF_CONNECTING_IP"])?$_SERVER["HTTP_CF_CONNECTING_IP"]:$_SERVER['REMOTE_ADDR'])), "id", $result['id']);
						die('{"result":"Error [0x00000005]"}');//hwid updated
					}
				}
				else{
					die('{"result":"Error [0x00000002]"}');//password
				}
			}
			else{
				die('{"result":"Error [0x00000001]"}');//username
			}
		}
		
		public function getCountry(){
			$ip = (isset($_SERVER["HTTP_CF_CONNECTING_IP"])?$_SERVER["HTTP_CF_CONNECTING_IP"]:$_SERVER['REMOTE_ADDR']);
			$details = json_decode(file_get_contents("http://ip-api.com/json/{$ip}"));
			return $details->country;
		}
		
		public function getCity(){
			$ip = (isset($_SERVER["HTTP_CF_CONNECTING_IP"])?$_SERVER["HTTP_CF_CONNECTING_IP"]:$_SERVER['REMOTE_ADDR']);
			$details = json_decode(file_get_contents("http://ip-api.com/json/{$ip}"));
			return $details->city;
		}
		
		public function Freemode(){
			$query = $this->db->prepare("SELECT * FROM `settings` WHERE `id` = :sid");
			$query->execute(array("sid"=>1));
			$result = $query->fetch(PDO::FETCH_ASSOC);
			if($result){
				switch($result['freemode']){
					case 1: return true; break;
					case 0: return false; break;
					default: return false;
				}
			}
		}
		
		public function TokenCheck($token){
			$query = $this->db->prepare("SELECT * FROM `register_tokens` WHERE `token` = :t");
			$query->execute(array("t"=>$token));
			$result = $query->fetch(PDO::FETCH_ASSOC);
			if($result){
				if($result['used'] == 0)
					return true;
				else
					return false;
			}
			else
				return false;
		}
	}