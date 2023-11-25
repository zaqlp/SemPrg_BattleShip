using System.Net;
using System.Net.Sockets;
using System.Text;
using BattleShipEngine;

namespace BattleShipExternalStrategies;

public class ExternalGameStrategy : IGameStrategy
{
    private Socket _socket;
    private Socket _client;
    private int _errors = 0;

    public ExternalGameStrategy(int port)
    {
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddr = ipHost.AddressList[0];
        Console.WriteLine("Opening at " + ipAddr.ToString());
        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);
        
        _socket = new Socket(ipAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
        _socket.Bind(localEndPoint);
        _socket.Listen(10);
        Console.WriteLine(port.ToString() + ": Waiting for connection.");
        _client = _socket.Accept();
        Console.WriteLine(port.ToString() + ": Connected.");

        _errors = 0;
    }
    
    public Int2 GetMove()
    {
        try
        {
            while (true)
            {
                _client.Send("Enter your move (in the format x,y)<EOF>"u8.ToArray());

                byte[] bytes = new Byte[1024];
                string input = "";
 
                while (true) {
 
                    int numByte = _client.Receive(bytes);
                 
                    input += Encoding.ASCII.GetString(bytes,
                        0, numByte);
                                            
                    if (input.IndexOf("<EOF>") > -1)
                        break;
                }
                
                var parts = input.Split(',');

                if (!int.TryParse(parts[0], out var row))
                {
                    _client.Send("Invalid X"u8.ToArray());
                    continue;
                }

                if (!int.TryParse(parts[1], out var column))
                {
                    _client.Send("Invalid Y"u8.ToArray());
                    continue;
                }

                return new Int2(row, column);
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("ERROR: {0}", e.Message);
        }

        _errors++;
        if (_errors > 20)
            throw new Exception("Too many errors with the external strategy.");
        return new Int2(0, 0);
    }

    public void RespondHit()
    {
        try
        {
            _client.Send("HIT!<EOF>"u8.ToArray());
        }
        catch (IOException e)
        {
            Console.WriteLine("ERROR: {0}", e.Message);
        }
    }

    public void RespondSunk()
    {
        try
        {
            _client.Send("SUNK!!!<EOF>"u8.ToArray());
        }
        catch (IOException e)
        {
            Console.WriteLine("ERROR: {0}", e.Message);
        }
    }

    public void RespondMiss()
    {
        try
        {
            _client.Send("MISS.<EOF>"u8.ToArray());
        }
        catch (IOException e)
        {
            Console.WriteLine("ERROR: {0}", e.Message);
        }
    }

    public void Start(GameSetting setting)
    {
        if (setting.Width == 0 || setting.Height == 0)
        {
            Close();
            return;
        }
        try
        {
            _client.Send(Encoding.ASCII.GetBytes(
                "Width: " + setting.Width.ToString() + '\n'));
            _client.Send(Encoding.ASCII.GetBytes(
                "Height: " + setting.Height.ToString() + '\n'));
            _client.Send(Encoding.ASCII.GetBytes("Boats: "));
            for (int i = 0; i < setting.BoatCount.Length; i++)
                _client.Send(Encoding.ASCII.GetBytes(
                    (i + 1).ToString() + ": " + setting.BoatCount[i].ToString() + ' '));
            _client.Send("<EOF>"u8.ToArray());
        }
        catch (IOException e)
        {
            Console.WriteLine("ERROR: {0}", e.Message);
        }
    }

    private void Close()
    {
        try
        {
            _client.Send("Turn off.<EOF>"u8.ToArray());
            _client.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        catch (IOException e)
        {
            Console.WriteLine("ERROR: {0}", e.Message);
        }
    }
}