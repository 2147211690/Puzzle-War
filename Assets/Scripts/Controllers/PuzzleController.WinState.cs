using Models;
using Tools;
using UnityEngine;

namespace Controllers
{
    partial class PuzzleController
    {
        public class WinState : State
        {
            public WinState(PuzzleController owner) : base(owner) {}
            public override void OnEnter()
            {
                Owner.PuzzleModel.FillAll();
                Owner.puzzleView.Wim();
            }

            public override void OnExit()
            {
                
            }

            public override void Init(Vector2Int puzzleSize, Texture2D texture2D)
            {
                Owner._stateMachine.ChangeState(Owner._playState).Init(puzzleSize, texture2D);
            }

            public override void Init(PuzzleModel puzzleModel)
            {
                Owner._stateMachine.ChangeState(Owner._playState).Init(puzzleModel);
            }

            private void RegesterClick()
            {
            }
            public override void OnClickPiece(int id)
            {
            }

            public override void OnClickBarrier(in Barrier barrier)
            {
            }

            public override void OnClickTool(ToolTypeEnum toolType)
            {
            }

            public override void OnClickEventButton(GameEventEnum gameEvent)
            {
            }

            public override void OnWinComplete()
            {
                Owner.puzzleUi.OpenPopup("Win");
            }
        }
    }
}