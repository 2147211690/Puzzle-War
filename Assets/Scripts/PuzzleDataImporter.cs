// Editor/PuzzleDataImporter.cs
// 放在 Assets/Editor 文件夹下
#if UNITY_EDITOR
using System;
using System.IO;
using Models;
using UnityEditor;
using UnityEngine;

public class PuzzleDataImporter : EditorWindow
{
    private string jsonContent = "";
    private Vector2 scrollPosition;
    private string savePath = "Assets/Puzzles";
    private string fileName = "NewPuzzle";

    [MenuItem("Tools/Puzzle Data/Import from JSON")]
    public static void ShowWindow()
    {
        GetWindow<PuzzleDataImporter>("Puzzle JSON 导入器");
    }

    private void OnGUI()
    {
        GUILayout.Label("PuzzleData JSON 导入器", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        // JSON 输入区域
        GUILayout.Label("粘贴 JSON 内容:", EditorStyles.label);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        jsonContent = EditorGUILayout.TextArea(jsonContent, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(10);

        // 保存路径设置
        GUILayout.Label("保存设置:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("路径:", GUILayout.Width(50));
        savePath = EditorGUILayout.TextField(savePath);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            string selected = EditorUtility.SaveFolderPanel("选择保存文件夹", "Assets", "");
            if (!string.IsNullOrEmpty(selected))
            {
                // 转换为相对路径
                savePath = selected.Replace(Application.dataPath, "Assets").Replace("\\", "/");
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("文件名:", GUILayout.Width(50));
        fileName = EditorGUILayout.TextField(fileName);
        GUILayout.Label(".asset", GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(20);

        // 按钮
        EditorGUILayout.BeginHorizontal();
        
        GUI.enabled = !string.IsNullOrWhiteSpace(jsonContent);
        if (GUILayout.Button("导入并创建", GUILayout.Height(40)))
        {
            ImportFromJson();
        }
        GUI.enabled = true;

        if (GUILayout.Button("清空", GUILayout.Height(40), GUILayout.Width(80)))
        {
            jsonContent = "";
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // 快捷按钮：从文件加载 JSON
        if (GUILayout.Button("从文件加载 JSON..."))
        {
            LoadJsonFromFile();
        }
    }

    private void LoadJsonFromFile()
    {
        string path = EditorUtility.OpenFilePanel("选择 JSON 文件", "", "json");
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            jsonContent = File.ReadAllText(path);
            // 自动提取文件名
            fileName = Path.GetFileNameWithoutExtension(path);
            Repaint();
            ShowNotification(new GUIContent("JSON 已加载"), 2);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("错误", $"读取文件失败: {e.Message}", "确定");
        }
    }

    private void ImportFromJson()
    {
        try
        {
            // 1. 创建临时对象用于反序列化
            TempPuzzleData tempData = JsonUtility.FromJson<TempPuzzleData>(jsonContent);
            
            if (tempData == null)
            {
                EditorUtility.DisplayDialog("错误", "JSON 解析失败，请检查格式", "确定");
                return;
            }

            // 2. 创建真正的 PuzzleData
            PuzzleData puzzleData = ScriptableObject.CreateInstance<PuzzleData>();
            
            // 3. 复制基础数据
            puzzleData.size = tempData.size;
            puzzleData._pieceModels = tempData._pieceModels ?? Array.Empty<PieceModel>();
            puzzleData._barriers = tempData._barriers ?? Array.Empty<Barrier>();

            // 4. 处理 Texture（从 JSON 中的 instanceID 无法恢复，需要手动设置）
            if (tempData.texture != null && tempData.texture.instanceID != 0)
            {
                // 尝试从当前选择的对象中找纹理，或提示用户
                EditorUtility.DisplayDialog(
                    "提示", 
                    "Texture 无法从 JSON 自动恢复，请在创建后手动赋值。\n" +
                    "建议：将图片放在 Resources 文件夹，使用路径引用替代。", 
                    "继续"
                );
            }

            // 5. 确保目录存在
            string fullFolderPath = Path.Combine(Application.dataPath, savePath.Replace("Assets/", ""));
            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
            }

            // 6. 生成唯一路径
            string assetPath = $"{savePath}/{fileName}.asset";
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            // 7. 创建 Asset
            AssetDatabase.CreateAsset(puzzleData, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 8. 高亮显示
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = puzzleData;

            ShowNotification(new GUIContent($"已创建: {assetPath}"), 3);
            Debug.Log($"PuzzleData 已创建: {assetPath}");

            // 9. 可选：自动打开编辑器
            EditorApplication.delayCall += () =>
            {
                EditorGUIUtility.PingObject(puzzleData);
            };
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("错误", $"导入失败: {e.Message}\n\n{e.StackTrace}", "确定");
            Debug.LogException(e);
        }
    }

    // 临时类用于 JSON 反序列化（匹配您的 JSON 结构）
    [Serializable]
    private class TempPuzzleData
    {
        public Vector2Int size;
        public TempTexture texture;
        public PieceModel[] _pieceModels;
        public Barrier[] _barriers;
    }

    [Serializable]
    private class TempTexture
    {
        public int instanceID;
    }
}
#endif
