using FishNet;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private string _serverAddress = "localhost";
    [SerializeField] private ushort _port = 7770;

    public string CurrentHostAddress { get; private set; }

    public void CreateRoom()
    {
        ConfigureRoom();

        CurrentHostAddress = GetLocalIpv4();

        StartServer();
        StartClient(_serverAddress);
    }

    public void JoinRoom(string address)
    {
        StartClient(address);
    }

    public void LeaveRoom()
    {
        StopConnections();
    }

    private void ConfigureRoom()
    {
        InstanceFinder.TransportManager
            .Transport
            .SetMaximumClients(2);
    }

    private void StartServer()
    {
        InstanceFinder.ServerManager.StartConnection(_port);
    }

    private void StartClient(string address)
    {
        InstanceFinder.ClientManager.StartConnection(
            address,
            _port);
    }

    private void StopConnections()
    {
        InstanceFinder.ClientManager.StopConnection();

        InstanceFinder.ServerManager.StopConnection(true);
    }

    private string GetLocalIpv4()
    {
        IPHostEntry host =
            Dns.GetHostEntry(Dns.GetHostName());

        foreach (IPAddress ipAddress in host.AddressList)
        {
            if (ipAddress.AddressFamily ==
                AddressFamily.InterNetwork)
            {
                return ipAddress.ToString();
            }
        }

        return "127.0.0.1";
    }
}