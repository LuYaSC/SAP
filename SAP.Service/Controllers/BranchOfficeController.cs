﻿using Microsoft.AspNetCore.Mvc;
using SAP.Core.Business;
using SAP.Model.TypeBusiness;
using SAP.Repository.SAPRepository.Entities;
using SAP.RuleEngine.TypeBusinessService;

namespace SAP.Service.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/[controller]/[action]")]
    public class BranchOfficeController : ControllerBase
    {
        ITypeBusinessService<BranchOffice> service;

        public BranchOfficeController(ITypeBusinessService<BranchOffice> service)
        {
            this.service = service;
        }

        [HttpPost]
        public Result<GetTypeResult> GetById([FromBody] GetTypeByIdDto dto) => service.GetById(dto);

        [HttpGet]
        public Result<List<GetTypeResult>> GetAll() => service.GetAll();

        [HttpPost]
        public Result<string> Update([FromBody] GetTypeByIdDto dto) => service.Update(dto);

        [HttpPost]
        public Result<string> Create([FromBody] CreateTypeDto dto) => service.Create(dto);
    }
}
