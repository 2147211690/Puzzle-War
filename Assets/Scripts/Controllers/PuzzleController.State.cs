using Models;
using Tools;
using UnityEngine;

namespace Controllers
{
    partial class PuzzleController
    {
        public abstract class State : IState
        {
            protected State(PuzzleController owner)
            {
                Owner = owner;
            }
            public PuzzleController Owner { get; }
            public abstract void OnEnter();
            public abstract void OnExit();
            public abstract void Init(Vector2Int puzzleSize, Texture2D texture2D);
            public abstract void OnClickPiece(int id);
            public abstract void OnClickBarrier(in Barrier barrier);
            public abstract void OnClickTool(ToolTypeEnum toolType);
        }
    }
}