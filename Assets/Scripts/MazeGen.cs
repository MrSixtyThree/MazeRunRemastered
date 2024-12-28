using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.AI.Navigation;
//using Unity.PlasticSCM.Editor.WebApi;
//using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.AI;
//using UnityEngine.Experimental.GlobalIllumination;
//using UnityEngine.UIElements;
//using static UnityEditor.PlayerSettings;
//using static UnityEngine.GraphicsBuffer;
using LightType = UnityEngine.LightType;

[System.Serializable]
public struct RoomCell
{
    //Allows us to ID the cell
    public int rowID;
    public int columnID;

    //Checks if mazeGen program has checked this cell before
    public bool checkedTracker;

    //Allows us to make steps backwards
    public int previousRowID;
    public int previousColumnID;

    //Determines if walls exist on render
    public bool southWall;
    public bool eastWall;

    public RoomCell(int r, int c, bool t, int pr, int pc, bool s, bool e)
    {
        this.rowID = r;
        this.columnID = c;
        this.checkedTracker = t;
        this.previousRowID = pr;
        this.previousColumnID = pc;
        this.southWall = s;
        this.eastWall = e;
    }
}

public struct coordinates
{
    public int x;
    public int y;
}

public class MazeGen : MonoBehaviour
{
    [SerializeField] NavMeshSurface surface;
    [SerializeField] GameObject MainCamera;
    [SerializeField] GameObject firstFloor;
    [SerializeField] GameObject playerObject;
    [SerializeField] GameObject mazeObject;
    [SerializeField] GameObject entitiesObject;
    [SerializeField] Material blue;
    [SerializeField] Material red;
    [SerializeField] Material yellow;
    [SerializeField] Material green;
    [SerializeField] Material artifact;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstacleMask1;
    [SerializeField] LayerMask obstacleMask2;
    [SerializeField] LayerMask allyMask;
    RoomCell[,] grid;
    public int MazeHeight;
    int MazeWidth;
    bool flipFlop = false;
    float cellSize = 2.5f;
    List<coordinates> duplicatePrevention = new List<coordinates>();
    Weapon.WeaponType selectedWeapon = Weapon.WeaponType.Pistol;

    private void Start()
    {
        mazeObject = GameObject.FindGameObjectWithTag("Maze");
        entitiesObject = GameObject.FindGameObjectWithTag("Entities");
    }

    public void clearMaze()
    {
        DestroyImmediate(entitiesObject);
        DestroyImmediate(mazeObject);
        mazeObject = new GameObject();
        mazeObject.tag = "Maze";
        mazeObject.name = "Maze";
        entitiesObject = new GameObject();
        entitiesObject.tag = "Entities";
        entitiesObject.name = "Entities";
    }

    void populate()
    {
        for (int i = 0; i < MazeHeight; i++)
        {
            for (int j = 0; j < MazeWidth; j++)
            {
                RoomCell temp = new RoomCell(i, j, false, 0, 0, true, true);
                grid[i, j] = temp;
            }
        }
    }

    int Check(int i, int j)
    {

        List<int> validDirections = new List<int>();

        if (i + 1 < MazeHeight && !grid[i + 1, j].checkedTracker)
        {
            validDirections.Add(1);
        }
        if (i - 1 >= 0 && !grid[i - 1, j].checkedTracker)
        {
            validDirections.Add(2);
        }
        if (j + 1 < MazeWidth && !grid[i, j + 1].checkedTracker)
        {
            validDirections.Add(3);
        }
        if (j - 1 >= 0 && !grid[i, j - 1].checkedTracker)
        {
            validDirections.Add(4);
        }
        if (validDirections.Count == 0)
        {
            return 0;
        }
        else
        {
            int randomIndex = Random.Range(0, validDirections.Count);
            return validDirections[randomIndex];
        }

    }

