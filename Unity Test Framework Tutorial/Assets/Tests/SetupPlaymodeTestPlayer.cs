/*
    https://unity.com/how-to/automated-tests-unity-test-framework#splitting-build-and-run
*/

using System.IO;
using UnityEditor;
using UnityEditor.TestTools;

[assembly: TestPlayerBuildModifier(typeof(SetupPlaymodeTestPlayer))]
public class SetupPlaymodeTestPlayer : ITestPlayerBuildModifier
{
    // Occurs when tests are run in Play Mode
    public BuildPlayerOptions ModifyOptions(BuildPlayerOptions playerOptions)
    {
        // Disables auto-run for built players and skips the player option that tries to connect to the hot it is running on
        playerOptions.options &= ~(BuildOptions.AutoRunPlayer | BuildOptions.ConnectToHost);

        // Changes the build path location to a dedicated path within the project ("TestPlayers")
        var buildLocation = Path.GetFullPath("TestPlayers");
        var fileName = Path.GetFileName(playerOptions.locationPathName);
        if (!string.IsNullOrEmpty(fileName))
            buildLocation = Path.Combine(buildLocation, fileName);
        playerOptions.locationPathName = buildLocation;

        return playerOptions;
    }
}