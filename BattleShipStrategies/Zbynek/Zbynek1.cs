using BattleShipEngine;

namespace BattleShipStrategies.Slavek;

public class Zbynek1 : IGameStrategy
{
    private List<Int2> shots = new List<Int2>();
    private List<Int2> shotsHit = new List<Int2>();
    private GameSetting _setting;
    Int2 poslednistrela = new Int2();
    Int2 poslednistrelaPotopena = new Int2();
    public int boat4 = 0;
    public int boat3 = 0;
    public int boat2 = 0;
    public int boat1 = 0;
    public int Vrade = 0;

    public Int2 GetMove()
    {
        while (true)
        {


            if (poslednistrelaPotopena == (-1, -1)){
                int v = 0;
                int s = 0;
                while (true)
                {
                    if (!shots.Contains(v, s))
                    {
                        shots.Add(v, s);

                        Int2 shot = new Int2(v, s);
                        poslednistrela = (v, s);
                        boat4--;
                    }
                    v =v+ 4;
                    if (v >= _setting.Width)
                    {
                        v == v - 11;
                        s++;
                    }
                    if (s >= _setting.Height) break;
                }
                Int2 shot = new Int2(
                Random.Shared.Next(_setting.Width),
                Random.Shared.Next(_setting.Height)
            );
                if (!shots.Contains(shot))
                {
                    shots.Add(shot);
                    poslednistrela = shot;
                    return shot;
                }
            }





            int i = 0;
            while (true)
            {
                if (poslednistrelaPotopena.X + i >= _setting.Width) break;

                if (!shotsHit.Contains(poslednistrelaPotopena.X + i, poslednistrelaPotopena.Y) && shots.Contains(poslednistrelaPotopena.X + i, poslednistrelaPotopena.Y)) break;
                if (!shots.Contains(poslednistrelaPotopena.X + i, poslednistrelaPotopena.Y))
                {
                    shots.Add(poslednistrelaPotopena.X + i, poslednistrelaPotopena.Y);

                    Int2 shot = new Int2(poslednistrelaPotopena.X + i, poslednistrelaPotopena.Y);
                    poslednistrela = (poslednistrelaPotopena.X + i, poslednistrelaPotopena.Y);
                    boat4--;
                    return shot;

                }

                i++;
            }

            i = 0;
            while (true)
            {
                if (poslednistrelaPotopena.X - i < 0) break;
                if (!shotsHit.Contains(poslednistrelaPotopena.X - i, poslednistrelaPotopena.Y) && shots.Contains(poslednistrelaPotopena.X - i, poslednistrelaPotopena.Y)) break;
                if (!shots.Contains(poslednistrelaPotopena.X - i, poslednistrelaPotopena.Y))
                {
                    shots.Add(poslednistrelaPotopena.X - i, poslednistrelaPotopena.Y);

                    Int2 shot = new Int2(poslednistrelaPotopena.X - i, poslednistrelaPotopena.Y);
                    poslednistrela = (poslednistrelaPotopena.X - i, poslednistrelaPotopena.Y);
                    boat4--;
                    return shot;

                }

                i++;
            }






            int i = 0;
            while (true)
            {
                if (poslednistrelaPotopena.Y + i >= _setting.Width) break;

                if (!shotsHit.Contains(poslednistrelaPotopena.X, poslednistrelaPotopena.Y + i) && shots.Contains(poslednistrelaPotopena.X, poslednistrelaPotopena.Y + i)) break;
                if (!shots.Contains(poslednistrelaPotopena.X, poslednistrelaPotopena.Y + i))
                {
                    shots.Add(poslednistrelaPotopena.X, poslednistrelaPotopena.Y + i);

                    Int2 shot = new Int2(poslednistrelaPotopena.X, poslednistrelaPotopena.Y + i);
                    poslednistrela = (poslednistrelaPotopena.X, poslednistrelaPotopena.Y + i);
                    boat4--;
                    return shot;

                }

                i++;
            }

            i = 0;
            while (true)
            {
                if (poslednistrelaPotopena.Y - i < 0) break;
                if (!shotsHit.Contains(poslednistrelaPotopena.X, poslednistrelaPotopena.Y - i) && shots.Contains(poslednistrelaPotopena.X, poslednistrelaPotopena.Y - i)) break;
                if (!shots.Contains(poslednistrelaPotopena.X, poslednistrelaPotopena.Y - i))
                {
                    shots.Add(poslednistrelaPotopena.X, poslednistrelaPotopena.Y - i);

                    Int2 shot = new Int2(poslednistrelaPotopena.X, poslednistrelaPotopena.Y - i);
                    poslednistrela = (poslednistrelaPotopena.X, poslednistrelaPotopena.Y - i);
                    boat4--;
                    return shot;

                }

                i++;
            }















            if (boat4 > 0)
            {

            }





            Int2 shot = new Int2(
                Random.Shared.Next(_setting.Width),
                Random.Shared.Next(_setting.Height)
            );
            if (!shots.Contains(shot))
            {
                shots.Add(shot);
                poslednistrela = shot;
                return shot;
            }
        }
    }

    public void RespondHit()
    {
        shotsHit.Add(poslednistrela);
        poslednistrelaPotopena = poslednistrela;
        


        



    }

    public void RespondSunk()
    {
        poslednistrelaPotopena=(-1,-1);
        shotsHit.Clear;
    }

    public void RespondMiss()
    {
    }

    public void Start(GameSetting setting)
    {
        shots = new List<Int2>();
        _setting = setting;
        boat1 = _setting.BoatCount[0];
        boat2 = _setting.BoatCount[1];
        boat3 = _setting.BoatCount[2];
        boat4 = _setting.BoatCount[3];
    }
}