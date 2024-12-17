using System.Collections.Generic;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public static class PlayFabLogin
{
    // Static class to hold user data globally
    public static class UserData
    {
        public static string Role { get; set; }
        public static string Subject { get; set; }
    }

    public static async Task<bool> Login(string email, string password)
    {
        var tcs = new TaskCompletionSource<bool>();

        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest()
        {
            Email = email,
            Password = password
        },
        async result =>
        {
            Debug.Log("Logged in successfully");

            // Fetch and set user data after successful login
            bool dataFetched = await FetchUserData();
            tcs.SetResult(dataFetched);
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
            tcs.SetResult(false); // Set result to false on failure
        });

        return await tcs.Task; // Return true or false
    }

    public static async Task<bool> Register(string email, string password, string username, string role, string subject = "")
    {
        var tcs = new TaskCompletionSource<bool>();

        // Register the user
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest()
        {
            Email = email,
            Password = password,
            Username = username
        },
        async result =>
        {
            Debug.Log("Registered successfully");

            // Upload additional data after successful registration
            bool dataUploaded = await UploadUserData(role, subject);
            tcs.SetResult(dataUploaded);
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
            tcs.SetResult(false);
        });

        return await tcs.Task;
    }

    private static Task<bool> UploadUserData(string role, string subject)
    {
        var tcs = new TaskCompletionSource<bool>();

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { "Role", role },
                { "Subject", subject }
            }
        },
        result =>
        {
            Debug.Log("User data uploaded successfully");
            tcs.SetResult(true);
        },
        error =>
        {
            Debug.LogError("Failed to upload user data: " + error.GenerateErrorReport());
            tcs.SetResult(false);
        });

        return tcs.Task;
    }

    private static Task<bool> FetchUserData()
    {
        var tcs = new TaskCompletionSource<bool>();

        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            Keys = null // Retrieve all data
        },
        result =>
        {
            Debug.Log("User data retrieved successfully");

            if (result.Data.ContainsKey("Role"))
            {
                UserData.Role = result.Data["Role"].Value;
                Debug.Log("Role: " + UserData.Role);
            }

            if (result.Data.ContainsKey("Subject"))
            {
                UserData.Subject = result.Data["Subject"].Value;
                Debug.Log("Subject: " + UserData.Subject);
            }

            tcs.SetResult(true);
        },
        error =>
        {
            Debug.LogError("Failed to fetch user data: " + error.GenerateErrorReport());
            tcs.SetResult(false);
        });

        return tcs.Task;
    }
}
