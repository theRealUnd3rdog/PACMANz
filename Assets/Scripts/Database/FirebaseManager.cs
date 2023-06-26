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
    // firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth Auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    // login variables
    [Header("Login")]
    [SerializeField] private TMP_InputField _emailLoginField;
    [SerializeField] private TMP_InputField _passwordLoginField;
    [SerializeField] private TMP_Text _warningLoginText;
    [SerializeField] private TMP_Text _confirmLoginText;

    // register variables
    [Header("Register")]
    [SerializeField] private TMP_InputField _usernameRegisterField;
    [SerializeField] private TMP_InputField _emailRegisterField;
    [SerializeField] private TMP_InputField _passwordRegisterField;
    [SerializeField] private TMP_InputField _passwordRegisterVerifyField;
    [SerializeField] private TMP_Text _warningRegisterText;

    // user data variables
    [Header("User Data")]
    [SerializeField] private TMP_InputField _playerScoreField;
    [SerializeField] private Transform _scoreboardContent;
    [SerializeField] private PlayerScoreUI _scoreElement;

    private void Awake()
    {
        // Check that all of the necessary dependancies for firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => 
        {
            dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                // if they are available initialize firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependancies {dependencyStatus}");
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");

        // Set the authentication instance object
        Auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void ClearLoginFields()
    {
        _emailLoginField.text = string.Empty;
        _passwordLoginField.text = string.Empty;
    }

    public void ClearRegisterFields()
    {
        _usernameRegisterField.text = string.Empty;
        _emailRegisterField.text = string.Empty;
        _passwordRegisterField.text = string.Empty;
        _passwordRegisterVerifyField.text = string.Empty;
    }

    public void LoginButton()
    {
        // Call the login coroutine passing the email and password;
        StartCoroutine(Login(_emailLoginField.text, _passwordLoginField.text));
    }

    public void RegisterButton()
    {
        // Call the register coroutine passing the email, password and username
        StartCoroutine(Register(_emailRegisterField.text, _passwordRegisterField.text, _usernameRegisterField.text));
    }

    public void SignOutButton()
    {
        Auth.SignOut();

        UIManager.instance.LoginScreen();
        ClearLoginFields();
        ClearRegisterFields();
    }

    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(Auth.CurrentUser.DisplayName));
        StartCoroutine(UpdateUsernameDatabase(Auth.CurrentUser.DisplayName));

        StartCoroutine(UpdateScore(int.Parse(_playerScoreField.text)));
    }

    public void ScoreBoardButton()
    {
        StartCoroutine(LoadScoreboardData());
    }

    public void ResetScoreButton()
    {
        StartCoroutine(DeleteUserScore());
    }

    private IEnumerator Login(string email, string password)
    {
        // Call the Firebase auth signin function passing the email and password
        var LoginTask = Auth.SignInWithEmailAndPasswordAsync(email, password);

        // wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            // If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseException = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseException.ErrorCode;

            string message = "Login Failed!";

            switch(errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;

                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;

                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;

                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }

            _warningLoginText.text = message;
        }
        else
        {
            // User is now successfully logged in

            // Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            _warningLoginText.text = string.Empty;
            _confirmLoginText.text = "Logged In";

            StartCoroutine(LoadUserData());

            yield return new WaitForSecondsRealtime(1.5f);

            UIManager.instance.UserDataScreen();
            _confirmLoginText.text = string.Empty;

            ClearLoginFields();
            ClearRegisterFields();
        }
    }

    private IEnumerator Register(string email, string password, string username)
    {
        if (username == string.Empty)
        {
            // if the username field is blank show a warning
            _warningRegisterText.text = "Missing Username";
        }
        else if (_passwordRegisterField.text != _passwordRegisterVerifyField.text)
        {
            // if the password does not match show a warning
            _warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            // Call the Firebase auth signin function passing the email and password
            var RegisterTask = Auth.CreateUserWithEmailAndPasswordAsync(email, password);

            // Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                // If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseException = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseException.ErrorCode;

                string message = "Register Failed!";
                
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;

                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;

                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }

                _warningRegisterText.text = message;
            }
            else
            {
                // user has been created, grab result

                User = RegisterTask.Result;

                if (User != null)
                {
                    // Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = username};

                    // Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);

                    // Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        // if there are errors handle them
                        
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseException = ProfileTask.Exception.GetBaseException() as FirebaseException;

                        AuthError errorCode = (AuthError)firebaseException.ErrorCode;
                        _warningRegisterText.text = $"Username set failed {errorCode}";
                    }
                    else
                    {
                        // user is set, return to login screen
                        UIManager.instance.LoginScreen();
                        _warningRegisterText.text = string.Empty;

                        ClearLoginFields();
                        ClearRegisterFields();
                    }
                }
            }
        }
    }

    private IEnumerator UpdateUsernameAuth(string username)
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

    private IEnumerator UpdateUsernameDatabase(string username)
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

    private IEnumerator UpdateScore(int score)
    {
        // firebase query to get score in users and set the value
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("score").SetValueAsync(score);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Score is now updated

            // If score is updated then load the scoreboard data (This is where it crashes)
            StartCoroutine(LoadScoreboardData());
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
            _playerScoreField.text = "0";
        }
        else
        {
            // Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            _playerScoreField.text = snapshot.Child("score").Value.ToString();
        }
    }

    private IEnumerator LoadScoreboardData()
    {
        var DBTask = DBreference.Child("users").OrderByChild("score").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            // Destroy any existing scoreboard elements if present
            foreach (Transform child in _scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            // Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                string score = childSnapshot.Child("score").Value.ToString();

                // Instantiate new scoreboard elements
                PlayerScoreUI scoreBoardElement = Instantiate(_scoreElement, _scoreboardContent);
                scoreBoardElement.NewScoreElement(username, score);
            }

            UIManager.instance.ShowScoreboard();
        }
    }

    private IEnumerator DeleteUserScore()
    {
        // Get the reference to the user data node for the current user
        DatabaseReference userRef = DBreference.Child("users").Child(User.UserId);

        // Delete the user data by removing the entire node
        var deleteTask = userRef.RemoveValueAsync();

        yield return new WaitUntil(() => deleteTask.IsCompleted);

        if (deleteTask.Exception != null)
        {
            Debug.LogWarning($"Failed to delete user data with {deleteTask.Exception}");
        }
        else
        {
            Debug.Log($"User data of {User.UserId} deleted successfully");
        }
    }
}
