using Models;
using Tools;
using UnityEngine;
using Views;

namespace Controllers
{
    public partial class PuzzleController : MonoBehaviour
    {
        public PuzzleView puzzleView = null!;
        public int shuffleSteps = 150;
        private PuzzleModel _puzzleModel = null!;
        private StateMachine<State> _stateMachine = null!;

        private PlayState _playState;
        private HammerToolState _hammerToolState;
        private ScissorsToolState _scissorsToolState;

        private void Awake()
        {
            _playState = new PlayState(this);
            _scissorsToolState = new ScissorsToolState(this);
            _hammerToolState = new HammerToolState(this);
            _stateMachine = new StateMachine<State>(_playState);
        }

        private void Start()
        {
            _stateMachine.Init();
            puzzleView.ToolClicked += OnToolClicked;
        }

        private void OnToolClicked(object sender, ToolTypeEnum e)
        {
            _stateMachine.CurrentState.OnClickTool(e);
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
            if (!coords.InSize(_puzzleModel.Size)) return false;
            var piece = _puzzleModel[coords];
            if (piece.IsEmpty || piece.Type == PieceTypeEnum.Fixed) return false;
            var dir = PuzzleTools.GetTypePieceMoveDirections(piece.Type);
            for (int i = 0; i < dir.Count; i++)
            {
                var checkCoords = coords + dir[i];
                if (!checkCoords.InSize(_puzzleModel.Size) ||
                    !_puzzleModel[checkCoords].IsEmpty ||
                    _puzzleModel.IsBarrier(new(coords, checkCoords))) continue;
                _puzzleModel.Swap(coords, checkCoords);
                puzzleView.SwapPiece(coords, checkCoords);
                _puzzleModel.StepCount++;
                return true;
            }
            return false;
        }

        private bool CheckWin()
        {
            for (int i = 0; i < _puzzleModel.Size.x; i++)
            {
                for (int j = 0; j < _puzzleModel.Size.y; j++)
                {
                    if (!_puzzleModel[i, j].IsEmpty && _puzzleModel[i, j].Id != i + j * _puzzleModel.Size.x) return false;
                }
            }
            return true;
        }
    }
}