using Application.Common.Interfaces;

namespace BasicWebApi.Application.Common.Interfaces;

public interface IExcelHelper : IScopedService
{
    List<T> Import<T>(string filePath) where T : new();
}