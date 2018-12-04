using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI {
    private Difficulty diff;
    int[] marbleArray = new int[16];
    List<int> noGoList = new List<int>();

    static float[] resetTime = { 1, 0.5f, 0.25f };

    private static int safety = 10;
    
    public AI(Difficulty diff, List<int> nogo)
    {
        this.diff = diff;
        noGoList = nogo;
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
            case Difficulty.hard:
                int bestMove = -1;
                SimulateRound(marbleArray,noGoList,out bestMove);
                safety = 10;
                return bestMove;
            default:
                return -1;
        }
    }

    //Returns highest marble pool
    private int SimulateRound(int[] world, List<int> nogo, out int bestMove)
    {
        int highestScore = 0;
        bestMove = -1;
        for (int i = 8; i < 15; i++)
        {
            int newScore = SimulateMove(DuplicateWorld(world), nogo, i);
            if (newScore > highestScore)
            {
                highestScore = newScore;
                bestMove = i;
            }
        }
        return highestScore;
    }

    private int SimulateMove(int[] world, List<int> nogo, int move)
    {
        int virtualHand = world[move];
        world[move] = 0;
        int pointer = move;
        while (virtualHand > 0)
        {
            //Move hand forward
            pointer--;
            if (pointer < 0)
            {
                pointer = 14;
            }
            if (nogo.Contains(pointer))
            {
                pointer--;
                if (pointer < 15)
                {
                    pointer = 14;
                }
            }

            //Put marble
            world[pointer]++;
            virtualHand--;

            //If hand empty
            if (virtualHand == 0)
            {
                if (pointer == 7)
                {
                    //Simulate Turn
                    int unused = 0;
                    safety--;
                    if (safety > 0)
                    {
                        return SimulateRound(DuplicateWorld(world), nogo, out unused);
                    }
                } else if (world[pointer] == 1)
                {
                    //Check for bomb
                    if (pointer >= 8 && pointer <= 14)
                    {
                        //Bomb
                        world[7] += world[pointer];
                        world[7] += world[14 - pointer];
                    }
                    return world[7];
                } else
                {
                    //Resume
                    virtualHand = world[pointer];
                    world[pointer] = 0;
                }
            }
        }
        return world[7];
    }

    private int[] DuplicateWorld(int[] world)
    {
        int[] newWorld = new int[world.Length];
        for (int i = 0; i < world.Length; i++)
        {
            newWorld[i] = world[i];
        }
        return newWorld;
    }
}
