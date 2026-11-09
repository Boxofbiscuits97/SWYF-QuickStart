using System.Reflection;
using BepInEx;
using BepInEx.Logging;

namespace QuickStart;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;

    private const string GAME_SCENE_NAME = "ScammerGetScammed";
    private static bool QuickStartCompleted = false;

    public static int QuickStartLobbyType;
    public static bool QuickStartEnabled;


    public enum LobbyType
    {
        Public,
        Private,
        FriendsOnly
    }

    private void Awake()
    {
        Log = Logger;

        QuickStartEnabled = Config.Bind("General", "Enabled", true, "Whether QuickStart is enabled").Value;
        QuickStartLobbyType = (int)Config.Bind("General", "LobbyType", LobbyType.Private, "The visibility type of the lobby").Value;

        Log.LogInfo($"Plugin {Name} is loaded!");
    }

    private void Update()
    {
        if (!QuickStartEnabled || QuickStartCompleted)
            return;

        QuickStartCompleted = QuickStartGame();
    }

    private bool QuickStartGame()
    {
        var mainMenu = FindAnyObjectByType<MainMenuUI>();
        if (mainMenu == null)
        {
            Logger.LogError("No MainMenuUI instance was found.");
            return false;
        }

        var method = typeof(MainMenuUI).GetMethod(
            "TryHostGame",
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(int), typeof(string)],
            null);

        if (method == null)
        {
            Logger.LogError("No TryHostGame method was found.");
            return false;
        }

        bool result = (bool)method.Invoke(mainMenu, [QuickStartLobbyType, GAME_SCENE_NAME]);
        return result;
    }
}
