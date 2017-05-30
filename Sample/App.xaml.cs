using Prism.Unity;
using Sample.Views;
using Sample.Models;
using Microsoft.Practices.Unity;
using Xamarin.Forms;

namespace Sample
{
	public partial class App : PrismApplication
	{
		public App(IPlatformInitializer initializer = null) : base(initializer) { }

		protected override void OnInitialized()
		{
			InitializeComponent();

			NavigationService.NavigateAsync("NavigationPage/MainPage");
		}

		protected override void RegisterTypes()
		{
			Container.RegisterType<ToDo,ToDo>();
			Container.RegisterTypeForNavigation<MainPage>();
			Container.RegisterTypeForNavigation<NavigationPage>();
		}
	}
}

