using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessRegistrationSystem.Models
{
    public class Shareholder
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid BusinessRegistrationId { get; set; }

        [ForeignKey("BusinessRegistrationId")]
        public BusinessRegistration? BusinessRegistration { get; set; }

        [Required]
        [StringLength(20)]
        public string NIC { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Title { get; set; } = string.Empty; // Mr, Mrs, Miss, Dr, Prof, Rev

        [Required]
        [StringLength(200)]
        public string FirstNames { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public int NumberOfShares { get; set; }
    }
}
