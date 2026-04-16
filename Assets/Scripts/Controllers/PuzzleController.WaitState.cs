using Models;
using Tools;
using UnityEngine;

namespace Controllers
{
    partial class PuzzleController
    {
        public class WaitState : State
        {
            public WaitState(PuzzleController owner) : base(owner) {}
            public override void OnEnter()
            {
                
            }

            public override void OnExit()
            {
                
            }

            public override void Init(Vector2Int puzzleSize, Texture2D texture2D)
            {
            }

            public override void Init(PuzzleModel puzzleModel)
            {
            }

            private void RegesterClick()
            {
                foreach (var pieceView in Owner.puzzleView.PuzzlePieceViews)
                {
                    pieceView.AddClickEvent(Owner.OnClickPiece);
                }

                foreach (var barrierView in Owner.puzzleView.BarrierViews.Values)
                {
                    barrierView.AddClickEvent(Owner.OnClickBarrier);
                }
            }
            public override void OnClickPiece(int id)
            {
                var coords = Owner.PuzzleModel[id];
                if (!Owner.TrySwapCoords(coords)) return;
                Owner.PuzzleModel.StepCount++;
                Owner.CurrentScore--;
                AudioManager.Instance.PlaySfx("move");
                if (Owner.CheckWin())
                {
                    Owner._stateMachine.ChangeState(Owner._winState);
                }
            }

            public override void OnClickBarrier(in Barrier barrier)
            {
                
            }

            public override void OnClickTool(ToolTypeEnum toolType)
            {
                if (toolType == ToolTypeEnum.Hammer) Owner._stateMachine.ChangeState(Owner._hammerToolState);
                else if (toolType == ToolTypeEnum.Scissors) Owner._stateMachine.ChangeState(Owner._scissorsToolState);
            }

            public override void OnClickEventButton(GameEventEnum gameEvent)
            {
                if (gameEvent == GameEventEnum.StartLevel) Owner._stateMachine.ChangeState(Owner._playState);
            }
        }
    }
}