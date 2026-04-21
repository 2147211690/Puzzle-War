using System;
using Models;
using UnityEngine;

[CreateAssetMenu(menuName = "PuzzleData"), Serializable]
public class PuzzleData : ScriptableObject
{
    public Vector2Int size;
    public int difficulty = 1;
    public int textureId = 0;
    public PieceModel[] _pieceModels;
    public Barrier[] _barriers;
}