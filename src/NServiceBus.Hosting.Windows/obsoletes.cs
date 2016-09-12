#pragma warning disable 1591
namespace NServiceBus
{
    using System;

    [ObsoleteEx(
        TreatAsErrorFromVersion = "7.0",
        RemoveInVersion = "8.0",
        Message = "WCF integration has been removed from the host. Refer to the NServiceBus.Host Version 7 upgrade guide for further information.")]
    public interface IWcfService<TRequest, TResponse>
    {
        [ObsoleteEx(
            TreatAsErrorFromVersion = "7.0",
            RemoveInVersion = "8.0",
            Message = "WCF integration has been removed from the host. Refer to the NServiceBus.Host Version 7 upgrade guide for further information.")]
        IAsyncResult BeginProcess(TRequest request, AsyncCallback callback, object state);

        [ObsoleteEx(
            TreatAsErrorFromVersion = "7.0",
            RemoveInVersion = "8.0",
            Message = "WCF integration has been removed from the host. Refer to the NServiceBus.Host Version 7 upgrade guide for further information.")]
        TResponse EndProcess(IAsyncResult asyncResult);
    }

    [ObsoleteEx(
        TreatAsErrorFromVersion = "7.0",
        RemoveInVersion = "8.0",
        Message = "WCF integration has been removed from the host. Refer to the NServiceBus.Host Version 7 upgrade guide for further information.")]
    public abstract class WcfService<TRequest, TResponse> : IWcfService<TRequest, TResponse>
    {
        [ObsoleteEx(
            TreatAsErrorFromVersion = "7.0",
            RemoveInVersion = "8.0",
            Message = "WCF integration has been removed from the host. Refer to the NServiceBus.Host Version 7 upgrade guide for further information.")]
        IAsyncResult IWcfService<TRequest, TResponse>.BeginProcess(TRequest request, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        [ObsoleteEx(
            TreatAsErrorFromVersion = "7.0",
            RemoveInVersion = "8.0",
            Message = "WCF integration has been removed from the host. Refer to the NServiceBus.Host Version 7 upgrade guide for further information.")]
        TResponse IWcfService<TRequest, TResponse>.EndProcess(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }
    }
}