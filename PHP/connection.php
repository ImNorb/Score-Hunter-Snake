<?php 
const connectionData = array(
	"type" => "mysql",
	"host" => "<host_here>",
	"username" => "<username_here>",
	"password" => "<password_here>",
	"dbname" => "<database_here>"
);

function getConnection() {
	$connection = new PDO(connectionData['type'].':host='.connectionData['host'].';dbname='.connectionData['dbname'].';',connectionData['username'],connectionData['password']);
	$connection->exec("SET NAMES utf8");
	return $connection;
}

function getRecord($queryString, $queryParams = []){
	$connection = getConnection();  
	$statement = $connection->prepare($queryString);
	$success = $statement->execute($queryParams);
	$result = $success ? $statement->fetch() : [];
	$statement->closeCursor();
	$connection = null;
	return $result;
}

function getList($queryString, $queryParams = []){
	$connection = getConnection();  
	$statement = $connection->prepare($queryString);
	$success = $statement->execute($queryParams);
	$result = $success ? $statement->fetchAll() : [];
	$statement->closeCursor();
	$connection = null;    
	return $result;
}

function executeDML($queryString, $queryParams = []){
	$connection = getConnection();  
	$statement = $connection->prepare($queryString);
	$success = $statement->execute($queryParams);
	$statement->closeCursor();
	$connection = null;
	return $success;
}
?>