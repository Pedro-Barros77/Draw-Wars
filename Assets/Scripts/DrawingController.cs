using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

#region JSON_Classes
[Serializable]
public class Drawings
{
    public Shape[] Shapes;
    public Drawings() { }
    public Drawings(Shape[] shapes) { this.Shapes = shapes; }
}

[Serializable]
public class Shape
{
    public bool Drawn { get; private set; } = false;
    public void SetDrawn(bool value) { Drawn = value; }
    public Coord[] Coords;
    public Shape(Coord[] coords) { this.Coords = coords; }
}

[Serializable]
public class Coord
{
    public bool Drawn { get; private set; } = false;
    public void SetDrawn(bool value) { Drawn = value; }
    public string x, y;
    public Coord(float x, float y)
    {
        this.x = RoundToString(x);
        this.y = RoundToString(y);
    }


    string RoundToString(float value)
    {
        CultureInfo info = CultureInfo.CurrentCulture;
        char separator = info.NumberFormat.NumberDecimalSeparator[0];

        string[] text = Math.Round(value, 2).ToString().Split(separator);
        string result = text[0] + separator;
        if (text.Length == 1) return result;
        if (text[1].Length > 2) result += text[1].Substring(0, 2);
        else result += text[1];
        return result;
    }
}
#endregion

public class DrawingController : MonoBehaviour
{
    public Camera drawingCamera;
    public GameObject brush, enemyBrush;
    public RenderTexture drawingTexture, enemyDrawingTexture;
    public Transform myDrawingsContainer, enemyDrawingsContainer;

    [SerializeField] private Client client;

    public Drawings MyDrawings;
    public Drawings EnemyDrawings;
    public List<List<Vector2>> DrawingsList = new List<List<Vector2>>();

    LineRenderer lineRenderer, enemyLineRenderer;
    int screenWidth, screenHeight;
    Vector2 lastPos;
    PlayerController playerCTRL;
    DateTime lastEnemyDraw;
    float enemyDrawDelay = 1000f;
    Coord currentEnemyCoord;

    private void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        if (drawingCamera == null)
        {
            drawingCamera = Camera.main;
        }

        var filesPath = Path.Combine(Application.dataPath, "temp_data", "Last_Game_Drawings");
        if (Directory.Exists(filesPath))
        {
            var files = Directory.GetFiles(filesPath);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        MyDrawings = new Drawings();
        EnemyDrawings = new Drawings();
        myDrawingsContainer = GameObject.Find("My_Drawings").transform;
        enemyDrawingsContainer = GameObject.Find("Enemy_Drawings").transform;
        playerCTRL = gameObject.GetComponent<PlayerController>();
        lastEnemyDraw = DateTime.Now;
    }

    private void Update()
    {
        Draw();

        if (IsConnected() && NetworkManager.Instance.GetPlayersCount() > 1)
        {
            if (lastEnemyDraw.AddMilliseconds(enemyDrawDelay) > DateTime.Now)
            {
                DrawEnemy();
                lastEnemyDraw = DateTime.Now;
            }
        }

        if (client.hasResult)
        {
            Debug.Log(client.lastResult);
            client.lastResult = 0;
            client.hasResult = false;
        }
    }

    void Draw()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            lastPos = CreateBrush();

