using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Game
{
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int P = int.Parse(inputs[0]); // number of players in the game (2 to 4 players)
        int ID = int.Parse(inputs[1]); // ID of your player (0, 1, 2, or 3)
        int D = int.Parse(inputs[2]); // number of drones in each team (3 to 11)
        int Z = int.Parse(inputs[3]); // number of zones on the map (4 to 8)

        //Zones
        int[][] zones = new int[Z][];
        for (int i = 0; i < Z; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int X = int.Parse(inputs[0]); // corresponds to the position of the center of a zone. A zone is a circle with a radius of 100 units.
            int Y = int.Parse(inputs[1]);
            
            zones[i] = new int[3] { X, Y, 0 };
        }

        //Zone HQ & Priority
        int HQ = Calcs.ZoneHQ(zones);
        int[] priority = new int[Z];
        priority = Calcs.ZonePriority(HQ, zones);

        // Zone Distance Array
        double[] zoneDist = new double[Z];
        for (int i = 0; i < Z; i++)
        {
            zoneDist[i] = Calcs.Distance(zones[HQ][0], zones[HQ][1], zones[i][0], zones[i][1]);
        }


        //Drones
        Drone[][] drones = new Drone[P][];
        for (int i = 0; i < P; i++)
        {
            drones[i] = new Drone[D];

            for (int j = 0; j < D; j++)
            {
                drones[i][j] = new Drone();
            }

        }

        //Drone distances from HQ
        double[][] droneDist = new double[P][];
        for (int i = 0; i < P; i++)
        {
            droneDist[i] = new double[D];
        }

        
        // game loop
        while (true)
        {
            // List of who controls each zone by team ID
            for (int i = 0; i < Z; i++)
            {
                int TID = int.Parse(Console.ReadLine());
                zones[i][2] = TID;
            }

            // Drone info for turn & update distances
            for (int i = 0; i < P; i++)
            {
                for (int j = 0; j < D; j++)
                {
                    inputs = Console.ReadLine().Split(' ');
                    int DX = int.Parse(inputs[0]); 
                    int DY = int.Parse(inputs[1]);

                    drones[i][j].Update(DX, DY);
                    droneDist[i][j] = Calcs.Distance(zones[HQ][0], zones[HQ][1], DX, DY);
                }

                Array.Sort(droneDist[i]);

                //Debug.DisplayDoubleArray(droneDist[i]);

            }


            // OUTPUT
            for (int i = 0; i < D; i++)
            {
                double targetDist = zoneDist[priority[priority.Length - 1]];
                int targetZone = 0;

                // Check distance
                for (int j = 0; j < P; j++)
                {
                    if (droneDist[j][i] < targetDist && j != ID)
                    {
                        targetDist = droneDist[j][i];
                    }
                }

                // Check for zone in distance
                for (int j = 0; j < Z; j++)
                {
                    if (zoneDist[priority[j]] > targetDist)
                    {
                        break;
                    }

                    if (zones[priority[Math.Max(j - 1, 0)]][2] == ID)
                    {
                        targetZone = priority[j];
                    }
                    else
                    {
                        targetZone = priority[Math.Max(j - 1, 0)];
                    }
                    
                }


                Console.WriteLine(zones[targetZone][0]+" "+ zones[targetZone][1]);
            }
        }
    }
}


class Drone
{
    int x = 0;
    int xLast;
    int y = 0;
    int yLast;

    // Update info for turn
    public void Update(int X, int Y)
    {
        xLast = x;
        yLast = y;

        x = X;
        y = Y;
    }

    
    public int GetX()
    {
        return x;
    }


    public int GetY()
    {
        return y;
    }


}


class Calcs
{

    // Calculate distance
    public static double Distance(int X1, int Y1, int X2, int Y2)
    {
        double xDiff = (double)Math.Abs(X1 - X2);
        double yDiff = (double)Math.Abs(Y1 - Y2);

        double output = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));

        return output;
    }


    //Calculate Zone HQ (Closest to a corner)
    public static int ZoneHQ(int[][] zones)
    {
        int output = 0;
        double dist = 100000;

        for (int i = 0; i < zones.Length; i++)
        {

            //Check distance to each corner
            if (dist > Distance(zones[i][0],zones[i][1],0,0))
            {
                dist = Distance(zones[i][0], zones[i][1], 0, 0);
                output = i;
            }

            if (dist > Distance(zones[i][0], zones[i][1], 0, 1800))
            {
                dist = Distance(zones[i][0], zones[i][1], 0, 1800);
                output = i;
            }

            if (dist > Distance(zones[i][0], zones[i][1], 4000, 0))
            {
                dist = Distance(zones[i][0], zones[i][1], 4000, 0);
                output = i;
            }

            if (dist > Distance(zones[i][0], zones[i][1], 4000, 1800))
            {
                dist = Distance(zones[i][0], zones[i][1], 4000, 1800);
                output = i;
            }
        }

        return output;
    }



    // Priority of zones based on distance to HQ
    public static int[] ZonePriority(int HQ, int[][] zones)
    {
        // intialize output
        int[] output = new int[zones.Length];
        for (int i = 0; i < zones.Length; i++)
        {
            output[i] = 0;
        }

        // Distance array
        double[] dists = new double[zones.Length];
        for (int i = 0; i < zones.Length; i++)
        {
            dists[i] = Distance(zones[HQ][0], zones[HQ][1], zones[i][0], zones[i][1]);
        }


        // Make priority list
        for (int i = 0; i < zones.Length; i++)
        {
            double distance = 100000;

            for (int j = 0; j < zones.Length; j++)
            {

                if (dists[j] < distance && dists[j] >= 0)
                {
                    output[i] = j;
                    distance = dists[j];
                }

            }

            dists[output[i]] = -1;

        }

        //Debug.DisplayIntArray(output);

        return output;
    }



}


class Debug
{

    public static void DisplayIntArray(int[] array)
    {
        Console.Error.WriteLine("IntArray:");
        for (int i = 0; i < array.Length; i++)
        {
            Console.Error.WriteLine(array[i]);
        }
    }


    public static void DisplayDoubleArray(double[] array)
    {
        Console.Error.WriteLine("DoubleArray:");
        for (int i = 0; i < array.Length; i++)
        {
            Console.Error.WriteLine(array[i]);
        }
    }

}