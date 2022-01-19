using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using BLogic.Models.Clients;
using BLogic.Models;

namespace BLogic.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //1:N (klient:smlouvy)
            builder.Entity<Client>()
                .HasMany(c => c.Contracts)
                .WithOne(x => x.Client);
            //M:N (poradci:smlouvy)
            builder.Entity<AdvisorContract>()
                .HasKey(ac => new { ac.AdvisorId, ac.ContractId });
            builder.Entity<AdvisorContract>()
                .HasOne(ac => ac.Advisor)
                .WithMany(c => c.AdvisorContracts)
                .HasForeignKey(ac => ac.AdvisorId);
            builder.Entity<AdvisorContract>()
                .HasOne(ac => ac.Contract)
                .WithMany(c => c.AdvisorContracts)
                .HasForeignKey(ac => ac.ContractId);
        }
    }
}
