using System;
using System.Collections.Generic;
using System.Linq;
using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformEFCoreRepository : IPlatformRepository
{
    private readonly AppDbContext _context;

    public PlatformEFCoreRepository(AppDbContext context)
    {
        _context = context;
    }
        
    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }

    public IEnumerable<Platform> GetAll()
    {
        return _context.Platforms.ToList();
    }

    public Platform? GetById(int id)
    {
        return _context.Platforms.FirstOrDefault(_ => _.Id == id);
    }

    public void Create(Platform platform)
    {
        if (platform == null) throw new ArgumentNullException(nameof(platform));
        _context.Platforms.Add(platform);
    }
}