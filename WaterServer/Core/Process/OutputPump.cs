using System.Diagnostics;

namespace Water.Host
{
    public static class OutputPump
    {
        public static void Attach(Process process, LogBuffer log)
        {
            process.OutputDataReceived += (_, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                    log.Add(8, args.Data);
            };
            process.ErrorDataReceived += (_, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                    log.Add(12, args.Data);
            };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }
    }
}
