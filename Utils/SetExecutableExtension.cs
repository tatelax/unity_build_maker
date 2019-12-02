using System;

namespace BuildMaker.Utils
{
    public static class SetExecutableExtension
    {
        public static string Get(Platform platform)
        {
            switch (platform)
            {
                case Platform.Windows_x86_64:
                    return ".exe";
                case Platform.Windows_x86:
                    return ".exe";
                case Platform.macOS:
                    return ".app";
                case Platform.Linux:
                    throw new NotImplementedException();
                case Platform.Android:
                    return ".apk";
                case Platform.iOS:
                    throw new NotImplementedException();
                case Platform.WebGL:
                    throw new NotImplementedException();
                case Platform.PlayStation4:
                    throw new NotImplementedException();
                case Platform.XboxOne:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
        }
    }
}
