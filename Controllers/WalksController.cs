using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly NZWalksDbContext _context;
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;
        public WalksController(NZWalksDbContext context, IMapper mapper,IWalkRepository walkRepository)
        {
            _context = context;
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }

        //GET Walks
        //GET: /api/walks?filterOn=Name&filterQuery=Track&SortBy=Name&isAscending=true&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery, 
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber =1, [FromQuery] int pageSize = 1000)
        {
            var walks = await walkRepository.GetAllAsync(filterOn,filterQuery,sortBy,isAscending ?? true, pageNumber,pageSize);

            //Map Domain to Dto

            return Ok(mapper.Map<List<WalkDto>>(walks)); 

        }


        //CREATE Walks
        //POST: /api/walks
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            
                //map AddWalkRequestDto to Walk domain model
                var walk = mapper.Map<Walk>(addWalkRequestDto);
                await walkRepository.CreateAsync(walk);

                //map Walk domain model to WalkDto(sending back to client)

                return Ok(mapper.Map<WalkDto>(walk));

        }

        //Get Walk By Id
        //GET: /api/walks/{id}
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walk = await walkRepository.GetByIdAsync(id);
            if(walk == null)
            {
                return NotFound();
            }
            //Map Domain model to Dto
            return Ok(mapper.Map<WalkDto>(walk));
        }

        //Update Walk
        //PUT: /api/walks/{id}
        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequest)
        {
         
                //Dto to domain model
                var walk = mapper.Map<Walk>(updateWalkRequest);
                walk = await walkRepository.UpdateAsync(id, walk);
                if (walk == null)
                {
                    return NotFound();
                }
                //Map domain model to Dto
                return Ok(mapper.Map<WalkDto>(walk));
            
            
        }

        //Delete Walk
        //DELETE /api/walks/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walk = walkRepository.DeleteAsync(id);
            if (walk == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<WalkDto>(walk));
        }
    }
}
