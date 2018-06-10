using Prism.Commands;
using Prism.Windows.Mvvm;

namespace WebScraipingApp.ViewModels
{
    class MainPageViewModel : ViewModelBase
    {

#region "Hello World"


        private string input;

        public string Input {
            get { return input; }
            set { SetProperty(ref input, value); }
        }

        private string output;

        public string Output
        {
            get { return output; }
            set { SetProperty(ref output, value); }
        }

        public DelegateCommand CreateOutputCommand { get; }

        public MainPageViewModel(){
            CreateOutputCommand = new DelegateCommand(() =>
            {
                Output = $"{Input}が入力されました。";

            }, () => !string.IsNullOrWhiteSpace(Input)).ObservesProperty(() => Input);
        }

        #endregion

#region


#endregion

    }
}
