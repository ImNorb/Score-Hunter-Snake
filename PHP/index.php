<?php 
if(getAuth('admin')) {
	$method = $_SERVER['REQUEST_METHOD'];
	switch($method) {
		case "GET":
		if(array_key_exists('M', $_GET) && !empty($_GET['M']))
			loadGame($_GET['M']);
		break;

		case "POST":
		if(array_key_exists('M', $_GET) && !empty($_GET['M']))
			saveGame($_GET['M']);
		break;

		case "PUT":
			jsonApp('CONNECTED');
		break;

		default:
			header("HTTP/1.0 405 Method Not Allowed");	
		break;
	}
} else jsonApp('WRONG AUTH INFO');

function saveGame($mode) {
	if($mode == 'c' || $mode == 'sh') {
		$table = $mode == 'c' ? 'sb_classic' : 'sb_scorehunt';
		$data = json_decode(file_get_contents('php://input'), true);
		$query = 'INSERT INTO '.$table.' (name, score) VALUES (:name, :score)';
		$params = [
			':name' => $data["Name"],
			':score' => $data["Score"]
		];
		include_once 'connection.php';
		$response = executeDML($query, $params);
		jsonApp($response);
	} else jsonApp('Wrong gamemode');
}

function loadGame($mode) {
	if($mode == 'c' || $mode == 'sh') {
		$table = $mode == 'c' ? 'sb_classic' : 'sb_scorehunt';
		$query = 'SELECT name, score FROM '.$table.' ORDER BY score DESC LIMIT 10';
		include_once 'connection.php';
		$response = getList($query);
		jsonApp($response);
	} else jsonApp('Wrong gamemode');
}

function jsonApp($response) {
	header('Content-Type: application/json');
	echo json_encode($response, JSON_UNESCAPED_UNICODE);
}

function getAuth($authString) {
	return array_key_exists('A', $_GET) && $_GET['A'] == $authString;
}	
?>