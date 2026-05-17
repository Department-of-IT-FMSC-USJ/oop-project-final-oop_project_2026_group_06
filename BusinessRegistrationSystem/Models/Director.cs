using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessRegistrationSystem.Models
{
    public class Director
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
        [StringLength(100)]
        public string FirstNames { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string District { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DivisionalSecretariatDivision { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [StringLength(20)]
        public string ResidencePhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Occupation { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public bool IsShareholder { get; set; } = false;
        
        public int? NumberOfShares { get; set; }
    }
}
