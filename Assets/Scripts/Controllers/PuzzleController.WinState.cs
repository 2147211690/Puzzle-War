using Models;
using Tools;
using Uis;
using UnityEngine;

namespace Controllers
{
    partial class PuzzleController
    {
        public class WinState : State
        {
            public WinState(PuzzleController owner) : base(owner) {}
            public override void OnEnter(IState prevState)
            {
                Owner.PuzzleModel.FillAll();
                Owner.puzzleView.Wim(2f);
                AudioManager.Instance.PlaySfx("win");
                PlayerData.CurrentLevel = Mathf.Min(Owner.levelCount - 1, PlayerData.CurrentLevel + 1);
                PlayerData.MaxUnlockLevel = Mathf.Max(PlayerData.MaxUnlockLevel, PlayerData.CurrentLevel);

                SetUi(false);
            }

            private void SetUi(bool value)
            {
                Owner.puzzleUi.replayButton.interactable = value;
                Owner.puzzleUi.homeButton.interactable = value;
                Owner.puzzleUi.settingButton.interactable = value;
            }

            public override void OnExit(IState nextState)
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
            
            public override void OnWinComplete()
            {
                SetUi(true);
                base.OnWinComplete();
                Owner.puzzleUi.OpenWinPopup(Owner.PuzzleModel.Score, r =>
                {
                    if (r == PuzzleUi.WimPopupResult.NextLevel)
                    {
                        Owner._stateMachine.ChangeState(Owner._playState);
                        Owner.Init(PlayerData.CurrentLevel);
                    }
                    else if (r == PuzzleUi.WimPopupResult.Home)
                    {
                        OnHome();
                    }
                });
                DyAdManager.Instance.ShowInter();
            }

            public override void OnHome()
            {
                base.OnHome();
                Owner._stateMachine.ChangeState(Owner._homeState);
            }
        }
    }
}