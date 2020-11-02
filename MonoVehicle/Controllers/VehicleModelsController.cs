﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MonoVehicle.Data;
using MonoVehicle.Models;
using MonoVehicle.Models.ViewModels;

namespace MonoVehicle.Controllers
{
    public class VehicleModelsController : Controller
    {
        private readonly VehicleContext _context;

        public VehicleModelsController(VehicleContext context)
        {
            _context = context;
        }

        // GET: VehicleModels
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["AbrvSortParm"] = sortOrder == "Abrv" ? "abrv_desc" : "Abrv";
            ViewData["MakeSortParm"] = sortOrder == "MakeName" ? "make_desc" : "MakeName";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;


            //var models = (from model in this._context.VehicleModels
            //              join make in this._context.VehicleMakes on model.MakeId equals make.Id
            //              select new VehicleModelViewModel
            //              {
            //                  Id = model.Id,

            //                  Name = model.Name,

            //                  MakeId = model.MakeId,

            //                  Abrv = model.Abrv,

            //                  MakeName = make.Name

            //                  //}).ToListAsync();
            //              });

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    // filtering by Make only!
            //    models = models.Where(m => m.MakeName.Contains(searchString));
            //}

            //switch (sortOrder)
            //{
            //    case "name_desc":
            //        models = models.OrderByDescending(m => m.Name);
            //        break;
            //    case "Abrv":
            //        models = models.OrderBy(m => m.Abrv);
            //        break;
            //    case "abrv_desc":
            //        models = models.OrderByDescending(m => m.Abrv);
            //        break;
            //    case "make_desc":
            //        models = models.OrderByDescending(m => m.MakeName);
            //        break;
            //    case "MakeName":
            //        models = models.OrderBy(m => m.MakeName);
            //        break;
            //    default:
            //        models = models.OrderBy(m => m.Name);
            //        break;
            //}

            //int pageSize = 3;

            ////return View(await vehicleContext.ToListAsync());            
            ////return View(models);
            ////return View(await models.ToListAsync());
            //return View(await PaginatedList<VehicleModelViewModel>.CreateAsync(models.AsNoTracking(), pageNumber ?? 1, pageSize));

            // Define as IQueryable (hack?) https://entityframeworkcore.com/knowledge-base/50216493/ef-core-conditional--add--includes-to-an-iqueryable
            
            IQueryable<VehicleModel> models = _context.VehicleModels.Include(v => v.Make);


            if (!String.IsNullOrEmpty(searchString))
            {
                // search by Make only!
                // Contains method on IQueryable object performs case-insensitive comparison(SQL default)
                // and on IEnumerable object(List<>) performs case-sensitive comparison(.NET Frmw default)
                // so .ToUpper() makes test explicity case-insensitive
                models = models.Where(m => m.Make.Name.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    models = models.OrderByDescending(m => m.Name);
                    break;
                case "Abrv":
                    models = models.OrderBy(m => m.Abrv);
                    break;
                case "abrv_desc":
                    models = models.OrderByDescending(m => m.Abrv);
                    break;
                case "make_desc":
                    models = models.OrderByDescending(m => m.Make.Name);
                    break;
                case "MakeName":
                    models = models.OrderBy(m => m.Make.Name);
                    break;
                default:
                    models = models.OrderBy(m => m.Name);
                    break;
            }

            int pageSize = 3;

            var list = new List<VehicleModelViewModel>();


            foreach (var item in models)

            {

                VehicleModelViewModel objcvm = new VehicleModelViewModel(); // ViewModel

                objcvm.Name = item.Name;

                objcvm.Id = item.Id;

                objcvm.MakeId = item.MakeId;

                objcvm.MakeName = item.Make.Name;

                objcvm.Abrv = item.Abrv;

                list.Add(objcvm);

            }

            //return View(models);

            // Call CreateAsync(source, pageIndex, pageSize) on PaginatedList<T> and pass it to View
            // converts the student query to a single page of students in a collection type that supports paging. 
            // That single page of students is then passed to the view.
            
            // koristi dodanu non-async metodu Create() umjesto CreateAsync - zato je Index IActionResult a ne Task<IActionResult>
            return View(PaginatedList<VehicleModelViewModel>.Create(list, pageNumber ?? 1, pageSize));
        }

        // GET: VehicleModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleModel = await _context.VehicleModels
                .Include(v => v.Make)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleModel == null)
            {
                return NotFound();
            }

            return View(vehicleModel);
        }

        // GET: VehicleModels/Create
        public IActionResult Create()
        {
            ViewData["MakeId"] = new SelectList(_context.VehicleMakes, "Id", "Name");
            return View();
        }

        // POST: VehicleModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MakeId,Name,Abrv")] VehicleModel vehicleModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(vehicleModel);
                    await _context.SaveChangesAsync();
                    // Display Success message after adding
                    TempData["Message"] = "Success! New Vehicle Model added.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes...");
            }

            ViewData["MakeId"] = new SelectList(_context.VehicleMakes, "Id", "Name", vehicleModel.MakeId);
            return View(vehicleModel);
        }

        // GET: VehicleModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleModel = await _context.VehicleModels.FindAsync(id);
            if (vehicleModel == null)
            {
                return NotFound();
            }
            ViewData["MakeId"] = new SelectList(_context.VehicleMakes, "Id", "Name", vehicleModel.MakeId);
            return View(vehicleModel);
        }

        // POST: VehicleModels/Edit/5        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleModelToUpdate = await _context.VehicleModels.FirstOrDefaultAsync(v => v.Id == id);

            if (await TryUpdateModelAsync<VehicleModel>(
                vehicleModelToUpdate,
                "",
                v => v.MakeId, v => v.Name, v => v.Name, v => v.Abrv))
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes...");
                }
                // Display Success message after updating
                TempData["Message"] = "Vehicle Make is updated!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["MakeId"] = new SelectList(_context.VehicleMakes, "Id", "Name", vehicleModelToUpdate.MakeId);
            return View(vehicleModelToUpdate);

            //if (id != vehicleModel.Id)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(vehicleModel);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!VehicleModelExists(vehicleModel.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["MakeId"] = new SelectList(_context.VehicleMakes, "Id", "Name", vehicleModel.MakeId);
            //return View(vehicleModel);
        }

        // GET: VehicleModels/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleModel = await _context.VehicleModels
                .Include(v => v.Make)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleModel == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete Failed";
            }

            return View(vehicleModel);
        }

        // POST: VehicleModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicleModel = await _context.VehicleModels.FindAsync(id);
            if (vehicleModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.VehicleModels.Remove(vehicleModel);
                await _context.SaveChangesAsync();
                // Display Success message after updating
                TempData["Message"] = "Vehicle Make is deleted!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }

        }

        //private bool VehicleModelExists(int id)
        //{
        //    return _context.VehicleModels.Any(e => e.Id == id);
        //}
    }
}
