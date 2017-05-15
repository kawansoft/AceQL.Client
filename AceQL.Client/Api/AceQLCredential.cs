using System;

namespace AceQL.Client.Api
{
    /// <summary>
    /// <see cref="AceQLCredential"/> provides a little more secure way than using a connection string to specify the username or password for a login attempt.
    /// Note that implementation uses a string for password, as password will anyway be transformed finally into a string when sent using HTTP protocol.
    /// This could/should be improved in the future (by encrypting the password and decrypting on the server.)
    /// </summary>
    public class AceQLCredential
    {
        private string username;
        private string password;

        /// <summary>
        /// Creates an object of type <see cref="AceQLCredential"/>.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <exception cref="System.ArgumentNullException">If username or password is null. </exception>
        public AceQLCredential(string username, string password)
        {
            this.username = username ?? throw new ArgumentNullException("username is null!");
            this.password = password ?? throw new ArgumentNullException("password is null!");
        }

        /// <summary>
        /// Returns the username component of the <see cref="AceQLCredential"/> object.
        /// </summary>
        public string Username { get => username; }

        /// <summary>
        /// Returns the password component of the <see cref="AceQLCredential"/> object.
        /// </summary>
        public string Password { get => password; }
    }
}