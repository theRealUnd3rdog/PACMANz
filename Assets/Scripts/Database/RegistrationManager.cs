using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;

public class RegistrationManager : MonoBehaviour
{
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
        FirebaseManager.DatabaseManager.Auth.SignOut();

        MenuUIManager.instance.LoginScreen();
        ClearLoginFields();
        ClearRegisterFields();
    }

    // BELOW ARE ALL REGISTRATION METHODS
    private IEnumerator Login(string email, string password)
    {
        // Call the Firebase auth signin function passing the email and password
        var LoginTask = FirebaseManager.DatabaseManager.Auth.SignInWithEmailAndPasswordAsync(email, password);

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
            FirebaseManager.DatabaseManager.User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0}", LoginTask.Result.DisplayName);
            _warningLoginText.text = string.Empty;
            _confirmLoginText.text = "Logged In";

            yield return new WaitForSecondsRealtime(1.5f);

            MenuUIManager.instance.UserDataScreen();
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
            var RegisterTask = FirebaseManager.DatabaseManager.Auth.CreateUserWithEmailAndPasswordAsync(email, password);

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
                FirebaseUser User = FirebaseManager.DatabaseManager.User;
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
                        MenuUIManager.instance.LoginScreen();
                        _warningRegisterText.text = string.Empty;

                        ClearLoginFields();
                        ClearRegisterFields();
                    }
                }
            }
        }
    }
}
