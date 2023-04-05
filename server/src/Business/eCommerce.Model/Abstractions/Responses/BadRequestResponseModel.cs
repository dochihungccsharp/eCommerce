using System.Net;

namespace eCommerce.Model.Abstractions.Responses;

public class BadRequestResponseModel : BaseResponseModel
{
    public BadRequestResponseModel() { }

    public BadRequestResponseModel(string errorMessage) : base(HttpStatusCode.BadRequest, errorMessage) { }
}