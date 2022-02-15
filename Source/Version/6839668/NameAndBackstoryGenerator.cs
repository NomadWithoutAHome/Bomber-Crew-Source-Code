using System.Collections.Generic;
using BomberCrewCommon;
using UnityEngine;

public class NameAndBackstoryGenerator : Singleton<NameAndBackstoryGenerator>, LoadableSystem
{
	public class DataSet
	{
		public List<string> m_surnames = new List<string>();

		public List<string> m_firstNames = new List<string>();

		public List<string> m_firstNamesF = new List<string>();

		public List<string> m_towns = new List<string>();
	}

	[SerializeField]
	private TextAsset m_firstNameData;

	[SerializeField]
	private TextAsset m_firstNameFData;

	[SerializeField]
	private TextAsset m_surnameData;

	[SerializeField]
	private CrewmanTrait[] m_traitsData;

	[SerializeField]
	private TextAsset m_firstNameDataUSA;

	[SerializeField]
	private TextAsset m_firstNameFDataUSA;

	[SerializeField]
	private TextAsset m_surnameDataUSA;

	[SerializeField]
	private CrewmanTrait[] m_traitsDataUSA;

	[SerializeField]
	private TextAsset m_townsData;

	[SerializeField]
	private TextAsset m_townsDataUSA;

	[SerializeField]
	private AnimationCurve m_ageGeneratorCurve;

	private DataSet m_default = new DataSet();

	private DataSet m_usaData = new DataSet();

	private bool m_initialised;

	private void Awake()
	{
		Singleton<SystemLoader>.Instance.RegisterLoadableSystem(this);
	}

	public void SetAllTraits(CrewmanTrait[] traits)
	{
		m_traitsData = traits;
	}

	private void InitialiseDataSet(DataSet ds, TextAsset namesM, TextAsset namesF, TextAsset surnamesA, TextAsset townsA)
	{
		string[] array = namesM.text.Split(",\n".ToCharArray());
		int num = 5;
		for (int i = 0; i < array.Length; i++)
		{
			for (int j = 0; j < num; j++)
			{
				ds.m_firstNames.Add(array[i]);
			}
			if (i % (array.Length / 5) == 0)
			{
				num--;
				if (num == 0)
				{
					num = 1;
				}
			}
		}
		array = namesF.text.Split(",\n".ToCharArray());
		num = 5;
		for (int k = 0; k < array.Length; k++)
		{
			for (int l = 0; l < num; l++)
			{
				ds.m_firstNamesF.Add(array[k]);
			}
			if (k % (array.Length / 5) == 0)
			{
				num--;
				if (num == 0)
				{
					num = 1;
				}
			}
		}
		array = surnamesA.text.Split(",\n".ToCharArray());
		ds.m_surnames.AddRange(array);
		string[] collection = townsA.text.Split(",\n".ToCharArray());
		ds.m_towns.AddRange(collection);
	}

	private void Initialise()
	{
		if (!m_initialised)
		{
			InitialiseDataSet(m_default, m_firstNameData, m_firstNameFData, m_surnameData, m_townsData);
			InitialiseDataSet(m_usaData, m_firstNameDataUSA, m_firstNameFDataUSA, m_surnameDataUSA, m_townsDataUSA);
			m_initialised = true;
		}
	}

	public int GetAge()
	{
		return Mathf.RoundToInt(m_ageGeneratorCurve.Evaluate(Random.Range(0f, 1f)));
	}

	public string GenerateFirstName(bool female, bool usa)
	{
		if (!m_initialised)
		{
			Initialise();
		}
		DataSet dataSet = m_default;
		if (usa)
		{
			dataSet = m_usaData;
		}
		if (female)
		{
			return dataSet.m_firstNamesF[Random.Range(0, dataSet.m_firstNamesF.Count)];
		}
		return dataSet.m_firstNames[Random.Range(0, dataSet.m_firstNames.Count)];
	}

	public string GenerateSurame(bool usa)
	{
		if (!m_initialised)
		{
			Initialise();
		}
		DataSet dataSet = m_default;
		if (usa)
		{
			dataSet = m_usaData;
		}
		return dataSet.m_surnames[Random.Range(0, dataSet.m_surnames.Count)];
	}

	public string GenerateTown(bool usa)
	{
		if (!m_initialised)
		{
			Initialise();
		}
		DataSet dataSet = m_default;
		if (usa)
		{
			dataSet = m_usaData;
		}
		return dataSet.m_towns[Random.Range(0, dataSet.m_towns.Count)];
	}

	public CrewmanTraitJS GenerateTrait(bool usa)
	{
		if (usa)
		{
			return new CrewmanTraitJS(m_traitsDataUSA[Random.Range(0, m_traitsDataUSA.Length)]);
		}
		return new CrewmanTraitJS(m_traitsData[Random.Range(0, m_traitsData.Length)]);
	}

	public bool IsLoadComplete()
	{
		return m_initialised;
	}

	public void StartLoad()
	{
		Initialise();
	}

	public void ContinueLoad()
	{
	}

	public string GetName()
	{
		return "NamesAndBackStories";
	}

	public LoadableSystem[] GetDependencies()
	{
		return null;
	}
}
