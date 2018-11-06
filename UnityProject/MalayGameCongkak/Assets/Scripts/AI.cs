using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI {
    private Difficulty diff;
    int[] marbleArray = new int[16];

    static float[] resetTime = { 1, 0.5f, 0.25f };
    
    public AI(Difficulty diff)
    {
        this.diff = diff;
    }

    public void UpdateWorld(Slot[] slots)
    {
        foreach(Slot slot in slots)
        {
            marbleArray[slot.slotID] = slot.MarbleAmount();
        }
    }

    public float WaitTime()
    {
        return resetTime[(int)diff];
    }

    public int NextMove()
    {
        List<int> possible = new List<int>();
        for (int i = 8; i < 15; i++)
        {
            if (marbleArray[i] > 0)
            {
                possible.Add(i);
            }
        }
        switch (diff)
        {
            case Difficulty.easy:
                return possible[Mathf.RoundToInt(Random.Range(0, possible.Count - 1))];
            case Difficulty.medium:
                //Free Points
                for (int i = 8; i < 15; i++)
                {
                    if (marbleArray[i] == i - 7)
                    {
                        return i;
                    }
                }

                //Attacking
                for (int i = 8; i < 15; i++)
                {
                    if (marbleArray[i] == 0 && marbleArray[14 - i] > 3)
                    {
                        for (int j = 1; j + i < 15; j++)
                        {
                            if (marbleArray[j+i] == j)
                            {
                                return j+i;
                            }
                        }
                    }
                }

                //defending
                int most = 8;
                for (int i = 8; i < 15; i++)
                {
                    if (marbleArray[most] < marbleArray[i])
                    {
                        most = i;
                    }
                }
                return most;
            default:
                return -1;
        }
    }
}
