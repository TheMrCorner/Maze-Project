/**
 * Random Maze Generator. 
 * 
 * After compilation this will generate a .dll to include in the project. 
 * 
 * This is an implementation of Prim's algorithm in C# to generate a random but
 * interest maze. 
 * This works like this: 
 * 
 * Beginning with a matrix of walls, rooms and pillars. Each wall begins closed to
 * make this faster. Then, a room is selected to begin the path that will solve the
 * maze. (This is good because doesn't matter which room is selected, there will 
 * always be a path to solve the maze.) 
 * Then, begin generating the maze this way: 
 * - Try to make a path through each of the walls surrounding the initial room. 
 * - Check if the path can continue that way
 * - Then check if the next room is in the path and add it to the path
 * - Repeat with each room of the maze and every wall
 * 
 * After that, save that maze with a specific codification in a .txt file. 
 * 
 * (This can be extended to generate mazes and use them in any way. At this moment
 * only works with a specific codification and a specific project.) 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LaberynthGenerator
{
    public class MazeGenerator
    {
        /**
         * Struct to store the coordinates of the different cells of the maze. 
         */
        struct Coord
        {
            /**
             * Internal value of the coordinates. Accesible from outside to
             * make things easier. 
             */
            public int y;
            public int x;

            /**
             * Constructor of the struct. Receives the position. 
             * @param newY Type: int 
             * @param newX Type: int
             */
            public Coord(int newY, int newX)
            {
                x = newX;
                y = newY;
            }
        }

        //-------------- VARIABLES -----------------
        // Matrix with the information of the maze
        static int[,] map;

        // Values of the maze
        static int generalHeight;  
        static int generalWidth; 

        //Legend to understand the map:
        //  1 = empty room
        //  2 = Pillar
        //  3 = ClosedWall
        //  4 = OpenWall

        // For generation of the maze, all used for calculations
        static int numOfRooms;
        static List<Coord> path = new List<Coord>(); // Saves the path that solves the maze
        static List<Coord> wall_list = new List<Coord>(); 
           
        /**
         * This method creates a Blank Maze. This means, all walls 
         * are closed and there is no possible path because the 
         * graph is not connected. 
         */
        static int[,] CreateBlankLabyrinth()
        {
            // Atributes
            int[,] tempMap = new int[generalHeight, generalWidth];

            // Function
            for (int i = 0; i < generalHeight; i++)
            {
                for (int j = 0; j < generalWidth; j++)
                {
                    // Need to difference between a line with rooms and
                    // a line with pillars.

                    if ((i % 2) == 0) //Line with pillars
                    {
                        // Wee need to alternate between walls and pillars
                        if ((j % 2) == 0) // pair position, pillar
                        {
                            tempMap[i, j] = 2;
                        } // if
                        else 
                        {
                            tempMap[i, j] = 3;
                        } // else
                    } // if
                    else //Line with rooms
                    { 
                        // Alternate between walls and rooms
                        if ((j % 2) == 0)
                        {
                            tempMap[i, j] = 3;
                        } // if
                        else
                        {
                            tempMap[i, j] = 1;
                        } // else
                    } // else
                } // for
            } // for

            return tempMap;
        } // CreateBlankLabyrinth()

        /**
         * Method that checks if a room is in the path of the maze. 
         * 
         * @return True if room is in the path, false any other situation.
         */
        static bool SearchRoomPath(Coord room)
        {   // TODO: Could do better with a BinarySearch (FUTURE CHANGE)
            // Atributes
            bool isInPath = false;
            int i = 0; // Used to iterate 

            // Function
            while (i < path.Count() && !isInPath)
            {
                if ((path[i].x == room.x) && (path[i].y == room.y))
                {
                    isInPath = true;
                } // if
                i++;
            } // while

            return isInPath;
        } // SearchRoomPath

        /**
         * Check if the walls around the room are open. If any 
         * of the walls is open, return true because the room 
         * is connected
         * 
         * @param Coord room Coordinates of the room to be checked.
         * 
         * @return true when the room is connected, false when it is not
         */
        static bool IsRoomConnected(Coord room)
        {
            // TODO: This should be reviewed, clean it
            if (map[room.y - 1, room.x] == 4)
            {
                return true;
            } // if

            else if (map[room.y + 1, room.x] == 4)
            {
                return true;
            } // else if

            else if (map[room.y, room.x - 1] == 4)
            {
                return true;
            } // else if

            else if (map[room.y, room.x + 1] == 4)
            {
                return true;
            } // else if

            else
            {
                return false;
            } // else
        } // IsRoomConnected

        /**
         * Method that adds a wall to the wall list. This is used to check 
         * walls that are open and walls that need to be checked. 
         */
        static void AddWallsToList(Coord room)
        {   
            // TODO: try to clean this, create a method to check if list contains a wall or not
            // If wall is open, don't add it; if there is
            if (map[room.y - 1, room.x] == 3 || map[room.y - 1, room.x] == 4)
            {
                // This can be cleaner
                if (!wall_list.Contains(new Coord(room.y - 1, room.x)) && (map[room.y - 1, room.x] == 3))
                {
                    wall_list.Add(new Coord(room.y - 1, room.x));
                } // if 
            } // if 

            if (map[room.y + 1, room.x] == 3 || map[room.y + 1, room.x] == 4)
            {
                if (!wall_list.Contains(new Coord(room.y + 1, room.x)) && (map[room.y + 1, room.x] == 3))
                {
                    wall_list.Add(new Coord(room.y + 1, room.x));
                } // if 
            } // if 

            if (map[room.y, room.x - 1] == 3 || map[room.y, room.x - 1] == 4)
            {
                if (!wall_list.Contains(new Coord(room.y, room.x - 1)) && (map[room.y, room.x - 1] == 3))
                {
                    wall_list.Add(new Coord(room.y, room.x - 1));
                } // if
            } // if

            if (map[room.y, room.x + 1] == 3 || map[room.y, room.x + 1] == 4)
            {
                if (!wall_list.Contains(new Coord(room.y, room.x + 1)) && (map[room.y, room.x + 1] == 3))
                {
                    wall_list.Add(new Coord(room.y, room.x + 1));
                } // if
            } // if
        } // AddWallsToList

        /**
         * Selects a random room between all possible rooms in the maze. 
         * 
         * @return Coord A cell of the maze that is a room. 
         */
        static Coord RandomRoom() //This only works for Rooms
        {
            // TODO: again, clean this 
            // Atributes
            System.Random rndNumber = new System.Random();
            Coord firstRoom = new Coord(1, 1); // First room is always (1, 1) 
            int y, x;
            bool finish = false;

            // Function
            while (!finish)
            {
                // Generate random coordinates
                y = rndNumber.Next(1, generalHeight);
                x = rndNumber.Next(1, generalWidth);

                if (y >= 1 && x >= 1) //Rooms are always in odd numbers coords
                {
                    if ((y % 2) == 0) //No pair numbers, change it to odd
                    {
                        if (y == generalHeight - 1) //We can't take generalHeight as Room
                        {
                            y--;
                        } // if
                        else
                        {
                            y++;
                        } // else

                        firstRoom.y = y;
                    } // if
                    else
                    {
                        firstRoom.y = y;
                    } // else 

                    if ((x % 2) == 0) //Same for X
                    {
                        if (x == generalWidth - 1) //We can't take generalWidth as Room
                        {
                            x--;
                        } // if
                        else
                        {
                            x++;
                        } // else

                        firstRoom.x = x;
                    } // if 
                    else
                    {
                        firstRoom.x = x;
                    } // else

                    finish = true;
                } // if 
            } // while

            return firstRoom;
        } // RandomRoom

        /**
         * Method that searches an adjacent room to a wall. 
         * 
         * CLEAN THIS LOL
         */
        static bool SearchAdjacentRoomToWall(Coord wall)
        {
            // TODO: clean this method, repeated code
            // Atributes
            int numOfRooms = 0;
            bool addedRoom = false;

            // Function
            if (((wall.y - 1) > 0) && map[wall.y - 1, wall.x] == 1)
            {
                if (!SearchRoomPath(new Coord(wall.y - 1, wall.x)))
                {
                    path.Add(new Coord(wall.y - 1, wall.x));
                    AddWallsToList(new Coord(wall.y - 1, wall.x));
                    addedRoom = true;
                } // if
                numOfRooms++;
            } // if 

            if (((wall.y + 1) < generalHeight) && map[wall.y + 1, wall.x] == 1)
            {
                if (!SearchRoomPath(new Coord(wall.y + 1, wall.x)))
                {
                    path.Add(new Coord(wall.y + 1, wall.x));
                    AddWallsToList(new Coord(wall.y + 1, wall.x));
                    addedRoom = true;
                } // if
                numOfRooms++;
            } // if 

            if (((wall.x - 1) > 0) && map[wall.y, wall.x - 1] == 1)
            {
                if (!SearchRoomPath(new Coord(wall.y, wall.x - 1)))
                {
                    path.Add(new Coord(wall.y, wall.x - 1));
                    AddWallsToList(new Coord(wall.y, wall.x - 1));
                    addedRoom = true;
                } // if
                numOfRooms++;
            } // if 

            if (((wall.x + 1) < generalWidth) && map[wall.y, wall.x + 1] == 1)
            {
                if (!SearchRoomPath(new Coord(wall.y, wall.x + 1)))
                {
                    path.Add(new Coord(wall.y, wall.x + 1));
                    AddWallsToList(new Coord(wall.y, wall.x + 1));
                    addedRoom = true;
                } // if
                numOfRooms++;
            } // if 

            return (addedRoom && numOfRooms == 2);
        } // SearchAdjacentRoomToWall

        /**
         * Method that creates the maze implementing the Prim's algorithm 
         */
        static void CreateLabyrinth()
        {
            // Atributes
            //First Room in the path, random 
            Coord firstRoom = RandomRoom();
            path.Add(firstRoom); //Begin the path
            int i = 0;

            // Function
            AddWallsToList(firstRoom);

            while (wall_list.Count() > 0)
            {
                if (SearchAdjacentRoomToWall(wall_list[i]))
                {
                    map[wall_list[i].y, wall_list[i].x] = 4;
                    wall_list.Remove(wall_list[i]);
                } // if
                else
                {
                    wall_list.Remove(wall_list[i]);
                } // else

                if (i < wall_list.Count - 1)
                {
                    i++;
                } // if
                else
                {
                    i = 0;
                } // else
            } // while 
        } // CreateLabyrinth


        //----------------- Methods to save and paint -------------------

        /**
         * Method that saves the generated maze to a .txt file to be read later. 
         * 
         * TODO: Investigar si se puede hacer con JSON y mejorar un poco este sistema
         */
        static void SaveLabyrinth(int count, int game) //Saves the mazes temporarly into a folder called Labyrinths
        {
            //Atributes
            StreamWriter mapWriter;
            string folderName = "Game " + game;
            string fileName = "Labyrinth" + count + ".txt";
            string topFolderName = @"Assets/Labyrinths";
            string pathSaver = System.IO.Path.Combine(topFolderName, folderName);

            //Function
            if (!Directory.Exists(pathSaver))
            {
                System.IO.Directory.CreateDirectory(pathSaver);
            } // if 

            pathSaver = System.IO.Path.Combine(pathSaver, fileName);

            File.WriteAllText(pathSaver, "Number of Labyrinth: " + count);

            mapWriter = File.AppendText(pathSaver);

            if (map != null)
            {
                mapWriter.WriteLine(generalHeight);
                mapWriter.WriteLine(generalWidth);

                for (int i = 0; i < generalHeight; i++)
                {
                    for (int j = 0; j < generalWidth; j++)
                    {
                        mapWriter.Write(map[i, j]);
                    } // for 
                    mapWriter.WriteLine();
                } // for
            } // if

            mapWriter.Close();
        } // SaveLabyrinth

        /**
         * Method that paints the maze in the console. Used for debugging only. 
         * 
         * TODO: ahora mismo no se puede usar, revisar como hacerlo de alguna manera
         */
        static void PaintLabyrinth(int count)
        {
            // Atributes
            StreamReader readerLab = new StreamReader("Labyrinth" + count + ".txt");
            string lane;
            char[] laneChar; 

            // Function
            for (int i = 0; i < generalHeight; i++)
            {
                lane = readerLab.ReadLine();

                laneChar = lane.ToCharArray();

                for (int j = 0; j < lane.Length; j++)
                {
                    if (laneChar[j] == '1')
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Write("OO");
                        Console.BackgroundColor = ConsoleColor.Blue;
                    } // if
                    else if (laneChar[j] == '2')
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("PP");
                        Console.BackgroundColor = ConsoleColor.Blue;
                    } // else if
                    else if (laneChar[j] == '3')
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("PP");
                        Console.BackgroundColor = ConsoleColor.Blue;
                    } // else if 

                    else if (laneChar[j] == '4')
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Write("OO");
                        Console.BackgroundColor = ConsoleColor.Blue;
                    } // else if
                } // for
                Console.WriteLine();
            } // for

            readerLab.Close();
        } // PaintLabyrinth

        /**
         * Method used to generate a Maze with some specific parameters defined
         * when the method is called. 
         * 
         * @param int Game number
         * @param int number of the maze (used for a game)
         * @param int height of the maze
         * @param int width of the maze
         * 
         * @return True when maze is generated. 
         */
        public bool GenLab(int game, int numLab, int height, int width)
        {
            // TODO: Revisar esto y poner errores y demás.
            generalHeight = height;
            generalWidth = width;
            int count = numLab;
            int gameNum = game;

            map = new int[generalHeight, generalWidth];
            map = CreateBlankLabyrinth();
            CreateLabyrinth();

            SaveLabyrinth(count, gameNum);

            return true;
        } //GenLab
    } // class MazeGenerator
} // namespace LabyrinthGenerator
