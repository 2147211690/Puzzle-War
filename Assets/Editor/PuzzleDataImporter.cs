using System;
using System.Collections.Generic;
using System.IO;
using Models;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PuzzleDataImporter : EditorWindow
    {
        private TextAsset jsonFile;
        private string outputFolder = "Assets/PuzzleDatas";
        private Vector2 scrollPos;
        private string statusMessage = "";
        private bool isImporting = false;

        [MenuItem("Tools/Puzzle Data Importer")]
        public static void ShowWindow()
        {
            GetWindow<PuzzleDataImporter>("Puzzle Data Importer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Puzzle Data 批量导入工具", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            // JSON文件选择
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("JSON文件:", GUILayout.Width(70));
            jsonFile = EditorGUILayout.ObjectField(jsonFile, typeof(TextAsset), false) as TextAsset;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // 输出文件夹
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("输出路径:", GUILayout.Width(70));
            outputFolder = EditorGUILayout.TextField(outputFolder);
            if (GUILayout.Button("浏览", GUILayout.Width(50)))
            {
                string selected = EditorUtility.OpenFolderPanel("选择输出文件夹", "Assets", "");
                if (!string.IsNullOrEmpty(selected))
                {
                    // 转换为相对路径
                    if (selected.StartsWith(Application.dataPath))
                    {
                        outputFolder = "Assets" + selected.Substring(Application.dataPath.Length);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // 预览信息
            if (jsonFile != null)
            {
                EditorGUILayout.LabelField("文件大小:", jsonFile.text.Length.ToString("N0") + " 字符");

                // 尝试解析预览
                try
                {
                    var wrapper = JsonUtility.FromJson<PuzzleDataWrapper>(jsonFile.text);
                    if (wrapper?.puzzleDatas != null)
                    {
                        EditorGUILayout.LabelField("包含关卡数:", wrapper.puzzleDatas.Count.ToString());

                        EditorGUILayout.Space(5);
                        GUILayout.Label("关卡预览:", EditorStyles.boldLabel);

                        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
                        for (int i = 0; i < wrapper.puzzleDatas.Count; i++)
                        {
                            var data = wrapper.puzzleDatas[i];
                            EditorGUILayout.BeginHorizontal("box");
                            GUILayout.Label($"{i}", GUILayout.Width(30));
                            GUILayout.Label($"{data.size.x}x{data.size.y}", GUILayout.Width(50));
                            GUILayout.Label($"难度:{data.difficulty}", GUILayout.Width(60));
                            GUILayout.Label($"碎片:{data._pieceModels?.Length ?? 0}", GUILayout.Width(60));
                            GUILayout.Label($"障碍:{data._barriers?.Length ?? 0}", GUILayout.Width(60));
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndScrollView();
                    }
                }
                catch (Exception e)
                {
                    EditorGUILayout.HelpBox($"JSON解析错误: {e.Message}", MessageType.Error);
                }
            }

            EditorGUILayout.Space(10);

            // 导入按钮
            EditorGUI.BeginDisabledGroup(jsonFile == null || isImporting);
            if (GUILayout.Button("开始导入", GUILayout.Height(40)))
            {
                ImportData();
            }
            EditorGUI.EndDisabledGroup();

            // 状态信息
            if (!string.IsNullOrEmpty(statusMessage))
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox(statusMessage, 
                    statusMessage.Contains("错误") || statusMessage.Contains("失败") ? MessageType.Error : MessageType.Info);
            }
        }

        private void ImportData()
        {
            isImporting = true;
            statusMessage = "正在导入...";

            try
            {
                // 解析JSON
                var wrapper = JsonUtility.FromJson<PuzzleDataWrapper>(jsonFile.text);
                if (wrapper?.puzzleDatas == null || wrapper.puzzleDatas.Count == 0)
                {
                    statusMessage = "错误: JSON中没有找到puzzleDatas数据";
                    isImporting = false;
                    return;
                }

                // 确保输出文件夹存在
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                    AssetDatabase.Refresh();
                }

                int successCount = 0;
                int failCount = 0;

                for (int i = 0; i < wrapper.puzzleDatas.Count; i++)
                {
                    var tempData = wrapper.puzzleDatas[i];

                    try
                    {
                        // 创建PuzzleData实例
                        PuzzleData puzzleData = ScriptableObject.CreateInstance<PuzzleData>();

                        // 复制数据
                        puzzleData.size = tempData.size;
                        puzzleData.difficulty = tempData.difficulty;
                        puzzleData.textureId = i; // 使用索引作为textureId
                        puzzleData._pieceModels = tempData._pieceModels;
                        puzzleData._barriers = tempData._barriers;

                        // 构建保存路径
                        string fileName = $"{i}.asset";
                        string fullPath = Path.Combine(outputFolder, fileName);

                        // 如果文件已存在，删除旧文件
                        if (File.Exists(fullPath))
                        {
                            AssetDatabase.DeleteAsset(fullPath);
                        }

                        // 创建Asset
                        AssetDatabase.CreateAsset(puzzleData, fullPath);

                        successCount++;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"导入关卡 {i} 失败: {e.Message}");
                        failCount++;
                    }
                }

                // 保存所有更改
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                statusMessage = $"导入完成! 成功: {successCount}, 失败: {failCount}. 文件保存在: {outputFolder}";
                Debug.Log(statusMessage);
            }
            catch (Exception e)
            {
                statusMessage = $"导入过程中发生错误: {e.Message}";
                Debug.LogError(e);
            }
            finally
            {
                isImporting = false;
            }
        }

        // JSON数据结构（用于解析）
        [Serializable]
        private class PuzzleDataWrapper
        {
            public List<TempPuzzleData> puzzleDatas = new List<TempPuzzleData>();
        }

        [Serializable]
        private class TempPuzzleData
        {
            public Vector2Int size;
            public int difficulty = 1;
            public int textureId;
            public PieceModel[] _pieceModels;
            public Barrier[] _barriers;
        }
    }
}