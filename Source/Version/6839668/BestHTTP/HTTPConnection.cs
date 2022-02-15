using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BestHTTP.Authentication;
using BestHTTP.Extensions;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.TcpClient.General;

namespace BestHTTP;

internal sealed class HTTPConnection : ConnectionBase
{
	private TcpClient Client;

	private Stream Stream;

	private KeepAliveHeader KeepAlive;

	public override bool IsRemovable
	{
		get
		{
			if (base.IsRemovable)
			{
				return true;
			}
			if (base.IsFree && KeepAlive != null && DateTime.UtcNow - LastProcessTime >= KeepAlive.TimeOut)
			{
				return true;
			}
			return false;
		}
	}

	internal HTTPConnection(string serverAddress)
		: base(serverAddress)
	{
	}

	protected override void ThreadFunc(object param)
	{
		bool flag = false;
		bool flag2 = false;
		RetryCauses retryCauses = RetryCauses.None;
		try
		{
			if (!base.HasProxy && base.CurrentRequest.HasProxy)
			{
				base.Proxy = base.CurrentRequest.Proxy;
			}
			if (Client != null && !Client.IsConnected())
			{
				Close();
			}
			do
			{
				if (retryCauses == RetryCauses.Reconnect)
				{
					Close();
					Thread.Sleep(100);
				}
				base.LastProcessedUri = base.CurrentRequest.CurrentUri;
				retryCauses = RetryCauses.None;
				Connect();
				if (base.State == HTTPConnectionStates.AbortRequested)
				{
					throw new Exception("AbortRequested");
				}
				bool flag3 = false;
				try
				{
					Client.NoDelay = base.CurrentRequest.TryToMinimizeTCPLatency;
					base.CurrentRequest.SendOutTo(Stream);
					flag3 = true;
				}
				catch (Exception ex)
				{
					Close();
					if (base.State == HTTPConnectionStates.TimedOut || base.State == HTTPConnectionStates.AbortRequested)
					{
						throw new Exception("AbortRequested");
					}
					if (flag || base.CurrentRequest.DisableRetry)
					{
						throw ex;
					}
					flag = true;
					retryCauses = RetryCauses.Reconnect;
				}
				if (!flag3)
				{
					continue;
				}
				bool flag4 = Receive();
				if (base.State == HTTPConnectionStates.TimedOut || base.State == HTTPConnectionStates.AbortRequested)
				{
					throw new Exception("AbortRequested");
				}
				if (!flag4 && !flag && !base.CurrentRequest.DisableRetry)
				{
					flag = true;
					retryCauses = RetryCauses.Reconnect;
				}
				if (base.CurrentRequest.Response == null)
				{
					continue;
				}
				switch (base.CurrentRequest.Response.StatusCode)
				{
				case 401:
				{
					string text2 = DigestStore.FindBest(base.CurrentRequest.Response.GetHeaderValues("www-authenticate"));
					if (!string.IsNullOrEmpty(text2))
					{
						Digest orCreate2 = DigestStore.GetOrCreate(base.CurrentRequest.CurrentUri);
						orCreate2.ParseChallange(text2);
						if (base.CurrentRequest.Credentials != null && orCreate2.IsUriProtected(base.CurrentRequest.CurrentUri) && (!base.CurrentRequest.HasHeader("Authorization") || orCreate2.Stale))
						{
							retryCauses = RetryCauses.Authenticate;
						}
					}
					break;
				}
				case 407:
				{
					if (!base.CurrentRequest.HasProxy)
					{
						break;
					}
					string text = DigestStore.FindBest(base.CurrentRequest.Response.GetHeaderValues("proxy-authenticate"));
					if (!string.IsNullOrEmpty(text))
					{
						Digest orCreate = DigestStore.GetOrCreate(base.CurrentRequest.Proxy.Address);
						orCreate.ParseChallange(text);
						if (base.CurrentRequest.Proxy.Credentials != null && orCreate.IsUriProtected(base.CurrentRequest.Proxy.Address) && (!base.CurrentRequest.HasHeader("Proxy-Authorization") || orCreate.Stale))
						{
							retryCauses = RetryCauses.ProxyAuthenticate;
						}
					}
					break;
				}
				case 301:
				case 302:
				case 307:
				case 308:
					if (base.CurrentRequest.RedirectCount < base.CurrentRequest.MaxRedirects)
					{
						base.CurrentRequest.RedirectCount++;
						string firstHeaderValue = base.CurrentRequest.Response.GetFirstHeaderValue("location");
						if (string.IsNullOrEmpty(firstHeaderValue))
						{
							throw new MissingFieldException($"Got redirect status({base.CurrentRequest.Response.StatusCode.ToString()}) without 'location' header!");
						}
						Uri redirectUri = GetRedirectUri(firstHeaderValue);
						if (HTTPManager.Logger.Level == Loglevels.All)
						{
							HTTPManager.Logger.Verbose("HTTPConnection", string.Format("{0} - Redirected to Location: '{1}' redirectUri: '{1}'", base.CurrentRequest.CurrentUri.ToString(), firstHeaderValue, redirectUri));
						}
						if (!base.CurrentRequest.CallOnBeforeRedirection(redirectUri))
						{
							HTTPManager.Logger.Information("HTTPConnection", "OnBeforeRedirection returned False");
							break;
						}
						base.CurrentRequest.RemoveHeader("Host");
						base.CurrentRequest.SetHeader("Referer", base.CurrentRequest.CurrentUri.ToString());
						base.CurrentRequest.RedirectUri = redirectUri;
						base.CurrentRequest.Response = null;
						bool flag5 = true;
						base.CurrentRequest.IsRedirected = flag5;
						flag2 = flag5;
					}
					break;
				}
				if (base.CurrentRequest.Response != null && base.CurrentRequest.Response.IsClosedManually)
				{
					continue;
				}
				bool flag6 = base.CurrentRequest.Response == null || base.CurrentRequest.Response.HasHeaderWithValue("connection", "close");
				bool flag7 = !base.CurrentRequest.IsKeepAlive;
				if (flag6 || flag7)
				{
					Close();
				}
				else
				{
					if (base.CurrentRequest.Response == null)
					{
						continue;
					}
					List<string> headerValues = base.CurrentRequest.Response.GetHeaderValues("keep-alive");
					if (headerValues != null && headerValues.Count > 0)
					{
						if (KeepAlive == null)
						{
							KeepAlive = new KeepAliveHeader();
						}
						KeepAlive.Parse(headerValues);
					}
				}
			}
			while (retryCauses != 0);
		}
		catch (TimeoutException exception)
		{
			base.CurrentRequest.Response = null;
			base.CurrentRequest.Exception = exception;
			base.CurrentRequest.State = HTTPRequestStates.ConnectionTimedOut;
			Close();
		}
		catch (Exception exception2)
		{
			if (base.CurrentRequest != null)
			{
				base.CurrentRequest.Response = null;
				switch (base.State)
				{
				case HTTPConnectionStates.AbortRequested:
				case HTTPConnectionStates.Closed:
					base.CurrentRequest.State = HTTPRequestStates.Aborted;
					break;
				case HTTPConnectionStates.TimedOut:
					base.CurrentRequest.State = HTTPRequestStates.TimedOut;
					break;
				default:
					base.CurrentRequest.Exception = exception2;
					base.CurrentRequest.State = HTTPRequestStates.Error;
					break;
				}
			}
			Close();
		}
		finally
		{
			if (base.CurrentRequest != null)
			{
				lock (HTTPManager.Locker)
				{
					if (base.CurrentRequest != null && base.CurrentRequest.Response != null && base.CurrentRequest.Response.IsUpgraded)
					{
						base.State = HTTPConnectionStates.Upgraded;
					}
					else
					{
						base.State = (flag2 ? HTTPConnectionStates.Redirected : ((Client != null) ? HTTPConnectionStates.WaitForRecycle : HTTPConnectionStates.Closed));
					}
					if (base.CurrentRequest.State == HTTPRequestStates.Processing && (base.State == HTTPConnectionStates.Closed || base.State == HTTPConnectionStates.WaitForRecycle))
					{
						if (base.CurrentRequest.Response != null)
						{
							base.CurrentRequest.State = HTTPRequestStates.Finished;
						}
						else
						{
							base.CurrentRequest.Exception = new Exception($"Remote server closed the connection before sending response header! Previous request state: {base.CurrentRequest.State.ToString()}. Connection state: {base.State.ToString()}");
							base.CurrentRequest.State = HTTPRequestStates.Error;
						}
					}
					if (base.CurrentRequest.State == HTTPRequestStates.ConnectionTimedOut)
					{
						base.State = HTTPConnectionStates.Closed;
					}
					LastProcessTime = DateTime.UtcNow;
					if (OnConnectionRecycled != null)
					{
						RecycleNow();
					}
				}
			}
		}
	}

