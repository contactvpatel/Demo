﻿using Demo.Core.Models;
using Demo.Util.Models;

namespace Demo.Core.Repositories
{
    public interface IAddressRepository
    {
        Task<HttpResponseModel> GetDynamic(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
        Task<ListResponseToModel<CustomerAddressModel>> Get(string fields = "", string filters = "", string include = "", string sort = "", int pageNo = 0, int pageSize = 0);
    }
}
