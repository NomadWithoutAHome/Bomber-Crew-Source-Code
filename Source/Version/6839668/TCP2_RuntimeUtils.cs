using System.Collections.Generic;
using UnityEngine;

public static class TCP2_RuntimeUtils
{
	private const string BASE_SHADER_PATH = "Toony Colors Pro 2/";

	private const string VARIANT_SHADER_PATH = "Hidden/Toony Colors Pro 2/Variants/";

	private const string BASE_SHADER_NAME = "Desktop";

	private const string BASE_SHADER_NAME_MOB = "Mobile";

	private static List<string[]> ShaderVariants = new List<string[]>
	{
		new string[2] { "Specular", "TCP2_SPEC" },
		new string[3] { "Reflection", "TCP2_REFLECTION", "TCP2_REFLECTION_MASKED" },
		new string[2] { "Matcap", "TCP2_MC" },
		new string[2] { "Rim", "TCP2_RIM" },
		new string[2] { "RimOutline", "TCP2_RIMO" },
		new string[2] { "Outline", "OUTLINES" },
		new string[2] { "OutlineBlending", "OUTLINE_BLENDING" }
	};

	public static Shader GetShaderWithKeywords(Material material)
	{
		string text = ((!(material.shader != null) || !material.shader.name.ToLower().Contains("mobile")) ? "Desktop" : "Mobile");
		string text2 = text;
		foreach (string[] shaderVariant in ShaderVariants)
		{
			string[] shaderKeywords = material.shaderKeywords;
			foreach (string text3 in shaderKeywords)
			{
				for (int j = 1; j < shaderVariant.Length; j++)
				{
					if (text3 == shaderVariant[j])
					{
						text2 = text2 + " " + shaderVariant[0];
					}
				}
			}
		}
		text2 = text2.TrimEnd();
		string text4 = "Toony Colors Pro 2/";
		if (text2 != text)
		{
			text4 = "Hidden/Toony Colors Pro 2/Variants/";
		}
		return Shader.Find(text4 + text2);
	}
}
