﻿using Microsoft.EntityFrameworkCore;

namespace GreenPointsAPI.Data;

public class GreenPointsContext : DbContext
{
    public GreenPointsContext(DbContextOptions<GreenPointsContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<GreenPoint> GreenPoints => Set<GreenPoint>();
    public DbSet<DescriptionProperty> DescriptionProperties => Set<DescriptionProperty>();
}