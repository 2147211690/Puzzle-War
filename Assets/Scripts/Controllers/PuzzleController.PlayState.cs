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
            public override void OnEnter(IState prevState)
            {
                if (prevState == Owner._homeState || prevState == Owner._winState)
                {
                    DyAdManager.Instance.ShowInter();
                }
                DyAdManager.Instance.ShowBannerAd();
            }

            public override void OnExit(IState nextState)
            {
                
            }

            public override void Init(Vector2Int puzzleSize, Texture2D texture2D)
            {
                // 自动切割大图 → 小Sprite列表
                Owner.PuzzleModel = PuzzleTools.CutTextureToSprites(texture2D, -1, puzzleSize)
                    .RandomPiecesType(3,2,2)
                    .RandomBarriers(5)
                    .ShufflePieces(Owner.shuffleSteps);
                // 初始化 View
                Owner.puzzleView.SetPuzzleGame(Owner.PuzzleModel);
                RegesterClick();
                Owner.CurrentScore = 100;
            }
            

            public override void Init(PuzzleModel puzzleModel)
            {
                Owner.PuzzleModel = puzzleModel;
                // 初始化 View
                Owner.puzzleView.SetPuzzleGame(Owner.PuzzleModel);
                RegesterClick();
                Owner.CurrentScore = 100;
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

            public override void OnReplay()
            {
                base.OnReplay();
                Owner.Init(PlayerData.CurrentLevel);
            }

            public override void OnHome()
            {
                base.OnHome();
                Owner._stateMachine.ChangeState(Owner._homeState);
            }

            public override void OnClickTool(ToolTypeEnum toolType)
            {
                if (toolType == ToolTypeEnum.Hammer)
                {
                    if (Owner.HammerCountMV <= 0)
                    {
                        DyAdManager.Instance.ShowReward(r =>
                        {
                            if (r) Owner.HammerCountMV++;
                        });
                        return;
                    }
                    Owner._stateMachine.ChangeState(Owner._hammerToolState);
                }
                else if (toolType == ToolTypeEnum.Scissors)
                {
                    if (Owner.ScissorsCountMV <= 0)
                    {
                        DyAdManager.Instance.ShowReward(r =>
                        {
                            if (r) Owner.ScissorsCountMV++;
                        });
                        return;
                    }
                    Owner._stateMachine.ChangeState(Owner._scissorsToolState);
                }
            }
        }
    }
}