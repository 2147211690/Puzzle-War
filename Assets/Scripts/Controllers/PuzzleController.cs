using System;
using System.Collections.Generic;
using Models;
using Tools;
using TTSDK;
using TTSDK.UNBridgeLib.LitJson;
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
        public int levelCount = 10;
        private StateMachine<State> _stateMachine = null!;

        private PlayState _playState;
        private HammerToolState _hammerToolState;
        private ScissorsToolState _scissorsToolState;
        private WinState _winState;
        private HomeState _homeState;
        private bool _isFromSidebar;

        private int HammerCountMV
        {
            get => PlayerData.HammerCount;
            set
            {
                PlayerData.HammerCount = value;
                puzzleUi.hammerText.text = value.ToString();
            }
        }

        private int ScissorsCountMV
        {
            get => PlayerData.ScissorsCount;
            set
            {
                PlayerData.ScissorsCount = value;
                puzzleUi.scissorsText.text = value.ToString();
            }
        }

        private void Awake()
        {
            _playState = new PlayState(this);
            _winState = new WinState(this);
            _scissorsToolState = new ScissorsToolState(this);
            _hammerToolState = new HammerToolState(this);
            _homeState = new HomeState(this);
            _stateMachine = new StateMachine<State>(_homeState);
        }

        private void Start()
        {
            puzzleView.ToolClicked += OnToolClicked;
            puzzleUi.StartGame += OnStartGame;
            puzzleView.WinComplete += OnWinComplete;
            
            puzzleUi.settingButton.onClick.AddListener(OnSetting);
            puzzleUi.replayButton.onClick.AddListener(OnReplay);
            puzzleUi.homeButton.onClick.AddListener(OnHome);
            puzzleUi.levelButton.onClick.AddListener(OnLevel);
            puzzleUi.enterSideButton.onClick.AddListener(OnEnterSideBarAward);
            puzzleUi.levelPopup.LevelCount = levelCount;
            
            TTInit();
            DateInit();
            _stateMachine.Init();
            
            puzzleUi.hammerText.text = PlayerData.HammerCount.ToString();
            puzzleUi.scissorsText.text = PlayerData.ScissorsCount.ToString();
            
            AudioManager.Instance.PlayBGM("bgm");
        }


        private void DateInit()
        {
            if (PlayerData.IsFirstGame)
            {
                PlayerData.InitDate();
                PlayerData.IsFirstGame = false;
            }
        }
        private void TTInit()
        {
            if (!TT.InContainerEnv) return;
            TT.CheckScene(TTSideBar.SceneEnum.SideBar, b =>
                {
                    Debug.Log("check scene 调用成功," + b);
                    if (b)
                    {
                        Debug.Log("支持侧边栏");
                        puzzleView.sideBarButton.gameObject.SetActive(true);

                    }
                    else
                    {
                        Debug.Log("不支持侧边栏");
                    }
                }, () => { Debug.Log("check scene 接口调用结束的回调函数（调用成功、失败都会执行）"); },
                (errCode, errMsg) => { Debug.Log($"check scene 接口调用失败的回调函数, errCode:{errCode}, errMsg:{errMsg}"); });
            TT.GetAppLifeCycle().OnShow += OnOnShow;

            void OnOnShow(Dictionary<string, object> param)
            {
                //判断用户是否是从侧边栏进来的
                Debug.Log(param);
                foreach (var item in param)
                {
                    Debug.Log($"显示回调 key:{item.Key}\tvalue:{item.Value}");
                }
                _isFromSidebar = (param["launchFrom"].ToString() == "homepage" && param["location"].ToString() == "sidebar_card");
                //if (param.ContainsKey("launch_from") && param.ContainsKey("location"))
                if (_isFromSidebar)
                {
                    Debug.Log("从侧边栏进来的");
                    // 在游戏开始时或用户尝试领取奖励时调用
                    OnEnterSideBarAward();
                }
                else
                {
                    //否则反之
                    Debug.Log("正常进来的");
                }
            }
        }

        private void OnEnterSideBarAward()
        {
            puzzleUi.OpenEnterSideBarAwardPopup(_isFromSidebar && PlayerData.HasEnterSideBarAward, r =>
            {
                if (r == PuzzleUi.EnterSidePopupResult.GetAward)
                {
                    HammerCountMV += 3;
                    ScissorsCountMV += 3;
                    PlayerData.SideBarEnterTime = DateTime.Today;
                }
                else if (r == PuzzleUi.EnterSidePopupResult.Ok)
                {
                    var data = new JsonData
                    {
                        ["scene"] = "sidebar",
                    };
                    TT.NavigateToScene(data, null, null, null);
                }
            });
        }
        
        private void OnLevel()
        {
            puzzleUi.OpenLevelPopup(levelCount, PlayerData.MaxUnlockLevel, PlayerData.CurrentLevel, i =>
            {
                _stateMachine.CurrentState.OnSelectLevel(i);
            });
        }

        private void OnSetting()
        {
            puzzleUi.OpenSettingPopup((r) =>
            {
                AudioManager.Instance.SetBgmVolume(r.BgmVolume);
                AudioManager.Instance.SetSfxVolume(r.SfxVolume);
            });
        }

        private void OnReplay()
        {
            puzzleUi.OpenComfirmPopup("你要重试吗?", c =>
            {
                if (c) _stateMachine.CurrentState.OnReplay();
            });
        }

        private void OnHome()
        {
            puzzleUi.OpenComfirmPopup("你要返回主页吗?", c =>
            {
                if (c) _stateMachine.CurrentState.OnHome();
            });
        }
        
        private void OnStartGame(object sender, EventArgs e)
        {
            _stateMachine.CurrentState.OnStartGame();
        }

        private void OnToolClicked(object sender, ToolTypeEnum e)
        {
            _stateMachine.CurrentState.OnClickTool(e);
        }
        private void OnWinComplete(object sender, EventArgs e)
        {
            _stateMachine.CurrentState.OnWinComplete();
        }
        
        public void Init(Vector2Int puzzleSize, Texture2D texture2D)
        {
            _stateMachine.CurrentState.Init(puzzleSize, texture2D);
            UpdateScore();
        }
        public void Init(PuzzleModel puzzleModel)
        {
            _stateMachine.CurrentState.Init(puzzleModel);
            UpdateScore();
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
            if (PuzzleModel is null) return false;
            var r = PuzzleModel.GetValidSwapCoords(coords);
            if (r == null) return false;
            PuzzleModel.Swap(coords, r.Value);
            puzzleView.SwapPiece(coords, r.Value);
            PuzzleModel.StepCount++;
            UpdateScore();
            return true;
        }

        private void UpdateScore()
        {
            if (PuzzleModel is null) return;
            PuzzleModel.Score = PuzzleModel.CalculateScore();
            puzzleUi.scoreText.text = PuzzleModel.Score.ToString();
        }
        private bool CheckWin()
        {
            if (PuzzleModel is null) return false;
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