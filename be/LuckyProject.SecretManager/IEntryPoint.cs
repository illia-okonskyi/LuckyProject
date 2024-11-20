﻿using LuckyProject.Lib.Hosting.EntryPoint;
using System.Threading.Tasks;

namespace LuckyProject.SecretManager
{
    public interface IEntryPoint : IGenericHostEntryPoint
    {
        Task RunAsync();
    }
}
