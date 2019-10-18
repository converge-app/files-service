using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Helpers;
using Application.Models.DataTransferObjects;
using Application.Models.Entities;
using Application.Repositories;
using Application.Services;
using Application.Utility;
using Application.Utility.Exception;
using Application.Utility.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace Application.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IFileRepository _fileRepository;
        private readonly IFileservice _fileservice;

        public FilesController(IFileservice fileservice, IFileRepository fileRepository, IMapper mapper)
        {
            _fileservice = fileservice;
            _fileRepository = fileRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> OpenFile([FromBody] FileCreationDto fileDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var createFile = _mapper.Map<File>(fileDto);
            try
            {
                var createdFile = await _fileservice.Open(createFile);
                return Ok(createdFile);
            }
            catch (UserNotFound)
            {
                return NotFound(new MessageObj("User not found"));
            }
            catch (EnvironmentNotSet)
            {
                throw;
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }
        }

        [HttpPut("{fileId}")]
        public async Task<IActionResult> AcceptFile([FromHeader] string authorization, [FromRoute] string fileId, [FromBody] FileUpdateDto fileDto)
        {
            if (fileId != fileDto.Id)
                return BadRequest(new MessageObj("Invalid id(s)"));

            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var updateFile = _mapper.Map<File>(fileDto);
            try
            {
                if (await _fileservice.Accept(updateFile, authorization.Split(' ') [1]))
                    return Ok();
                throw new InvalidFile();
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var files = await _fileRepository.Get();
            var fileDtos = _mapper.Map<IList<FileDto>>(files);
            return Ok(fileDtos);
        }

        [HttpGet("freelancer/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByFreelancerId(string id)
        {
            var files = await _fileRepository.GetByFreelancerId(id);
            var filesDto = _mapper.Map<IList<FileDto>>(files);
            return Ok(filesDto);
        }

        [HttpGet("project/{projectId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProjectId([FromRoute] string projectId)
        {
            var files = await _fileRepository.GetByProjectId(projectId);
            var fileDtos = _mapper.Map<IList<FileDto>>(files);
            return Ok(fileDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            var file = await _fileRepository.GetById(id);
            var fileDto = _mapper.Map<FileDto>(file);
            return Ok(fileDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _fileRepository.Remove(id);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }

            return Ok();
        }
    }
}