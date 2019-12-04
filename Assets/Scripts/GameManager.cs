using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using mazedll;

public class GameManager : MonoBehaviour
{
    //Publics 
    public static GameManager instance;

    //Privates
    int numLab = 0;
    int genHeight = 27;
    int genWwidth = 27;
    int game = 0;
    MazeGen mg;

    //Current game
    int currHeight = 0;
    int currWidth = 0;

    int [,] currMaze;

    // Start is called before the first frame update
    void Start()
    {
        mg = new MazeGen();
        bool generatedLab = false;
        instance = this;

        generatedLab = mg.GenLab(game, numLab, genHeight, genWwidth);

        if (generatedLab)
        {
            Debug.Log("Se ha generado un laberinto, número: " + numLab);
            numLab++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            currMaze = mg.ReadMaze(numLab - 1, game, ref currHeight, ref currWidth);
        }
    }
    
    private void OnApplicationQuit()
    {
        string[] filesToDelete;
        int files = 0;

        for (int i = 0; i < numLab; i++)
        {
            filesToDelete = Directory.GetFiles(@"Assets/Labyrinths/Game " + i);

            while (files < filesToDelete.Length)
            {
                File.Delete(filesToDelete[files]);
                files++;
            }
            
            Directory.Delete(@"Assets/Labyrinths/Game " + i);
            File.Delete(@"Assets/Labyrinths/Game " + i + ".meta");
            Debug.Log("Carpeta Eliminada");

            files = 0;
        }
    }
}
