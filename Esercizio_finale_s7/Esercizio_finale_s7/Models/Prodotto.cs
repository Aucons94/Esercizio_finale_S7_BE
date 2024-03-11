namespace Esercizio_finale_s7.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Prodotto")]
    public partial class Prodotto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Prodotto()
        {
            DettaglioOrdine = new HashSet<DettaglioOrdine>();
        }

        [Key]
        public int IdProdotto { get; set; }

        [StringLength(255)]
        public string Nome { get; set; }

        [StringLength(255)]
        public string FotoUrl { get; set; }

        public decimal Prezzo { get; set; }

        public int TempoConsegna { get; set; }

        [StringLength(255)]
        public string Ingredienti { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DettaglioOrdine> DettaglioOrdine { get; set; }
    }
}
