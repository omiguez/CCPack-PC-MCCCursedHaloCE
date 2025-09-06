using System.Net.Http;
using CcLog = CrowdControl.Common.Log;

namespace CrowdControl.Games.Packs.MCCCursedHaloCE;

/// <summary>
/// Monitors race progress and reports level/stage completion to a remote server.
/// This class is designed to be run in its own thread.
/// </summary>
internal class RaceProgress
{
   private readonly string _username;
    private readonly string _password;
    private readonly MCCCursedHaloCE _mccCursedHaloCe;

    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "https://cursedhaloforcharity.com/Race/";

    /// <summary>
    /// Initializes a new instance of the RaceProgress class.
    /// </summary>
    public RaceProgress(string username, string password, MCCCursedHaloCE mccCursedHaloCe)
    {
        _username = username;
        _password = password;
        _mccCursedHaloCe = mccCursedHaloCe;
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Starts the monitoring process in a new background thread.
    /// </summary>
    public void StartMonitoring()
    {
        CcLog.Message($"Starting monitoring for racer: {_username}...");
        Task loopTask = Task.Run(MonitorLoop);
        CcLog.Message("Monitoring started. The application will continue running in the background.");
        loopTask.Wait();
    }

    /// <summary>
    /// The main loop that periodically checks for progress and reports it.
    /// </summary>
    private async Task MonitorLoop() {
        byte[]? initialStatus;
        while (!_mccCursedHaloCe.GetRaceStatus(out initialStatus) || initialStatus == null) {
            Thread.Sleep(1000);
        }
        
        int lastLevel = initialStatus[1];
        int lastStage = initialStatus[2];
        while (true)
        {
            try
            {
                if (!_mccCursedHaloCe.GetRaceStatus(out byte[]? status) || status == null)
                    continue;
               
                int currentLevel = status[1];
                int currentStage = status[2];
                
                if (currentLevel == lastLevel && currentStage == lastStage)
                    continue;

                CcLog.Message($"Detected progress: Level {currentLevel}, Stage {currentStage}. Reporting to server...");
                lastLevel = currentLevel;
                lastStage = currentStage;

                // Report the progress to the web server
                await SetLevel(currentLevel, currentStage);
            }
            catch (Exception ex)
            {
                CcLog.Message($"An error occurred in the monitoring loop: {ex.Message}");
            }

            // Wait for a specified interval before checking again to avoid spamming the server.
            Thread.Sleep(TimeSpan.FromSeconds(30));
        }
    }

    /// <summary>
    /// Sends the current race progress to the server endpoint.
    /// </summary>
    /// <param name="level">The current level.</param>
    /// <param name="stage">The current stage.</param>
    private async Task SetLevel(int level, int stage)
    {
        // Construct the full URL with query parameters
        string requestUrl = $"{ApiBaseUrl}SetLevel?racerName={Uri.EscapeDataString(_username)}&level={level}&stage={stage}&password={Uri.EscapeDataString(_password)}";

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                CcLog.Message("Successfully reported progress to the server.");
            }
            else
            {
                // Log the error response from the server
                string errorContent = await response.Content.ReadAsStringAsync();
                CcLog.Message($"Failed to report progress. Server responded with {response.StatusCode}: {errorContent}");
            }
        }
        catch (HttpRequestException e)
        {
            CcLog.Message($"Error making the GET request: {e.Message}");
        }
    }

    #region Placeholder Methods
    // These methods are placeholders. You should replace their logic with
    // how you actually determine the racer's progress.

    private int GetCurrentLevelFromGame()
    {
        // Example: read from a game file, memory, etc.
        // Returning a random number for demonstration purposes.
        return new Random().Next(1, 10);
    }

    private int GetCurrentStageFromGame()
    {
        // Example: read from a game file, memory, etc.
        // Returning a random number for demonstration purposes.
        return new Random().Next(1, 5);
    }
    #endregion
}