	private void Connect()
	{
		Uri uri = ((!base.CurrentRequest.HasProxy) ? base.CurrentRequest.CurrentUri : base.CurrentRequest.Proxy.Address);
		if (this.Client == null)
		{
			this.Client = new TcpClient();
		}
		if (!this.Client.Connected)
		{
			this.Client.ConnectTimeout = base.CurrentRequest.ConnectTimeout;
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("HTTPConnection", string.Format("'{0}' - Connecting to {1}:{2}", base.CurrentRequest.CurrentUri.ToString(), uri.Host, uri.Port.ToString()));
			}
			this.Client.SendBufferSize = HTTPManager.SendBufferSize;
			this.Client.ReceiveBufferSize = HTTPManager.ReceiveBufferSize;
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("HTTPConnection", string.Format("'{0}' - Buffer sizes - Send: {1} Receive: {2} Blocking: {3}", new object[]
				{
						base.CurrentRequest.CurrentUri.ToString(),
						this.Client.SendBufferSize.ToString(),
						this.Client.ReceiveBufferSize.ToString(),
						this.Client.Client.Blocking.ToString()
				}));
			}
			this.Client.Connect(uri.Host, uri.Port);
			if (HTTPManager.Logger.Level <= Loglevels.Information)
			{
				HTTPManager.Logger.Information("HTTPConnection", "Connected to " + uri.Host + ":" + uri.Port.ToString());
			}
		}
		else if (HTTPManager.Logger.Level <= Loglevels.Information)
		{
			HTTPManager.Logger.Information("HTTPConnection", "Already connected to " + uri.Host + ":" + uri.Port.ToString());
		}
		base.StartTime = DateTime.UtcNow;
		if (this.Stream == null)
		{
			bool flag = HTTPProtocolFactory.IsSecureProtocol(base.CurrentRequest.CurrentUri);
			this.Stream = this.Client.GetStream();
			if (base.HasProxy && (!base.Proxy.IsTransparent || (flag && base.Proxy.NonTransparentForHTTPS)))
			{
				BinaryWriter binaryWriter = new BinaryWriter(this.Stream);
				for (; ; )
				{
					bool flag2 = false;
					string text = string.Format("CONNECT {0}:{1} HTTP/1.1", base.CurrentRequest.CurrentUri.Host, base.CurrentRequest.CurrentUri.Port);
					HTTPManager.Logger.Information("HTTPConnection", "Sending " + text);
					binaryWriter.SendAsASCII(text);
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.SendAsASCII("Proxy-Connection: Keep-Alive");
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.SendAsASCII("Connection: Keep-Alive");
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.SendAsASCII(string.Format("Host: {0}:{1}", base.CurrentRequest.CurrentUri.Host, base.CurrentRequest.CurrentUri.Port));
					binaryWriter.Write(HTTPRequest.EOL);
					if (base.HasProxy && base.Proxy.Credentials != null)
					{
						AuthenticationTypes type = base.Proxy.Credentials.Type;
						if (type != AuthenticationTypes.Basic)
						{
							if (type == AuthenticationTypes.Unknown || type == AuthenticationTypes.Digest)
							{
								Digest digest = DigestStore.Get(base.Proxy.Address);
								if (digest != null)
								{
									string text2 = digest.GenerateResponseHeader(base.CurrentRequest, base.Proxy.Credentials, true);
									if (!string.IsNullOrEmpty(text2))
									{
										string text3 = string.Format("Proxy-Authorization: {0}", text2);
										if (HTTPManager.Logger.Level <= Loglevels.Information)
										{
											HTTPManager.Logger.Information("HTTPConnection", "Sending proxy authorization header: " + text3);
										}
										binaryWriter.Write(text3.GetASCIIBytes());
										binaryWriter.Write(HTTPRequest.EOL);
									}
								}
							}
						}
						else
						{
							binaryWriter.Write(string.Format("Proxy-Authorization: {0}", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(base.Proxy.Credentials.UserName + ":" + base.Proxy.Credentials.Password))).GetASCIIBytes());
							binaryWriter.Write(HTTPRequest.EOL);
						}
					}
					binaryWriter.Write(HTTPRequest.EOL);
					binaryWriter.Flush();
					base.CurrentRequest.ProxyResponse = new HTTPResponse(base.CurrentRequest, this.Stream, false, false);
					if (!base.CurrentRequest.ProxyResponse.Receive(-1, true))
					{
						break;
					}
					if (HTTPManager.Logger.Level <= Loglevels.Information)
					{
						HTTPManager.Logger.Information("HTTPConnection", string.Concat(new object[]
						{
								"Proxy returned - status code: ",
								base.CurrentRequest.ProxyResponse.StatusCode,
								" message: ",
								base.CurrentRequest.ProxyResponse.Message,
								" Body: ",
								base.CurrentRequest.ProxyResponse.DataAsText
						}));
					}
					int statusCode = base.CurrentRequest.ProxyResponse.StatusCode;
					if (statusCode != 407)
					{
						if (!base.CurrentRequest.ProxyResponse.IsSuccess)
						{
							goto Block_27;
						}
					}
					else
					{
						string text4 = DigestStore.FindBest(base.CurrentRequest.ProxyResponse.GetHeaderValues("proxy-authenticate"));
						if (!string.IsNullOrEmpty(text4))
						{
							Digest orCreate = DigestStore.GetOrCreate(base.Proxy.Address);
							orCreate.ParseChallange(text4);
							if (base.Proxy.Credentials != null && orCreate.IsUriProtected(base.Proxy.Address) && (!base.CurrentRequest.HasHeader("Proxy-Authorization") || orCreate.Stale))
							{
								flag2 = true;
							}
						}
					}
					if (!flag2)
					{
						goto IL_6B0;
					}
				}
				throw new Exception("Connection to the Proxy Server failed!");
			Block_27:
				throw new Exception(string.Format("Proxy returned Status Code: \"{0}\", Message: \"{1}\" and Response: {2}", base.CurrentRequest.ProxyResponse.StatusCode, base.CurrentRequest.ProxyResponse.Message, base.CurrentRequest.ProxyResponse.DataAsText));
			}
		IL_6B0:
			if (flag)
			{
				SslStream sslStream = new SslStream(this.Client.GetStream(), false, (object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors) => base.CurrentRequest.CallCustomCertificationValidator(cert, chain));
				if (!sslStream.IsAuthenticated)
				{
					sslStream.AuthenticateAsClient(base.CurrentRequest.CurrentUri.Host);
				}
				this.Stream = sslStream;
			}
		}
	}

	private bool Receive()
	{
		SupportedProtocols protocol = ((base.CurrentRequest.ProtocolHandler != 0) ? base.CurrentRequest.ProtocolHandler : HTTPProtocolFactory.GetProtocolFromUri(base.CurrentRequest.CurrentUri));
		if (HTTPManager.Logger.Level == Loglevels.All)
		{
			HTTPManager.Logger.Verbose("HTTPConnection", $"{base.CurrentRequest.CurrentUri.ToString()} - Receive - protocol: {protocol.ToString()}");
		}
		base.CurrentRequest.Response = HTTPProtocolFactory.Get(protocol, base.CurrentRequest, Stream, base.CurrentRequest.UseStreaming, isFromCache: false);
		if (!base.CurrentRequest.Response.Receive())
		{
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("HTTPConnection", $"{base.CurrentRequest.CurrentUri.ToString()} - Receive - Failed! Response will be null, returning with false.");
			}
			base.CurrentRequest.Response = null;
			return false;
		}
		if (base.CurrentRequest.Response.StatusCode == 304)
		{
			return false;
		}
		if (HTTPManager.Logger.Level == Loglevels.All)
		{
			HTTPManager.Logger.Verbose("HTTPConnection", $"{base.CurrentRequest.CurrentUri.ToString()} - Receive - Finished Successfully!");
		}
		return true;
	}

	private Uri GetRedirectUri(string location)
	{
		Uri uri = null;
		try
		{
			uri = new Uri(location);
			if (uri.IsFile || uri.AbsolutePath == location)
			{
				uri = null;
			}
		}
		catch (UriFormatException)
		{
			uri = null;
		}
		if (uri == null)
		{
			Uri uri2 = base.CurrentRequest.Uri;
			UriBuilder uriBuilder = new UriBuilder(uri2.Scheme, uri2.Host, uri2.Port, location);
			uri = uriBuilder.Uri;
		}
		return uri;
	}

	internal override void Abort(HTTPConnectionStates newState)
	{
		base.State = newState;
		HTTPConnectionStates state = base.State;
		if (state == HTTPConnectionStates.TimedOut)
		{
			base.TimedOutStart = DateTime.UtcNow;
		}
		if (Stream != null)
		{
			try
			{
				Stream.Dispose();
			}
			catch
			{
			}
		}
	}

	private void Close()
	{
		KeepAlive = null;
		base.LastProcessedUri = null;
		if (Client == null)
		{
			return;
		}
		try
		{
			Client.Close();
		}
		catch
		{
		}
		finally
		{
			Stream = null;
			Client = null;
		}
	}

	protected override void Dispose(bool disposing)
	{
		Close();
		base.Dispose(disposing);
	}
}
