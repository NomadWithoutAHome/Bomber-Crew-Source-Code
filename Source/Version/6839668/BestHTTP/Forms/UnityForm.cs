using UnityEngine;

namespace BestHTTP.Forms;

public sealed class UnityForm : HTTPFormBase
{
	public WWWForm Form { get; set; }

	public UnityForm()
	{
	}

	public UnityForm(WWWForm form)
	{
		Form = form;
	}

	public override void CopyFrom(HTTPFormBase fields)
	{
		base.Fields = fields.Fields;
		base.IsChanged = true;
		if (Form != null)
		{
			return;
		}
		Form = new WWWForm();
		if (base.Fields == null)
		{
			return;
		}
		for (int i = 0; i < base.Fields.Count; i++)
		{
			HTTPFieldData hTTPFieldData = base.Fields[i];
			if (string.IsNullOrEmpty(hTTPFieldData.Text) && hTTPFieldData.Binary != null)
			{
				Form.AddBinaryData(hTTPFieldData.Name, hTTPFieldData.Binary, hTTPFieldData.FileName, hTTPFieldData.MimeType);
			}
			else
			{
				Form.AddField(hTTPFieldData.Name, hTTPFieldData.Text, hTTPFieldData.Encoding);
			}
		}
	}

	public override void PrepareRequest(HTTPRequest request)
	{
		if (Form.headers.ContainsKey("Content-Type"))
		{
			request.SetHeader("Content-Type", Form.headers["Content-Type"]);
		}
		else
		{
			request.SetHeader("Content-Type", "application/x-www-form-urlencoded");
		}
	}

	public override byte[] GetData()
	{
		return Form.data;
	}
}
