#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the ValueMin of an RangeSlider.
	/// </summary>
	public class RangeSliderValueMinObserver : ComponentDataObserver<UIWidgets.RangeSlider, int>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.RangeSlider target)
		{
			target.OnValuesChange.AddListener(OnValuesChangeRangeSlider);
		}

		/// <inheritdoc />
		protected override int GetValue(UIWidgets.RangeSlider target)
		{
			return target.ValueMin;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.RangeSlider target)
		{
			target.OnValuesChange.RemoveListener(OnValuesChangeRangeSlider);
		}

		void OnValuesChangeRangeSlider(int arg0, int arg1)
		{
			OnTargetValueChanged();
		}
	}
}
#endif