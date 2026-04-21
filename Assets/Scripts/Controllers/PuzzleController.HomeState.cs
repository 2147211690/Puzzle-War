using Models;
using Tools;
using UnityEngine;

namespace Controllers
{
    partial class PuzzleController
    {
        public class HomeState : State
        {
            public HomeState(PuzzleController owner) : base(owner) {}
            public override void OnEnter(IState prevState)
            {
                Owner.puzzleUi.homeUi.SetActive(true);
                Owner.puzzleUi.gammingUi.SetActive(false);
                Owner.Init(PlayerData.CurrentLevel);
                Owner.puzzleUi.titleText.text = $"当前:{PlayerData.CurrentLevel + 1}关";
                
                DyAdManager.Instance.ShowBannerAd();
            }

            public override void OnExit(IState nextState)
            {
                Owner.puzzleUi.homeUi.SetActive(false);
                Owner.puzzleUi.gammingUi.SetActive(true);
            }

            public override void Init(Vector2Int puzzleSize, Texture2D texture2D)
            {
            }

            public override void Init(PuzzleModel puzzleModel)
            {
                Owner.puzzleView.SetPuzzlePreview(puzzleModel, PlayerData.CurrentLevel < PlayerData.MaxUnlockLevel);
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

            public override void OnStartGame()
            {
                base.OnStartGame();
                Owner._stateMachine.ChangeState(Owner._playState);
                Owner.Init(PlayerData.CurrentLevel);
            }

            public override void OnSelectLevel(int e)
            {
                base.OnSelectLevel(e);
                OnEnter(Owner._homeState);
            }

            public override void OnClickTool(ToolTypeEnum toolType)
            {
                if (toolType == ToolTypeEnum.Hammer)
                {
                    DyAdManager.Instance.ShowReward(r =>
                    {
                        if (r) Owner.HammerCountMV++;
                    });
                }
                else if (toolType == ToolTypeEnum.Scissors)
                {
                    DyAdManager.Instance.ShowReward(r =>
                    {
                        if (r) Owner.ScissorsCountMV++;
                    });
                }
            }
        }
    }
}