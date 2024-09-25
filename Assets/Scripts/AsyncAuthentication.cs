using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

public class AsyncAuthentication : MonoBehaviour
{
    private async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();

            // Check if the player is already signed in
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("Player is already signed in.");
            }
            else
            {
                // Start the sign-in process if the player is not signed in
                Debug.Log("Player is not signed in. Initiating sign-in process.");
                await StartSignIn();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        // Attach event handlers
        PlayerAccountService.Instance.SignedIn += OnSigningIn;
        SetupOtherEvents();
    }
    
    [Button]
    public async Task StartSignIn()
    {
        // Initiate the sign-in process
        await PlayerAccountService.Instance.StartSignInAsync();
    }

    // Method called by the SignedIn event
    private async void OnSigningIn()
    {
        try
        {
            string accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(accessToken);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    // Handles the sign-in
    private async Task SignInWithUnityAsync(string accessToken)
    {
        // Sign in with the Unity authentication service using the access token
        await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
        
        // Set a default player name
        await SetPlayerNameAsync("LordJNeto");

        // Log sign-in details
        Debug.Log("SignIn is successful.");
        Debug.Log($"Player Name: {AuthenticationService.Instance.PlayerName}");
        Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
        Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
    }

    // Setup additional authentication event handlers
    private void SetupOtherEvents() 
    {
        AuthenticationService.Instance.SignInFailed += (err) => 
        {
            Debug.LogWarning("ERROR ON PLAYER SIGN IN!!!");
            Debug.LogError(err);
        };
        AuthenticationService.Instance.SignedOut += () => 
        {
            Debug.Log("Player signed out.");
        };
        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };
    }

    private async Task SetPlayerNameAsync(string playerName)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
            Debug.Log($"Player name set to: {playerName}");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError($"Failed to set player name: {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        PlayerAccountService.Instance.SignedIn -= OnSigningIn;
    }
}