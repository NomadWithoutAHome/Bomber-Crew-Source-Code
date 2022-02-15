using System;
using System.Collections.Generic;
using System.Text;
using BomberCrewCommon;
using UnityEngine;

public class InputLayerInterface : Singleton<InputLayerInterface>
{
	[Serializable]
	public class InputLayer
	{
		public string LayerID;

		public tk2dUICamera UICamera;
	}

	[SerializeField]
	private bool m_debug;

	[SerializeField]
	private List<InputLayer> m_inputLayers;

	private Dictionary<string, int> m_layerStates;

	private Dictionary<Collider, int> m_disabledColliders = new Dictionary<Collider, int>();

	private Dictionary<string, int> LayerStates
	{
		get
		{
			if (m_layerStates == null)
			{
				m_layerStates = new Dictionary<string, int>();
				foreach (InputLayer inputLayer in m_inputLayers)
				{
					m_layerStates.Add(inputLayer.LayerID, 0);
				}
			}
			return m_layerStates;
		}
	}

	public List<string> GetLayerIDs()
	{
		List<string> list = new List<string>();
		foreach (InputLayer inputLayer in m_inputLayers)
		{
			list.Add(inputLayer.LayerID);
		}
		return list;
	}

	public InputLayer LayerForID(string id)
	{
		return m_inputLayers.Find((InputLayer il) => il.LayerID == id);
	}

	public void EnableAllLayers()
	{
		List<string> list = new List<string>(LayerStates.Keys);
		foreach (string item in list)
		{
			LayerStates[item] = Mathf.Max(LayerStates[item] - 1, 0);
		}
		if (!m_debug)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("[InputLayerInterface] EnableAllLayers:");
		string[] names = Enum.GetNames(typeof(InputLayer));
		foreach (KeyValuePair<string, int> layerState in LayerStates)
		{
			stringBuilder.AppendLine("[InputLayerInterface] m_gameBoardLayerCount[ " + layerState.Key + " ] == " + layerState.Value);
		}
	}

	public void DisableAllLayers()
	{
		List<string> list = new List<string>(LayerStates.Keys);
		foreach (string item in list)
		{
			LayerStates[item]++;
		}
		if (!m_debug)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("[InputLayerInterface] DisableAllLayers:");
		string[] names = Enum.GetNames(typeof(InputLayer));
		foreach (KeyValuePair<string, int> layerState in LayerStates)
		{
			stringBuilder.AppendLine("[InputLayerInterface] m_gameBoardLayerCount[ " + layerState.Key + " ] == " + layerState.Value);
		}
	}

	public void EnableLayerInput(string layerId)
	{
		LayerStates[layerId] = Mathf.Max(LayerStates[layerId] - 1, 0);
		if (!m_debug)
		{
		}
	}

	public void DisableLayerInput(string layerId)
	{
		LayerStates[layerId]++;
		if (!m_debug)
		{
		}
	}

	public void EnableCollider(Collider c, bool debug = false)
	{
		if (m_disabledColliders.ContainsKey(c))
		{
			m_disabledColliders[c] = Mathf.Max(m_disabledColliders[c] - 1, 0);
		}
		else
		{
			m_disabledColliders.Add(c, 0);
		}
		c.enabled = m_disabledColliders[c] == 0;
		if (!debug)
		{
		}
	}

	public void DisableCollider(Collider c, bool debug = false)
	{
		if (m_disabledColliders.ContainsKey(c))
		{
			m_disabledColliders[c]++;
		}
		else
		{
			m_disabledColliders.Add(c, 1);
		}
		c.enabled = m_disabledColliders[c] == 0;
		if (!debug)
		{
		}
	}

	private void Update()
	{
		foreach (KeyValuePair<string, int> layerState in LayerStates)
		{
			tk2dUICamera uICamera = LayerForID(layerState.Key).UICamera;
			uICamera.enabled = layerState.Value == 0;
		}
	}
}