    void DebugCheck(RoomCell check)
    {
        string str = "ID: ";
        str += check.rowID.ToString();
        str += ".";
        str += check.columnID.ToString();
        str += "___";
        if(check.checkedTracker)
        {
            str += "CHECKED";
        }
        else
        {
            str += "NOT CHECKED";
        }
        str += "___";
        if (check.southWall)
        {
            if (check.eastWall)
            {
                str += "Walls: South and East";
            }
            else
            {
                str += "Walls: South Only";
            }
        }
        else
        {
            if (check.eastWall)
            {
                str += "Walls: East Only";
            }
            else
            {
                str += "Walls: None";
            }
        }
        str += "___";
        str += "Previous ID: ";
        str += check.previousRowID.ToString();
        str += ".";
        str += check.previousColumnID.ToString();
        str += "___";
        Debug.Log(str);
    }

    void mazeLogic(string difficulty)
    {
        int cellCount = MazeWidth * MazeHeight;
        int checkedCells = 0;
        int whileLoops = 0;
        int row = 0;
        int column = 0;
        int tempRow = row;
        int tempColumn = column;
        grid[0, 0].previousRowID = -1;
        grid[0, 0].previousColumnID = -1;

        while (checkedCells < cellCount)
        {
            whileLoops++;
            //DebugCheck(grid[row, column]);
            int direction = Check(row, column);
            //bool localCheck = true;

            if (!grid[row, column].checkedTracker)
            {
                //localCheck = false;
                grid[row, column].checkedTracker = true;
                checkedCells++;
            }

            // IF we get back 0, we hit a dead end, backtrack and try again.
            if (direction == 0)
            {
                if (grid[row, column].previousRowID == -1 || grid[row, column].previousColumnID == -1)
                {
                    //break;  // We are at the start, no way to backtrack further
                }
                tempRow = row;
                tempColumn = column;
                row = grid[tempRow, tempColumn].previousRowID;
                column = grid[tempRow, tempColumn].previousColumnID;
            }
            //Otherwise, remove the relevant wall, mark as checked, track total walls checked and set up for next interation. 
            else if (direction == 1)
            {
                tempRow = row;
                tempColumn = column;

                grid[row, column].southWall = false;
                row++;
                grid[row, column].previousRowID = tempRow;
                grid[row, column].previousColumnID = tempColumn;
            }
            else if (direction == 2)
            {
                tempRow = row;
                tempColumn = column;

                grid[row - 1, column].southWall = false;
                row--;
                grid[row, column].previousRowID = tempRow;
                grid[row, column].previousColumnID = tempColumn;
            }
            else if (direction == 3)
            {
                tempRow = row;
                tempColumn = column;

                grid[row, column].eastWall = false;
                column++;
                grid[row, column].previousRowID = tempRow;
                grid[row, column].previousColumnID = tempColumn;
            }
            else if (direction == 4)
            {
                tempRow = row;
                tempColumn = column;

                grid[row, column - 1].eastWall = false;
                column--;
                grid[row, column].previousRowID = tempRow;
                grid[row, column].previousColumnID = tempColumn;
            }

            if (whileLoops > (MazeHeight * MazeWidth) * 1000)
            {
                Debug.Log("ERROR");
                break;
            }
        }
        if (difficulty == "EASY")
        {
            grid[1, 1].southWall = false;
            grid[3, 1].eastWall = false;
            grid[2, 3].southWall = false;
            grid[1, 2].eastWall = false;
        }
        if (difficulty == "MEDIUM")
        {
            grid[2, 2].southWall = false;
            grid[4, 2].eastWall = false;
            grid[3, 4].southWall = false;
            grid[2, 3].eastWall = false;

            int chance = Random.Range(1, 3);

            if(chance == 1)
            {
                grid[0, 3].southWall = false;
                grid[5, 3].southWall = false;
                grid[3, 0].eastWall = false;
                grid[3, 5].eastWall = false;
            }
            else
            {
                grid[0, 0].eastWall = false;
                grid[0, 6].southWall = false;
                grid[6, 5].eastWall = false;
                grid[5, 0].southWall = false;
            }
        }
        if (difficulty == "HARD")
        {
            int randomRow = Random.Range(1, 6) + 2;
            int randomColumn = Random.Range(1, 6) + 2;

            grid[randomRow, randomColumn].southWall = false;
            grid[randomRow, randomColumn].eastWall = false;
            grid[randomRow - 1, randomColumn].southWall = false;
            grid[randomRow, randomColumn - 1].eastWall = false;

            grid[0, 4].southWall = false;
            grid[7, 4].southWall = false;
            grid[4, 0].eastWall = false;
            grid[4, 7].eastWall = false;

            grid[7, 6].eastWall = false;
            grid[6, 1].southWall = false;
            grid[1, 1].eastWall = false;
            grid[1, 7].southWall = false;

            int chance = Random.Range(1, 3);

            if(chance == 1)
            {
                grid[2, 1].eastWall = false;
                grid[2, 6].eastWall = false;
                grid[6, 6].eastWall = false;
                grid[6, 1].eastWall = false;
            }
            else
            {
                grid[1, 2].southWall = false;
                grid[1, 2].southWall = false;
                grid[6, 2].southWall = false;
                grid[6, 6].southWall = false;
            }
        }
        if (difficulty == "DEADLY")
        {
            int chance = Random.Range(1, 3);

            if (chance == 1)
            {
                grid[0, 0].eastWall = false;
                grid[0, 5].southWall = false;
                grid[5, 4].eastWall = false;
                grid[4, 0].southWall = false;
            }
            else
            {
                grid[0, 0].southWall = false;
                grid[5, 0].eastWall = false;
                grid[4, 5].southWall = false;
                grid[0, 4].eastWall = false;
            }

            chance = Random.Range(1, 3);

            if (chance == 1)
            {
                grid[0, 6].eastWall = false;
                grid[0, 11].southWall = false;
                grid[5, 10].eastWall = false;
                grid[4, 6].southWall = false;
            }
            else
            {
                grid[0, 6].southWall = false;
                grid[5, 6].eastWall = false;
                grid[4, 11].southWall = false;
                grid[0, 10].eastWall = false;
            }

            chance = Random.Range(1, 3);

            if (chance == 1)
            {
                grid[6, 0].eastWall = false;
                grid[6, 5].southWall = false;
                grid[11, 4].eastWall = false;
                grid[10, 0].southWall = false;
            }
            else
            {
                grid[6, 0].southWall = false;
                grid[11, 0].eastWall = false;
                grid[10, 5].southWall = false;
                grid[6, 4].eastWall = false;
            }

            chance = Random.Range(1, 3);

            if (chance == 1)
            {
                grid[6, 6].eastWall = false;
                grid[6, 11].southWall = false;
                grid[11, 10].eastWall = false;
                grid[10, 6].southWall = false;
            }
            else
            {
                grid[6, 6].southWall = false;
                grid[11, 6].eastWall = false;
                grid[10, 11].southWall = false;
                grid[6, 10].eastWall = false;
            }

            chance = Random.Range(1, 3);

            if (chance == 1)
            {
                grid[5, 5].eastWall = false;
                grid[6, 5].eastWall = false;
            }
            else
            {
                grid[5, 5].southWall = false;
                grid[5, 6].southWall = false;
            }

            grid[5, 9].southWall = false;
            grid[9, 5].eastWall = false;
            grid[5, 2].southWall = false;
            grid[2, 5].eastWall = false;
        }
        if (difficulty == "IMPOSSIBLE")
        {
            int chance = Random.Range(1, 3);


            //TOP LEFT
            if (chance == 1)
            {
                grid[0, 0].eastWall = false;
                grid[0, 5].southWall = false;
                grid[5, 4].eastWall = false;
                grid[4, 0].southWall = false;
            }
            else
            {
                grid[0, 0].southWall = false;
                grid[5, 0].eastWall = false;
                grid[4, 5].southWall = false;
                grid[0, 4].eastWall = false;
            }

            chance = Random.Range(1, 3);

            //TOP MIDDLE
            if (chance == 1)
            {
                grid[0, 7].eastWall = false;
                grid[0, 12].southWall = false;
                grid[5, 11].eastWall = false;
                grid[4, 7].southWall = false;
            }
            else
            {
                grid[0, 7].southWall = false;
                grid[5, 7].eastWall = false;
                grid[4, 12].southWall = false;
                grid[0, 11].eastWall = false;
            }

            chance = Random.Range(1, 3);

            //TOP RIGHT
            if (chance == 1)
            {
                grid[0, 14].eastWall = false;
                grid[0, 19].southWall = false;
                grid[5, 18].eastWall = false;
                grid[4, 14].southWall = false;
            }
            else
            {
                grid[0, 7].southWall = false;
                grid[5, 7].eastWall = false;
                grid[4, 12].southWall = false;
                grid[0, 11].eastWall = false;
            }

            chance = Random.Range(1, 3);


            //MID LEFT
            if (chance == 1)
            {
                grid[7, 0].eastWall = false;
                grid[7, 5].southWall = false;
                grid[12, 4].eastWall = false;
                grid[11, 0].southWall = false;
            }
            else
            {
                grid[7, 0].southWall = false;
                grid[12, 0].eastWall = false;
                grid[11, 5].southWall = false;
                grid[7, 4].eastWall = false;
            }

            chance = Random.Range(1, 3);

            //MIDDLE
            if (chance == 1)
            {
                grid[7, 7].eastWall = false;
                grid[7, 12].southWall = false;
                grid[12, 11].eastWall = false;
                grid[11, 7].southWall = false;
            }
            else
            {
                grid[7, 7].southWall = false;
                grid[12, 7].eastWall = false;
                grid[11, 12].southWall = false;
                grid[7, 11].eastWall = false;
            }

            chance = Random.Range(1, 3);

            //MID RIGHT
            if (chance == 1)
            {
                grid[7, 14].eastWall = false;
                grid[7, 19].southWall = false;
                grid[12, 18].eastWall = false;
                grid[11, 14].southWall = false;
            }
            else
            {
                grid[7, 7].southWall = false;
                grid[12, 7].eastWall = false;
                grid[11, 12].southWall = false;
                grid[7, 11].eastWall = false;
            }

            chance = Random.Range(1, 3);


            //BOTTOM LEFT
            if (chance == 1)
            {
                grid[14, 0].eastWall = false;
                grid[14, 5].southWall = false;
                grid[19, 4].eastWall = false;
                grid[18, 0].southWall = false;
            }
            else
            {
                grid[14, 0].southWall = false;
                grid[19, 0].eastWall = false;
                grid[18, 5].southWall = false;
                grid[14, 4].eastWall = false;
            }

            chance = Random.Range(1, 3);

            //BOTTOM MIDDLE
            if (chance == 1)
            {
                grid[14, 7].eastWall = false;
                grid[14, 12].southWall = false;
                grid[19, 11].eastWall = false;
                grid[18, 7].southWall = false;
            }
            else
            {
                grid[14, 7].southWall = false;
                grid[19, 7].eastWall = false;
                grid[18, 12].southWall = false;
                grid[14, 11].eastWall = false;
            }

            chance = Random.Range(1, 3);

            //BOTTOM RIGHT
            if (chance == 1)
            {
                grid[14, 14].eastWall = false;
                grid[14, 19].southWall = false;
                grid[19, 18].eastWall = false;
                grid[18, 14].southWall = false;
            }
            else
            {
                grid[14, 7].southWall = false;
                grid[19, 7].eastWall = false;
                grid[18, 12].southWall = false;
                grid[14, 11].eastWall = false;
            }

            chance = Random.Range(1, 3);

            if (chance == 1)
            {
                grid[5, 5].eastWall = false;
                grid[6, 5].eastWall = false;
            }
            else
            {
                grid[5, 5].southWall = false;
                grid[5, 6].southWall = false;
            }

            chance = Random.Range(1, 3);

            if (chance == 1)
            {
                grid[5, 12].eastWall = false;
                grid[6, 12].eastWall = false;
            }
            else
            {
                grid[5, 12].southWall = false;
                grid[5, 13].southWall = false;
            }

            chance = Random.Range(1, 3);

            if (chance == 1)
            {
                grid[12, 5].eastWall = false;
                grid[13, 5].eastWall = false;
            }
            else
            {
                grid[12, 5].southWall = false;
                grid[12, 6].southWall = false;
            }

            chance = Random.Range(1, 3);

            if (chance == 1)
            {
                grid[12, 12].eastWall = false;
                grid[13, 12].eastWall = false;
            }
            else
            {
                grid[12, 12].southWall = false;
                grid[12, 13].southWall = false;
            }

            chance = Random.Range(1, 3);

            if (chance == 1)
            {
                grid[9, 9].eastWall = false;
                grid[10, 9].eastWall = false;
            }
            else
            {
                grid[9, 9].southWall = false;
                grid[9, 10].southWall = false;
            }

            grid[14, 9].eastWall = false;
            grid[5, 9].eastWall = false;
            grid[9, 5].eastWall = false;
            grid[9, 14].eastWall = false;
        }

        if(difficulty == "TEST")
        {
            for(int i = 0; i < MazeWidth - 1; i++)
            {   
                for(int j = 0; j < MazeHeight - 1; j++)
                {
                    if (i != MazeWidth)
                    {
                        grid[i, j].eastWall = false;
                    }

                    if(j != MazeHeight)
                    {
                        grid[i, j].southWall = false;
                    }
                }
            }
        }
    }

