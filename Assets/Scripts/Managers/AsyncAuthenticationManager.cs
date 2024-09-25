using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

public class AsyncAuthenticationManager : MonoBehaviour
{
    
    private async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        PlayerAccountService.Instance.SignedIn += OnSigningIn;
        SetupOtherEvents();
    }

    [Button]
    public async Task StartSignIn()
    {
        await PlayerAccountService.Instance.StartSignInAsync();
    }
    
    // Method called by te SignedIn event.
    private async void OnSigningIn()
    {
        try
        {
            string accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(accessToken);
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
    
    // Handles the signing.
    private async Task SignInWithUnityAsync(string accessToken)
    {
        // Can raise exceptions.
        await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
        // Shows that the log-in worked fine.
        Debug.Log("SignIn is successful.");
        // Shows how to get a playerName
        Debug.Log($"Player Name: {AuthenticationService.Instance.PlayerName}");
        // Shows how to get a playerID
        Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
        // Shows how to get an access token
        Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
    }
    
    // Setup authentication event handlers if desired
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
    
    private void OnDestroy()
    {
        PlayerAccountService.Instance.SignedIn -= OnSigningIn;
    }
    
}
