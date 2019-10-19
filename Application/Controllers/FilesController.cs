using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Models.DataTransferObjects;
using Application.Repositories;
using Application.Services;
using Application.Utility.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Application.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IFileRepository _fileRepository;
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService, IFileRepository fileRepository, IMapper mapper,
            IHostingEnvironment hostingEnvironment)
        {
            _fileService = fileService;
            _fileRepository = fileRepository;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                if (file != null)
                {
                    string ext = Path.GetExtension(file.FileName);

                    var newFile = new Application.Models.Entities.File
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        UserId = userId
                    };

                    var fileModel = await _fileService.UploadFile(newFile, file);
                    return Ok(fileModel);
                }
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }

            return BadRequest(new MessageObj("Couldn't upload file'"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var files = await _fileRepository.Get();
            var fileDtos = _mapper.Map<IList<FileDto>>(files);
            return Ok(fileDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var file = await _fileRepository.GetById(id);
            var fileDto = _mapper.Map<FileDto>(file);
            return Ok(fileDto);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] string userId)
        {
            var file = await _fileRepository.GetById(userId);
            var fileDto = _mapper.Map<FileDto>(file);
            return Ok(fileDto);
        }
    }
}