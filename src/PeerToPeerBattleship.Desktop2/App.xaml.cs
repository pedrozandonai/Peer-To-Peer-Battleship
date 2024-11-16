namespace PeerToPeerBattleship.Desktop
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "PeerToPeerBattleship.Desktop" };
        }
    }
}
