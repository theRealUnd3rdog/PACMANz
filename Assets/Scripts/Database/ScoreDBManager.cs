using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;

public class ScoreDBManager : MonoBehaviour
{
    public static ScoreDBManager Instance;

    [Header("User Data")]
    [SerializeField] private Transform _scoreboardContent;
    [SerializeField] private PlayerScoreUI _scoreElement;

    private void Awake()
    {
        Instance = this;
    }

    public void ScoreBoardButton()
    {
        StartCoroutine(LoadScoreboardData());
    }

    public void ResetScoreButton()
    {
        StartCoroutine(DeleteUserScore());
    }

    public void SaveData(float time)
    {
        string displayName = FirebaseManager.DatabaseManager.Auth.CurrentUser.DisplayName;

        StartCoroutine(FirebaseManager.DatabaseManager.UpdateUsernameAuth(displayName));
        StartCoroutine(FirebaseManager.DatabaseManager.UpdateUsernameDatabase(displayName));

        StartCoroutine(LoadUserData(time));
    }

    private IEnumerator LoadUserData(float currentTime)
    {
        DatabaseReference DBreference = FirebaseManager.DatabaseManager.DBreference;
        FirebaseUser User = FirebaseManager.DatabaseManager.User;

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

            var timeObj = snapshot.Child("time");

            if (timeObj.Exists)
            {
                Debug.Log($"The {timeObj.Value}");
                float time = float.Parse(timeObj.Value.ToString());

                if (currentTime < time)
                    StartCoroutine(UpdateTime(currentTime));
            }
            else
            {
                StartCoroutine(UpdateTime(currentTime));
            }
        }
    }

    private IEnumerator UpdateTime(float score)
    {
        DatabaseReference DBreference = FirebaseManager.DatabaseManager.DBreference;
        FirebaseUser User = FirebaseManager.DatabaseManager.User;

        // firebase query to get score in users and set the value
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("time").SetValueAsync(score);

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

    private IEnumerator LoadScoreboardData()
    {
        DatabaseReference DBreference = FirebaseManager.DatabaseManager.DBreference;

        var DBTask = DBreference.Child("users").OrderByChild("time").GetValueAsync();

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

            // Loop through every users UID << or you can loop through top 10
            foreach (DataSnapshot childSnapshot in snapshot.Children)
            {
                string username = childSnapshot.Child("username").Value.ToString();
                var timeObj = childSnapshot.Child("time");

                if (!timeObj.Exists)
                    continue;

                float time = float.Parse(timeObj.Value.ToString());
                string timeFormat = PlayerUI.FormatTime(time);

                // Instantiate new scoreboard elements
                PlayerScoreUI scoreBoardElement = Instantiate(_scoreElement, _scoreboardContent);
                scoreBoardElement.NewScoreElement(username, timeFormat);
            }
        }
    }

    private IEnumerator DeleteUserScore()
    {
        DatabaseReference DBreference = FirebaseManager.DatabaseManager.DBreference;
        FirebaseUser User = FirebaseManager.DatabaseManager.User;

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
            Debug.Log($"User data of {User.DisplayName} deleted successfully");
        }
    }
}
