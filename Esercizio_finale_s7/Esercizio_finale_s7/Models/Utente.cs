namespace Esercizio_finale_s7.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Utente")]
    public partial class Utente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Utente()
        {
            Ordine = new HashSet<Ordine>();
        }

        [Key]
        public int IdUtente { get; set; }

        [StringLength(255)]
        public string Nome { get; set; }

        [StringLength(255)]
        public string Cognome { get; set; }

        [StringLength(255)]
        public string Provincia { get; set; }

        [StringLength(255)]
        public string Citta { get; set; }

        [StringLength(255)]
        public string Indirizzo { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string Password { get; set; }

        public int? IdRuolo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ordine> Ordine { get; set; }

        public virtual Ruolo Ruolo { get; set; }
        public bool IsDeleted { get; set; }

    }
}
