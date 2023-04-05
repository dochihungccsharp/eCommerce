using eCommerce.Model.Abstractions.Audits;

namespace eCommerce.Model.Suppliers;


    public class SupplierDetailsModel : IAuditModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public decimal TotalAmountOwed { get; set; }
        public bool Status { get; set; }

        #region [AUDIT PROPERTIES]
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        #endregion
    }
