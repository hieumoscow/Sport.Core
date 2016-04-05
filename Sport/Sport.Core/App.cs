using MvvmCross.Platform.IoC;

namespace Sport.Core
{
    public enum Platform
    {
        iOS,
        WindowsPhone,
        Android
    }
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        private static Data _data;

        public static Data Data => _data ?? (_data = new Data());

        public static Platform Platform { get; set; }
        public App(Platform platform)
        {
            Platform = platform;
        }
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterAppStart<ViewModels.FirstViewModel>();
        }
    }
}
