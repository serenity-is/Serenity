using Serenity.Data;
using System;

namespace Serenity
{
    public interface IProgressManager
    {
        IDisposable EnterSubProcess(double percent);
        void ReportProgress(double percent, string status);
        bool IsCancelled { get; }
        double Percent { get; set; }
        string Status { get; set; }
    }

    public interface IProgressDialog
    {
        bool StartWorker(object data, Func<IProgressManager, object, object> worker);
    }

    public static class ProgressManagerExtensions
    {
        public static void SetProgress(this IProgressManager pm, int current, int total, string status)
        {
            pm.ReportProgress(total == 0 ? 0d : (double)current * 100d / (double)total, status);
        }

        public static void SetProgress(this IProgressManager pm, int current, int total)
        {
            pm.Percent = (total == 0 ? 0d : (double)current * 100d / (double)total);
        }

        public static IDisposable EnterSubProcess(this IProgressManager pm, int sub, int total)
        {
            return pm.EnterSubProcess(total == 0 ? 0d : (double)sub * 100d / (double)total);
        }
    }

    /*Dependency.Resolve<IProgressDialog>().Start((pm, data) => {
    });*/
}