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
            public abstract void OnEnter(IState prevState);
            public abstract void OnExit(IState nextState);
            public abstract void Init(Vector2Int puzzleSize, Texture2D texture2D);
            public abstract void Init(PuzzleModel puzzleModel);
            public abstract void OnClickPiece(int id);
            public abstract void OnClickBarrier(in Barrier barrier);
            public abstract void OnClickTool(ToolTypeEnum toolType);
            public virtual void OnWinComplete(){}
            public virtual void OnReplay(){}
            public virtual void OnStartGame(){}
            public virtual void OnHome(){}
            public virtual void OnSelectLevel(int e)
            {
                Owner.puzzleUi.titleText.text = $"当前:{e + 1}关";
                PlayerData.CurrentLevel = e;
            }
            public virtual void OnSettingChanged(in (float SfxVolume, float BgmVolume) e)
            {
                AudioManager.Instance.SetSfxVolume(e.SfxVolume);
                AudioManager.Instance.SetBgmVolume(e.BgmVolume);
            }
        }
    }
}