<?php
// Database connection parameters
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "player_db";
// Get data from Unity
// Get data from Unity
$playerName = $_POST['player_name']; // Assuming data is sent as POST
$winsToAdd = $_POST['wins_to_add']; // Assuming data is sent as POST

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

// Check if player_name exists in the database
$checkQuery = "SELECT * FROM player_table WHERE player_name = '$playerName'";
$checkResult = $conn->query($checkQuery);

if ($checkResult->num_rows > 0) {
    // Player exists, update the wins
    $updateQuery = "UPDATE player_table SET wins = wins + $winsToAdd WHERE player_name = '$playerName'";
    
    if ($conn->query($updateQuery) === TRUE) {
        echo "Wins updated successfully";
    } else {
        echo "Error updating wins: " . $conn->error;
    }
} else {
    // Player does not exist, insert a new record
    $insertQuery = "INSERT INTO player_table (player_name, wins) VALUES ('$playerName', $winsToAdd)";
    
    if ($conn->query($insertQuery) === TRUE) {
        echo "Player added successfully with initial wins";
    } else {
        echo "Error adding player: " . $conn->error;
    }
}

// Close connection
$conn->close();
?>