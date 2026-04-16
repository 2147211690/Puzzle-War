using Models;
using Tools;
using UnityEngine;

namespace Controllers
{
    partial class PuzzleController
    {
        public class HammerToolState : State
        {
            public HammerToolState(PuzzleController owner) : base(owner) {}
            public override void OnEnter()
            {
                int count = 0;
                for (int i = 0; i < Owner.PuzzleModel.Size.x; i++)
                {
                    for (int j = 0; j < Owner.PuzzleModel.Size.y; j++)
                    {
                        if (Owner.PuzzleModel[i, j] is { IsEmpty: false, Type: not PieceTypeEnum.Free })
                        {
                            Owner.puzzleView.PuzzlePieceViews[i, j].highlightable.enabled = true;
                            count++;
                        }
                        else
                        {
                            Owner.puzzleView.PuzzlePieceViews[i, j].button.enabled = false;
                        }
                    }
                }
                if (count == 0)
                {
                    Owner._stateMachine.ChangeState(Owner._playState);
                }
            }

            public override void OnExit()
            {
                for (int i = 0; i < Owner.PuzzleModel.Size.x; i++)
                {
                    for (int j = 0; j < Owner.PuzzleModel.Size.y; j++)
                    {
                        if (Owner.PuzzleModel[i, j] is { IsEmpty: false, Type: not PieceTypeEnum.Free })
                        {
                            Owner.puzzleView.PuzzlePieceViews[i, j].highlightable.enabled = false;
                        }
                        else
                        {
                            Owner.puzzleView.PuzzlePieceViews[i, j].button.enabled = true;
                        }
                    }
                }
            }

            public override void Init(Vector2Int puzzleSize, Texture2D texture2D)
            {
            }

            public override void Init(PuzzleModel puzzleModel)
            {
            }

            private void RegesterClick()
            {
                
            }
            public override void OnClickPiece(int id)
            {
                var coords = Owner.PuzzleModel[id];
                Owner.PuzzleModel.SetPieceType(coords, PieceTypeEnum.Free);
                Owner.puzzleView.PuzzlePieceViews[coords.x, coords.y].Type = PieceTypeEnum.Free;
                Owner.puzzleView.PuzzlePieceViews[coords.x, coords.y].highlightable.enabled = false;
                Owner._stateMachine.ChangeState(Owner._playState);
            }

            public override void OnClickBarrier(in Barrier barrier)
            {
                
            }

            public override void OnClickTool(ToolTypeEnum toolType)
            {
                if (toolType == ToolTypeEnum.Hammer) Owner._stateMachine.ChangeState(Owner._playState);
            }

            public override void OnClickEventButton(GameEventEnum gameEvent)
            {
            }
        }
    }
}