
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;
using System;
using System.Linq;

using RouteModel = RaceDay.API.Models.Route;

namespace RaceDay.API.Data;

public class RaceDayDbContext : DbContext
{
    public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Organiser> Organisers => Set<Organiser>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<Weather> WeatherData => Set<Weather>();
    public DbSet<RouteModel> Routes => Set<RouteModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Organiser>()
            .HasOne(o => o.User)
            .WithOne(u => u.Organiser)
            .HasForeignKey<Organiser>(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Participant>()
            .HasOne(p => p.User)
            .WithOne(u => u.Participant)
            .HasForeignKey<Participant>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Participant>()
            .HasIndex(p => p.IdentityNumber)
            .IsUnique();

        modelBuilder.Entity<Enrolment>()
            .HasIndex(e => new { e.ParticipantId, e.CategoryId })
            .IsUnique();

        modelBuilder.Entity<Enrolment>()
            .HasIndex(e => e.BibNumber)
            .IsUnique()
            .HasFilter("[BibNumber] IS NOT NULL");

        modelBuilder.Entity<Result>()
            .HasOne(r => r.Enrolment)
            .WithOne(e => e.Result)
            .HasForeignKey<Result>(r => r.EnrolmentId)
            .OnDelete(DeleteBehavior.Cascade);

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        
        // Use RouteModel instead of Route in the route seed section.
    }
}
