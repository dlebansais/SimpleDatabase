using System;

namespace Database
{
    #region Interface
    /// <summary>
    ///     Represents the user identifier and destination server to use to connect to a database.
    /// </summary>
    public interface ICredential
    {
        /// <summary>
        ///     Gets the server.
        /// </summary>
        /// <returns>
        ///     The server.
        /// </returns>
        string Server { get; }

        /// <summary>
        ///     Gets the user identifier.
        /// </summary>
        /// <returns>
        ///     The user identifier.
        /// </returns>
        string UserId { get; }

        /// <summary>
        ///     Gets the password.
        /// </summary>
        /// <returns>
        ///     The password.
        /// </returns>
        string Password { get; }

        /// <summary>
        ///     Gets the default schema to use.
        /// </summary>
        /// <returns>
        ///     The default schema to use. Null if none.
        /// </returns>
        ISchemaDescriptor Schema { get; }
    }
    #endregion

    /// <summary>
    ///     Represents the user identity and destination server to use to connect to a database.
    /// </summary>
    public class Credential : ICredential
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="Credential"/> class.
        /// </summary>
        /// <parameters>
        /// <param name="server">The server.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="password">The password.</param>
        /// <param name="schema">The default schema to use.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="server"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="userId"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="password"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="schema"/> is null.
        /// </exception>
        public Credential(string server, string userId, string password, ISchemaDescriptor schema)
        {
            Server = server ?? throw new ArgumentNullException(nameof(server));
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Credential"/> class.
        ///     There is no default schema associated to the connection.
        /// </summary>
        /// <parameters>
        /// <param name="server">The server.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="password">The password.</param>
        /// </parameters>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="server"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="userId"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="password"/> is null.
        /// </exception>
        internal Credential(string server, string userId, string password)
        {
            Server = server ?? throw new ArgumentNullException(nameof(server));
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Schema = null;
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the server.
        /// </summary>
        /// <returns>
        ///     The server.
        /// </returns>
        public string Server { get; }

        /// <summary>
        ///     Gets the user identifier.
        /// </summary>
        /// <returns>
        ///     The user identifier.
        /// </returns>
        public string UserId { get; }

        /// <summary>
        ///     Gets the password.
        /// </summary>
        /// <returns>
        ///     The password.
        /// </returns>
        public string Password { get; }

        /// <summary>
        ///     Gets the default schema to use.
        /// </summary>
        /// <returns>
        ///     The default schema to use. Null if none.
        /// </returns>
        public ISchemaDescriptor Schema { get; }
        #endregion
    }
}
