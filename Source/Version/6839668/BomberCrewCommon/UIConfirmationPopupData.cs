using System;

namespace BomberCrewCommon;

public class UIConfirmationPopupData : UIPopupData
{
	public Action<UIPopUp> m_confirmationHandler;

	public Action<UIPopUp> m_cancelHandler;

	public UIConfirmationPopupData()
	{
	}

	public UIConfirmationPopupData(Action<UIPopUp> confirmationHandler, Action<UIPopUp> cancelHandler)
	{
		m_confirmationHandler = confirmationHandler;
		m_cancelHandler = cancelHandler;
	}
}
