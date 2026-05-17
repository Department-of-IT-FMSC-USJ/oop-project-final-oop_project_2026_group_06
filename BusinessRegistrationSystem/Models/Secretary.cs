using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessRegistrationSystem.Models
{
    public class Secretary
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid BusinessRegistrationId { get; set; }

        [ForeignKey("BusinessRegistrationId")]
        public BusinessRegistration? BusinessRegistration { get; set; }

        [Required]
        [StringLength(50)]
        public string SecretaryType { get; set; } = "OnlineBR";

        [StringLength(20)]
        public string? NIC { get; set; }

        [StringLength(10)]
        public string? Title { get; set; }

        [StringLength(200)]
        public string? FirstNames { get; set; }

        [StringLength(200)]
        public string? Surname { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }
    }
}