    void createCell(int i, int j, Material floorColor)
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.transform.localScale = new Vector3(cellSize, 0.5f, cellSize);
        floor.transform.SetPositionAndRotation(new Vector3((cellSize * i), -0.25f, (cellSize * j)), Quaternion.Euler(new Vector3(0, 0, 0)));
        floor.name = i.ToString() + ", " + j.ToString();
        floor.GetComponent<Renderer>().material = floorColor;
        floor.transform.parent = GameObject.FindGameObjectWithTag("Maze").transform;
        floor.layer = 3;

        GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roof.transform.localScale = new Vector3(cellSize, 0.5f, cellSize);
        roof.transform.SetPositionAndRotation(new Vector3((cellSize * i), 2.5f, (cellSize * j)), Quaternion.Euler(new Vector3(0, 0, 0)));
        roof.name = i.ToString() + ", " + j.ToString();
        roof.GetComponent<Renderer>().material = floorColor;
        roof.transform.parent = GameObject.FindGameObjectWithTag("Maze").transform;
        roof.layer = 9;

        if (grid[i, j].southWall)
        {
            GameObject sWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sWall.transform.parent = floor.transform;
            sWall.transform.localScale = new Vector3(1.0f, 5.0f, 0.05f);
            sWall.transform.localPosition = new Vector3(0.5f, 2.5f, 0.0f);
            sWall.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
            sWall.name = "South Wall";
            sWall.GetComponent<Renderer>().material = red;
            sWall.layer = 6;
        }

