using eCommerce.Model.Suppliers;
using eCommerce.Model.Users;

namespace eCommerce.Model.PurchaseOrders
{
    public class PurchaseOrderDetailsModel
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalMoney { get; set; }
        public string Note { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public SupplierModel _Supplier { get; set; }
        public UserModel? _User { get; set; }
        public List<PurchaseOrderDetails.PurchaseOrderDetailsModel> _PurchaseOrderDetails { get; set; }

        #region [AUDIT PROPERTIES]
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        #endregion
    }
}