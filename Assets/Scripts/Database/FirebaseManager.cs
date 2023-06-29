using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager DatabaseManager;

    // firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth Auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;
    private bool _signedIn;

    // user data variables
    

    private void Awake()
    {
        if (DatabaseManager == null)
        {
            DatabaseManager = this;

            DontDestroyOnLoad(this.gameObject);

            return;
        }

        Destroy(this.gameObject);
    }

    private void Start()
    {
        //StartCoroutine(InitializeAndLogin());
    }

    public IEnumerator InitializeAndLogin()
    {
        Debug.Log("Initializing Firebase and logging in...");
        yield return StartCoroutine(InitializeFirebase());

        if (_signedIn)
        {
            Debug.Log("Logged in " + User.UserId);
            MenuUIManager.instance.UserDataScreen();
        }
    }

    private void OnDestroy()
    {
        if (Auth == null)
            return;

        Auth.StateChanged -= AuthStateChanged;
        Auth = null;
    }

    private IEnumerator InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");

        // Set the authentication instance object
        Auth = FirebaseAuth.DefaultInstance;
        Auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

        DBreference = FirebaseDatabase.DefaultInstance.RootReference;

        // Wait until the dependency check is complete
        yield return FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependencyStatus == DependencyStatus.Available)
        {
            Debug.Log("Firebase initialized successfully.");
        }
        else
        {
            Debug.LogError($"Could not resolve all Firebase dependencies {dependencyStatus}");
        }
    }

    private void AuthStateChanged(object sender, System.EventArgs eventArgs) 
    {
        if (Auth.CurrentUser != User) 
        {
            _signedIn = User != Auth.CurrentUser && Auth.CurrentUser != null;

            if (!_signedIn && User != null) 
            {
                Debug.Log("Signed out " + User.UserId);
            }

            User = Auth.CurrentUser;
        }
    }

    private IEnumerator LoadUserData()
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            // No data exists yet
            //_playerScoreField.text = "0";
        }
        else
        {
            // Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            //_playerScoreField.text = snapshot.Child("time").Value.ToString();
        }
    }

    public IEnumerator UpdateUsernameAuth(string username)
    {
        // Create a user profile and set the username
        UserProfile profile = new UserProfile {DisplayName = username};

        // Call the firebase auth update user profile function passing the profile with the username
        var ProfileTask = User.UpdateUserProfileAsync(profile);

        // Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            // Auth username is now updated
            Debug.Log("Auth completed!");
        }
    }

    public IEnumerator UpdateUsernameDatabase(string username)
    {   
        // firebase query to get username and set value
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database username is now updated
            Debug.Log("Username updated");
        }
    }
}