        if (grid[i, j].eastWall)
        {
            GameObject eWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            eWall.transform.parent = floor.transform;
            eWall.transform.localScale = new Vector3(1.0f, 5.0f, 0.05f);
            eWall.transform.localPosition = new Vector3(0.0f, 2.5f, 0.5f);
            eWall.name = "East Wall";
            eWall.GetComponent<Renderer>().material = red;
            eWall.layer = 6;
        }

        if(i == 0)
        {
            GameObject nWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nWall.transform.parent = floor.transform;
            nWall.transform.localScale = new Vector3(1.0f, 5.0f, 0.05f);
            nWall.transform.localPosition = new Vector3(-0.5f, 2.5f, 0.0f);
            nWall.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
            nWall.name = "North Wall";
            nWall.GetComponent<Renderer>().material = red;
            nWall.layer = 6;
        }

        if (j == 0)
        {
            GameObject wWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wWall.transform.parent = floor.transform;
            wWall.transform.localScale = new Vector3(1.0f, 5.0f, 0.05f);
            wWall.transform.localScale = new Vector3(1.0f, 5.0f, 0.05f);
            wWall.transform.localPosition = new Vector3(0.0f, 2.5f, -0.5f);
            wWall.name = "West Wall";
            wWall.GetComponent<Renderer>().material = red;
            wWall.layer = 6;
        }

