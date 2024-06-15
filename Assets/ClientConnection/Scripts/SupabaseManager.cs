using System;
using Supabase;
using Supabase.Gotrue;
using TMPro;
using UnityEngine;
using Client = Supabase.Client;

namespace com.example
{
    public class SupabaseManager : MonoBehaviour
    {
        // Public Unity references
        public SessionListener SessionListener = null!;
        public SupabaseSettings SupabaseSettings = null!;
        public TMP_Text ErrorText = null!;

        // Public in case other components are interested in network status
        private readonly NetworkStatus _networkStatus = new();

        // Internals
        private Client? _client;

        // Expose the client to other components
        public Client? Supabase() => _client;

        private async void Start()
        {
            SupabaseOptions options = new()
            {
                AutoRefreshToken = true // Automatically refresh the token
            };

            // Initialize the Supabase client
            Client client = new(SupabaseSettings.SupabaseURL, SupabaseSettings.SupabaseAnonKey, options);

            // Attach the debug listener
            client.Auth.AddDebugListener(DebugListener);    

            // Set up the network status listener
            _networkStatus.Client = (Supabase.Gotrue.Client)client.Auth;

            // Set up session persistence
            client.Auth.SetPersistence(new UnitySession());

            // Set up the session state change listener
            client.Auth.AddStateChangedListener(SessionListener.UnityAuthListener);

            // Load the session from the persistence layer
            client.Auth.LoadSession();

            // Allow unconfirmed user sessions
            client.Auth.Options.AllowUnconfirmedUserSessions = true;

            // Check the network status
            string url = $"{SupabaseSettings.SupabaseURL}/auth/v1/settings?apikey={SupabaseSettings.SupabaseAnonKey}";
            try
            {
                client.Auth.Online = await _networkStatus.StartAsync(url);
            }
            catch (NotSupportedException)
            {
                client.Auth.Online = true;
            }
            catch (Exception e)
            {
                ErrorText.text = e.Message;
                Debug.Log(e.Message, gameObject);
                Debug.LogException(e, gameObject);
                client.Auth.Online = false;
            }

            if (client.Auth.Online)
            {
                await client.InitializeAsync();

                // Fetch and log server settings
                Settings serverConfiguration = (await client.Auth.Settings())!;
                Debug.Log($"Auto-confirm emails on this server: {serverConfiguration.MailerAutoConfirm}");
            }
            _client = client;
        }

        private void DebugListener(string message, Exception e)
        {
            ErrorText.text = message;
            Debug.Log(message, gameObject);
            if (e != null)
                Debug.LogException(e, gameObject);
        }

        private void OnApplicationQuit()
        {
            if (_client != null)
            {
                _client?.Auth.Shutdown();
                _client = null;
            }
        }
    }
}
