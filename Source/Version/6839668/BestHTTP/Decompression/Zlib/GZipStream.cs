using System;
using System.IO;
using System.Text;

namespace BestHTTP.Decompression.Zlib;

internal class GZipStream : Stream
{
	public DateTime? LastModified;

	private int _headerByteCount;

	internal ZlibBaseStream _baseStream;

	private bool _disposed;

	private bool _firstReadDone;

	private string _FileName;

	private string _Comment;

	private int _Crc32;

	internal static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	internal static readonly Encoding iso8859dash1 = Encoding.GetEncoding("iso-8859-1");

	public string Comment
	{
		get
		{
			return _Comment;
		}
		set
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			_Comment = value;
		}
	}

	public string FileName
	{
		get
		{
			return _FileName;
		}
		set
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			_FileName = value;
			if (_FileName != null)
			{
				if (_FileName.IndexOf("/") != -1)
				{
					_FileName = _FileName.Replace("/", "\\");
				}
				if (_FileName.EndsWith("\\"))
				{
					throw new Exception("Illegal filename");
				}
				if (_FileName.IndexOf("\\") != -1)
				{
					_FileName = Path.GetFileName(_FileName);
				}
			}
		}
	}

	public int Crc32 => _Crc32;

	public virtual FlushType FlushMode
	{
		get
		{
			return _baseStream._flushMode;
		}
		set
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			_baseStream._flushMode = value;
		}
	}

	public int BufferSize
	{
		get
		{
			return _baseStream._bufferSize;
		}
		set
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			if (_baseStream._workingBuffer != null)
			{
				throw new ZlibException("The working buffer is already set.");
			}
			if (value < 1024)
			{
				throw new ZlibException($"Don't be silly. {value} bytes?? Use a bigger buffer, at least {1024}.");
			}
			_baseStream._bufferSize = value;
		}
	}

	public virtual long TotalIn => _baseStream._z.TotalBytesIn;

	public virtual long TotalOut => _baseStream._z.TotalBytesOut;

	public override bool CanRead
	{
		get
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			return _baseStream._stream.CanRead;
		}
	}

	public override bool CanSeek => false;

	public override bool CanWrite
	{
		get
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("GZipStream");
			}
			return _baseStream._stream.CanWrite;
		}
	}

	public override long Length
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override long Position
	{
		get
		{
			if (_baseStream._streamMode == ZlibBaseStream.StreamMode.Writer)
			{
				return _baseStream._z.TotalBytesOut + _headerByteCount;
			}
			if (_baseStream._streamMode == ZlibBaseStream.StreamMode.Reader)
			{
				return _baseStream._z.TotalBytesIn + _baseStream._gzipHeaderByteCount;
			}
			return 0L;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public GZipStream(Stream stream, CompressionMode mode)
		: this(stream, mode, CompressionLevel.Default, leaveOpen: false)
	{
	}

	public GZipStream(Stream stream, CompressionMode mode, CompressionLevel level)
		: this(stream, mode, level, leaveOpen: false)
	{
	}

	public GZipStream(Stream stream, CompressionMode mode, bool leaveOpen)
		: this(stream, mode, CompressionLevel.Default, leaveOpen)
	{
	}

	public GZipStream(Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen)
	{
		_baseStream = new ZlibBaseStream(stream, mode, level, ZlibStreamFlavor.GZIP, leaveOpen);
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			if (!_disposed)
			{
				if (disposing && _baseStream != null)
				{
					_baseStream.Close();
					_Crc32 = _baseStream.Crc32;
				}
				_disposed = true;
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	public override void Flush()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("GZipStream");
		}
		_baseStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("GZipStream");
		}
		int result = _baseStream.Read(buffer, offset, count);
		if (!_firstReadDone)
		{
			_firstReadDone = true;
			FileName = _baseStream._GzipFileName;
			Comment = _baseStream._GzipComment;
		}
		return result;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotImplementedException();
	}

	public override void SetLength(long value)
	{
		_baseStream.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("GZipStream");
		}
		if (_baseStream._streamMode == ZlibBaseStream.StreamMode.Undefined)
		{
			if (!_baseStream._wantCompress)
			{
				throw new InvalidOperationException();
			}
			_headerByteCount = EmitHeader();
		}
		_baseStream.Write(buffer, offset, count);
	}

	private int EmitHeader()
	{
		byte[] array = ((Comment != null) ? iso8859dash1.GetBytes(Comment) : null);
		byte[] array2 = ((FileName != null) ? iso8859dash1.GetBytes(FileName) : null);
		int num = ((Comment != null) ? (array.Length + 1) : 0);
		int num2 = ((FileName != null) ? (array2.Length + 1) : 0);
		int num3 = 10 + num + num2;
		byte[] array3 = new byte[num3];
		int num4 = 0;
		array3[num4++] = 31;
		array3[num4++] = 139;
		array3[num4++] = 8;
		byte b = 0;
		if (Comment != null)
		{
			b = (byte)(b ^ 0x10u);
		}
		if (FileName != null)
		{
			b = (byte)(b ^ 8u);
		}
		array3[num4++] = b;
		if (!LastModified.HasValue)
		{
			LastModified = DateTime.Now;
		}
		int value = (int)(LastModified.Value - _unixEpoch).TotalSeconds;
		Array.Copy(BitConverter.GetBytes(value), 0, array3, num4, 4);
		num4 += 4;
		array3[num4++] = 0;
		array3[num4++] = byte.MaxValue;
		if (num2 != 0)
		{
			Array.Copy(array2, 0, array3, num4, num2 - 1);
			num4 += num2 - 1;
			array3[num4++] = 0;
		}
		if (num != 0)
		{
			Array.Copy(array, 0, array3, num4, num - 1);
			num4 += num - 1;
			array3[num4++] = 0;
		}
		_baseStream._stream.Write(array3, 0, array3.Length);
		return array3.Length;
	}

	public static byte[] CompressString(string s)
	{
		using MemoryStream memoryStream = new MemoryStream();
		Stream compressor = new GZipStream(memoryStream, CompressionMode.Compress, CompressionLevel.BestCompression);
		ZlibBaseStream.CompressString(s, compressor);
		return memoryStream.ToArray();
	}

	public static byte[] CompressBuffer(byte[] b)
	{
		using MemoryStream memoryStream = new MemoryStream();
		Stream compressor = new GZipStream(memoryStream, CompressionMode.Compress, CompressionLevel.BestCompression);
		ZlibBaseStream.CompressBuffer(b, compressor);
		return memoryStream.ToArray();
	}

	public static string UncompressString(byte[] compressed)
	{
		using MemoryStream stream = new MemoryStream(compressed);
		Stream decompressor = new GZipStream(stream, CompressionMode.Decompress);
		return ZlibBaseStream.UncompressString(compressed, decompressor);
	}

	public static byte[] UncompressBuffer(byte[] compressed)
	{
		using MemoryStream stream = new MemoryStream(compressed);
		Stream decompressor = new GZipStream(stream, CompressionMode.Decompress);
		return ZlibBaseStream.UncompressBuffer(compressed, decompressor);
	}
}
