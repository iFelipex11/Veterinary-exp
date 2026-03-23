using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Veterinary.Shared.Entities;

namespace Veterinary.API.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<PetType> PetTypes => Set<PetType>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
    public DbSet<History> Histories => Set<History>();
    public DbSet<Agenda> Agendas => Set<Agenda>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Owner>().HasIndex(owner => owner.Document).IsUnique();
        modelBuilder.Entity<PetType>().HasIndex(petType => petType.Name).IsUnique();
        modelBuilder.Entity<ServiceType>().HasIndex(serviceType => serviceType.Name).IsUnique();

        modelBuilder.Entity<Pet>()
            .HasOne(pet => pet.Owner)
            .WithMany(owner => owner.Pets)
            .HasForeignKey(pet => pet.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pet>()
            .HasOne(pet => pet.PetType)
            .WithMany(petType => petType.Pets)
            .HasForeignKey(pet => pet.PetTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<History>()
            .HasOne(history => history.Pet)
            .WithMany(pet => pet.Histories)
            .HasForeignKey(history => history.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<History>()
            .HasOne(history => history.ServiceType)
            .WithMany(serviceType => serviceType.Histories)
            .HasForeignKey(history => history.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Agenda>()
            .HasOne(agenda => agenda.Owner)
            .WithMany(owner => owner.Agendas)
            .HasForeignKey(agenda => agenda.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Agenda>()
            .HasOne(agenda => agenda.Pet)
            .WithMany(pet => pet.Agendas)
            .HasForeignKey(agenda => agenda.PetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
