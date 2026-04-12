using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public static class UGSLeaderboardService
{
    private const string LEADERBOARD_ID = "2D_TANKS_LEADERBOARD";
    private static bool m_Initialized = false;

    public static async Task InitializeAsync()
    {
        if (m_Initialized) return;

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        m_Initialized = true;
    }

    public static async Task SubmitScoreAsync(string playerName, int stage)
    {
        try
        {
            await InitializeAsync();

            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);

            await LeaderboardsService.Instance.AddPlayerScoreAsync(
                LEADERBOARD_ID, stage
            );

        }
        catch (Exception e)
        {
            Debug.LogError($"Submit error: {e.Message}");
            if (e.InnerException != null)
                Debug.LogError($"Inner: {e.InnerException.Message}");
        }
    }

    public static async Task<List<PlayerData>> GetTopScoresAsync(int limit = 10)
    {
        try
        {
            await InitializeAsync();

            var options = new GetScoresOptions { Limit = limit };
            LeaderboardScoresPage response = await LeaderboardsService.Instance
                .GetScoresAsync(LEADERBOARD_ID, options);

            var result = new List<PlayerData>();
            foreach (var entry in response.Results)
            {
                string name = string.IsNullOrEmpty(entry.PlayerName)
                    ? "Anonymous"
                    : entry.PlayerName;

                result.Add(new PlayerData(name, (int)entry.Score));
            }

            return result;
        }
        catch (Exception e)
        {
            Debug.LogError($"Fetch error: {e.Message}");
            return new List<PlayerData>();
        }
    }
}
