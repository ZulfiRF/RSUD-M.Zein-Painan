//
// ChunkedInputStream.cs
//	Copied from System.Net.ChunkedInputStream.cs
//
// Authors:
//	Gonzalo Paniagua Javier (gonzalo@novell.com)
//
// Copyright (c) 2005 Novell, Inc (http://www.novell.com)
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
using System.IO;
using System.Runtime.InteropServices;

namespace Core.Framework.Messaging.Clients.Net {

	class ChunkedInputStream : RequestStream {

		class ReadBufferState {

			public HttpStreamAsyncResult Ares;
			public byte []               Buffer;
			public int                   Count;
			public int                   InitialCount;
			public int                   Offset;

			public ReadBufferState (
				byte [] buffer, int offset, int count, HttpStreamAsyncResult ares)
			{
				Buffer = buffer;
				Offset = offset;
				Count = count;
				InitialCount = count;
				Ares = ares;
			}
		}

		#region Fields

		HttpListenerContext context;
		ChunkStream         decoder;
		bool                disposed;
		bool                no_more_data;

		#endregion

		#region Constructor

		public ChunkedInputStream (
			HttpListenerContext context, Stream stream, byte [] buffer, int offset, int length)
			: base (stream, buffer, offset, length)
		{
			this.context = context;
			WebHeaderCollection coll = (WebHeaderCollection) context.Request.Headers;
			decoder = new ChunkStream (coll);
		}

		#endregion

		#region Property

		public ChunkStream Decoder {
			get { return decoder; }
			set { decoder = value; }
		}

		#endregion

		#region Private Method

		void OnRead (IAsyncResult base_ares)
		{
			var rb = (ReadBufferState) base_ares.AsyncState;
			var ares = rb.Ares;
			try {
				int nread = base.EndRead (base_ares);
				decoder.Write (ares.Buffer, ares.Offset, nread);
				nread = decoder.Read (rb.Buffer, rb.Offset, rb.Count);
				rb.Offset += nread;
				rb.Count -= nread;
				if (rb.Count == 0 || !decoder.WantMore || nread == 0) {
					no_more_data = !decoder.WantMore && nread == 0;
					ares.Count = rb.InitialCount - rb.Count;
					ares.Complete ();
					return;
				}

				ares.Offset = 0;
				ares.Count = Math.Min (8192, decoder.ChunkLeft + 6);
				base.BeginRead (ares.Buffer, ares.Offset, ares.Count, OnRead, rb);
			} catch (Exception e) {
				context.Connection.SendError (e.Message, 400);
				ares.Complete (e);
			}
		}

		#endregion

		#region Public Methods

		public override IAsyncResult BeginRead (
			byte [] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			if (disposed)
				throw new ObjectDisposedException (GetType ().ToString ());

			if (buffer == null)
				throw new ArgumentNullException ("buffer");

			int len = buffer.Length;
			if (offset < 0 || offset > len)
				throw new ArgumentOutOfRangeException ("'offset' exceeds the size of buffer.");

			if (count < 0 || offset > len - count)
				throw new ArgumentOutOfRangeException ("'offset' + 'count' exceeds the size of buffer.");

			var ares = new HttpStreamAsyncResult ();
			ares.Callback = cback;
			ares.State = state;
			if (no_more_data) {
				ares.Complete ();
				return ares;
			}

			int nread = decoder.Read (buffer, offset, count);
			offset += nread;
			count -= nread;
			if (count == 0) {
				// got all we wanted, no need to bother the decoder yet
				ares.Count = nread;
				ares.Complete ();
				return ares;
			}

			if (!decoder.WantMore) {
				no_more_data = nread == 0;
				ares.Count = nread;
				ares.Complete ();
				return ares;
			}

			ares.Buffer = new byte [8192];
			ares.Offset = 0;
			ares.Count = 8192;
			var rb = new ReadBufferState (buffer, offset, count, ares);
			rb.InitialCount += nread;
			base.BeginRead (ares.Buffer, ares.Offset, ares.Count, OnRead, rb);
			return ares;
		}

		public override void Close ()
		{
			if (!disposed) {
				disposed = true;
				base.Close ();
			}
		}

		public override int EndRead (IAsyncResult ares)
		{
			if (disposed)
				throw new ObjectDisposedException (GetType ().ToString ());

			if (ares == null)
				throw new ArgumentException ("Invalid IAsyncResult.", "ares");

			if (!ares.IsCompleted)
				ares.AsyncWaitHandle.WaitOne ();

			var ares_ = ares as HttpStreamAsyncResult;
			if (ares_.Error != null)
				throw new HttpListenerException (400, "I/O operation aborted.");

			return ares_.Count;
		}

		public override int Read ([In,Out] byte [] buffer, int offset, int count)
		{
			var ares = BeginRead (buffer, offset, count, null, null);
			return EndRead (ares);
		}

		#endregion
	}
}
