using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace MazeProject.Mazes
{
    /// <summary>
    /// 
    /// Random Maze generator. 
    /// 
    /// This script implements the Prim's Agorithm in C# to generate a
    /// random maze but a little bit interesting. The Algorithm creates 
    /// a graph's covering tree, meaning that there will always be a 
    /// way between every position. 
    /// 
    /// Description of the algorithm:
    /// 
    /// Firstly, it begins creating a matrix of walls, rooms ans pillars
    /// (rooms are the nodes of the graph). Every wall starts closed to 
    /// improve working time. Then a room is chosen as the begining of the
    /// path. 
    /// 
    /// Then, the algorithm does this things: 
    /// - Tries to extend the path through each surrounding wall of the room
    /// - Checks that the path can continue through each wall
    /// - Checks if the next room is already in the path, if not adds it
    /// - Repeat the process with every room and wall
    /// 
    /// This can be extent to every different maze generation. 
    /// 
    /// </summary>
    public class MazeGenerator
    {
        #region Coord
        /// <summary>
        /// 
        /// Stores maze coordinates.
        /// 
        /// </summary>
        struct Coord
        {
            public int y;
            public int x;

            /// <summary>
            /// 
            /// struct constructor. 
            /// 
            /// </summary>
            /// <param name="Y"> (int) New Y coordinate </param>
            /// <param name="X"> (int) New X coordinate </param>
            public Coord(int Y, int X)
            {
                x = X;
                y = Y;
            } // constructor
        } // struct
        #endregion

        #region Variables
        // Matrix of the maze's info
        private int[,] _map;

        // Values of the maze
        private int _height;
        private int _width;

        // Maze codification's legend:
        // 0 = Initial Room / Entrance
        // 1 = Room
        // 2 = Pillar
        // 3 = Closed Wall
        // 4 = Open Wall
        // 5 = Exit Wall
        // 6 = Enemy Spawn

        // Variables used for calculating the maze
        private int _roomNumber;
        private Coord _init;
        private List<Coord> _path = new List<Coord>();
        private Queue<Coord> _walls = new Queue<Coord>();

        // Room generator

        #endregion

        #region Utilities
        /// <summary>
        /// 
        /// Converts even number into odd.
        /// 
        /// </summary>
        /// <param name="num"> (int) Number to convert. </param>
        /// <param name="limit"> (int) Limit of the matrix. </param>
        private void MakeItOdd(ref int num, int limit)
        {
            int temp = num;

            if((temp + 1) == (limit - 1))
            {
                num--;
            } // if
            else
            {
                num++;
            } // else
        } // MakeItOdd

        /// <summary>
        /// 
        /// Method to create a blank maze: all walls are closed and 
        /// now there is a grid of closed walls and rooms. 
        /// 
        /// </summary>
        /// <param name="map"> (int[,]) Matrix to generate. </param>
        private void CreateBlankMaze()
        {
            _map = new int[_height, _width];

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if ((i % 2) == 0) // Pillar row
                    {
                        // Pillar
                        if ((j % 2) == 0)
                        {
                            _map[i, j] = 2;
                        } // if
                        // Closed Wall
                        else
                        {
                            _map[i, j] = 3;
                        } // else
                    } // if
                    else // room row
                    {
                        // Closed Wall
                        if ((j % 2) == 0)
                        {
                            _map[i, j] = 3;
                        } // if
                        // Room
                        else
                        {
                            _map[i, j] = 1;
                        } // else
                    } // else
                } // for
            } // for
        } // CreateBlankMaze

        /// <summary>
        /// 
        /// Method to check if a specific room is in the path.
        /// 
        /// </summary>
        /// <param name="room"> (Coord) Room to search. </param>
        /// <returns> (bool) Wether the room is or not in the path. </returns>
        private bool SearchRoomInPath(Coord room)
        {
            return _path.Contains(room);
        } // SearchRoomInPath
        #endregion

        #region Generation
        /// <summary>
        /// 
        /// Checks if the walls surrounding a room are open, if so, it means 
        /// that the room is connected with the path. 
        /// 
        /// </summary>
        /// <param name="room"> (Coord) Rooms coordinates. </param>
        /// <returns> (bool) Wether the room is connected or not. </returns>
        private bool IsRoomConnected(Coord room)
        {
            // Comprueba los muros que rodean la habitación uno por uno
            if (_map[room.y - 1, room.x] == 4)
            {
                return true;
            } // if
            else if (_map[room.y + 1, room.x] == 4)
            {
                return true;
            } // else if
            else if (_map[room.y, room.x - 1] == 4)
            {
                return true;
            } // else if
            else if (_map[room.y, room.x + 1] == 4)
            {
                return true;
            } // else if
            else
            {
                return false;
            } // else
        } // IsRoomConnected

        /// <summary>
        /// 
        /// Method to add the walls of a room to the exploration list. 
        /// 
        /// </summary>
        /// <param name="room"> (Coord) Room to explore. </param>
        private void AddWallsToList(Coord room)
        {
            // Comprobamos si un muro está abierto, si es así, no lo añadimos a la lista
            if (_map[room.y - 1, room.x] == 3 || _map[room.y - 1, room.x] == 4)
            {
                if (!_walls.Contains(new Coord(room.y - 1, room.x)) && (_map[room.y - 1, room.x] == 3))
                {
                    _walls.Enqueue(new Coord(room.y - 1, room.x));
                } // if 
            } // if 

            if (_map[room.y + 1, room.x] == 3 || _map[room.y + 1, room.x] == 4)
            {
                if (!_walls.Contains(new Coord(room.y + 1, room.x)) && (_map[room.y + 1, room.x] == 3))
                {
                    _walls.Enqueue(new Coord(room.y + 1, room.x));
                } // if 
            } // if 

            if (_map[room.y, room.x - 1] == 3 || _map[room.y, room.x - 1] == 4)
            {
                if (!_walls.Contains(new Coord(room.y, room.x - 1)) && (_map[room.y, room.x - 1] == 3))
                {
                    _walls.Enqueue(new Coord(room.y, room.x - 1));
                } // if
            } // if

            if (_map[room.y, room.x + 1] == 3 || _map[room.y, room.x + 1] == 4)
            {
                if (!_walls.Contains(new Coord(room.y, room.x + 1)) && (_map[room.y, room.x + 1] == 3))
                {
                    _walls.Enqueue(new Coord(room.y, room.x + 1));
                } // if
            } // if
        } // AddWallsToList

        bool SearchAdjacentRoomToWall(Coord wall)
        {
            int numRooms = 0;
            bool addedRoom = false;

            // Funcion
            if (((wall.y - 1) > 0) && _map[wall.y - 1, wall.x] == 1)
            {
                if (!SearchRoomInPath(new Coord(wall.y - 1, wall.x)))
                {
                    _path.Add(new Coord(wall.y - 1, wall.x));
                    AddWallsToList(new Coord(wall.y - 1, wall.x));
                    addedRoom = true;
                } // if
                numRooms++;
            } // if 

            if (((wall.y + 1) < _height) && _map[wall.y + 1, wall.x] == 1)
            {
                if (!SearchRoomInPath(new Coord(wall.y + 1, wall.x)))
                {
                    _path.Add(new Coord(wall.y + 1, wall.x));
                    AddWallsToList(new Coord(wall.y + 1, wall.x));
                    addedRoom = true;
                } // if
                numRooms++;
            } // if 

            if (((wall.x - 1) > 0) && _map[wall.y, wall.x - 1] == 1)
            {
                if (!SearchRoomInPath(new Coord(wall.y, wall.x - 1)))
                {
                    _path.Add(new Coord(wall.y, wall.x - 1));
                    AddWallsToList(new Coord(wall.y, wall.x - 1));
                    addedRoom = true;
                } // if
                numRooms++;
            } // if 

            if (((wall.x + 1) < _width) && _map[wall.y, wall.x + 1] == 1)
            {
                if (!SearchRoomInPath(new Coord(wall.y, wall.x + 1)))
                {
                    _path.Add(new Coord(wall.y, wall.x + 1));
                    AddWallsToList(new Coord(wall.y, wall.x + 1));
                    addedRoom = true;
                } // if
                numRooms++;
            } // if 

            return (addedRoom && (numRooms == 2));
        } // SearchAdjacentRoomToWall

        Coord RandomRoom()
        {
            System.Random randGen = new System.Random();
            Coord firstRoom = new Coord(1, 1);
            int y, x;

            y = randGen.Next(1, _height - 1);
            x = randGen.Next(1, _width - 1);

            // Check it is a room 
            if((y % 2) == 0)
            {
                MakeItOdd(ref y, _height);
            } // if
            if((x % 2) == 0)
            {
                MakeItOdd(ref x, _width);
            } // if

            firstRoom.y = y;
            firstRoom.x = x;

            return firstRoom;
        } // RandomRoom

        private void CreateMaze()
        {
            Coord firstRoom = RandomRoom();
            _path.Add(firstRoom);

            AddWallsToList(firstRoom);

            while(_walls.Count() > 0)
            {
                Coord wall = _walls.Dequeue();
                if (SearchAdjacentRoomToWall(wall))
                {
                    _map[wall.y, wall.x] = 4;
                } // if
            } // while
        } // CreateMaze
        #endregion

        #region AfterGeneration

        private void EstablishInit()
        {
            _init = RandomRoom();

            _map[_init.y, _init.x] = 0;
        } // EstablishInit

        /// <summary>
        /// 
        /// Method to establish the exit of the maze. This will be done 
        /// trying to put it as far away from the beginning as possible.
        /// 
        /// To achieve this, the maze is divided in quadrants, then depending
        /// on which quadrant is it the exit will be established in one of
        /// the other three- 
        /// 
        /// </summary>
        private void EstablishExit()
        {
            System.Random rnd = new System.Random();

            int Y = 0;

            // Upper half
            if (_init.y < _height / 2)
            {
                Y = rnd.Next(_height / 2, _height);
            } // if
            else
            {
                Y = rnd.Next(1, _height / 2);
            } // else

            if ((Y % 2) == 0)
            {
                MakeItOdd(ref Y, _height);
            } // if

            // Left half of the maze
            if (_init.x < _width / 2)
            {
                _map[Y, _width - 1] = 5;
            } // if
            else
            {
                _map[Y, 0] = 5;
            } // else
        } // EstablishExit

        #endregion

        #region SaveMaze
        /// <summary>
        /// 
        /// Method to save the maze into a .txt file, used for 
        /// debugging and exporting the maze to other project.
        /// 
        /// </summary>
        /// <param name="counter"> (int) Counter of mazes generated. </param>
        /// <param name="session"> (int) Counter of the sessions. </param>
        void SaveMaze(int counter, int session)
        {
            //Atributes
            StreamWriter mapWriter;
            string directory = "Session_" + session;
            string file = "Maze_" + counter + ".txt";
            string upperDirectory = @"Assets/Mazes";
            string pathSaver = System.IO.Path.Combine(upperDirectory, directory);

            //Function
            if (!Directory.Exists(pathSaver))
            {
                System.IO.Directory.CreateDirectory(pathSaver);
            } // if 

            pathSaver = System.IO.Path.Combine(pathSaver, file);

            File.WriteAllText(pathSaver, "Maze's Number: " + counter);

            mapWriter = File.AppendText(pathSaver);

            if (_map != null)
            {
                mapWriter.WriteLine(_height);
                mapWriter.WriteLine(_width);

                for (int i = 0; i < _height; i++)
                {
                    for (int j = 0; j < _width; j++)
                    {
                        mapWriter.Write(_map[i, j]);
                    } // for 
                    mapWriter.WriteLine();
                } // for
            } // if

            mapWriter.Close();
        } // SalvarLaberinto
        #endregion

        public int[,] GenMaze(int height, int width, bool rooms, int roomSize)
        {
            if(height%2 == 0 || width%2 == 0)
            {
                return null;
            } // if

            _height = height;
            _width = width;

            _map = new int[_height, _width];
            CreateBlankMaze();
            CreateMaze();

            if (rooms)
            {

            }

            EstablishInit();
            EstablishExit();

            return _map;
        } // GenMaze
    } // class MazeGenerator
} // namespace
