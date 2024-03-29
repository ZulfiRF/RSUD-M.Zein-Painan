//
// ListenerPrefix.cs
//	Copied from System.ListenerPrefix.cs
//
// Author:
//	Gonzalo Paniagua Javier (gonzalo@novell.com)
//	Oleg Mihailik (mihailik gmail co_m)
//
// Copyright (c) 2005 Novell, Inc. (http://www.novell.com)
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
using System.Net;

namespace Core.Framework.Messaging.Clients.Net {

	sealed class ListenerPrefix {

		#region Private Fields

		IPAddress [] addresses;
		string       host;
		string       original;
		string       path;
		ushort       port;
		bool         secure;

		#endregion

		#region Public Field

		public HttpListener Listener;

		#endregion

		#region Constructor

		// Must be called after calling ListenerPrefix.CheckUri.
		public ListenerPrefix (string prefix)
		{
			original = prefix;
			Parse (prefix);
		}

		#endregion

		#region Properties

		public IPAddress [] Addresses {
			get { return addresses; }
			set { addresses = value; }
		}

		public string Host {
			get { return host; }
		}

		public int Port {
			get { return (int) port; }
		}

		public string Path {
			get { return path; }
		}

		public bool Secure {
			get { return secure; }
		}

		#endregion

		#region Private Method

		void Parse (string uri)
		{
			int default_port = (uri.StartsWith ("http://")) ? 80 : 443;
			if (default_port == 443)
				secure = true;

			int length = uri.Length;
			int start_host = uri.IndexOf (':') + 3;
			int colon = uri.IndexOf (':', start_host, length - start_host);
			int root;
			if (colon > 0) {
				host = uri.Substring (start_host, colon - start_host);
				root = uri.IndexOf ('/', colon, length - colon);
				port = (ushort) Int32.Parse (uri.Substring (colon + 1, root - colon - 1));
				path = uri.Substring (root);
			} else {
				root = uri.IndexOf ('/', start_host, length - start_host);
				host = uri.Substring (start_host, root - start_host);
				path = uri.Substring (root);
				port = (ushort) default_port;
			}

			if (path.Length != 1)
				path = path.Substring (0, path.Length - 1);
		}

		#endregion

		#region public Methods

		public static void CheckUri (string uri)
		{
			if (uri == null)
				throw new ArgumentNullException ("uri");

			int default_port = (uri.StartsWith ("http://")) ? 80 : -1;
			if (default_port == -1)
				default_port = (uri.StartsWith ("https://")) ? 443 : -1;
			if (default_port == -1)
				throw new ArgumentException ("Only 'http' and 'https' schemes are supported.");

			int length = uri.Length;
			int start_host = uri.IndexOf (':') + 3;
			if (start_host >= length)
				throw new ArgumentException ("No host specified.");

			int colon = uri.IndexOf (':', start_host, length - start_host);
			if (start_host == colon)
				throw new ArgumentException ("No host specified.");

			int root;
			if (colon > 0) {
				root = uri.IndexOf ('/', colon, length - colon);
				if (root == -1)
					throw new ArgumentException ("No path specified.");

				try {
					int p = Int32.Parse (uri.Substring (colon + 1, root - colon - 1));
					if (p <= 0 || p >= 65536)
						throw new Exception ();
				} catch {
					throw new ArgumentException ("Invalid port.");
				}
			} else {
				root = uri.IndexOf ('/', start_host, length - start_host);
				if (root == -1)
					throw new ArgumentException ("No path specified.");
			}

			if (uri [uri.Length - 1] != '/')
				throw new ArgumentException ("The prefix must end with '/'.");
		}

		// Equals and GetHashCode are required to detect duplicates in HttpListenerPrefixCollection.
		public override bool Equals (object o)
		{
			var other = o as ListenerPrefix;
			if (other == null)
				return false;

			return (original == other.original);
		}

		public override int GetHashCode ()
		{
			return original.GetHashCode ();
		}

		public override string ToString ()
		{
			return original;
		}

		#endregion
	}
}
