//
// HttpListenerContext.cs
//	Copied from System.Net.HttpListenerContext.cs
//
// Author:
//	Gonzalo Paniagua Javier (gonzalo@novell.com)
//
// Copyright (c) 2005 Novell, Inc. (http://www.novell.com)
// Copyright (c) 2012-2013 sta.blockhead (sta.blockhead@gmail.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Security.Principal;
using System.Text;
using Core.Framework.Messaging.Clients.Net.WebSockets;

namespace Core.Framework.Messaging.Clients.Net {

	/// <summary>
	/// Provides access to the HTTP request and response objects used by the <see cref="HttpListener"/> class.
	/// </summary>
	/// <remarks>
	/// The HttpListenerContext class cannot be inherited.
	/// </remarks>
	public sealed class HttpListenerContext {

		#region Private Fields

		HttpConnection       cnc;
		string               error;
		int                  err_status;
		HttpListenerRequest  request;
		HttpListenerResponse response;
		IPrincipal           user;

		#endregion

		#region Internal Fields

		internal HttpListener Listener;

		#endregion

		#region Constructor

		internal HttpListenerContext (HttpConnection cnc)
		{
			this.cnc   = cnc;
			err_status = 400;
			request    = new HttpListenerRequest (this);
			response   = new HttpListenerResponse (this);
		}

		#endregion

		#region Internal Properties

		internal HttpConnection Connection {
			get { return cnc; }
		}

		internal string ErrorMessage {
			get { return error; }
			set { error = value; }
		}

		internal int ErrorStatus {
			get { return err_status; }
			set { err_status = value; }
		}

		internal bool HaveError {
			get { return (error != null); }
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the <see cref="HttpListenerRequest"/> that contains the HTTP request from a client.
		/// </summary>
		/// <value>
		/// A <see cref="HttpListenerRequest"/> that contains the HTTP request objects.
		/// </value>
		public HttpListenerRequest Request {
			get { return request; }
		}

		/// <summary>
		/// Gets the <see cref="HttpListenerResponse"/> that contains the HTTP response to send to
		/// the client in response to the client's request.
		/// </summary>
		/// <value>
		/// A <see cref="HttpListenerResponse"/> that contains the HTTP response objects.
		/// </value>
		public HttpListenerResponse Response {
			get { return response; }
		}

		/// <summary>
		/// Gets the client information (identity, authentication information and security roles).
		/// </summary>
		/// <value>
		/// A <see cref="IPrincipal"/> contains the client information.
		/// </value>
		public IPrincipal User {
			get { return user; }
		}

		#endregion

		#region Internal Methods

		internal void ParseAuthentication (AuthenticationSchemes expectedSchemes)
		{
			if (expectedSchemes == AuthenticationSchemes.Anonymous)
				return;

			// TODO: Handle NTLM/Digest modes
			string header = request.Headers ["Authorization"];
			if (header == null || header.Length < 2)
				return;

			string [] authenticationData = header.Split (new char [] {' '}, 2);
			if (string.Compare (authenticationData [0], "basic", true) == 0) {
				user = ParseBasicAuthentication (authenticationData [1]);
			}
			// TODO: throw if malformed -> 400 bad request
		}

		internal IPrincipal ParseBasicAuthentication (string authData)
		{
			try {
				// Basic AUTH Data is a formatted Base64 String
				//string domain = null;
				string user       = null;
				string password   = null;
				int    pos        = -1;
				string authString = Encoding.Default.GetString (Convert.FromBase64String (authData));

				// The format is DOMAIN\username:password
				// Domain is optional

				pos = authString.IndexOf (':');

				// parse the password off the end
				password = authString.Substring (pos+1);

				// discard the password
				authString = authString.Substring (0, pos);

				// check if there is a domain
				pos = authString.IndexOf ('\\');

				if (pos > 0) {
					//domain = authString.Substring (0, pos);
					user = authString.Substring (pos);
				} else {
					user = authString;
				}

				var identity = new System.Net.HttpListenerBasicIdentity (user, password);
				// TODO: What are the roles MS sets
				return new GenericPrincipal (identity, new string [0]);
			} catch (Exception) {
				// Invalid auth data is swallowed silently
				return null;
			} 
		}

		#endregion

		#region Public Method

		/// <summary>
		/// Accepts a WebSocket connection by the <see cref="HttpListener"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="HttpListenerWebSocketContext"/> that contains a WebSocket connection.
		/// </returns>
		public HttpListenerWebSocketContext AcceptWebSocket ()
		{
			return new HttpListenerWebSocketContext (this);
		}

		#endregion
	}
}
