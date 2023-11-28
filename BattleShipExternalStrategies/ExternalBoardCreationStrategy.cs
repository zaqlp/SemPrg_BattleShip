using System.Net;
using System.Net.Sockets;
using System.Text;
using BattleShipEngine;

namespace BattleShipExternalStrategies;

public class ExternalBoardCreationStrategy : IBoardCreationStrategy
{
    private Socket _socket;
    private Socket _client;

    public ExternalBoardCreationStrategy(int port)
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
    }
    
    public Int2[] GetBoatPositions(GameSetting setting)
    {
        if (setting.Width == 0 || setting.Height == 0)
        {
            Close();
            return new Int2[] {};
        }
        try
        {
            string s = "New game starts.\n";
            s += "Width: " + setting.Width.ToString() + '\n';
            s += "Height: " + setting.Height.ToString() + '\n';
            s += "Boats: ";
            for (int i = 0; i < setting.BoatCount.Length; i++)
            {
                s += (i + 1).ToString() + ": " + setting.BoatCount[i].ToString();
                if (i < setting.BoatCount.Length - 1)
                    s += ", ";
            }
            _client.Send(Encoding.ASCII.GetBytes(s + "<EOF>"));
            _client.Send(
                "Send list of boats' coordinates as '0,0+0,1+2,0+...'<EOF>"u8.ToArray());
            
            byte[] bytes = new Byte[1024];
            string input = "";
 
            while (true)
            {
 
                int numByte = _client.Receive(bytes);
                 
                input += Encoding.ASCII.GetString(bytes,
                    0, numByte);
                                            
                if (input.IndexOf("<EOF>") > -1)
                    break;
            }

            input = input.Replace("<EOF>", "");
            var parts = input.Split('+');
            
            List<Int2> boats = new List<Int2>();

            foreach (var part in parts)
            {
                var place = part.Split(',');
                
                if (!int.TryParse(place[0], out var row))
                {
                    _client.Send("Invalid X<EOF>"u8.ToArray());
                    throw new Exception("Invalid map");
                }
                if (!int.TryParse(place[1], out var column))
                {
                    _client.Send("Invalid Y<EOF>"u8.ToArray());
                    throw new Exception("Invalid map");
                }
                
                boats.Add(new Int2(row, column));
            }

            return boats.ToArray();
        }
        catch (IOException e)
        {
            Console.WriteLine("ERROR: {0}", e.Message);
        }

        return new Int2[] {};
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