<?php

    class database{
        public $db;
        private $db_host = "localhost";
        private $db_user = "root";
        private $db_password = "";
        private $db_name = "simpleauth";

        public function connect_to_database(){
			try {
				$this->db = new PDO("mysql:host=".$this->db_host.";dbname=".$this->db_name, $this->db_user, $this->db_password);
			}
			catch(PDOException $e){
				die("<center><h1>Error: ".$e->getMessage()."</h1></center>");
				exit;
			}
        }
        public function custom_query($query,$arr = NULL){
            $q = $this->db->prepare($query);
            $q->execute($arr);
            return $q->fetchall();
        }
        public function delete_all($table){
            $q = $this->db->prepare("TRUNCATE TABLE $table");
            return $q->execute();
        }
        public function select_all($table){
            $q = $this->db->prepare("SELECT * FROM $table");
            $q->execute();
            return $q->fetchall();
        }
        public function select($what,$table, $specifier = NULL,$val = NULL, $type = NULL){
            if(!$type){
                $q = $this->db->prepare("SELECT $what FROM $table WHERE $specifier = :s");
                $q->execute(array("s"=>$val));
                return $q->fetchall();
            }else{
                $q = $this->db->prepare("SELECT $what FROM $table");
                $q->execute(array("s"=>$val));
                return $q->fetchall();
            }
        }
        public function insert_query($where, $col){
            $vals = NULL;
            $column = NULL;
            foreach ($col as $key => $val){
                if($column){
                    $column .= " , `".$key."`";
                }else{
                    $column .= "`".$key."`";
                }
                if($vals){
                    $vals .= ",:".$key;
                }else{
                    $vals .= ":".$key;
                }
            }
            $q = $this->db->prepare("INSERT INTO $where ($column) VALUES ($vals) ");
            return $q->execute($col);
        }
        public function update($where,$col,$specifier,$spec){
            $vals = NULL;
            $column = NULL;
            foreach ($col as $key => $val){
                if($column){
                    $column .= " , `".$key."` = :".$key;
                }else{
                    $column .= "`".$key."` = :".$key;
                }
            }
            $q = $this->db->prepare("UPDATE $where SET $column WHERE $specifier = :spec ");
            $col['spec'] = $spec;
            return $q->execute($col);

        }
        static function sanitize($val){
            return htmlspecialchars($val);
        }
    }