        if (Input.GetKey(KeyCode.Mouse0))
        {

            Vector2 mousePos = drawingCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 roundedPos = Round(mousePos);

            if (Vector2.Distance(roundedPos, lastPos) > 0.1f)
            {
                AddPoint(roundedPos);
                lastPos = roundedPos;

                MyDrawings.Shapes = DrawingsList.Select(d => new Shape(d.Select(s => new Coord(s.x, s.y)).ToArray())).ToArray();
            }
        }
        else
        {
            lineRenderer = null;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                string fileName = "carro.jpg";
                string pythonFolder = Path.Combine(Application.dataPath, "..\\PythonAPI");
                byte[] carro = File.ReadAllBytes(Path.Combine(pythonFolder, fileName));

                byte[] myDrawing = SaveDrawing();
                client.SendImages(myDrawing, carro);
                DrawingsList.Clear();
                playerCTRL.ClearDrawings();
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            string json = JsonUtility.ToJson(MyDrawings, true);
            Debug.Log(json);
        }
    }

    void DrawEnemy()
    {
        if (EnemyDrawings == null || EnemyDrawings.Shapes == null || EnemyDrawings.Shapes.Length == 0) return; //Não tem desenhos
        if (EnemyDrawings.Shapes.All(s => s.Drawn)) return; //Já desenhou todos

        Debug.Log("DrawingEnemy...");
        if (EnemyDrawings.Shapes.All(s => !s.Drawn && s.Coords.All(c => !c.Drawn))) //Se for o primeiro traço, criar pincel
        {
            CreateEnemyBrush(EnemyDrawings.Shapes[0].Coords[0]);
        }

        currentEnemyCoord = EnemyDrawings.Shapes.First(s => !s.Drawn && s.Coords.Any(c => !c.Drawn)).Coords.First(c => !c.Drawn);

        if (currentEnemyCoord == null) return;

        AddEnemyPoint(currentEnemyCoord);
        currentEnemyCoord.SetDrawn(true);

        var currentShape = EnemyDrawings.Shapes.First(s => s.Coords.Contains(currentEnemyCoord));

        if (currentEnemyCoord == currentShape.Coords.Last()) //Se esse foi o ultimo traço, próximo desenho
        {
            currentEnemyCoord = null;
            currentShape.SetDrawn(true);
            if (EnemyDrawings.Shapes.Any(s => !s.Drawn)) //Se tem próximo desenho, criar pincel
            {
                var nextShape = EnemyDrawings.Shapes.First(s => !s.Drawn && s.Coords.All(c => !c.Drawn));
                if (nextShape != null)
                    CreateEnemyBrush(nextShape.Coords[0]);
            }
        }

    }

    byte[] SaveDrawing()
    {
        if (!playerCTRL._isPlayerOwner) return new byte[0];

        RenderTexture.active = drawingTexture;
        Texture2D virtualPhoto = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);

        virtualPhoto.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);

        RenderTexture.active = null;

        Texture2D scaledDownPhoto = ScaleTexture(virtualPhoto, 640, 360);

        byte[] bytes;
        bytes = scaledDownPhoto.EncodeToJPG();

        if (!Directory.Exists(Path.Combine(Application.dataPath,"temp_data", "Last_Game_Drawings")))
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "temp_data", "Last_Game_Drawings"));
        }

        string filePath = Path.Combine(Application.dataPath, "temp_data", "Last_Game_Drawings");
        string[] files = Directory.GetFiles(filePath).Where(f => f.EndsWith(".jpg")).ToArray();
        string filename = $"My_Drawing_{files.Length + 1}.jpg";

        GUIUtility.systemCopyBuffer = filePath;

        File.WriteAllBytes(Path.Combine(filePath, filename), bytes);

        foreach (Transform drawing in myDrawingsContainer)
        {
            Destroy(drawing.gameObject);
        }
        DrawingsList.Clear();

        return bytes;
    }

    public void ClearEnemyDrawings()
    {
        EnemyDrawings = new Drawings();
        foreach (Transform drawing in enemyDrawingsContainer)
        {
            Destroy(drawing.gameObject);
        }
        currentEnemyCoord = null;
    }

    Vector2 CreateBrush()
    {
        GameObject brushInstance = Instantiate(brush, myDrawingsContainer);
        brushInstance.name = "Drawing_" + DrawingsList.Count;
        lineRenderer = brushInstance.GetComponent<LineRenderer>();
        lineRenderer.sortingLayerName = "Drawing";
        lineRenderer.sortingOrder = 10;

        Vector2 mousePos = drawingCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 roundedPos = Round(mousePos);

        lineRenderer.SetPosition(0, roundedPos);
        lineRenderer.SetPosition(1, roundedPos);

        DrawingsList.Add(new List<Vector2> { roundedPos, roundedPos });
        return roundedPos;
    }

    void CreateEnemyBrush(Coord startPos)
    {
        GameObject brushInstance = Instantiate(enemyBrush, enemyDrawingsContainer);
        brushInstance.name = "Enemy_Drawing_" + EnemyDrawings.Shapes.Count(c => c.Drawn) + 1;
        enemyLineRenderer = brushInstance.GetComponent<LineRenderer>();
        enemyLineRenderer.sortingLayerName = "Drawing";
        enemyLineRenderer.sortingOrder = 10;

        enemyLineRenderer.SetPosition(0, new Vector2(float.Parse(startPos.x), float.Parse(startPos.y)));
        enemyLineRenderer.SetPosition(1, new Vector2(float.Parse(startPos.x), float.Parse(startPos.y)));
    }

    void AddPoint(Vector2 pointPos)
    {
        lineRenderer.positionCount++;
        int posIndex = lineRenderer.positionCount - 1;
        lineRenderer.SetPosition(posIndex, pointPos);

        DrawingsList[DrawingsList.Count - 1].Add(pointPos);
    }

    void AddEnemyPoint(Coord pointPos)
    {
        enemyLineRenderer.positionCount++;
        int posIndex = enemyLineRenderer.positionCount - 1;
        enemyLineRenderer.SetPosition(posIndex, new Vector2(float.Parse(pointPos.x), float.Parse(pointPos.y)));
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = ((float)1 / source.width) * ((float)source.width / targetWidth);
        float incY = ((float)1 / source.height) * ((float)source.height / targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth),
                              incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    bool IsConnected()
    {
        return NetworkManager.Instance != null && NetworkManager.Instance.IsConnected();
    }

    float Round(float value)
    {
        CultureInfo info = CultureInfo.CurrentCulture;
        char separator = info.NumberFormat.NumberDecimalSeparator[0];

        string[] text = value.ToString().Split(separator);

        string result = text[0] + separator;
        if (text.Length == 1) return float.Parse(result);
        if (text[1].Length > 2) result += text[1].Substring(0, 2);
        else result += text[1];
        return float.Parse(result);
    }

    Vector2 Round(Vector2 value)
    {
        float x = Round(value.x);
        float y = Round(value.y);
        return new Vector2(x, y);
    }
}
