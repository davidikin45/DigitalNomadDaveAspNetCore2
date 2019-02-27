using AspNetCore.Base.Alerts;
using AspNetCore.Base.Data.RepositoryFileSystem;
using AspNetCore.Base.Dtos;
using AspNetCore.Base.Email;
using AspNetCore.Base.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCore.Base.Controllers.File
{

    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    [Authorize(Roles = "admin")]
    public abstract class MvcControllerFileMetadataAuthorizeBase : MvcControllerFileMetadataReadOnlyAuthorizeBase
    {
        public MvcControllerFileMetadataAuthorizeBase(string physicalPath, Boolean includeSubDirectories, Boolean admin, IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, IMapper mapper = null, IEmailService emailService = null)
        : base(physicalPath, includeSubDirectories, admin, fileSystemGenericRepositoryFactory, mapper, emailService)
        {
        }

        // GET: Default/Edit/5
        [Route("edit/{*id}")]
        public virtual async Task<ActionResult> Edit(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            FileInfo data = null;
            try
            {
                var repository = FileSystemGenericRepositoryFactory.CreateFileRepositoryReadOnly(cts.Token, PhysicalPath, IncludeSubDirectories);
                data = await repository.GetByPathAsync(id.Replace("/", "\\"));

                var dto = Mapper.Map<FileMetadataDto>(data);
                dto.Id = dto.Id.Replace(PhysicalPath, "");

                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("Edit", dto);
            }
            catch { 
                return HandleReadException();
            }
        }

        // POST: Default/Edit/5
        [HttpPost]
        [Route("edit/{*id}")]
        public virtual ActionResult Edit(string id, FileMetadataDto dto)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (ModelState.IsValid)
            {
                try
                {
                    var oldPath = PhysicalPath + id.Replace("/", "\\");
    
                    var fileInfo = new FileInfo(oldPath);

                    string fileName = Path.GetFileNameWithoutExtension(dto.Caption) + Path.GetExtension(oldPath);
                    var newPath = Path.GetDirectoryName(oldPath) + "\\" + fileName;

                    if (oldPath.ToLower() != newPath.ToLower())
                    {
                        fileInfo.MoveTo(newPath);
                    }

                    System.IO.File.SetLastWriteTime(newPath, dto.CreationTime);

                    //await Service.UpdateAsync(dto, cts.Token);
                    return RedirectToControllerDefault().WithSuccess(this, Messages.UpdateSuccessful);
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }

            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            return View("Edit", dto);
        }

        // GET: Default/Delete/5
        [Route("delete/{*id}")]
        public virtual async Task<ActionResult> Delete(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            FileInfo data = null;
            try
            {

                var repository = FileSystemGenericRepositoryFactory.CreateFileRepositoryReadOnly(cts.Token, PhysicalPath, IncludeSubDirectories);
                data = await repository.GetByPathAsync(id.Replace("/", "\\"));

                var dto = Mapper.Map<FileMetadataDto>(data);
                dto.Id = dto.Id.Replace(PhysicalPath, "");

                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("Delete", dto);
            }
            catch
            {
                return HandleReadException();
            }
        }

        // POST: Default/Delete/5
        [HttpPost, ActionName("Delete"), Route("delete/{*id}")]
        public virtual ActionResult DeleteConfirmed(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (ModelState.IsValid)
            {
                try
                {
                    var repository = FileSystemGenericRepositoryFactory.CreateFileRepository(cts.Token, PhysicalPath, IncludeSubDirectories);
                    repository.Delete(id.Replace("/", "\\"));

                    return RedirectToControllerDefault().WithSuccess(this, Messages.DeleteSuccessful);
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }
            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            return View("Delete", id);
        }
    }
}

