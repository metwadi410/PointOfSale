using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Business.Contracts;
using PointOfSale.Model;
using PointOfSale.Models;
using PointOfSale.Utilities.Response;
using System.Security.Claims;

namespace PointOfSale.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly ITypeDocumentSaleService _typeDocumentSaleService;
        private readonly ISaleService _saleService;
        private readonly IMapper _mapper;
        public SalesController(ITypeDocumentSaleService typeDocumentSaleService,
            ISaleService saleService, IMapper mapper)
        {
            _typeDocumentSaleService = typeDocumentSaleService;
            _saleService = saleService;
            _mapper = mapper;
        }
        public IActionResult NewSale()
        {
            return View();
        }

        public IActionResult SalesHistory()
        {
            return View();
        }



        [HttpGet]
        public async Task<IActionResult> ListTypeDocumentSale()
        {
            List<VMTypeDocumentSale> vmListTypeDocumentSale = _mapper.Map<List<VMTypeDocumentSale>>(await _typeDocumentSaleService.List());
            return StatusCode(StatusCodes.Status200OK, vmListTypeDocumentSale);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(string search)
        {
            List<VMProduct> vmListProducts = _mapper.Map<List<VMProduct>>(await _saleService.GetProducts(search));
            return StatusCode(StatusCodes.Status200OK, vmListProducts);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterSale([FromBody] VMSale model)
        {
            GenericResponse<VMSale> gResponse = new GenericResponse<VMSale>();
            try
            {

                ClaimsPrincipal claimuser = HttpContext.User;

                string idUsuario = claimuser.Claims
                        .Where(c => c.Type == ClaimTypes.NameIdentifier)
                        .Select(c => c.Value).SingleOrDefault();

                model.IdUsers = int.Parse(idUsuario);


                Sale sale_created = await _saleService.Register(_mapper.Map<Sale>(model));
                model = _mapper.Map<VMSale>(sale_created);

                gResponse.State = true;
                gResponse.Object = model;
            }
            catch (Exception ex)
            {
                gResponse.State = false;
                gResponse.Message = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }


        [HttpGet]
        public async Task<IActionResult> History(string saleNumber, string startDate, string endDate)
        {

            List<VMSale> vmHistorySale= _mapper.Map<List<VMSale>>(await _saleService.SaleHistory(saleNumber, startDate, endDate));
            return StatusCode(StatusCodes.Status200OK, vmHistorySale);
        }

    }
}
