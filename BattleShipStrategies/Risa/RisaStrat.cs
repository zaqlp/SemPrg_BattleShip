using BattleShipEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipStrategies.Risa
{
    public class RisaStrat : IGameStrategy
    {
        int[,] canBeShip { get; set; }
        int[] shipCount { get; set; }

        List<(int, int)> missList { get; set; }

        (int, int) lastShot { get; set; }

        List<(int, int)> lastShip { get; set; }

        char dir {get; set;}
        public Int2 GetMove()
        {
            if (lastShip.Count > 0)
            {
                if(dir == 'v')
                {
                    if (canBeShip[lastShip[lastShip.Count-1].Item1, lastShip[lastShip.Count - 1].Item2+1] == 0)
                    {
                        return new Int2
                        {
                            X = lastShip[lastShip.Count - 1].Item1 - 1,
                            Y = lastShip[lastShip.Count - 1].Item2
                        };
                    }
                    if (canBeShip[lastShip[lastShip.Count - 1].Item1, lastShip[lastShip.Count - 1].Item2 - 1] == 0)
                    {
                        return new Int2
                        {
                            X = lastShip[lastShip.Count - 1].Item1 - 1,
                            Y = lastShip[lastShip.Count - 1].Item2 - 2
                        };
                    }
                    if (canBeShip[lastShip[0].Item1, lastShip[0].Item2 - 1] == 0)
                    {
                        return new Int2
                        {
                            X = lastShip[0].Item1 - 1,
                            Y = lastShip[0].Item2 - 2
                        };
                    }
                    throw new Exception("moveNotFound");
                }
                if (dir == 'h')
                {
                    if (canBeShip[lastShip[lastShip.Count - 1].Item1 + 1, lastShip[lastShip.Count - 1].Item2] == 0)
                    {
                        return new Int2
                        {
                            X = lastShip[lastShip.Count - 1].Item1,
                            Y = lastShip[lastShip.Count - 1].Item2 - 1
                        };
                    }
                    if (canBeShip[lastShip[lastShip.Count - 1].Item1- 1, lastShip[lastShip.Count - 1].Item2] == 0)
                    {
                        return new Int2
                        {
                            X = lastShip[lastShip.Count - 1].Item1 -2,
                            Y = lastShip[lastShip.Count - 1].Item2-1
                        };
                    }
                    if (canBeShip[lastShip[0].Item1 - 1, lastShip[0].Item2] == 0)
                    {
                        return new Int2
                        {
                            X = lastShip[0].Item1 - 2,
                            Y = lastShip[0].Item2 - 1
                        };
                    }
                    throw new Exception("moveNotFound");
                }
                if (canBeShip[lastShip[0].Item1 + 1, lastShip[0].Item2] == 0)
                {
                    return new Int2
                    {
                        X = lastShip[0].Item1,
                        Y = lastShip[0].Item2-1
                    };
                }
                if (canBeShip[lastShip[0].Item1, lastShip[0].Item2 + 1] == 0)
                {
                    return new Int2
                    {
                        X = lastShip[0].Item1-1,
                        Y = lastShip[0].Item2
                    };
                }
                if (canBeShip[lastShip[0].Item1 - 1, lastShip[0].Item2] == 0)
                {
                    return new Int2
                    {
                        X = lastShip[0].Item1 - 2,
                        Y = lastShip[0].Item2-1
                    };
                }
                if (canBeShip[lastShip[0].Item1, lastShip[0].Item2 - 1] == 0)
                {
                    return new Int2
                    {
                        X = lastShip[0].Item1-1,
                        Y = lastShip[0].Item2 - 2
                    };
                }
                throw new Exception("moveNotFound");
            }

            else
            {
                bool[,] visited = new bool[canBeShip.GetLength(0), canBeShip.GetLength(1)];
                Queue<(int, int)> queue = new Queue<(int, int)>();
                foreach ((int, int) miss in missList)
                {
                    queue.Enqueue(miss);
                    visited[miss.Item1, miss.Item2] = true;
                }
                lastShot = (-1, -1);
                while (queue.Count > 0)
                {
                    (int, int) current = queue.Dequeue();
                    if (canBeShip[current.Item1, current.Item2] == 0)
                    {
                        lastShot = current;
                    }
                    if (current.Item1 > 0 && !visited[current.Item1 - 1, current.Item2])
                    {
                        visited[current.Item1 - 1, current.Item2] = true;
                        queue.Enqueue((current.Item1 - 1, current.Item2));
                    }
                    if (current.Item1 < canBeShip.GetLength(0)-1 && !visited[current.Item1 + 1, current.Item2])
                    {
                        visited[current.Item1 + 1, current.Item2] = true;
                        queue.Enqueue((current.Item1 + 1, current.Item2));
                    }
                    if (current.Item2 > 0 && !visited[current.Item1, current.Item2 - 1])
                    {
                        visited[current.Item1, current.Item2 - 1] = true;
                        queue.Enqueue((current.Item1, current.Item2 - 1));
                    }
                    if (current.Item2 < canBeShip.GetLength(1)-1 && !visited[current.Item1, current.Item2 + 1])
                    {
                        visited[current.Item1, current.Item2 + 1] = true;
                        queue.Enqueue((current.Item1, current.Item2 + 1));
                    }
                }
                return new Int2
                {
                    X = lastShot.Item1 - 1,
                    Y = lastShot.Item2 - 1,
                };
            }
        }

        public void RespondHit()
        {
            if (lastShip.Count > 0 && dir == 'n')
            {
                if(lastShot.Item1 == lastShip[0].Item1) 
                {
                    dir = 'v';
                }
                else if (lastShot.Item2 == lastShip[0].Item2)
                {
                    dir = 'h';
                }
            }
            lastShip.Add(lastShot);
            canBeShip[lastShot.Item1, lastShot.Item2] = 1;

        }

        public void RespondMiss()
        {
            missList.Add(lastShot);
            canBeShip[lastShot.Item1, lastShot.Item2] = -1;
        }

        public void RespondSunk()
        {
            shipCount[lastShip.Count]--;
            missList.AddRange(lastShip);
            lastShip.Clear();
            dir = 'n';
            foreach ((int, int) square in lastShip)
            {
                canBeShip[square.Item1 - 1, square.Item2 - 1] = -1;
                canBeShip[square.Item1 - 1, square.Item2] = -1;
                canBeShip[square.Item1 - 1, square.Item2 + 1] = -1;
                canBeShip[square.Item1, square.Item2 - 1] = -1;
                canBeShip[square.Item1, square.Item2 + 1] = -1;
                canBeShip[square.Item1 + 1, square.Item2 - 1] = -1;
                canBeShip[square.Item1 + 1, square.Item2] = -1;
                canBeShip[square.Item1 + 1, square.Item2 + 1] = -1;
            }
            foreach ((int, int) square in lastShip)
            {
                canBeShip[square.Item1, square.Item2] = 1;
            }
        }

        public void Start(GameSetting setting)
        {
            canBeShip = new int[setting.Height+2, setting.Width+2];
            shipCount = setting.BoatCount;
            missList = new List<(int, int)>();
            lastShot = (-1, -1);
            lastShip = new List<(int, int)>();
            dir = 'n';
            for (int i = 0; i < setting.Height+2; i++) 
            {
                canBeShip[i, 0] = 0;
                canBeShip[i, setting.Width + 1] = 0;
                missList.Add((i, 0));
                missList.Add((i, setting.Width + 1));
            }
            for (int i = 0; i < setting.Width + 2; i++)
            {
                canBeShip[0, i] = 0;
                canBeShip[setting.Height + 1, 0] = 0;
                missList.Add((0, i));
                missList.Add((setting.Height + 1, i));
            }

        }
    }
}
