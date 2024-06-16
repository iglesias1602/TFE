using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.example
{
    public class SignInWithEmailPassword : MonoBehaviour
    {
        // Public Unity References
        public TMP_InputField EmailInput = null!;
        public TMP_InputField PasswordInput = null!;
        public TMP_Text ErrorText = null!;
        //public SupabaseManager SupabaseManager = null!;

        public string nextScene;


        // Private implementation
        private bool _doSignIn;
        private bool _doSignOut;

        // Unity does not allow async UI events, so we set a flag and use Update() to do the async work
        public void SignIn()
        {
            _doSignIn = true;
        }

        // Unity does not allow async UI events, so we set a flag and use Update() to do the async work
        public void SignOut()
        {
            _doSignOut = true;
        }

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        private async void Update()
        {
            if (_doSignOut)
            {
                _doSignOut = false;
                await SupabaseManager.Instance.Supabase()!.Auth.SignOut();
            }

            if (_doSignIn)
            {
                _doSignIn = false;
                await PerformSignIn();
            }
        }

        // This is where we do the async work and handle exceptions
        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        private async Task PerformSignIn()
        {
            try
            {
                Session session = (await SupabaseManager.Instance.Supabase()!.Auth.SignIn(EmailInput.text, PasswordInput.text))!;
                ErrorText.text = $"Success! Signed In as {session.User?.Email}";
                Debug.Log($"Success! Signed In as {session.User?.Email}");

                // Transition to the game scene upon successful sign-in
                SceneManager.LoadScene(nextScene);
            }
            catch (GotrueException goTrueException)
            {
                ErrorText.text = $"{goTrueException.Reason} {goTrueException.Message}";
                Debug.Log(goTrueException.Message, gameObject);
                Debug.LogException(goTrueException, gameObject);
            }
            catch (Exception e)
            {
                ErrorText.text = e.Message;
                Debug.Log(e.Message, gameObject);
                Debug.Log(e, gameObject);
            }

        }
    }
}
