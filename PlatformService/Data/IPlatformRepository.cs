using System.Collections.Generic;
using PlatformService.Models;

namespace PlatformService.Data;

public interface IPlatformRepository
{
    bool SaveChanges();

    IEnumerable<Platform> GetAll();
    Platform? GetById(int id);
    void Create(Platform platform);
}