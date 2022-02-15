using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BomberCrewCommon;

public abstract class FileSystem_VirtualFS : FileSystem_Null
{
	public class OfflineDataBlock
	{
		public class DataEntry
		{
			public string m_name;

			public byte[] m_data;
		}

		public List<DataEntry> m_dataEntry = new List<DataEntry>();

		private int m_padSize;

		public OfflineDataBlock(int padSize)
		{
			m_padSize = padSize;
		}

		public byte[] GetSerialised()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(m_dataEntry.Count);
			foreach (DataEntry item in m_dataEntry)
			{
				binaryWriter.Write(item.m_name);
				binaryWriter.Write(item.m_data.Length);
				binaryWriter.Write(item.m_data);
			}
			if (binaryWriter.BaseStream.Length < m_padSize)
			{
				binaryWriter.Write(new byte[m_padSize - binaryWriter.BaseStream.Length]);
			}
			binaryWriter.Flush();
			return memoryStream.GetBuffer();
		}

		public void DeserialiseFrom(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader binaryReader = new BinaryReader(input);
			int num = binaryReader.ReadInt32();
			m_dataEntry = new List<DataEntry>();
			for (int i = 0; i < num; i++)
			{
				DataEntry dataEntry = new DataEntry();
				dataEntry.m_name = binaryReader.ReadString();
				int count = binaryReader.ReadInt32();
				dataEntry.m_data = binaryReader.ReadBytes(count);
				m_dataEntry.Add(dataEntry);
			}
		}

		public byte[] GetFile(string filename)
		{
			foreach (DataEntry item in m_dataEntry)
			{
				if (item.m_name == filename)
				{
					return item.m_data;
				}
			}
			return null;
		}

		public void SetFile(string filename, byte[] data)
		{
			foreach (DataEntry item in m_dataEntry)
			{
				if (item.m_name == filename)
				{
					item.m_data = data;
					return;
				}
			}
			DataEntry dataEntry = new DataEntry();
			dataEntry.m_data = data;
			dataEntry.m_name = filename;
			m_dataEntry.Add(dataEntry);
		}
	}

	[SerializeField]
	private int m_padToSize = 10485760;

	private OfflineDataBlock m_dataBlock;

	protected void SetDataBlock(byte[] fromData)
	{
		m_dataBlock = new OfflineDataBlock(m_padToSize);
		m_dataBlock.DeserialiseFrom(fromData);
	}

	protected void SetNewDataBlock()
	{
		m_dataBlock = new OfflineDataBlock(m_padToSize);
	}

	protected void InvalidateDataBlock()
	{
		m_dataBlock = null;
	}

	protected byte[] GetDataBlockBuffer()
	{
		if (m_dataBlock != null)
		{
			return m_dataBlock.GetSerialised();
		}
		return null;
	}

	public override bool Exists(string filename)
	{
		if (m_dataBlock != null)
		{
			return m_dataBlock.GetFile(filename) != null;
		}
		return false;
	}

	public override byte[] ReadAllBytes(string filename)
	{
		if (m_dataBlock != null)
		{
			return m_dataBlock.GetFile(filename);
		}
		return null;
	}

	public override void WriteAllBytes(string filename, byte[] bytes)
	{
		if (m_dataBlock != null)
		{
			m_dataBlock.SetFile(filename, bytes);
		}
	}

	public override string ReadAllText(string filename)
	{
		if (m_dataBlock != null)
		{
			byte[] array = ReadAllBytes(filename);
			if (array != null)
			{
				BinaryReader binaryReader = new BinaryReader(new MemoryStream(array), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
				return binaryReader.ReadString();
			}
			return null;
		}
		return null;
	}

	public override void WriteAllText(string filename, string contents)
	{
		if (m_dataBlock != null)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
			binaryWriter.Write(contents);
			binaryWriter.Flush();
			byte[] buffer = memoryStream.GetBuffer();
			WriteAllBytes(filename, buffer);
		}
	}
}
