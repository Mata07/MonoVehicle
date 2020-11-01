using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MonoVehicle.Data;
using MonoVehicle.Models;

namespace MonoVehicle.Controllers
{
    public class VehicleMakesController : Controller
    {
        private readonly VehicleContext _context;

        public VehicleMakesController(VehicleContext context)
        {
            _context = context;
        }

        // GET: VehicleMakes
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["AbrvSortParm"] = sortOrder == "Abrv" ? "abrv_desc" : "Abrv";
            ViewData["CurrentFilter"] = searchString;

            var makes = from m in _context.VehicleMakes
                        select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                // Contains method on IQueryable object performs case-insensitive comparison(SQL default)
                makes = makes.Where(m => m.Name.Contains(searchString)
                                      || m.Abrv.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    makes = makes.OrderByDescending(m => m.Name);
                    break;
                case "Abrv":
                    makes = makes.OrderBy(m => m.Abrv);
                    break;
                case "abrv_desc":
                    makes = makes.OrderByDescending(m => m.Abrv);
                    break;
                default:
                    makes = makes.OrderBy(m => m.Name);
                    break;
            }

            return View(await makes.AsNoTracking().ToListAsync());
        }

        // GET: VehicleMakes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleMake = await _context.VehicleMakes
                .Include(v => v.VehicleModels)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleMake == null)
            {
                return NotFound();
            }

            return View(vehicleMake);
        }

        // GET: VehicleMakes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VehicleMakes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Abrv")] VehicleMake vehicleMake)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.VehicleMakes.Add(vehicleMake);
                    await _context.SaveChangesAsync();
                    // Display Success message after adding
                    TempData["Message"] = "Success! New Vehicle Make added.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes...");
            }

            return View(vehicleMake);
        }

        // GET: VehicleMakes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleMake = await _context.VehicleMakes.FindAsync(id);
            if (vehicleMake == null)
            {
                return NotFound();
            }
            return View(vehicleMake);
        }

        // POST: VehicleMakes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleMakeToUpdate = await _context.VehicleMakes.FirstOrDefaultAsync(m => m.Id == id);

            if (await TryUpdateModelAsync<VehicleMake>(
                vehicleMakeToUpdate,
                "",
                m => m.Name, m => m.Abrv))
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

            return View(vehicleMakeToUpdate);
        }


        // GET: VehicleMakes/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleMake = await _context.VehicleMakes
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleMake == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete Failed";
            }

            return View(vehicleMake);
        }

        // POST: VehicleMakes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicleMake = await _context.VehicleMakes.FindAsync(id);
            if (vehicleMake == null)
            {
                // Display message if it's null
                TempData["Message"] = "No Vehicle make with that Id";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.VehicleMakes.Remove(vehicleMake);
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

    }
}
