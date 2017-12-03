using System;

namespace Security.Service
{
    public interface ISecurityService
    {
        string Login { get; }
        string GetPassword(DateTimeOffset time);
    }
}
