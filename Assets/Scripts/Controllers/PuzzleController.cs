using System;
using Models;
using Tools;
using Uis;
using UnityEngine;
using UnityEngine.Serialization;
using Views;

namespace Controllers
{
    public partial class PuzzleController : MonoBehaviour
    {
        public PuzzleView puzzleView = null!;
        [FormerlySerializedAs("startUi")] public PuzzleUi puzzleUi = null!;
        public int shuffleSteps = 150;
        public int CurrentScore { get; private set; }
        public PuzzleModel? PuzzleModel { get; private set; }
        private StateMachine<State> _stateMachine = null!;

        private PlayState _playState;
        private HammerToolState _hammerToolState;
        private ScissorsToolState _scissorsToolState;
        private WinState _winState;
        private WaitState _waitState;

        private void Awake()
        {
            _playState = new PlayState(this);
            _winState = new WinState(this);
            _scissorsToolState = new ScissorsToolState(this);
            _hammerToolState = new HammerToolState(this);
            _waitState = new WaitState(this);
            _stateMachine = new StateMachine<State>(_playState);
        }

        private void Start()
        {
            _stateMachine.Init();
            puzzleView.ToolClicked += OnToolClicked;
            puzzleUi.StartGame += OnPuzzleGame;
            puzzleView.WinComplete += OnWinComplete;
        }

        private void OnPuzzleGame(object sender, EventArgs e)
        {
            Init(PlayerData.CurrentLevel);
        }

        private void OnToolClicked(object sender, ToolTypeEnum e)
        {
            _stateMachine.CurrentState.OnClickTool(e);
        }
        private void OnWinComplete(object sender, EventArgs e)
        {
            _stateMachine.CurrentState.OnWinComplete();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _stateMachine.CurrentState.OnClickTool(ToolTypeEnum.Hammer);
            }
            if (Input.GetKeyDown(KeyCode.S))
                _stateMachine.CurrentState.OnClickTool(ToolTypeEnum.Scissors);
        }

        public void Init(Vector2Int puzzleSize, Texture2D texture2D)
        {
            _stateMachine.CurrentState.Init(puzzleSize, texture2D);
        }
        public void Init(PuzzleModel puzzleModel)
        {
            _stateMachine.CurrentState.Init(puzzleModel);
        }

        public void Init(int level)
        {
            var puzzleData = Resources.Load<PuzzleData>($"LevelData/{level}");
            Init(new PuzzleModel(puzzleData));
        }

        private void OnClickPiece(int id)
        {
            _stateMachine.CurrentState.OnClickPiece(id);
        }
        private void OnClickBarrier(Barrier barrier)
        {
            _stateMachine.CurrentState.OnClickBarrier(barrier);
        }
        
        private bool TrySwapCoords(Vector2Int coords)
        {
            if (!coords.InSize(PuzzleModel.Size)) return false;
            var piece = PuzzleModel[coords];
            if (piece.IsEmpty || piece.Type == PieceTypeEnum.Fixed) return false;
            var dir = PuzzleTools.GetTypePieceMoveDirections(piece.Type);
            for (int i = 0; i < dir.Count; i++)
            {
                var checkCoords = coords + dir[i];
                if (!checkCoords.InSize(PuzzleModel.Size) ||
                    !PuzzleModel[checkCoords].IsEmpty ||
                    PuzzleModel.IsBarrier(new(coords, checkCoords))) continue;
                PuzzleModel.Swap(coords, checkCoords);
                puzzleView.SwapPiece(coords, checkCoords);
                PuzzleModel.StepCount++;
                return true;
            }
            return false;
        }

        private bool CheckWin()
        {
            for (int i = 0; i < PuzzleModel.Size.x; i++)
            {
                for (int j = 0; j < PuzzleModel.Size.y; j++)
                {
                    if (!PuzzleModel[i, j].IsEmpty && PuzzleModel[i, j].Id != i + j * PuzzleModel.Size.x) return false;
                }
            }
            return true;
        }
    }
}