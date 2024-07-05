namespace FitCom
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            GoToAsync("//MainPage");
        }
    }
}
