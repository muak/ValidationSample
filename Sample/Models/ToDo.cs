using System;
using Prism.Mvvm;
namespace Sample.Models
{
	public class ToDo:BindableBase
	{
		private string _Title;
		public string Title {
			get { return _Title; }
			set { SetProperty(ref _Title, value); }
		}

		private string _Descriptions;
		public string Descriptions {
			get { return _Descriptions; }
			set { SetProperty(ref _Descriptions, value); }
		}

        private DateTime _DateTo = DateTime.Today.AddDays(1);
        public DateTime DateTo {
            get { return _DateTo; }
            set { SetProperty(ref _DateTo, value); }
        }

        private DateTime _DateFrom = DateTime.Today;
        public DateTime DateFrom {
            get { return _DateFrom; }
            set { SetProperty(ref _DateFrom, value); }
        }

        private int _Priority;
        public int Priority {
            get { return _Priority; }
            set { SetProperty(ref _Priority, value); }
        }
	}
}
