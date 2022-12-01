using Microsoft.AspNetCore.Mvc;
using Infrastructure.Data;
using System.Collections.Generic;
using Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.Specifications;
using API.Dtos;
using AutoMapper;
using API.Errors;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
    //[ApiController]
    //[Route("api/[controller]")]
    public class ProductsController : BaseApiController
    {
        //private readonly IProductRepository _repo;

        //public ProductsController(IProductRepository repo)
        //{
            //_repo= repo;
        //}
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productsRepo, IGenericRepository<ProductBrand> productBrandRepo,IGenericRepository<ProductType> productTypeRepo,IMapper mapper)
        {
            _productsRepo=productsRepo;
            _productBrandRepo=productBrandRepo;
            _productTypeRepo=productTypeRepo;
            _mapper=mapper;
        }


        [HttpGet]
        //public async Task<ActionResult<List<Product>>> GetProducts()
        //public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts(string sort , int? brandId , int? typeId)
        //public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams productParams)
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams productParams)
        {
            //var products = await _repo.GetProductsAsync();
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);
            var countSpec = new ProductWithFiltersForCountSpecification(productParams);
            var totalItems=await _productsRepo.CountAsync(countSpec);
            //var products = await _productsRepo.ListAllAsync();
            var products = await _productsRepo.ListAsync(spec);
            //return Ok(products);
            //return "This will be a list of products!";
            //return products.Select(product=>new ProductToReturnDto
            //{
                //Id=product.Id,
                //Name=product.Name,
                //Description=product.Description,
                //PictureUrl=product.PictureUrl,
                //Price=product.Price,
                //ProductBrand=product.ProductBrand.Name,
                //ProductType=product.ProductType.Name
            //}).ToList();
            var data=_mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products);
            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex,productParams.PageSize,totalItems,data));
        }

        [HttpGet("{id}")]
        //[ProductResponseType(statusCodes.Status200OK)]
        //[ProductResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            //var product = await _repo.GetProductByIdAsync(id);
            //var product = await _productsRepo.GetByIdAsync(id);
            var spec=new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _productsRepo.GetEntityWithSpec(spec);
            //return Ok(products);
            //return new ProductToReturnDto
            //{
               // Id=product.Id,
                //Name=product.Name,
               // Description=product.Description,
              //  PictureUrl=product.PictureUrl,
               // Price=product.Price,
               // ProductBrand=product.ProductBrand.Name,
              //  ProductType=product.ProductType.Name
           // };
           if(product ==null) return NotFound(new ApiResponse(404));

           return _mapper.Map<Product,ProductToReturnDto>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<List<ProductBrand>>> GetProductBrands()
        {
            //var productBrands=await _repo.GetProductBrandsAsync();
            var productBrands = await _productBrandRepo.ListAllAsync();
            return Ok(productBrands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<List<ProductType>>> GetProductTypes()
        {
            //var productTypes=await _repo.GetProductBrandsAsync();
            var productTypes = await _productTypeRepo.ListAllAsync();
            return Ok(productTypes);
        }
        
    }
}