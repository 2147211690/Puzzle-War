using System;
using Models;
using UnityEngine;

[CreateAssetMenu(menuName = "PuzzleData"), Serializable]
public class PuzzleData : ScriptableObject
{
    public Vector2Int size;
    public Texture2D texture;
    public PieceModel[] _pieceModels;
    public Barrier[] _barriers;
}