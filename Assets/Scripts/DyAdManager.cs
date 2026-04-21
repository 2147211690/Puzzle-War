
using System;
using TTSDK;
using UnityEngine;
using UnityEngine.Serialization;


/// <summary>
/// 定义一个管理微信广告的类
/// </summary>
public class DyAdManager : MonoBehaviour
{
    #region 广告ID
    public string bannerAdID = "1";
    public string rewardAdID = "2";
    public string interAdId = "3";
    #endregion
    
    public static DyAdManager Instance { get; private set; } = null!;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); 
            return;
        } 
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region 插屏广告
    // 插屏广告对象
    private TTInterstitialAd? _newInter;
    /// <summary>
    /// 初始化插屏广告的方法
    /// </summary>
    private void InitInter()
    {
        if (_newInter != null) return;
        try
        {
            Debug.Log("构建插屏广告");
            // 创建插屏广告实例
            _newInter = TT.CreateInterstitialAd(new CreateInterstitialAdParam { InterstitialAdId = interAdId });//  WX.CreateInterstitialAd(new WXCreateInterstitialAdParam
            Debug.Log("注册插屏广告相关事件");
            _newInter.OnClose += NewInterOnClose;
            _newInter.OnLoad += NewInterOnLoad;
            _newInter.OnError += NewInterOnError;
            _newInter.Load();
        }
        catch (Exception ex)
        {
            Debug.Log("显示插屏广告的方法:" + ex.Message);
            _newInter = null;
        }
        
        void NewInterOnError(int code, string message)
        {
            Debug.Log($"错误 ： {code}  {message}");
        }

        void NewInterOnLoad()
        {
            Debug.Log($"{DateTime.Now}:插屏广告加载");
            _newInter.Show();
        }

        void NewInterOnClose()
        {
            Debug.Log("插屏广告关闭");
            _newInter?.Destroy();
            _newInter = null;
        }
    }

    /// <summary>
    /// 显示插屏广告
    /// </summary>
    public void ShowInter()
    {
        InitInter();
    }

    
    #endregion

    #region 激励广告
    // 激励广告对象
    private TTRewardedVideoAd? _newReward;
    /// <summary>
    /// 用户看完广告的回调事件
    /// </summary>
    private Action<bool>? _showRewardEvent;
    /// <summary>
    /// 初始化激励广告
    /// </summary>
    private void InitReward()
    {
        if (_newReward != null) return;
        try
        {
            // 创建激励广告实例
            _newReward = TT.CreateRewardedVideoAd(new CreateRewardedVideoAdParam() { AdUnitId = rewardAdID }); //  WX.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam()
            _newReward.OnClose += NewRewardOnClose;
            _newReward.OnError += NewRewardOnError;
            _newReward.OnLoad += NewRewardOnLoad;
            _newReward.Load();
            _newReward.Show();
        }
        catch (Exception ex)
        {
            Debug.Log("初始化激励广告:" + ex.StackTrace);
            _showRewardEvent?.Invoke(false);
            _showRewardEvent = null;
            _newReward?.Destroy();
            _newReward = null;
        }

        void NewRewardOnLoad()
        {
            Debug.Log("激励视频加载成功");
            
        }

        void NewRewardOnError(int code, string message)
        {
            Debug.Log($"激励视频错误 errorCode: {code}\terrorMessage:{message}");
        }

        void NewRewardOnClose(bool isEnded, int count)
        {
            Debug.Log($"激励视频关闭 ended: {isEnded}, count: {count}");
            if (isEnded)
            {
                // 用户看完了广告，发放奖励
                Debug.Log("用户看完了广告，需要发放奖励");
            }
            else
            {
                Debug.Log("用户没有看完广告，没有奖励");
            }
            // 触发回调事件
            _showRewardEvent?.Invoke(isEnded);
            _showRewardEvent = null;
            _newReward.Destroy();
            _newReward = null;
        }
    }

    
    /// <summary>
    /// 显示激励广告
    /// </summary>
    public void ShowReward(Action<bool>? rewardEvent)
    {
        _showRewardEvent += rewardEvent;
        InitReward();
    }
    #endregion

    #region 原生广告
    private TTBannerAd? _bannerAd;
    private void InitBanner()
    {
        if (_bannerAd != null) return;
        Debug.Log("初始化Banner广告");
        try
        {
            _bannerAd = TT.CreateBannerAd(new CreateBannerAdParam() { AdIntervals = 30, BannerAdId = bannerAdID });
            _bannerAd.OnClose += BannerAdOnOnClose;
            _bannerAd.OnLoad += BannerAdOnOnLoad;
        }
        catch (Exception ex)
        {
            Debug.Log("初始化Banner广告:" + ex.StackTrace);
            _bannerAd = null;
        }
        void BannerAdOnOnLoad()
        {
            _bannerAd.Show();
        }
        void BannerAdOnOnClose()
        {
            _bannerAd?.Destroy();
            _bannerAd = null;
        }
    }

    /// <summary>
    /// 显示原生广告
    /// </summary>
    public void ShowBannerAd()
    {
        InitBanner();
    }
    /// <summary>
    /// 隐藏原生广告
    /// </summary>
    public void HideBannerAd()
    {
        _bannerAd?.Hide();
    } 
    #endregion

}