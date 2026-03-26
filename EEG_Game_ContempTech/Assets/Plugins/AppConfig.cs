/// <summary>
/// Contain configuration of a specific App.
/// </summary>
public static class AppConfig
{
    /*
    * Enter your application Client ID and Client Secret below.
    * You can obtain these credentials after registering your App ID with the Cortex SDK for development.
    * For instructions, visit: https://emotiv.gitbook.io/cortex-api#create-a-cortex-app
    */
    public static string ClientId = "sWPiqsQ94GOOoXLPgTAGumnxRQhKj4f9BWaNNuzT";
    public static string ClientSecret = "X8v6JIytCBbOJLbZueQrghuh8svJNwR38MvfnjYDVXHroLQzL3TggvT6aEYc67zLNPeDv1MEtKYnUCE0k98n099lqK6FuC5k353hDrIRGi5hFO9B6twWeS9puBjzyPtB";
    public static string LicenseKey = "3875537d-08c7-4061-adf2-b2fb72a5532e";
    public static string AppVersion = "1.1.0";
    public static string AppName = "ulstergames";
    public static bool IsDataBufferUsing = false; // Set false if you want to display data directly to MessageLog without storing in Data Buffer
    public static bool AllowSaveLogToFile = false; // Set true to save log to file and cortex token to local file for next time use

    #if !USE_EMBEDDED_LIB && !UNITY_ANDROID && !UNITY_IOS
    // only for desktop without embedded cortex
    public static string AppUrl = "wss://localhost:6868"; // for desktop without embedded cortex
    #else
    public static string AppUrl = ""; // Don't need AppUrl for mobile and embedded cortex
    #endif

}