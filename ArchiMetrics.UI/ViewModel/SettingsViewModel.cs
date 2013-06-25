namespace ArchiMetrics.UI.ViewModel
{
	using System;
	using System.ComponentModel;
	using System.Reactive.Concurrency;
	using System.Reactive.Linq;
	using Common;

	internal class SettingsViewModel : ViewModelBase
	{
		private readonly IDisposable _changeSubscription;
		private readonly ISolutionEdgeItemsRepositoryConfig _config;

		public SettingsViewModel(ISolutionEdgeItemsRepositoryConfig config)
			: base(config)
		{
			_config = config;
			_changeSubscription = Observable
				.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
					h => _config.PropertyChanged += h,
					h => _config.PropertyChanged -= h)
				.Select(x => x.EventArgs)
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(RaisePropertyChanged);
		}

		public string Path
		{
			get { return _config.Path; }
			set { _config.Path = value; }
		}

		public EdgeSource Source
		{
			get { return _config.Source; }
			set { _config.Source = value; }
		}

		public bool IncludeCodeReview
		{
			get { return _config.IncludeCodeReview; }
			set { _config.IncludeCodeReview = value; }
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_changeSubscription.Dispose();
			}

			base.Dispose(isDisposing);
		}
	}
}
