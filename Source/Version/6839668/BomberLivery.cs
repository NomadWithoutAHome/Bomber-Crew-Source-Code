using System.Collections;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class BomberLivery : MonoBehaviour
{
	[SerializeField]
	private LiveryRequirements m_liveryRequirements;

	[SerializeField]
	private Texture2D m_baseTexture;

	private Texture2D m_currentLiveryTexture;

	private bool m_isGenerating;

	private int m_queueNumber;

	private int m_currentPauses;

	private bool m_needsReset;

	private BomberUpgradeConfig m_upgradeConfig;

	private AirbaseCameraController m_pausesCreatedWith;

	public void SetBomberConfig(BomberUpgradeConfig buc)
	{
		m_upgradeConfig = buc;
	}

	public Texture2D GetCurrentLiveryTexture()
	{
		if (m_currentLiveryTexture == null)
		{
			Texture2D overrideBaseLiveryTexture = Singleton<GameFlow>.Instance.GetGameMode().GetOverrideBaseLiveryTexture();
			if (overrideBaseLiveryTexture != null)
			{
				m_currentLiveryTexture = new Texture2D(overrideBaseLiveryTexture.width, overrideBaseLiveryTexture.height, TextureFormat.RGBA32, mipmap: false);
			}
			else
			{
				m_currentLiveryTexture = new Texture2D(m_baseTexture.width, m_baseTexture.height, TextureFormat.RGBA32, mipmap: false);
			}
			m_currentLiveryTexture.wrapMode = TextureWrapMode.Clamp;
			m_currentLiveryTexture.filterMode = FilterMode.Point;
			RefreshInstant();
		}
		else if (m_needsReset)
		{
			RefreshInstant();
			m_needsReset = false;
		}
		return m_currentLiveryTexture;
	}

	public void Reset()
	{
		m_needsReset = true;
	}

	public void RefreshInstant()
	{
		StopAllCoroutines();
		while (m_currentPauses > 0)
		{
			if (Singleton<AirbaseCameraController>.Instance != null && m_pausesCreatedWith == Singleton<AirbaseCameraController>.Instance)
			{
				Singleton<AirbaseCameraController>.Instance.SetPaused(paused: false);
			}
			m_currentPauses--;
		}
		List<LiveryRenderer.LiveryDrawCommand> list = new List<LiveryRenderer.LiveryDrawCommand>();
		StartCoroutine(GenerateCommands(null, null, list, instant: true));
		Texture2D overrideBaseLiveryTexture = Singleton<GameFlow>.Instance.GetGameMode().GetOverrideBaseLiveryTexture();
		if (overrideBaseLiveryTexture != null)
		{
			Singleton<LiveryRenderer>.Instance.GenerateLiveryTexture(overrideBaseLiveryTexture, list, m_currentLiveryTexture);
		}
		else
		{
			Singleton<LiveryRenderer>.Instance.GenerateLiveryTexture(m_baseTexture, list, m_currentLiveryTexture);
		}
	}

	public void Refresh()
	{
		if (m_currentLiveryTexture == null)
		{
			Texture2D overrideBaseLiveryTexture = Singleton<GameFlow>.Instance.GetGameMode().GetOverrideBaseLiveryTexture();
			if (overrideBaseLiveryTexture != null)
			{
				m_currentLiveryTexture = new Texture2D(overrideBaseLiveryTexture.width, overrideBaseLiveryTexture.height, TextureFormat.RGBA32, mipmap: false);
			}
			else
			{
				m_currentLiveryTexture = new Texture2D(m_baseTexture.width, m_baseTexture.height, TextureFormat.RGBA32, mipmap: false);
			}
			m_currentLiveryTexture.wrapMode = TextureWrapMode.Clamp;
			m_currentLiveryTexture.filterMode = FilterMode.Point;
		}
		m_queueNumber++;
		if (this != null)
		{
			StartCoroutine(GenerateLiveryAsync(null, null, m_queueNumber));
		}
	}

	public void RefreshOverride(string uniqueIdOverride, LiveryEquippable liOverride)
	{
		m_queueNumber++;
		StartCoroutine(GenerateLiveryAsync(uniqueIdOverride, liOverride, m_queueNumber));
	}

	private IEnumerator GenerateLiveryAsync(string uniqueIdOverride, LiveryEquippable liOverride, int queueNumber)
	{
		AirbaseCameraController startCam = Singleton<AirbaseCameraController>.Instance;
		if (startCam != null)
		{
			while (startCam.IsMoving())
			{
				yield return null;
			}
			m_currentPauses++;
			m_pausesCreatedWith = startCam;
			startCam.SetPaused(paused: true);
		}
		else
		{
			m_currentPauses = 0;
		}
		while (m_isGenerating)
		{
			yield return null;
		}
		if (m_queueNumber == queueNumber)
		{
			m_isGenerating = true;
			yield return null;
			List<LiveryRenderer.LiveryDrawCommand> commandList = new List<LiveryRenderer.LiveryDrawCommand>();
			yield return StartCoroutine(GenerateCommands(uniqueIdOverride, liOverride, commandList, instant: false));
			yield return null;
			Texture2D overrideTex = Singleton<GameFlow>.Instance.GetGameMode().GetOverrideBaseLiveryTexture();
			if (overrideTex != null)
			{
				Singleton<LiveryRenderer>.Instance.GenerateLiveryTexture(overrideTex, commandList, m_currentLiveryTexture);
			}
			else
			{
				Singleton<LiveryRenderer>.Instance.GenerateLiveryTexture(m_baseTexture, commandList, m_currentLiveryTexture);
			}
			yield return null;
			m_isGenerating = false;
		}
		if (startCam != null)
		{
			startCam.SetPaused(paused: false);
			m_currentPauses--;
		}
		else
		{
			m_currentPauses = 0;
		}
	}

	public LiveryRequirements GetRequirements()
	{
		if (Singleton<GameFlow>.Instance.GetGameMode().GetOverrideLiveryRequirements() != null)
		{
			return Singleton<GameFlow>.Instance.GetGameMode().GetOverrideLiveryRequirements();
		}
		return m_liveryRequirements;
	}

	public bool IsGenerating()
	{
		return m_isGenerating;
	}

	private IEnumerator GenerateCommands(string uniqueIdOverride, LiveryEquippable liOverride, List<LiveryRenderer.LiveryDrawCommand> commandList, bool instant)
	{
		LiveryRequirements liveryRequirements = GetRequirements();
		LiveryRequirements.LiverySection[] lr = liveryRequirements.GetAllRequirements();
		BomberUpgradeConfig buc = ((m_upgradeConfig != null) ? m_upgradeConfig : Singleton<BomberContainer>.Instance.GetCurrentConfig());
		LiveryRequirements.LiverySection[] array = lr;
		foreach (LiveryRequirements.LiverySection section in array)
		{
			string eufbName = buc.GetUpgradeFor(section.m_uniqueId);
			LiveryEquippable foundLivery = null;
			if (!string.IsNullOrEmpty(eufbName))
			{
				EquipmentUpgradeFittableBase byName = Singleton<BomberUpgradeCatalogueLoader>.Instance.GetCatalogue().GetByName(eufbName);
				foundLivery = (LiveryEquippable)byName;
			}
			if (liOverride != null && section.m_uniqueId == uniqueIdOverride)
			{
				foundLivery = liOverride;
			}
			if (!(foundLivery != null))
			{
				continue;
			}
			bool isCorrectSubVariant = true;
			if (!string.IsNullOrEmpty(section.m_subIdVariant) && foundLivery.GetSubVariant() != section.m_subIdVariant)
			{
				isCorrectSubVariant = false;
			}
			if (!isCorrectSubVariant)
			{
				continue;
			}
			if (section.m_isText)
			{
				string upgradeDetail = buc.GetUpgradeDetail(section.m_uniqueId, "text");
				for (int k = 0; k < section.m_centrePositions.Length; k++)
				{
					LiveryRenderer.LiveryDrawCommand liveryDrawCommand = new LiveryRenderer.LiveryDrawCommand();
					Color color = foundLivery.GetColor();
					color.a = foundLivery.GetAlpha();
					liveryDrawCommand.m_color = color;
					liveryDrawCommand.m_positionCtr = section.m_centrePositions[k];
					liveryDrawCommand.m_size = section.m_sizes[k];
					liveryDrawCommand.m_font = foundLivery.GetFont(upgradeDetail.Length > section.m_lengthToSwitchToSmall);
					liveryDrawCommand.m_textToWrite = upgradeDetail;
					liveryDrawCommand.m_dontScaleFonts = true;
					commandList.Add(liveryDrawCommand);
				}
				continue;
			}
			for (int i = 0; i < section.m_centrePositions.Length; i++)
			{
				LiveryRenderer.LiveryDrawCommand ldc = new LiveryRenderer.LiveryDrawCommand
				{
					m_color = new Color(1f, 1f, 1f, foundLivery.GetAlpha()),
					m_positionCtr = section.m_centrePositions[i],
					m_size = foundLivery.AdjustSize(section.m_sizes[i]),
					m_useRoundMask = foundLivery.UseRoundMask()
				};
				if (instant)
				{
					ldc.m_drawTexture = foundLivery.GetTexture();
				}
				else
				{
					Texture2D qr;
					ResourceRequest rr = foundLivery.GetTextureRR(out qr);
					if (rr != null)
					{
						while (!rr.isDone)
						{
							yield return null;
						}
						ldc.m_drawTexture = (Texture2D)rr.asset;
					}
					else
					{
						ldc.m_drawTexture = qr;
					}
				}
				ldc.m_rotate = foundLivery.GetRotate();
				commandList.Add(ldc);
			}
		}
	}
}
