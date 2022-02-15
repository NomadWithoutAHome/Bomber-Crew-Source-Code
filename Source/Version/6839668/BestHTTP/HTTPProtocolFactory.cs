using System;
using System.IO;

namespace BestHTTP;

internal static class HTTPProtocolFactory
{
	public static HTTPResponse Get(SupportedProtocols protocol, HTTPRequest request, Stream stream, bool isStreamed, bool isFromCache)
	{
		return new HTTPResponse(request, stream, isStreamed, isFromCache);
	}

	public static SupportedProtocols GetProtocolFromUri(Uri uri)
	{
		if (uri == null || uri.Scheme == null)
		{
			throw new Exception("Malformed URI in GetProtocolFromUri");
		}
		string text = uri.Scheme.ToLowerInvariant();
		if (text != null)
		{
		}
		return SupportedProtocols.HTTP;
	}

	public static bool IsSecureProtocol(Uri uri)
	{
		if (uri == null || uri.Scheme == null)
		{
			throw new Exception("Malformed URI in IsSecureProtocol");
		}
		string text = uri.Scheme.ToLowerInvariant();
		if (text != null && text == "https")
		{
			return true;
		}
		return false;
	}
}
