using Models;
using Tools;
using UnityEngine;

namespace Controllers
{
    partial class PuzzleController
    {
        public class ScissorsToolState : State
        {
            public ScissorsToolState(PuzzleController owner) : base(owner) {}
            public override void OnEnter()
            {
                int count = 0;
                for (int i = 0; i < Owner._puzzleModel.Size.x; i++)
                {
                    for (int j = 0; j < Owner._puzzleModel.Size.y; j++)
                    {
                        Owner.puzzleView.PuzzlePieceViews[i, j].button.enabled = false;
                    }
                }
                foreach (var barrierView in Owner.puzzleView.BarrierViews.Values)
                {
                    count++;
                    barrierView.highlightable.enabled = true;
                    barrierView.button.enabled = true;
                }
                if (count == 0)
                {
                    Owner._stateMachine.ChangeState(Owner._playState);
                }
            }

            public override void OnExit()
            {
                for (int i = 0; i < Owner._puzzleModel.Size.x; i++)
                {
                    for (int j = 0; j < Owner._puzzleModel.Size.y; j++)
                    {
                        Owner.puzzleView.PuzzlePieceViews[i, j].button.enabled = true;
                    }
                }
                foreach (var barrierView in Owner.puzzleView.BarrierViews.Values)
                {
                    barrierView.highlightable.enabled = false;
                    barrierView.button.enabled = false;
                }
            }

            public override void Init(Vector2Int puzzleSize, Texture2D texture2D)
            {
               
            }
            private void RegesterClick()
            {
                
            }
            public override void OnClickPiece(int id)
            {
                
            }

            public override void OnClickBarrier(in Barrier barrier)
            {
                Owner._puzzleModel.RemoveBarrier(barrier);
                Owner.puzzleView.RemoveBarrier(barrier);
                Owner._stateMachine.ChangeState(Owner._playState);
            }

            public override void OnClickTool(ToolTypeEnum toolType)
            {
                if (toolType == ToolTypeEnum.Scissors) Owner._stateMachine.ChangeState(Owner._playState);
            }
        }
    }
}