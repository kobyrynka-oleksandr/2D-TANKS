using FishNet;
using FishNet.Managing;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private string m_ServerAddress = "localhost";
    [SerializeField] private ushort m_Port = 7770;

    public void CreateRoom()
    {
        InstanceFinder.TransportManager.Transport.SetMaximumClients(2);

        InstanceFinder.ServerManager.StartConnection(m_Port);
        InstanceFinder.ClientManager.StartConnection(m_ServerAddress, m_Port);
    }

    public void JoinRoom(string address)
    {
        InstanceFinder.ClientManager.StartConnection(address, m_Port);
    }

    public void LeaveRoom()
    {
        InstanceFinder.ClientManager.StopConnection();
        InstanceFinder.ServerManager.StopConnection(true);
    }
}