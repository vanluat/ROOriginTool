public enum EDevicePermissionResult
{
    None,
    Authorized,             // 用户允许
    Denied,                 // 用户拒绝
    NotDetermined,          // 用户没有做出选择(iOS)
    DeniedAndNoAsk,         // 用户拒绝并且不在询问(Android)
}