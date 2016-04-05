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
