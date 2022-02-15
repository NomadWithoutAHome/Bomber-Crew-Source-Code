using System;
using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class CrewmanGraphics : MonoBehaviour
{
	public class CombineMeshInfo
	{
		public Mesh m_meshTarget;

		public int m_cacheMeshTargetId;

		public int m_subMesh;

		public Material m_material;

		public SkinnedMeshRenderer m_skinnedMeshRenderer;
	}

	[SerializeField]
	private Animator m_animator;

	[SerializeField]
	private Transform m_modelRoot;

	[SerializeField]
	private SkinnedMeshRenderer m_clothingRendererSkinnedMesh;

	[SerializeField]
	private SkinnedMeshRenderer m_skinRendererSkinnedMesh;

	[SerializeField]
	private SkinnedMeshRenderer m_hairRendererSkinnedMesh;

	[SerializeField]
	private SkinnedMeshRenderer m_facialHairRendererSkinnedMesh;

	[SerializeField]
	private Renderer m_faceRenderer;

	[SerializeField]
	private Renderer m_faceRendererFemale;

	[SerializeField]
	private Renderer m_faceRendererMale;

	[SerializeField]
	private FlashManager m_flashManager;

	[SerializeField]
	private FlashManagerOutline m_outlineFlash;

	[SerializeField]
	private DamageFlash m_damageFlash;

	[SerializeField]
	private RagdollController m_ragdollController;

	[SerializeField]
	private Transform m_handTransform;

	[SerializeField]
	private GameObject m_onFireHierarchy;

	[SerializeField]
	private Transform m_pelvisTransform;

	[SerializeField]
	private Transform m_headTransform;

	[SerializeField]
	private SkinnedMeshRenderer m_baseReferenceMesh;

	[SerializeField]
	private Mesh m_skinMeshFemale;

	[SerializeField]
	private Mesh m_clothingMeshFemale;

	[SerializeField]
	private Mesh m_skinMeshMale;

	[SerializeField]
	private Mesh m_clothingMeshMale;

	private Material m_materialSkin;

	private Material m_materialClothing;

	private Material m_materialEyes;

	private Material m_materialMouth;

	private SkinnedMeshRenderer m_gearRendererBoots;

	private SkinnedMeshRenderer m_gearRendererGloves;

	private SkinnedMeshRenderer m_gearRendererHeadgear;

	private SkinnedMeshRenderer m_gearRendererO2Mask;

	private SkinnedMeshRenderer m_gearRendererVest;

	private Rigidbody[] m_rigidbodies;

	private Collider[] m_colliders;

	private bool m_isRagdoll;

	private Action m_exitStateCallbackCurrent;

	[SerializeField]
	private CrewmanFacialAnimController m_facialAnimController;

	[SerializeField]
	private GameObject m_extinguisherNozzle;

	[SerializeField]
	private Material m_statueMaterial;

	[SerializeField]
	private Material m_statueMaterialEyes;

	[SerializeField]
	private Material m_statueMaterialMouth;

	[SerializeField]
	private int m_previewLayer;

	[SerializeField]
	private RuntimeAnimatorController m_animationControllerKnackered;

	private RuntimeAnimatorController m_previousAnimationController;

	private int m_startLayer;

	private Camera m_cachedCamera;

	private Dictionary<SkinnedMeshRenderer, bool> m_isHidingHair = new Dictionary<SkinnedMeshRenderer, bool>();

	private Dictionary<SkinnedMeshRenderer, bool> m_isHidingFacialHair = new Dictionary<SkinnedMeshRenderer, bool>();

	public CrewmanFacialAnimController FacialAnimController => m_facialAnimController;

	private void Awake()
	{
		m_startLayer = base.gameObject.layer;
		m_rigidbodies = base.gameObject.GetComponentsInChildren<Rigidbody>(includeInactive: true);
		m_colliders = base.gameObject.GetComponentsInChildren<Collider>(includeInactive: true);
		SetRagdoll(ragdoll: false);
		m_gearRendererBoots = m_modelRoot.Find("Crewman_Gear_Boots").GetComponent<SkinnedMeshRenderer>();
		m_gearRendererGloves = m_modelRoot.Find("Crewman_Gear_Gloves").GetComponent<SkinnedMeshRenderer>();
		m_gearRendererHeadgear = m_modelRoot.Find("Crewman_Gear_Headgear").GetComponent<SkinnedMeshRenderer>();
		m_gearRendererO2Mask = m_modelRoot.Find("Crewman_Gear_Oxygen").GetComponent<SkinnedMeshRenderer>();
		m_gearRendererVest = m_modelRoot.Find("Crewman_Gear_Vest").GetComponent<SkinnedMeshRenderer>();
		SetupGearRenderer(m_gearRendererBoots);
		SetupGearRenderer(m_gearRendererGloves);
		SetupGearRenderer(m_gearRendererHeadgear);
		SetupGearRenderer(m_gearRendererO2Mask);
		SetupGearRenderer(m_gearRendererVest);
		m_materialClothing = m_clothingRendererSkinnedMesh.materials[0];
		m_materialSkin = m_skinRendererSkinnedMesh.materials[0];
		m_onFireHierarchy.SetActive(value: false);
		m_cachedCamera = Camera.main;
	}

	public bool IsVisibleRough(float maxForwardDist)
	{
		if (m_cachedCamera != null)
		{
			Vector3 lhs = base.transform.position - m_cachedCamera.transform.position;
			if (Vector3.Dot(lhs, m_cachedCamera.transform.forward) > 0f && lhs.magnitude < maxForwardDist)
			{
				Vector3 vector = m_cachedCamera.WorldToScreenPoint(base.transform.position);
				if (vector.x >= -64f && vector.y >= -64f && vector.x < (float)(Screen.width + 64) && vector.y < (float)(Screen.height + 64))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SwitchToKnackeredController()
	{
		if (m_previousAnimationController == null)
		{
			m_previousAnimationController = m_animator.runtimeAnimatorController;
		}
		m_animator.runtimeAnimatorController = m_animationControllerKnackered;
	}

	public void SwitchToNormalController()
	{
		if (m_previousAnimationController != null)
		{
			m_animator.runtimeAnimatorController = m_previousAnimationController;
		}
	}

	public void SetOnFire(bool onFire)
	{
		m_onFireHierarchy.SetActive(onFire);
	}

	public Transform GetHandTransform()
	{
		return m_handTransform;
	}

	public FlashManager GetFlashManager()
	{
		return m_flashManager;
	}

	public FlashManagerOutline GetOutlineFlash()
	{
		return m_outlineFlash;
	}

	public DamageFlash GetDamageFlash()
	{
		return m_damageFlash;
	}

	public RagdollController GetRagdollController()
	{
		return m_ragdollController;
	}

	public Transform GetPelvisTransform()
	{
		return m_pelvisTransform;
	}

	public Transform GetHeadTransform()
	{
		return m_headTransform;
	}

	public void SetFromCrewman(Crewman c)
	{
		SetUpSkinGraphics(c.GetSkinTone());
		m_hairRendererSkinnedMesh.sharedMesh = Singleton<CrewmanAppearanceCatalogue>.Instance.GetHairMesh(c.GetModelType(), c.GetHairVariation());
		int num = c.GetHairColor();
		if (num == 0 && m_hairRendererSkinnedMesh.sharedMesh != null)
		{
			num = 3;
		}
		SetUpHairGraphics(num);
		if (c.GetModelType() == Crewman.ModelType.Female)
		{
			m_skinRendererSkinnedMesh.sharedMesh = m_skinMeshFemale;
			m_clothingRendererSkinnedMesh.sharedMesh = m_clothingMeshFemale;
			m_facialHairRendererSkinnedMesh.enabled = false;
			m_facialHairRendererSkinnedMesh.sharedMesh = null;
			m_faceRenderer = m_faceRendererFemale;
			m_faceRendererMale.enabled = false;
			m_faceRendererFemale.enabled = true;
			m_facialAnimController.SetRenderer(m_faceRenderer);
		}
		else
		{
			m_skinRendererSkinnedMesh.sharedMesh = m_skinMeshMale;
			m_clothingRendererSkinnedMesh.sharedMesh = m_clothingMeshMale;
			m_faceRenderer = m_faceRendererMale;
			m_faceRendererMale.enabled = true;
			m_faceRendererFemale.enabled = false;
			Mesh facialHairMesh = Singleton<CrewmanAppearanceCatalogue>.Instance.GetFacialHairMesh(c.GetFacialHairVariation());
			if (facialHairMesh == null)
			{
				m_facialHairRendererSkinnedMesh.enabled = false;
				m_facialHairRendererSkinnedMesh.sharedMesh = null;
			}
			else
			{
				m_facialHairRendererSkinnedMesh.enabled = true;
				m_facialHairRendererSkinnedMesh.sharedMesh = facialHairMesh;
			}
		}
		m_materialEyes = m_faceRenderer.materials[0];
		m_materialMouth = m_faceRenderer.materials[1];
		SetUpEyesGraphics(c.GetEyeColor());
		SetUpMouthGraphics(c.GetMouthType());
		m_hairRendererSkinnedMesh.material = m_materialSkin;
		m_facialHairRendererSkinnedMesh.material = m_materialSkin;
	}

	public void SetEquipmentFromCrewman(Crewman c)
	{
		foreach (CrewmanEquipmentBase item in c.GetAllEquipment())
		{
			if (item != null)
			{
				SetupGearItemGraphics(item, c, asPreviewOnly: false);
			}
		}
	}

	public void SetUpClothingGraphics(int index)
	{
		m_materialClothing.SetTextureOffset("_MainTex", Singleton<CrewmanAppearanceCatalogue>.Instance.GetOffsetForClothingIndex(index));
	}

	private void SetUpSkinGraphics(int index)
	{
		m_materialSkin.SetTextureOffset("_MainTex", Singleton<CrewmanAppearanceCatalogue>.Instance.GetOffsetForSkinIndex(index));
	}

	private void SetUpHairGraphics(int index)
	{
		float x = m_materialSkin.GetTextureOffset("_MainTex").x;
		m_materialSkin.SetTextureOffset("_MainTex", new Vector2(x, Singleton<CrewmanAppearanceCatalogue>.Instance.GetOffsetForHairIndex(index).y));
	}

	private void SetUpEyesGraphics(int index)
	{
		float x = 0f;
		float num = 1f / (float)Singleton<CrewmanAppearanceCatalogue>.Instance.GetIndexCountEyes() * (float)index;
		m_materialEyes.SetTextureOffset("_MainTex", new Vector2(x, 0f - num));
	}

	private void SetUpMouthGraphics(int index)
	{
		float x = 0f;
		float num = 1f / (float)Singleton<CrewmanAppearanceCatalogue>.Instance.GetIndexCountMouth() * (float)index;
		m_materialMouth.SetTextureOffset("_MainTex", new Vector2(x, 0f - num));
	}

	private void SetupGearRenderer(SkinnedMeshRenderer gearSkinnedMeshRenderer)
	{
		gearSkinnedMeshRenderer.enabled = false;
		gearSkinnedMeshRenderer.sharedMesh = null;
	}

	private void SetHairHidden()
	{
		bool flag = false;
		foreach (KeyValuePair<SkinnedMeshRenderer, bool> item in m_isHidingHair)
		{
			flag |= item.Value;
			if (flag)
			{
				break;
			}
		}
		bool flag2 = false;
		foreach (KeyValuePair<SkinnedMeshRenderer, bool> item2 in m_isHidingFacialHair)
		{
			flag2 |= item2.Value;
			if (flag2)
			{
				break;
			}
		}
		if (flag)
		{
			m_hairRendererSkinnedMesh.enabled = false;
		}
		else
		{
			m_hairRendererSkinnedMesh.enabled = true;
		}
		if (flag2)
		{
			m_facialHairRendererSkinnedMesh.enabled = false;
		}
		else if (m_facialHairRendererSkinnedMesh.sharedMesh != null)
		{
			m_facialHairRendererSkinnedMesh.enabled = true;
		}
	}

	public void SetupGearItemGraphics(CrewmanEquipmentBase gearData, Crewman crewmanInfo, bool asPreviewOnly)
	{
		if (gearData == null)
		{
			return;
		}
		bool value = gearData.ShouldHideHair();
		bool value2 = gearData.ShouldHideFacialHair();
		if (asPreviewOnly)
		{
			value2 = false;
			value = false;
		}
		if (gearData.GetGearType() == CrewmanGearType.Flightsuit)
		{
			int textureOffsetIndex = gearData.GetTextureOffsetIndex();
			SetUpClothingGraphics((textureOffsetIndex != 0) ? textureOffsetIndex : crewmanInfo.GetCivilianClothing());
			return;
		}
		SkinnedMeshRenderer skinnedMeshRenderer = new SkinnedMeshRenderer();
		switch (gearData.GetGearType())
		{
		case CrewmanGearType.Boots:
			skinnedMeshRenderer = m_gearRendererBoots;
			break;
		case CrewmanGearType.Gloves:
			skinnedMeshRenderer = m_gearRendererGloves;
			break;
		case CrewmanGearType.Headgear:
			skinnedMeshRenderer = m_gearRendererHeadgear;
			break;
		case CrewmanGearType.Oxygen:
			skinnedMeshRenderer = m_gearRendererO2Mask;
			break;
		case CrewmanGearType.Vest:
			skinnedMeshRenderer = m_gearRendererVest;
			break;
		}
		if (skinnedMeshRenderer != null)
		{
			if (asPreviewOnly)
			{
				skinnedMeshRenderer.gameObject.layer = m_previewLayer;
			}
			else
			{
				skinnedMeshRenderer.gameObject.layer = m_startLayer;
			}
			skinnedMeshRenderer.sharedMesh = gearData.GetMesh();
			if (gearData.GetMaterials() != null)
			{
				skinnedMeshRenderer.sharedMaterials = gearData.GetMaterials();
			}
			if (skinnedMeshRenderer.sharedMesh != null)
			{
				skinnedMeshRenderer.enabled = true;
			}
			else
			{
				skinnedMeshRenderer.enabled = false;
			}
			m_isHidingHair[skinnedMeshRenderer] = value;
			m_isHidingFacialHair[skinnedMeshRenderer] = value2;
			SetHairHidden();
		}
	}

	public void SetIsMoving(bool moving)
	{
		m_animator.SetBool("Moving", moving);
	}

	public void SetIsCrawl(bool crawl)
	{
		m_animator.SetBool("Crawl", crawl);
	}

	public void SetIsInteracting(bool isInteracting, bool isHealing, bool isReparingEngine, bool isRepairFuelTank)
	{
		m_animator.SetBool("Interacting", isInteracting);
		m_animator.SetBool("Healing", isHealing);
		m_animator.SetBool("RepairEngine", isReparingEngine);
		m_animator.SetBool("RepairFuelTank", isRepairFuelTank);
	}

	public void SetIsFalling(bool falling)
	{
		m_animator.SetBool("Falling", falling);
	}

	public void SetIsBrace(bool brace)
	{
		m_animator.SetBool("Bracing", brace);
	}

	public void SetInteractionSpeed(float speed)
	{
		m_animator.SetFloat("InteractSpeed", speed);
	}

	public void SetIsAtStation(bool atStation)
	{
		m_animator.SetBool("AtStation", atStation);
	}

	public void SetIsProne(bool prone)
	{
		m_animator.SetBool("Prone", prone);
	}

	public void SetIsStandingAtStation(bool standingAtStation)
	{
		m_animator.SetBool("AtStationStanding", standingAtStation);
	}

	public void SetIsInFoetalPositionAtStation(bool inFoetalPosition)
	{
		m_animator.SetBool("AtStationInFoetalPosition", inFoetalPosition);
	}

	public void SetIsCollapsed(bool collapsed)
	{
		m_animator.SetBool("Collapse", collapsed);
	}

	public void SetIsFireFighting(bool firefighting)
	{
		m_animator.SetBool("FightFire", firefighting);
	}

	public void SetIsFiring(bool firing)
	{
		m_animator.SetBool("Firing", firing);
	}

	public void SetMoveSpeed(float moveSpeed)
	{
		m_animator.SetFloat("MoveSpeed", Mathf.Clamp(moveSpeed, 0f, 2f));
	}

	public void SetIsDead(bool dead)
	{
		m_animator.SetBool("Dead", dead);
	}

	public void DoExtinguisherEffects(bool doEffect)
	{
		m_extinguisherNozzle.SetActive(doEffect);
	}

	public void SetCurrentLean(float currentLean)
	{
		m_animator.SetFloat("LeanAmount", currentLean);
	}

	private void SetRagdoll(bool ragdoll)
	{
		m_isRagdoll = ragdoll;
		m_animator.enabled = !ragdoll;
		SetRigidbodiesKinematic(!ragdoll);
		EnableColliders(ragdoll);
	}

	public void SetStatue()
	{
		m_skinRendererSkinnedMesh.material = m_statueMaterial;
		m_clothingRendererSkinnedMesh.material = m_statueMaterial;
		if (m_hairRendererSkinnedMesh != null)
		{
			m_hairRendererSkinnedMesh.material = m_statueMaterial;
		}
		if (m_facialHairRendererSkinnedMesh != null)
		{
			m_facialHairRendererSkinnedMesh.material = m_statueMaterial;
		}
		m_gearRendererBoots.material = m_statueMaterial;
		m_gearRendererGloves.material = m_statueMaterial;
		m_gearRendererHeadgear.material = m_statueMaterial;
		m_gearRendererO2Mask.material = m_statueMaterial;
		m_gearRendererVest.material = m_statueMaterial;
		Vector2 textureOffset = m_faceRenderer.materials[0].GetTextureOffset("_MainTex");
		Vector2 textureOffset2 = m_faceRenderer.materials[1].GetTextureOffset("_MainTex");
		Material[] materials = new Material[2] { m_statueMaterialEyes, m_statueMaterialMouth };
		m_faceRenderer.materials = materials;
		m_faceRenderer.materials[0].SetTextureOffset("_MainTex", textureOffset);
		m_faceRenderer.materials[1].SetTextureOffset("_MainTex", textureOffset2);
		m_animator.SetLayerWeight(1, 0f);
		m_animator.SetBool("Statue", value: true);
		m_facialAnimController.SetStatue();
	}

	public void TriggerAnimation(string animationTriggerName, Action onAnimationComplete)
	{
		m_exitStateCallbackCurrent = onAnimationComplete;
		m_animator.SetTrigger(animationTriggerName);
	}

	public void ForceAnimationReTrigger(string animationTriggerName)
	{
		if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName(animationTriggerName))
		{
			m_animator.SetTrigger(animationTriggerName);
		}
	}

	public void SetUseRealtimeForAnimation(bool useRealtime)
	{
		if (useRealtime)
		{
			m_animator.updateMode = AnimatorUpdateMode.UnscaledTime;
			m_facialAnimController.SetUseRealtimeWaitForSeconds(useRealtime: true);
		}
		else
		{
			m_animator.updateMode = AnimatorUpdateMode.Normal;
			m_facialAnimController.SetUseRealtimeWaitForSeconds(useRealtime: false);
		}
	}

	public void OnStateExit()
	{
		if (m_exitStateCallbackCurrent != null)
		{
			m_exitStateCallbackCurrent();
			m_exitStateCallbackCurrent = null;
		}
	}

	private void SetRigidbodiesKinematic(bool kinematic)
	{
		for (int i = 0; i < m_rigidbodies.Length; i++)
		{
			m_rigidbodies[i].isKinematic = kinematic;
		}
	}

	private void EnableColliders(bool enable)
	{
		for (int i = 0; i < m_colliders.Length; i++)
		{
			m_colliders[i].enabled = enable;
		}
	}

	public Mesh CombineMeshFromInfo(List<CombineMeshInfo> listInfo, List<Material> materialList)
	{
		Dictionary<int, List<CombineMeshInfo>> dictionary = new Dictionary<int, List<CombineMeshInfo>>();
		Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
		HashSet<int> hashSet = new HashSet<int>();
		int num = 0;
		materialList.Clear();
		foreach (CombineMeshInfo item in listInfo)
		{
			List<CombineMeshInfo> value = null;
			int instanceID = item.m_material.GetInstanceID();
			dictionary.TryGetValue(instanceID, out value);
			if (value == null)
			{
				value = (dictionary[instanceID] = new List<CombineMeshInfo>());
				materialList.Add(item.m_material);
				dictionary2[instanceID] = 0;
			}
			dictionary2[instanceID] += item.m_meshTarget.GetTriangles(item.m_subMesh).Length;
			value.Add(item);
			item.m_cacheMeshTargetId = item.m_meshTarget.GetInstanceID();
			if (!hashSet.Contains(item.m_cacheMeshTargetId))
			{
				hashSet.Add(item.m_cacheMeshTargetId);
				num += item.m_meshTarget.vertexCount;
			}
		}
		Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
		int num2 = 0;
		Transform[] bones = m_baseReferenceMesh.bones;
		foreach (Transform transform in bones)
		{
			dictionary3[transform.name] = num2;
			num2++;
		}
		Mesh mesh = new Mesh();
		mesh.subMeshCount = dictionary.Count;
		Dictionary<int, int> dictionary4 = new Dictionary<int, int>();
		Vector3[] array = new Vector3[num];
		Vector3[] array2 = new Vector3[num];
		Vector2[] array3 = new Vector2[num];
		BoneWeight[] array4 = new BoneWeight[num];
		int num3 = 0;
		int num4 = 0;
		int[][] array5 = new int[dictionary.Keys.Count][];
		int num5 = 0;
		foreach (KeyValuePair<int, List<CombineMeshInfo>> item2 in dictionary)
		{
			int[] array6 = new int[dictionary2[item2.Key]];
			int num6 = 0;
			foreach (CombineMeshInfo item3 in item2.Value)
			{
				Dictionary<int, int> dictionary5 = new Dictionary<int, int>();
				int num7 = 0;
				Transform[] bones2 = item3.m_skinnedMeshRenderer.bones;
				foreach (Transform transform2 in bones2)
				{
					int num9 = (dictionary5[num7] = dictionary3[transform2.name]);
					num7++;
				}
				int value2 = 0;
				if (!dictionary4.TryGetValue(item3.m_cacheMeshTargetId, out value2))
				{
					int vertexCount = item3.m_meshTarget.vertexCount;
					Vector3[] vertices = item3.m_meshTarget.vertices;
					Vector3[] normals = item3.m_meshTarget.normals;
					Vector2[] uv = item3.m_meshTarget.uv;
					BoneWeight[] boneWeights = item3.m_meshTarget.boneWeights;
					for (int k = 0; k < vertexCount; k++)
					{
						ref Vector3 reference = ref array[num3];
						reference = vertices[k];
						ref Vector3 reference2 = ref array2[num3];
						reference2 = normals[k];
						ref Vector2 reference3 = ref array3[num3];
						reference3 = uv[k];
						boneWeights[k].boneIndex0 = dictionary5[boneWeights[k].boneIndex0];
						boneWeights[k].boneIndex1 = dictionary5[boneWeights[k].boneIndex1];
						boneWeights[k].boneIndex2 = dictionary5[boneWeights[k].boneIndex2];
						boneWeights[k].boneIndex3 = dictionary5[boneWeights[k].boneIndex3];
						ref BoneWeight reference4 = ref array4[num3];
						reference4 = boneWeights[k];
						num3++;
					}
					dictionary4[item3.m_cacheMeshTargetId] = num4;
					value2 = num4;
					num4 += item3.m_meshTarget.vertexCount;
				}
				int[] triangles = item3.m_meshTarget.GetTriangles(item3.m_subMesh);
				int[] array7 = triangles;
				foreach (int num10 in array7)
				{
					array6[num6] = num10 + value2;
					num6++;
				}
			}
			array5[num5] = array6;
			num5++;
		}
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = array3;
		mesh.boneWeights = array4;
		List<Matrix4x4> list2 = new List<Matrix4x4>();
		mesh.bindposes = m_baseReferenceMesh.sharedMesh.bindposes;
		int num11 = 0;
		int[][] array8 = array5;
		foreach (int[] triangles2 in array8)
		{
			mesh.SetTriangles(triangles2, num11);
			num11++;
		}
		mesh.UploadMeshData(markNoLogerReadable: false);
		return mesh;
	}

	public void CombineMeshes()
	{
		List<SkinnedMeshRenderer> list = new List<SkinnedMeshRenderer>();
		list.Add(m_clothingRendererSkinnedMesh);
		list.Add(m_skinRendererSkinnedMesh);
		if (m_gearRendererBoots.sharedMesh != null)
		{
			list.Add(m_gearRendererBoots);
		}
		if (m_gearRendererGloves.sharedMesh != null)
		{
			list.Add(m_gearRendererGloves);
		}
		if (m_gearRendererHeadgear.sharedMesh != null)
		{
			list.Add(m_gearRendererHeadgear);
		}
		if (m_gearRendererO2Mask.sharedMesh != null)
		{
			list.Add(m_gearRendererO2Mask);
		}
		if (m_gearRendererVest.sharedMesh != null)
		{
			list.Add(m_gearRendererVest);
		}
		if (m_hairRendererSkinnedMesh.sharedMesh != null && m_hairRendererSkinnedMesh.enabled)
		{
			list.Add(m_hairRendererSkinnedMesh);
		}
		if (m_facialHairRendererSkinnedMesh.sharedMesh != null && m_facialHairRendererSkinnedMesh.enabled)
		{
			list.Add(m_facialHairRendererSkinnedMesh);
		}
		List<CombineMeshInfo> list2 = new List<CombineMeshInfo>();
		for (int i = 0; i < list.Count; i++)
		{
			for (int j = 0; j < list[i].sharedMesh.subMeshCount; j++)
			{
				CombineMeshInfo combineMeshInfo = new CombineMeshInfo();
				combineMeshInfo.m_skinnedMeshRenderer = list[i];
				combineMeshInfo.m_meshTarget = list[i].sharedMesh;
				combineMeshInfo.m_subMesh = j;
				combineMeshInfo.m_material = list[i].sharedMaterials[j % list[i].sharedMaterials.Length];
				list2.Add(combineMeshInfo);
			}
		}
		List<Material> list3 = new List<Material>();
		Mesh sharedMesh = CombineMeshFromInfo(list2, list3);
		m_baseReferenceMesh.sharedMesh = sharedMesh;
		m_baseReferenceMesh.sharedMaterials = list3.ToArray();
		m_baseReferenceMesh.enabled = true;
		m_skinRendererSkinnedMesh.enabled = false;
		m_clothingRendererSkinnedMesh.enabled = false;
		m_gearRendererBoots.enabled = false;
		m_gearRendererGloves.enabled = false;
		m_gearRendererHeadgear.enabled = false;
		m_gearRendererO2Mask.enabled = false;
		m_gearRendererVest.enabled = false;
		m_facialHairRendererSkinnedMesh.enabled = false;
		m_hairRendererSkinnedMesh.enabled = false;
	}

	public void SetAnimatorController(RuntimeAnimatorController animController)
	{
		m_animator.runtimeAnimatorController = animController;
	}
}
