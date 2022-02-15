using System;

namespace BomberCrewCommon;

public class UIConfirmationPopup : UIPopUp
{
	public Action<UIPopUp> HandleConfirmation;

	public Action<UIPopUp> HandleCancel;

	public virtual void Confirm()
	{
		DoConfirm();
	}

	public virtual void Cancel()
	{
		DoCancel();
	}

	public override void SetupPopup(UIPopupData data)
	{
		base.SetupPopup(data);
		if (data is UIConfirmationPopupData)
		{
			UIConfirmationPopupData uIConfirmationPopupData = (UIConfirmationPopupData)data;
			if (uIConfirmationPopupData.m_confirmationHandler != null)
			{
				HandleConfirmation = uIConfirmationPopupData.m_confirmationHandler;
			}
			if (uIConfirmationPopupData.m_cancelHandler != null)
			{
				HandleCancel = uIConfirmationPopupData.m_cancelHandler;
			}
		}
	}

	private void DoConfirm()
	{
		DismissPopup();
		if (HandleConfirmation != null)
		{
			HandleConfirmation(this);
		}
	}

	private void DoCancel()
	{
		DismissPopup();
		if (HandleCancel != null)
		{
			HandleCancel(this);
		}
	}
}
