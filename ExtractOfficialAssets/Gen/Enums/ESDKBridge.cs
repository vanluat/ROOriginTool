namespace ExtractOfficialAssets.Gen.Enums;

public enum ESDKBridge
{
    None,
    Announce,           // 公告           [DefaultAnncounce,MSDKAnncounce]
    CrashReport,        // 错误日志上报   [BuglyCrashReport,FirebaseCrashReport]
    Device,             // joyyousdk      [AndroidDevice,IOSDevice,PCDevice]
    Login,              // 登录           [MSDKLogin,JoyyouLogin]
    Maple,              // 区服导航       [GCloudMaple]
    Pay,                // 支付           [MidasPay,JoyyouPay]
    Statistics,         // 数据统计       [GemStatistics,FirebaseStatistics]
    Tss,                // 屏蔽字 腾讯    [Tss]
    Voice,              // 语音           [GVoice]
    HotUpdate,          // 热更新         [MInternalFixer,MDolphinFixer,MLeBianFixer,MNoneFixer]
    PushNotification,   // 推送           [FirebasePushNotification]
    Share,              // 第三方分享     [JoyyouShare]
    Tracker,            // 追踪事件       [AdjustTracker]
    PkgDownloader,      // obb下载        [GooglePlayObbDownloader][CdnObbDownloader]
    HttpDNS,
    WebView,
    Friend,
    CustomService,
    Security
}