        GameObject previous = new GameObject();
        previous.transform.parent = floor.transform;
        string previousName = "[";
        previousName += grid[i, j].previousRowID;
        previousName += ", ";
        previousName += grid[i, j].previousColumnID;
        previousName += "]";
        previous.name = previousName;

        if(i == 0 && j == 0)
        {
            firstFloor = floor;
            floor.GetComponent<Renderer>().material = green;
        }
    }

    public void generateMaze(int x, int y, int emeny, int value, string difficulty)
    {
        
        MazeHeight = x;
        MazeWidth = y;
        grid = new RoomCell[MazeHeight, MazeWidth];
        populate();
        mazeLogic(difficulty);

        coordinates artifactLocation;
        artifactLocation.x = Random.Range(MazeHeight / 2, MazeHeight);
        artifactLocation.y = Random.Range(MazeWidth / 2, MazeWidth);

        string alternate = "blue";
        string lastFirst = alternate;
        for (int i = 0; i < MazeHeight; i++)
        {
            if(lastFirst == "blue")
            {
                alternate = "yellow";
            }
            else if(lastFirst == "yellow")
            {
                alternate = "blue";
            }
            lastFirst = alternate;

            for (int j = 0; j < MazeWidth; j++)
            {
                if (alternate == "blue")
                {
                    alternate = "yellow";
                    createCell(i, j, blue);
                }
                else if(alternate == "yellow")
                {
                    alternate = "blue";
                    createCell(i, j, yellow);
                }
            }
        }

        surface.BuildNavMesh();
        createPlayer();
        createArtifact(artifactLocation, value);
        
        //GameObject mazeEnd = (PrimitiveType.Cube);
    }

    public void createArtifact(coordinates pos, int value)
    {
        GameObject artifactObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        artifactObject.transform.localScale = new Vector3(0.5f * cellSize, 0.5f * cellSize, 0.5f * cellSize);
        artifactObject.transform.SetPositionAndRotation(new Vector3((cellSize * pos.x), 0.5f * cellSize, (cellSize * pos.y)), Quaternion.Euler(new Vector3(0, 0, 0)));
        artifactObject.name = "Artifact";
        artifactObject.tag = "Artifact";
        artifactObject.GetComponent<Renderer>().material = artifact;
        artifactObject.transform.parent = GameObject.FindGameObjectWithTag("Entities").transform;
        artifactObject.GetComponent<SphereCollider>().isTrigger = true;
        artifactObject.AddComponent<Rigidbody>();
        artifactObject.GetComponent<Rigidbody>().isKinematic = true;
        artifactObject.GetComponent<Rigidbody>().useGravity = false;

        //MainCamera.GetComponent<Compass>().SetArtifact(artifactObject.transform);

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Compass>().SetArtifact(artifactObject.transform); // Do no remove this line even though it is redundant
        GameObject.Find("PlayerCamera").GetComponent<Compass>().SetArtifact(artifactObject.transform); // Adds artifact to PlayerCamera


    }

    public void createPlayer()
    {
        GameObject player = Instantiate(playerObject); // Declan added this to create player

        //GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        coordinates pos = new coordinates { x = 0, y = 0 };

        player.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        player.transform.SetPositionAndRotation(new Vector3((cellSize * pos.x), 2.0f, (cellSize * pos.y)), Quaternion.Euler(new Vector3(0, 0, 0)));
        player.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f));
        //player.AddComponent<NavMeshAgent>();
        player.GetComponent<CapsuleCollider>().isTrigger = true;
        player.tag = "Player";
        player.transform.parent = GameObject.FindGameObjectWithTag("Entities").transform;
        player.layer = 7;

        player.gameObject.GetComponent<Weapon>().setWeaponType(selectedWeapon);
        //playerObject = player;
    }

    public void setWeaponPistol()
    {
        selectedWeapon = Weapon.WeaponType.Pistol;
    }
    public void setWeaponMachineGun()
    {
        selectedWeapon = Weapon.WeaponType.MachineGun;
    }
    public void setWeaponShotgun()
    {
        selectedWeapon = Weapon.WeaponType.Shotgun;
    }

    public void addEnemy()
    {
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        coordinates pos = new coordinates { x = 0, y = 0};
        bool noDuplicates = false;


        while (!noDuplicates)
        {
            noDuplicates = true;

            if (flipFlop == true)
            {
                pos.x = Random.Range(MazeHeight / 2, MazeHeight);
                pos.y = Random.Range(0, MazeWidth);

            }
            else if (flipFlop == false)
            {
                pos.x = Random.Range(0, MazeHeight);
                pos.y = Random.Range(MazeWidth / 2, MazeWidth);
            }
            flipFlop = !flipFlop;

            // Check to ensure far enough away from player

            Vector3 tempEnemyPosition = new Vector3((cellSize * pos.x), 1.0f, (cellSize * pos.y));
            //Debug.Log(tempEnemyPosition);
            Vector3 playerPos = playerObject.transform.position;
            //Debug.Log(playerPos);
            float distanceToPlayer = Vector3.Distance(tempEnemyPosition, playerPos);
            //Debug.Log(distanceToPlayer);

            float spawnDistThreshold = 5.0f; // Do not increase past 10.0f as 'Easy' map won't load if its any bigger

            if (distanceToPlayer < spawnDistThreshold)
            {
                noDuplicates = false;
            }

            // Check for duplicate enemies

            for (int i = 0; i < duplicatePrevention.Count; i++)
            {
                if ((duplicatePrevention[i].x == pos.x) && (duplicatePrevention[i].y == pos.y))
                {
                    noDuplicates = false;
                }
            }
        }


        enemy.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        enemy.transform.SetPositionAndRotation(new Vector3((cellSize * pos.x), 1.0f, (cellSize * pos.y)), Quaternion.Euler(new Vector3(0, 0, 0)));
        enemy.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f));
        enemy.AddComponent<NavMeshAgent>();
        enemy.AddComponent<EnemyUI>();
        enemy.GetComponent<EnemyUI>().setAgent(enemy);
        enemy.GetComponent<EnemyUI>().player = playerObject;
        enemy.GetComponent<EnemyUI>().setMasks(targetMask, obstacleMask1, obstacleMask2, allyMask);
        enemy.GetComponent<CapsuleCollider>().isTrigger = true;
        enemy.tag = "Enemy";
        enemy.transform.parent = GameObject.FindGameObjectWithTag("Entities").transform;

        enemy.AddComponent<Light>();
        enemy.GetComponent<Light>().transform.SetPositionAndRotation(new Vector3(0, 0.5f, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
        enemy.GetComponent<Light>().type = LightType.Spot;
        enemy.GetComponent<Light>().shadows = LightShadows.Soft;
        enemy.GetComponent<Light>().spotAngle = 75.0f;
        enemy.GetComponent<Light>().intensity = 5.0f;
        enemy.GetComponent<Light>().range = 20.0f;
        enemy.layer = 8;

        newTarget(enemy);
    }

    public void newTarget(GameObject enemy)
    {
        GameObject target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        coordinates pos;
        pos.x = Random.Range(0, MazeHeight);
        pos.y = Random.Range(0, MazeHeight);

        target.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        target.transform.SetPositionAndRotation(new Vector3((cellSize * pos.x), 0.5f, (cellSize * pos.y)), Quaternion.Euler(new Vector3(0, 0, 0)));
        target.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.0f, 1.0f, 0.0f));
        target.tag = "Goal";
        target.transform.parent = GameObject.FindGameObjectWithTag("Entities").transform;
        target.GetComponent<SphereCollider>().isTrigger = true;
        target.AddComponent<Rigidbody>();
        target.GetComponent<Rigidbody>().isKinematic = true;
        target.GetComponent<Rigidbody>().useGravity = false;
        target.GetComponent<MeshFilter>().sharedMesh = null;


        enemy.GetComponent<EnemyUI>().setGoal(target.transform);
    }

    
}

