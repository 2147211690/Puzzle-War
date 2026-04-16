using System.IO;
using Controllers;
using UnityEngine;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
    public PuzzleController puzzleController = null!;
    public Sprite puzzleSprite = null!;
    public Vector2Int puzzleSize = Vector2Int.one;
    public Texture2D texture = null!;
    public void OnGUI()
    {
        GUI.skin.button.fontSize = 24;
    
        // 整体区域居中，宽度固定
        GUILayout.BeginArea(new Rect(Screen.width/2 - 200, 50, 400, Screen.height));
        GUILayout.BeginVertical();

        // 按钮自动填满宽度，高度固定
        GUILayoutOption[] btnOptions = new GUILayoutOption[] 
        { 
            GUILayout.Height(60),
            GUILayout.ExpandWidth(true) 
        };

        if (GUILayout.Button("创建随机色棋盘", btnOptions))
        {
            puzzleController.Init(puzzleSize, GenerateRandomPixelTexture(puzzleSize));
        }
    
        GUILayout.Space(10);  // 按钮间距

        if (GUILayout.Button("创建图片棋盘", btnOptions))
        {
            puzzleController.Init(puzzleSize, texture);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("输出当前ModelJson", btnOptions))
        {
            var data = puzzleController.PuzzleModel.ToPuzzleData();
            var json = JsonUtility.ToJson(data);
            Debug.Log($"\n\n\n{json}\n\n\n");
        }

        GUILayout.Space(20);

        // 输入框
        GUILayout.Label("输入关卡:", GUILayout.Height(30));
        GUI.skin.textArea.fontSize = 20;
        levelInput = GUILayout.TextField(levelInput, GUILayout.Height(50));

        GUILayout.Space(10);

        if (GUILayout.Button("加载第n关", btnOptions))
        {
            if (int.TryParse(levelInput, out int level)) 
                puzzleController.Init(level);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private string levelInput = "1";
        
        
    public static Sprite CreateColorSprite(Color color, int width = 2, int height = 2)
    {
        // 创建小纹理（2x2足够，会自动拉伸）
        Texture2D tex = new Texture2D(width, height);
        tex.wrapMode = TextureWrapMode.Repeat;

        // 填充颜色
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;

        tex.SetPixels(pixels);
        tex.Apply();

        // 转 Sprite
        return Sprite.Create(
            tex,
            new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f)
        );
    }
        
    /// <summary>
    /// 创建一张每个像素都是随机颜色的 Texture2D
    /// </summary>
    public Texture2D GenerateRandomPixelTexture(Vector2Int size)
    {
        int width = size.x;
        int height = size.y;

        // 创建贴图
        Texture2D tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Point;  // 像素风格（不模糊）
        tex.wrapMode = TextureWrapMode.Clamp;

        // 给每个像素赋随机颜色
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color(
                Random.value,
                Random.value,
                Random.value
            );
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return tex;
    }
}