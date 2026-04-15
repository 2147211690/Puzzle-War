using Models;
using Tools;
using UnityEngine;

namespace Controllers
{
    partial class PuzzleController
    {
        public class PlayState : State
        {
            public PlayState(PuzzleController owner) : base(owner) {}
            public override void OnEnter()
            {
                
            }

            public override void OnExit()
            {
                
            }

            public override void Init(Vector2Int puzzleSize, Texture2D texture2D)
            {
                // 自动切割大图 → 小Sprite列表
                Owner._puzzleModel = PuzzleTools.CutTextureToSprites(texture2D, puzzleSize)
                    .RandomPiecesType(3,2,2)
                    .RandomBarriers(5)
                    .ShufflePieces(Owner.shuffleSteps);
                // 初始化 View
                Owner.puzzleView.Init(Owner._puzzleModel);
                RegesterClick();
                AudioManager.Instance.PlayBGM("bgm");
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
                var coords = Owner._puzzleModel[id];
                if (!Owner.TrySwapCoords(coords)) return;
                AudioManager.Instance.PlaySfx("move");
                if (Owner.CheckWin())
                {
                    Debug.Log("Win!");
                    Owner._puzzleModel.FillAll();
                    Owner.puzzleView.Wim();
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
        }
    }
}