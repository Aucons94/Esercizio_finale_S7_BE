using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Esercizio_finale_s7.Models
{
    public partial class ModelDbContext : DbContext
    {
        public ModelDbContext()
            : base("name=ModelDbContext")
        {
        }

        public virtual DbSet<DettaglioOrdine> DettaglioOrdine { get; set; }
        public virtual DbSet<Ordine> Ordine { get; set; }
        public virtual DbSet<Prodotto> Prodotto { get; set; }
        public virtual DbSet<Ruolo> Ruolo { get; set; }
        public virtual DbSet<Utente> Utente { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ordine>()
                .Property(e => e.IndirizzoConsegna)
                .IsUnicode(false);

            modelBuilder.Entity<Ordine>()
                .Property(e => e.NoteSpeciali)
                .IsUnicode(false);

            modelBuilder.Entity<Prodotto>()
                .Property(e => e.Nome)
                .IsUnicode(false);

            modelBuilder.Entity<Prodotto>()
                .Property(e => e.FotoUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Ruolo>()
                .Property(e => e.Role)
                .IsUnicode(false);

            modelBuilder.Entity<Utente>()
                .Property(e => e.Nome)
                .IsUnicode(false);

            modelBuilder.Entity<Utente>()
                .Property(e => e.Cognome)
                .IsUnicode(false);

            modelBuilder.Entity<Utente>()
                .Property(e => e.Provincia)
                .IsUnicode(false);

            modelBuilder.Entity<Utente>()
                .Property(e => e.Citta)
                .IsUnicode(false);

            modelBuilder.Entity<Utente>()
                .Property(e => e.Indirizzo)
                .IsUnicode(false);

            modelBuilder.Entity<Utente>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Utente>()
                .Property(e => e.Password)
                .IsUnicode(false);
        }
    }
}
