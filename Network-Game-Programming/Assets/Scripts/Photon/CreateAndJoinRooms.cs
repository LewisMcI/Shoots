using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI joinInput;
    public void CreateRoom()
    {         
        // Generate roomCode
        PhotonNetwork.CreateRoom("123");
    }    
    
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom("123");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("First Level");
    }

    public void BackToTitleScreen()
    {
        PhotonNetwork.LeaveLobby();
    }

    public override void OnLeftLobby()
    {
        SceneManager.LoadScene("Title Screen");
    }
}
