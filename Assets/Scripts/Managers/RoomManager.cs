using FishNet;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private string m_ServerAddress = "localhost";
    [SerializeField] private ushort m_Port = 7770;

    public string CurrentHostAddress { get; private set; }

    public void CreateRoom()
    {
        InstanceFinder.TransportManager.Transport.SetMaximumClients(2);

        CurrentHostAddress = GetLocalIPv4();

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

    private string GetLocalIPv4()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "127.0.0.1";
    }
}